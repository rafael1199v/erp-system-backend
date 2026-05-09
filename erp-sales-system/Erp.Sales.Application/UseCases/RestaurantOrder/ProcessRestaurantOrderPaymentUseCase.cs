using Erp.Inventory.Contracts;
using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;
using Erp.Sales.Domain.Enums;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class ProcessRestaurantOrderPaymentUseCase(
    IRestaurantOrderRepository restaurantOrderRepository,
    IRestaurantOrderDetailRepository restaurantOrderDetailRepository,
    IWarehouseConfigurationRepository warehouseConfigurationRepository,
    IPaymentTypeRepository paymentTypeRepository,
    IPaymentProcessRepository paymentProcessRepository,
    IInventoryService inventoryService) : IProcessRestaurantOrderPaymentUseCase
{
    public async Task<ProcessRestaurantOrderPaymentResultDto> ExecuteAsync(ProcessRestaurantOrderPaymentDto processRestaurantOrderPaymentDto)
    {
        var restaurantOrder = await restaurantOrderRepository.GetByIdAsync(processRestaurantOrderPaymentDto.RestaurantOrderId)
            ?? throw new KeyNotFoundException("La orden del restaurante no existe");

        if (restaurantOrder.Order.Status != OrderStatus.Open)
        {
            throw new InvalidOperationException("La orden no esta abierta para cobro");
        }

        if (restaurantOrder.WaiterId is null)
        {
            throw new InvalidOperationException("No se puede procesar el pago sin mesero asignado");
        }

        if (!await paymentTypeRepository.ExistsAsync(processRestaurantOrderPaymentDto.PaymentTypeId))
        {
            throw new InvalidOperationException("El metodo de pago no es valido");
        }

        var warehouseId = await warehouseConfigurationRepository.GetWarehouseIdByCompanyIdAsync(restaurantOrder.CompanyId);
        if (warehouseId is null)
        {
            throw new InvalidOperationException("No hay una bodega principal configurada para la compania");
        }

        string? companyCen = Normalize(restaurantOrder.CompanyCen);
        string? warehouseCen = Normalize(await warehouseConfigurationRepository
            .GetWarehouseCenByCompanyIdAsync(restaurantOrder.CompanyId));

        var orderDetails = await restaurantOrderDetailRepository.GetByRestaurantOrderIdAsync(processRestaurantOrderPaymentDto.RestaurantOrderId);
        var chargeableDetails = orderDetails
            .Where(detail => detail.Status != OrderDetailStatus.Canceled)
            .ToList();

        if (chargeableDetails.Count == 0)
        {
            throw new InvalidOperationException("No existen items cobrables en la orden");
        }

        string? inventoryDocumentCen = null;
        Dictionary<int, decimal> productPriceById;

        if (CanUseCenInventory(companyCen, warehouseCen, chargeableDetails))
        {
            var stockValidationResult = await inventoryService.ValidateStockAsync(
                companyCen!,
                CreateStockValidationRequest(warehouseCen!, restaurantOrder.Cen, chargeableDetails));

            if (!stockValidationResult.IsValid)
            {
                return ProcessRestaurantOrderPaymentResultDto.StockFailure(
                    MapContractInsufficiencies(stockValidationResult.Requirements, chargeableDetails));
            }

            var stockConsumeResult = await inventoryService.ConsumeStockAsync(
                companyCen!,
                CreateStockConsumeRequest(warehouseCen!, restaurantOrder.Cen, chargeableDetails));

            if (!stockConsumeResult.Success)
            {
                return ProcessRestaurantOrderPaymentResultDto.StockFailure(
                    MapContractInsufficiencies(stockConsumeResult.Requirements, chargeableDetails));
            }

            inventoryDocumentCen = stockConsumeResult.DocumentCen;
            productPriceById = await GetProductPricesByIdFromCenContractAsync(companyCen!, chargeableDetails);
        }
        else
        {
            var stockRequirements = chargeableDetails.Select(detail => new StockRequirementDto
            {
                ProductId = detail.ProductId,
                RequestedQuantity = detail.Quantity,
                WarehouseId = warehouseId.Value
            }).ToList();

            var stockValidationResult = await inventoryService.ValidateStockAvailabilityAsync(stockRequirements, restaurantOrder.CompanyId);
            if (!stockValidationResult.AllAvailable)
            {
                return ProcessRestaurantOrderPaymentResultDto.StockFailure(
                    stockValidationResult.Insufficiencies
                        .Select(insufficiency => new StockInsufficiencyResponseDto
                        {
                            ProductId = insufficiency.ProductId,
                            ProductName = insufficiency.ProductName,
                            RequestedQuantity = insufficiency.RequestedQuantity,
                            AvailableQuantity = insufficiency.AvailableQuantity,
                            MissingQuantity = insufficiency.RequestedQuantity - insufficiency.AvailableQuantity
                        })
                        .ToList<StockInsufficiencyResponseDto>());
            }

            await inventoryService.ExecutePaymentStockDiscountAsync(new CreatePaymentStockDiscountDto
            {
                RestaurantOrderId = processRestaurantOrderPaymentDto.RestaurantOrderId,
                CompanyId = restaurantOrder.CompanyId,
                WarehouseId = warehouseId.Value,
                PaymentDateUtc = DateTime.UtcNow,
                Items = chargeableDetails.Select(detail => new PaymentStockDiscountItemDto
                {
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    Reason = $"Pago de orden {processRestaurantOrderPaymentDto.RestaurantOrderId}"
                }).ToList()
            });

            productPriceById = await GetProductPricesByIdFromLegacyContractAsync(chargeableDetails);
        }

        var saleDetails = new List<SaleDetail>();
        foreach (var detail in chargeableDetails)
        {
            if (!productPriceById.TryGetValue(detail.ProductId, out var price))
            {
                throw new InvalidOperationException($"No se pudo obtener el precio del producto {detail.ProductId}");
            }

            saleDetails.Add(new SaleDetail
            {
                ProductId = detail.ProductId,
                ProductCen = detail.ProductCen,
                Price = price,
                Quantity = detail.Quantity
            });
        }

        var subtotalPrice = saleDetails.Sum(detail => detail.Price * detail.Quantity);

        var sale = new Sale
        {
            SubtotalPrice = subtotalPrice,
            TaxPrice = restaurantOrder.Order.TaxPrice,
            DiscountPercentage = 0,
            SaleDatetime = DateTime.UtcNow,
            CustomerId = restaurantOrder.CustomerId,
            PaymentTypeId = processRestaurantOrderPaymentDto.PaymentTypeId,
            CompanyId = restaurantOrder.CompanyId,
            CompanyCen = restaurantOrder.CompanyCen,
            SaleDetails = saleDetails
        };

        var saleId = await paymentProcessRepository.CreateSaleAndCloseOrderAsync(processRestaurantOrderPaymentDto.RestaurantOrderId, sale);

        return ProcessRestaurantOrderPaymentResultDto.Success(saleId, inventoryDocumentCen);
    }

    private static StockValidationContractRequest CreateStockValidationRequest(
        string warehouseCen,
        string ticketCen,
        List<RestaurantOrderDetail> chargeableDetails)
    {
        return new StockValidationContractRequest
        {
            WarehouseCen = warehouseCen,
            Source = "SALES",
            ReferenceCen = ticketCen,
            Items = chargeableDetails.Select(detail => new StockValidationItemContractDto
            {
                ProductCen = detail.ProductCen!,
                Quantity = detail.Quantity
            }).ToList()
        };
    }

    private static StockConsumeContractRequest CreateStockConsumeRequest(
        string warehouseCen,
        string ticketCen,
        List<RestaurantOrderDetail> chargeableDetails)
    {
        return new StockConsumeContractRequest
        {
            WarehouseCen = warehouseCen,
            Source = "SALES",
            ReferenceCen = ticketCen,
            Reason = $"Pago de ticket {ticketCen}",
            Items = chargeableDetails.Select(detail => new StockConsumeItemContractDto
            {
                ProductCen = detail.ProductCen!,
                Quantity = detail.Quantity
            }).ToList()
        };
    }

    private async Task<Dictionary<int, decimal>> GetProductPricesByIdFromCenContractAsync(
        string companyCen,
        List<RestaurantOrderDetail> chargeableDetails)
    {
        var products = await inventoryService.GetProductsAsync(companyCen);
        var productsByCen = products.ToDictionary(product => product.ProductCen, StringComparer.OrdinalIgnoreCase);
        var productPriceById = new Dictionary<int, decimal>();

        foreach (var detail in chargeableDetails)
        {
            if (!productsByCen.TryGetValue(detail.ProductCen!, out var product))
            {
                throw new InvalidOperationException($"No se pudo obtener el precio del producto {detail.ProductCen}");
            }

            productPriceById[detail.ProductId] = product.SalePrice;
        }

        return productPriceById;
    }

    private async Task<Dictionary<int, decimal>> GetProductPricesByIdFromLegacyContractAsync(
        List<RestaurantOrderDetail> chargeableDetails)
    {
        var productIds = chargeableDetails
            .Select(detail => detail.ProductId)
            .Distinct()
            .ToList();

        var products = await inventoryService.GetOrderDetailProductsByIdsAsync(productIds);
        return products.ToDictionary(product => product.ProductId, product => Convert.ToDecimal(product.SellPrice));
    }

    private static List<StockInsufficiencyResponseDto> MapContractInsufficiencies(
        List<StockRequirementContractDto> requirements,
        List<RestaurantOrderDetail> chargeableDetails)
    {
        var productIdsByCen = chargeableDetails
            .Where(detail => !string.IsNullOrWhiteSpace(detail.ProductCen))
            .GroupBy(detail => detail.ProductCen!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().ProductId, StringComparer.OrdinalIgnoreCase);

        return requirements.Select(requirement => new StockInsufficiencyResponseDto
        {
            ProductId = productIdsByCen.GetValueOrDefault(requirement.ProductCen),
            ProductCen = requirement.ProductCen,
            ProductName = requirement.ProductName,
            WarehouseCen = requirement.WarehouseCen,
            RequestedQuantity = ToIntQuantity(requirement.RequestedQuantity),
            AvailableQuantity = ToIntQuantity(requirement.AvailableQuantity),
            MissingQuantity = ToIntQuantity(requirement.MissingQuantity)
        }).ToList();
    }

    private static bool CanUseCenInventory(
        string? companyCen,
        string? warehouseCen,
        List<RestaurantOrderDetail> chargeableDetails)
    {
        return companyCen is not null
               && warehouseCen is not null
               && chargeableDetails.All(detail => !string.IsNullOrWhiteSpace(detail.ProductCen));
    }

    private static int ToIntQuantity(decimal quantity)
    {
        return decimal.ToInt32(decimal.Truncate(quantity));
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
