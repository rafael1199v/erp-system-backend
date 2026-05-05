using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public interface IUpdateRestaurantOrderDetailQuantityUseCase
{
    Task ExecuteAsync(UpdateRestaurantOrderDetailQuantityDto updateRestaurantOrderDetailQuantityDto);
}