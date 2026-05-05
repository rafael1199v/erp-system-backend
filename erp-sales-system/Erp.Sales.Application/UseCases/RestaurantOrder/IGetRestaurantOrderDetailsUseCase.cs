using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public interface IGetRestaurantOrderDetailsUseCase
{
    Task<List<RestaurantOrderDetailDto>> ExecuteAsync(int restaurantOrderId);
}