using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.UseCases.Movement;
using Erp.Inventory.Domain.Enums;
using Erp.Inventory.Presentation.ContractDtos;

namespace Erp.Inventory.Presentation.ContractAdapters;

public class InventoryMovementContractAdapter(
    IInventoryCenResolver cenResolver,
    IInventoryContractMapper mapper,
    ICreateMovementUseCase createMovementUseCase,
    ICreateAdjustmentMovementUseCase createAdjustmentMovementUseCase,
    IGetMovementsUseCase getMovementsUseCase) : IInventoryMovementContractAdapter
{
    public async Task<InventoryContractResult<InventoryDocumentContractDto>> CreateDocumentAsync(string companyCen, InventoryDocumentContractRequest request)
    {
        try
        {
            CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
            if (company is null)
            {
                return InventoryContractResult<InventoryDocumentContractDto>.NotFound("Empresa no encontrada");
            }

            ResolvedDocumentRequest? resolvedRequest = await ResolveDocumentRequestAsync(company.Id, request.WarehouseCen, request.Lines);
            if (resolvedRequest is null)
            {
                return InventoryContractResult<InventoryDocumentContractDto>.NotFound("Producto o almacen no encontrado");
            }

            string normalizedDocumentType = request.DocumentType.Trim().ToUpperInvariant();
            MovementTypeEnum movementType = normalizedDocumentType switch
            {
                "ENTRY" => MovementTypeEnum.Receipt,
                "EXIT" => MovementTypeEnum.Issue,
                "SALE_EXIT" => MovementTypeEnum.Issue,
                _ => throw new InvalidOperationException("Tipo de documento no valido")
            };

            TransactionTypeEnum transactionType = movementType == MovementTypeEnum.Receipt
                ? TransactionTypeEnum.In
                : TransactionTypeEnum.Out;

            string movementDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            InventoryMovementDTO movement = await createMovementUseCase.ExecuteAsync(new CreateInventoryMovementDTO
            {
                Title = request.Reason ?? normalizedDocumentType,
                ExternalReference = request.ExternalReference,
                MovementDate = movementDate,
                MovementType = (int)movementType,
                MovementStatus = (int)MovementStatusEnum.Completed,
                CompanyId = company.Id,
                Transactions = resolvedRequest.Lines.Select(line => new CreateTransactionDTO
                {
                    ProductId = line.ProductId,
                    WarehouseId = resolvedRequest.Warehouse.Id,
                    Quantity = transactionType == TransactionTypeEnum.Out ? -line.Quantity : line.Quantity,
                    Reason = request.Reason ?? normalizedDocumentType,
                    TransactionDate = movementDate,
                    TransactionType = (int)transactionType
                }).ToList()
            });

            return InventoryContractResult<InventoryDocumentContractDto>.Ok(
                mapper.ToInventoryDocumentContract(movement, normalizedDocumentType));
        }
        catch (Exception ex)
        {
            return InventoryContractResult<InventoryDocumentContractDto>.Invalid(ex.Message);
        }
    }

    public async Task<InventoryContractResult<List<InventoryDocumentContractDto>>> GetDocumentsAsync(
        string companyCen,
        string? documentType = null,
        DateTime? from = null,
        DateTime? to = null)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<List<InventoryDocumentContractDto>>.NotFound("Empresa no encontrada");
        }

        List<InventoryMovementDTO> movements = await getMovementsUseCase.GetMovementsAsync(company.Id);
        IEnumerable<InventoryDocumentContractDto> documents = movements.Select(movement => mapper.ToInventoryDocumentContract(movement));

        if (!string.IsNullOrWhiteSpace(documentType))
        {
            documents = documents.Where(document =>
                string.Equals(document.DocumentType, documentType, StringComparison.OrdinalIgnoreCase));
        }

        if (from.HasValue)
        {
            documents = documents.Where(document => document.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            documents = documents.Where(document => document.CreatedAt <= to.Value);
        }

        return InventoryContractResult<List<InventoryDocumentContractDto>>.Ok(documents.ToList());
    }

    public async Task<InventoryContractResult<InventoryAdjustmentContractResponse>> CreateAdjustmentAsync(string companyCen, InventoryAdjustmentContractRequest request)
    {
        try
        {
            CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
            if (company is null)
            {
                return InventoryContractResult<InventoryAdjustmentContractResponse>.NotFound("Empresa no encontrada");
            }

            List<InventoryDocumentLineContractRequest> lines = request.Lines.Select(line => new InventoryDocumentLineContractRequest
            {
                ProductCen = line.ProductCen,
                Quantity = line.Quantity,
                UnitCost = null
            }).ToList();

            ResolvedDocumentRequest? resolvedRequest = await ResolveDocumentRequestAsync(company.Id, request.WarehouseCen, lines);
            if (resolvedRequest is null)
            {
                return InventoryContractResult<InventoryAdjustmentContractResponse>.NotFound("Producto o almacen no encontrado");
            }

            string movementDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            InventoryMovementDTO movement = await createAdjustmentMovementUseCase.ExecuteAsync(new CreateInventoryMovementDTO
            {
                Title = request.Reason,
                MovementDate = movementDate,
                MovementType = (int)MovementTypeEnum.Adjustment,
                MovementStatus = (int)MovementStatusEnum.Completed,
                CompanyId = company.Id,
                Transactions = request.Lines.Select(line =>
                {
                    ResolvedDocumentLine resolvedLine = resolvedRequest.Lines.First(resolved => resolved.RequestedProductCen == line.ProductCen);
                    int quantity = InventoryContractAdapterDefaults.ToInternalQuantity(line.Quantity, "quantity");
                    string adjustmentType = line.AdjustmentType.Trim().ToUpperInvariant();
                    int signedQuantity = adjustmentType switch
                    {
                        "INCREASE" => quantity,
                        "DECREASE" => -quantity,
                        _ => throw new InvalidOperationException("Tipo de ajuste no valido")
                    };

                    return new CreateTransactionDTO
                    {
                        ProductId = resolvedLine.ProductId,
                        WarehouseId = resolvedRequest.Warehouse.Id,
                        Quantity = signedQuantity,
                        Reason = request.Reason,
                        TransactionDate = movementDate,
                        TransactionType = signedQuantity < 0 ? (int)TransactionTypeEnum.Out : (int)TransactionTypeEnum.In
                    };
                }).ToList()
            });

            List<GeneratedMovementContractDto> generatedMovements = movement.Transactions.Select((transaction, index) =>
            {
                InventoryAdjustmentLineContractRequest sourceLine = request.Lines[index];
                return new GeneratedMovementContractDto
                {
                    MovementCen = transaction.Cen,
                    ProductCen = sourceLine.ProductCen,
                    WarehouseCen = request.WarehouseCen,
                    Quantity = Math.Abs(transaction.Quantity),
                    MovementType = transaction.Quantity < 0 ? "ADJUSTMENT_OUT" : "ADJUSTMENT_IN"
                };
            }).ToList();

            return InventoryContractResult<InventoryAdjustmentContractResponse>.Ok(new InventoryAdjustmentContractResponse
            {
                AdjustmentCen = movement.Cen,
                Status = InventoryContractAdapterDefaults.RegisteredStatus,
                GeneratedMovements = generatedMovements
            });
        }
        catch (InvalidOperationException ex)
        {
            return InventoryContractResult<InventoryAdjustmentContractResponse>.Invalid(ex.Message);
        }
    }

    public async Task<InventoryContractResult<List<KardexMovementContractDto>>> GetKardexAsync(
        string companyCen,
        string productCen,
        string? warehouseCen = null,
        DateTime? from = null,
        DateTime? to = null)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<List<KardexMovementContractDto>>.NotFound("Empresa no encontrada");
        }

        CenLookup? product = await cenResolver.ResolveProductAsync(company.Id, productCen);
        if (product is null)
        {
            return InventoryContractResult<List<KardexMovementContractDto>>.NotFound("Producto no encontrado");
        }

        CenLookup? warehouse = null;
        if (!string.IsNullOrWhiteSpace(warehouseCen))
        {
            warehouse = await cenResolver.ResolveWarehouseAsync(company.Id, warehouseCen);
            if (warehouse is null)
            {
                return InventoryContractResult<List<KardexMovementContractDto>>.NotFound("Almacen no encontrado");
            }
        }

        List<InventoryMovementDTO> movements = await getMovementsUseCase.GetMovementsAsync(company.Id);
        IEnumerable<KardexMovementContractDto> kardex = movements
            .SelectMany(movement => movement.Transactions
                .Where(transaction => transaction.ProductId == product.Id
                                      && (!warehouseCen.HasValue() || transaction.WarehouseId == warehouse!.Id))
                .Select(transaction => mapper.ToKardexMovementContract(movement, transaction)));

        if (from.HasValue)
        {
            kardex = kardex.Where(movement => movement.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            kardex = kardex.Where(movement => movement.CreatedAt <= to.Value);
        }

        return InventoryContractResult<List<KardexMovementContractDto>>.Ok(kardex.ToList());
    }

    private async Task<ResolvedDocumentRequest?> ResolveDocumentRequestAsync(
        int companyId,
        string warehouseCen,
        IEnumerable<InventoryDocumentLineContractRequest> lines)
    {
        CenLookup? warehouse = await cenResolver.ResolveWarehouseAsync(companyId, warehouseCen);
        if (warehouse is null)
        {
            return null;
        }

        List<InventoryDocumentLineContractRequest> requestedLines = lines.ToList();
        IReadOnlyDictionary<string, CenLookup> products = await cenResolver.ResolveProductsAsync(
            companyId,
            requestedLines.Select(line => line.ProductCen));

        if (products.Count != requestedLines.Select(line => line.ProductCen.Trim()).Distinct().Count())
        {
            return null;
        }

        List<ResolvedDocumentLine> resolvedLines = requestedLines.Select(line =>
        {
            string normalizedProductCen = line.ProductCen.Trim();
            return new ResolvedDocumentLine(
                products[normalizedProductCen].Id,
                line.ProductCen,
                products[normalizedProductCen].Cen,
                InventoryContractAdapterDefaults.ToInternalQuantity(line.Quantity, "quantity"));
        }).ToList();

        return new ResolvedDocumentRequest(warehouse, resolvedLines);
    }

    private sealed record ResolvedDocumentRequest(CenLookup Warehouse, List<ResolvedDocumentLine> Lines);

    private sealed record ResolvedDocumentLine(int ProductId, string RequestedProductCen, string ProductCen, int Quantity);
}

internal static class StringExtensions
{
    public static bool HasValue(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}
