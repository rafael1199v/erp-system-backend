using Erp.Inventory.Contracts;
using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class GetRestaurantOrderDetailsUseCase(IInventoryService inventoryService, IRestaurantOrderDetailRepository restaurantOrderDetailRepository) : IGetRestaurantOrderDetailsUseCase
{
    public async Task<List<RestaurantOrderDetailDto>> ExecuteAsync(int restaurantOrderId)
    {
        var restaurantOrderDetails =
            await restaurantOrderDetailRepository.GetByRestaurantOrderIdAsync(restaurantOrderId);

        var restaurantOrderDetailProducts = await inventoryService.GetOrderDetailProductsByIdsAsync(restaurantOrderDetails.Select(rod => rod.ProductId).ToList());
        
        var productsMap = restaurantOrderDetailProducts.ToDictionary(p => p.ProductId, p => p);
        
        return restaurantOrderDetails.Select(rod => new RestaurantOrderDetailDto(
            ProductId: rod.ProductId,
            Name: productsMap[rod.ProductId].Name,
            UnitPrice: productsMap[rod.ProductId].SellPrice,
            Quantity: rod.Quantity,
            Note: rod.Note,
            RestaurantOrderDetailId: rod.Id,
            SentAt: rod.SentAt?.ToString("o"),
            RestaurantOrderStatusId: (int)rod.Status,
            RestaurantOrderStatus: rod.Status.ToString(),
            ResendCount: rod.ResendCount
        )).ToList();
    }
}