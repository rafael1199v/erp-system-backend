using Erp.Inventory.Contracts;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public interface IGetOrderDetailProductsUseCase
{
    Task<List<RestaurantOrderProductDto>> ExecuteAsync(int restaurantOrderId);
}
