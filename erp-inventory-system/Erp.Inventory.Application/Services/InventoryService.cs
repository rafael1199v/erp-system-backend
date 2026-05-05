using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.UseCases.Movement;
using Erp.Inventory.Contracts;
using Erp.Inventory.Domain.Enums;

namespace Erp.Inventory.Application.Services;

public class InventoryService(
    IProductRepository productRepository,
    IProductWarehouseRepository productWarehouseRepository,
    ICreateMovementUseCase createMovementUseCase) : IInventoryService
{
    public async Task<bool> IsProductActiveAsync(int productId, int companyId)
    {
        var productIsActive = await productRepository.IsProductActiveAsync(productId, companyId);
        return productIsActive;
    }

    public async Task<bool> HasAvailableStockAsync(int productId, int requestedQuantity, int companyId, int warehouseId)
    {
        var hasAvailableStock = await productWarehouseRepository.ProductHasAvailableStock(productId, requestedQuantity, companyId, warehouseId);
        return hasAvailableStock;
    }

    public async Task<List<RestaurantOrderProductDto>> GetOrderDetailProductsAsync(int companyId, int warehouseId)
    {
        return await productRepository.GetRestaurantOrderProductsAsync(companyId, warehouseId);
    }

    public async Task<List<RestaurantOrderDetailProductDto>> GetOrderDetailProductsByIdsAsync(List<int> productIds)
    {
        return await productRepository.GetRestaurantOrderDetailProductsAsync(productIds);
    }

    public async Task<StockValidationResultDto> ValidateStockAvailabilityAsync(List<StockRequirementDto> requirements, int companyId)
    {
        if (requirements.Count == 0)
        {
            return new StockValidationResultDto
            {
                AllAvailable = true
            };
        }

        var groupedRequirements = requirements
            .Where(r => r.RequestedQuantity > 0)
            .GroupBy(r => new { r.ProductId, r.WarehouseId })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.WarehouseId,
                RequestedQuantity = g.Sum(item => item.RequestedQuantity)
            })
            .ToList();

        var productIds = groupedRequirements.Select(r => r.ProductId).Distinct<int>().ToList();
        var warehouseIds = groupedRequirements.Select(r => r.WarehouseId).Distinct<int>().ToList();

        var productNames = (await productRepository.GetProductInformationAsync(companyId, productIds))
            .ToDictionary(x => x.Id, x => x.Name);

        var availableStocks =
            await productWarehouseRepository.GetAvailableStockAsync(companyId, productIds, warehouseIds);

        var stockLookup = availableStocks.ToDictionary(
            keySelector: x => (x.ProductId, x.WarehouseId),
            elementSelector: x => x.Quantity);

        var insufficiencies = new List<StockInsufficiencyDto>();

        foreach (var requirement in groupedRequirements)
        {
            var availableQuantity = stockLookup.GetValueOrDefault((requirement.ProductId, requirement.WarehouseId), 0);
            if (availableQuantity >= requirement.RequestedQuantity)
            {
                continue;
            }

            insufficiencies.Add(new StockInsufficiencyDto
            {
                ProductId = requirement.ProductId,
                ProductName = productNames.GetValueOrDefault(requirement.ProductId, "Producto no encontrado"),
                RequestedQuantity = requirement.RequestedQuantity,
                AvailableQuantity = availableQuantity
            });
        }

        return new StockValidationResultDto
        {
            AllAvailable = insufficiencies.Count == 0,
            Insufficiencies = insufficiencies
        };
    }

    public async Task ExecutePaymentStockDiscountAsync(CreatePaymentStockDiscountDto createPaymentStockDiscountDto)
    {
        if (createPaymentStockDiscountDto.Items.Count == 0)
        {
            throw new InvalidOperationException("No existen items para descontar stock");
        }

        if (createPaymentStockDiscountDto.WarehouseId <= 0)
        {
            throw new InvalidOperationException("La bodega seleccionada no es valida");
        }

        var groupedItems = Enumerable.Select(createPaymentStockDiscountDto.Items
                .GroupBy(item => item.ProductId), group => new
            {
                ProductId = group.Key,
                Quantity = group.Sum(item => item.Quantity),
                Reason = Enumerable.FirstOrDefault<string>(group.Select(item => item.Reason), reason => !string.IsNullOrWhiteSpace(reason))
            })
            .ToList();

        if (groupedItems.Any(item => item.Quantity <= 0))
        {
            throw new InvalidOperationException("Las cantidades a descontar deben ser mayores a cero");
        }

        var movementDate = createPaymentStockDiscountDto.PaymentDateUtc.ToString("yyyy-MM-dd");
        var movementDto = new CreateInventoryMovementDTO
        {
            Title = $"Salida por pago de orden {createPaymentStockDiscountDto.RestaurantOrderId}",
            MovementDate = movementDate,
            MovementType = (int)MovementTypeEnum.Issue,
            MovementStatus = (int)MovementStatusEnum.Completed,
            CompanyId = createPaymentStockDiscountDto.CompanyId,
            Transactions = groupedItems.Select(item => new CreateTransactionDTO
            {
                ProductId = item.ProductId,
                WarehouseId = createPaymentStockDiscountDto.WarehouseId,
                Quantity = -item.Quantity,
                Reason = item.Reason ?? $"Descuento por pago de orden {createPaymentStockDiscountDto.RestaurantOrderId}",
                TransactionDate = movementDate,
                TransactionType = (int)TransactionTypeEnum.Out
            }).ToList()
        };

        await createMovementUseCase.ExecuteAsync(movementDto);
    }
}