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
            return new TicketTotalsDto(0, restaurantOrder.Order.TaxPrice, restaurantOrder.Order.TaxPrice);
        }

        Dictionary<int, decimal> productPriceById = CanUseCenInventory(restaurantOrder.CompanyCen, chargeableDetails)
            ? await GetProductPricesByIdFromCenContractAsync(restaurantOrder.CompanyCen!, chargeableDetails)
            : await GetProductPricesByIdFromLegacyContractAsync(chargeableDetails);

        decimal subtotal = chargeableDetails.Sum(detail =>
        {
            if (!productPriceById.TryGetValue(detail.ProductId, out decimal price))
            {
                throw new InvalidOperationException($"No se pudo obtener el precio del producto {detail.ProductId}");
            }

            return price * detail.Quantity;
        });

        decimal taxAmount = restaurantOrder.Order.TaxPrice;
        return new TicketTotalsDto(subtotal, taxAmount, subtotal + taxAmount);
    }

    private async Task<Dictionary<int, decimal>> GetProductPricesByIdFromCenContractAsync(
        string companyCen,
        List<RestaurantOrderDetail> chargeableDetails)
    {
        List<ProductContractDto> products = await inventoryService.GetProductsAsync(companyCen);
        Dictionary<string, ProductContractDto> productsByCen = products.ToDictionary(
            product => product.ProductCen,
            StringComparer.OrdinalIgnoreCase);

        Dictionary<int, decimal> productPriceById = new();
        foreach (RestaurantOrderDetail detail in chargeableDetails)
        {
            if (!productsByCen.TryGetValue(detail.ProductCen!, out ProductContractDto? product))
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
        List<int> productIds = chargeableDetails
            .Select(detail => detail.ProductId)
            .Distinct()
            .ToList();

        List<RestaurantOrderDetailProductDto> products = await inventoryService.GetOrderDetailProductsByIdsAsync(productIds);
        return products.ToDictionary(product => product.ProductId, product => Convert.ToDecimal(product.SellPrice));
    }

    private static bool CanUseCenInventory(
        string? companyCen,
        List<RestaurantOrderDetail> chargeableDetails)
    {
        return !string.IsNullOrWhiteSpace(companyCen)
               && chargeableDetails.All(detail => !string.IsNullOrWhiteSpace(detail.ProductCen));
    }
}
