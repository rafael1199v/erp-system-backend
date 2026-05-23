using Erp.Inventory.Contracts;
using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;
using Erp.Sales.Domain.Enums;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class GetTicketTotalsUseCase(
    IRestaurantOrderRepository restaurantOrderRepository,
    IRestaurantOrderDetailRepository restaurantOrderDetailRepository,
    IInventoryService inventoryService) : IGetTicketTotalsUseCase
{
    public async Task<TicketTotalsDto> ExecuteAsync(int restaurantOrderId)
    {
        Domain.Entities.RestaurantOrder restaurantOrder = await restaurantOrderRepository.GetByIdAsync(restaurantOrderId)
            ?? throw new KeyNotFoundException("La orden del restaurante no existe");

        List<RestaurantOrderDetail> chargeableDetails = (await restaurantOrderDetailRepository
                .GetByRestaurantOrderIdAsync(restaurantOrderId))
            .Where(detail => detail.Status != OrderDetailStatus.Canceled)
            .ToList();

        if (chargeableDetails.Count == 0)
        {
            return new TicketTotalsDto(0, restaurantOrder.Order.TaxPrice, 0);
        }

        if (!CanUseCenInventory(restaurantOrder.CompanyCen, chargeableDetails))
        {
            throw new InvalidOperationException("No hay datos CEN suficientes para calcular totales");
        }

        Dictionary<string, decimal> productPriceByCen = await GetProductPricesByCenFromContractAsync(
            restaurantOrder.CompanyCen!,
            chargeableDetails);

        decimal subtotal = chargeableDetails.Sum(detail =>
        {
            string productCen = detail.ProductCen!;
            if (!productPriceByCen.TryGetValue(productCen, out decimal price))
            {
                throw new InvalidOperationException($"No se pudo obtener el precio del producto {productCen}");
            }

            return price * detail.Quantity;
        });

        decimal taxAmount = restaurantOrder.Order.TaxPrice;
        decimal total = (1 + taxAmount / 100) * subtotal;
        return new TicketTotalsDto(subtotal, taxAmount, total);
    }

    private async Task<Dictionary<string, decimal>> GetProductPricesByCenFromContractAsync(
        string companyCen,
        List<RestaurantOrderDetail> chargeableDetails)
    {
        List<ProductContractDto> products = await inventoryService.GetProductsAsync(companyCen);
        Dictionary<string, ProductContractDto> productsByCen = products.ToDictionary(
            product => product.ProductCen,
            StringComparer.OrdinalIgnoreCase);

        Dictionary<string, decimal> productPriceByCen = new(StringComparer.OrdinalIgnoreCase);
        foreach (RestaurantOrderDetail detail in chargeableDetails)
        {
            if (!productsByCen.TryGetValue(detail.ProductCen!, out ProductContractDto? product))
            {
                throw new InvalidOperationException($"No se pudo obtener el precio del producto {detail.ProductCen}");
            }

            productPriceByCen[detail.ProductCen!] = product.SalePrice;
        }

        return productPriceByCen;
    }

    private static bool CanUseCenInventory(
        string? companyCen,
        List<RestaurantOrderDetail> chargeableDetails)
    {
        return !string.IsNullOrWhiteSpace(companyCen)
               && chargeableDetails.All(detail => !string.IsNullOrWhiteSpace(detail.ProductCen));
    }
}
