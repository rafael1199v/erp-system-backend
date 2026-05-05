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

        var orderDetails = await restaurantOrderDetailRepository.GetByRestaurantOrderIdAsync(processRestaurantOrderPaymentDto.RestaurantOrderId);
        var chargeableDetails = orderDetails
            .Where(detail => detail.Status != OrderDetailStatus.Canceled)
            .ToList();

        if (chargeableDetails.Count == 0)
        {
            throw new InvalidOperationException("No existen items cobrables en la orden");
        }

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
                        AvailableQuantity = insufficiency.AvailableQuantity
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

        var productIds = chargeableDetails
            .Select(detail => detail.ProductId)
            .Distinct()
            .ToList();

        var products = await inventoryService.GetOrderDetailProductsByIdsAsync(productIds);
        var productPriceById = products.ToDictionary(product => product.ProductId, product => Convert.ToDecimal((double)product.SellPrice));

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
            SaleDetails = saleDetails
        };

        var saleId = await paymentProcessRepository.CreateSaleAndCloseOrderAsync(processRestaurantOrderPaymentDto.RestaurantOrderId, sale);

        return ProcessRestaurantOrderPaymentResultDto.Success(saleId);
    }
}
