using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public interface ICancelRestaurantOrderUseCase
{
    Task ExecuteAsync(CancelRestaurantOrderDto cancelRestaurantOrderDto);
}
