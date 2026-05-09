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
        Dictionary<string, decimal> productPriceByCen;

        if (!CanUseCenInventory(companyCen, warehouseCen, chargeableDetails))
        {
            throw new InvalidOperationException("No hay datos CEN suficientes para procesar el pago");
        }

        var stockValidationResult = await inventoryService.ValidateStockAsync(
            companyCen!,
            CreateStockValidationRequest(warehouseCen!, restaurantOrder.Cen, chargeableDetails));

        if (!stockValidationResult.IsValid)
        {
            return ProcessRestaurantOrderPaymentResultDto.StockFailure(
                MapContractInsufficiencies(stockValidationResult.Requirements));
        }

        var stockConsumeResult = await inventoryService.ConsumeStockAsync(
            companyCen!,
            CreateStockConsumeRequest(warehouseCen!, restaurantOrder.Cen, chargeableDetails));

        if (!stockConsumeResult.Success)
        {
            return ProcessRestaurantOrderPaymentResultDto.StockFailure(
                MapContractInsufficiencies(stockConsumeResult.Requirements));
        }

        inventoryDocumentCen = stockConsumeResult.DocumentCen;
        productPriceByCen = await GetProductPricesByCenFromContractAsync(companyCen!, chargeableDetails);

        var saleDetails = new List<SaleDetail>();
        foreach (var detail in chargeableDetails)
        {
            string productCen = detail.ProductCen!;
            if (!productPriceByCen.TryGetValue(productCen, out var price))
            {
                throw new InvalidOperationException($"No se pudo obtener el precio del producto {productCen}");
            }

            saleDetails.Add(new SaleDetail
            {
                ProductId = 0,
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

        var createdSale = await paymentProcessRepository.CreateSaleAndCloseOrderAsync(
            processRestaurantOrderPaymentDto.RestaurantOrderId,
            sale);

        return ProcessRestaurantOrderPaymentResultDto.Success(
            createdSale.Id,
            createdSale.Cen,
            subtotalPrice,
            restaurantOrder.Order.TaxPrice,
            inventoryDocumentCen);
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

    private async Task<Dictionary<string, decimal>> GetProductPricesByCenFromContractAsync(
        string companyCen,
        List<RestaurantOrderDetail> chargeableDetails)
    {
        var products = await inventoryService.GetProductsAsync(companyCen);
        var productsByCen = products.ToDictionary(product => product.ProductCen, StringComparer.OrdinalIgnoreCase);
        var productPriceByCen = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        foreach (var detail in chargeableDetails)
        {
            if (!productsByCen.TryGetValue(detail.ProductCen!, out var product))
            {
                throw new InvalidOperationException($"No se pudo obtener el precio del producto {detail.ProductCen}");
            }

            productPriceByCen[detail.ProductCen!] = product.SalePrice;
        }

        return productPriceByCen;
    }

    private static List<StockInsufficiencyResponseDto> MapContractInsufficiencies(
        List<StockRequirementContractDto> requirements)
    {
        return requirements.Select(requirement => new StockInsufficiencyResponseDto
        {
            ProductId = 0,
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
