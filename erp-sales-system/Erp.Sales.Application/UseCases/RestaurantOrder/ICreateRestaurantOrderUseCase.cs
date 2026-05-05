using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public interface ICreateRestaurantOrderUseCase
{
    Task<int> ExecuteAsync(CreateRestaurantOrderDto createRestaurantOrderDto);
}