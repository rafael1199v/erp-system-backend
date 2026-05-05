using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public interface IGetRestaurantOrdersUseCase
{
    Task<List<RestaurantOrderDto>> ExecuteAsync(int companyId);
}