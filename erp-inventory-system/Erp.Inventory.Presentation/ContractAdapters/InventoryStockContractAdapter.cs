using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.UseCases.Movement;
using Erp.Inventory.Application.UseCases.Product;
using Erp.Inventory.Contracts;
using Erp.Inventory.Domain.Enums;

namespace Erp.Inventory.Presentation.ContractAdapters;

public class InventoryStockContractAdapter(
    IInventoryCenResolver cenResolver,
    IInventoryContractMapper mapper,
    IGetProductWithWarehousesUseCase getProductWithWarehousesUseCase,
    IInventoryService inventoryService,
    ICreateMovementUseCase createMovementUseCase) : IInventoryStockContractAdapter
{
    public async Task<InventoryContractResult<List<StockItemContractDto>>> GetStockAsync(
        string companyCen,
        string? productCen = null,
        string? warehouseCen = null)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<List<StockItemContractDto>>.NotFound("Empresa no encontrada");
        }

        if (!string.IsNullOrWhiteSpace(productCen)
            && await cenResolver.ResolveProductAsync(company.Id, productCen) is null)
        {
            return InventoryContractResult<List<StockItemContractDto>>.NotFound("Producto no encontrado");
        }

        if (!string.IsNullOrWhiteSpace(warehouseCen)
            && await cenResolver.ResolveWarehouseAsync(company.Id, warehouseCen) is null)
        {
            return InventoryContractResult<List<StockItemContractDto>>.NotFound("Almacen no encontrado");
        }

        List<ProductWarehouseDTO> productsWithWarehouses = await getProductWithWarehousesUseCase.ExecuteAsync(company.Id);
        IEnumerable<StockItemContractDto> stockItems = productsWithWarehouses
            .SelectMany(product => product.Warehouses.Select(warehouse => mapper.ToStockItemContract(product.Product, warehouse)));

        if (!string.IsNullOrWhiteSpace(productCen))
        {
            stockItems = stockItems.Where(item =>
                string.Equals(item.ProductCen, productCen, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(warehouseCen))
        {
            stockItems = stockItems.Where(item =>
                string.Equals(item.WarehouseCen, warehouseCen, StringComparison.OrdinalIgnoreCase));
        }

        return InventoryContractResult<List<StockItemContractDto>>.Ok(stockItems.ToList());
    }

    public async Task<InventoryContractResult<StockValidationContractResponse>> ValidateStockAsync(string companyCen, StockValidationContractRequest request)
    {
        try
        {
            CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
            if (company is null)
            {
                return InventoryContractResult<StockValidationContractResponse>.NotFound("Empresa no encontrada");
            }

            ResolvedStockRequest? resolvedRequest = await ResolveStockRequestAsync(company.Id, request.WarehouseCen, request.Items);
            if (resolvedRequest is null)
            {
                return InventoryContractResult<StockValidationContractResponse>.NotFound("Producto o almacen no encontrado");
            }

            StockValidationResultDto validationResult = await inventoryService.ValidateStockAvailabilityAsync(
                resolvedRequest.Requirements,
                company.Id);

            StockValidationContractResponse response = mapper.ToStockValidationContract(
                validationResult,
                resolvedRequest.ProductCensById,
                resolvedRequest.WarehouseCensById);

            return InventoryContractResult<StockValidationContractResponse>.Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return InventoryContractResult<StockValidationContractResponse>.Invalid(ex.Message);
        }
    }

    public async Task<InventoryContractResult<StockConsumeContractResponse>> ConsumeStockAsync(string companyCen, StockConsumeContractRequest request)
    {
        StockValidationContractRequest validationRequest = new()
        {
            WarehouseCen = request.WarehouseCen,
            Source = request.Source,
            ReferenceCen = request.ReferenceCen,
            Items = request.Items.Select(item => new StockValidationItemContractDto
            {
                ProductCen = item.ProductCen,
                Quantity = item.Quantity
            }).ToList()
        };

        InventoryContractResult<StockValidationContractResponse> validationResult =
            await ValidateStockAsync(companyCen, validationRequest);

        if (!validationResult.Succeeded || validationResult.Value is null)
        {
            return validationResult.Status == InventoryContractResultStatus.Invalid
                ? InventoryContractResult<StockConsumeContractResponse>.Invalid(validationResult.Message ?? "No se pudo validar stock")
                : InventoryContractResult<StockConsumeContractResponse>.NotFound(validationResult.Message ?? "No se pudo validar stock");
        }

        if (!validationResult.Value.IsValid)
        {
            return InventoryContractResult<StockConsumeContractResponse>.Conflict(new StockConsumeContractResponse
            {
                Success = false,
                Requirements = validationResult.Value.Requirements
            }, "Stock insuficiente para completar la operacion");
        }

        CenLookup company = (await cenResolver.ResolveCompanyAsync(companyCen))!;
        ResolvedStockRequest resolvedRequest = (await ResolveStockRequestAsync(company.Id, request.WarehouseCen, validationRequest.Items))!;

        string movementDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        InventoryMovementDTO movement = await createMovementUseCase.ExecuteAsync(new CreateInventoryMovementDTO
        {
            Title = $"Salida por venta {request.ReferenceCen}",
            ExternalReference = $"SALES_PAYMENT:{request.ReferenceCen}",
            MovementDate = movementDate,
            MovementType = (int)MovementTypeEnum.Issue,
            MovementStatus = (int)MovementStatusEnum.Completed,
            CompanyId = company.Id,
            Transactions = resolvedRequest.Requirements.Select(requirement => new CreateTransactionDTO
            {
                ProductId = requirement.ProductId,
                WarehouseId = requirement.WarehouseId,
                Quantity = -requirement.RequestedQuantity,
                Reason = request.Reason ?? $"Venta pagada {request.ReferenceCen}",
                TransactionDate = movementDate,
                TransactionType = (int)TransactionTypeEnum.Out
            }).ToList()
        });

        return InventoryContractResult<StockConsumeContractResponse>.Ok(new StockConsumeContractResponse
        {
            Success = true,
            DocumentCen = movement.Cen,
            DocumentType = "SALE_EXIT",
            GeneratedMovementCens = movement.Transactions
                .Select(transaction => transaction.Cen)
                .Where(cen => !string.IsNullOrWhiteSpace(cen))
                .ToList()
        });
    }

    private async Task<ResolvedStockRequest?> ResolveStockRequestAsync(
        int companyId,
        string warehouseCen,
        IEnumerable<StockValidationItemContractDto> items)
    {
        CenLookup? warehouse = await cenResolver.ResolveWarehouseAsync(companyId, warehouseCen);
        if (warehouse is null)
        {
            return null;
        }

        List<StockValidationItemContractDto> requestedItems = items.ToList();
        IReadOnlyDictionary<string, CenLookup> products = await cenResolver.ResolveProductsAsync(
            companyId,
            requestedItems.Select(item => item.ProductCen));

        if (products.Count != requestedItems.Select(item => item.ProductCen.Trim()).Distinct().Count())
        {
            return null;
        }

        List<StockRequirementDto> requirements = requestedItems
            .Select(item =>
            {
                string normalizedProductCen = item.ProductCen.Trim();
                int quantity = InventoryContractAdapterDefaults.ToInternalQuantity(item.Quantity, "quantity");
                return new StockRequirementDto
                {
                    ProductId = products[normalizedProductCen].Id,
                    WarehouseId = warehouse.Id,
                    RequestedQuantity = quantity
                };
            })
            .ToList();

        return new ResolvedStockRequest(
            requirements,
            products.Values.ToDictionary(product => product.Id, product => product.Cen),
            new Dictionary<int, string> { [warehouse.Id] = warehouse.Cen });
    }

    public async Task<InventoryContractResult<string>> IncreaseStockAsync(string companyCen, StockIncreaseContractRequest request)
    {
        CenLookup company = (await cenResolver.ResolveCompanyAsync(companyCen))!;
      
        string movementDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        ResolvedStockRequest resolvedRequest = (await ResolveStockRequestAsync(company.Id, request.WarehouseCen, request.Items))!;

        InventoryMovementDTO movement = await createMovementUseCase.ExecuteAsync(new CreateInventoryMovementDTO
        {
            Title = $"Entrada por compra {request.ReferenceCen}",
            ExternalReference = $"PURCHASE:{request.ReferenceCen}",
            MovementDate = movementDate,
            MovementType = (int)MovementTypeEnum.Receipt,
            MovementStatus = (int)MovementStatusEnum.Completed,
            CompanyId = company.Id,
            Transactions = resolvedRequest.Requirements.Select(requirement => new CreateTransactionDTO
            {
                ProductId = requirement.ProductId,
                WarehouseId = requirement.WarehouseId,
                Quantity = requirement.RequestedQuantity,
                Reason = request.Reason ?? $"Compra realizada {request.ReferenceCen}",
                TransactionDate = movementDate,
                TransactionType = (int)TransactionTypeEnum.In
            }).ToList()
        });

        return InventoryContractResult<string>.Ok(movement.Cen);
    }


    private sealed record ResolvedStockRequest(
        List<StockRequirementDto> Requirements,
        IReadOnlyDictionary<int, string> ProductCensById,
        IReadOnlyDictionary<int, string> WarehouseCensById);
}
