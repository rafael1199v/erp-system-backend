using Erp.Sales.Domain.Entities;

namespace Erp.Sales.Application.Interfaces;

public interface IRestaurantOrderRepository
{
    Task<int> CreateAsync(RestaurantOrder restaurantOrder);
    Task<RestaurantOrder?> GetByIdAsync(int restaurantOrderId);
    Task CancelOrderAsync(int restaurantOrderId);
    Task<List<RestaurantOrder>> GetCurrentDayOrders(int companyId);
    Task AssignWaiter(int restaurantOrderId, int waiterId);
    Task<int?> GetCompanyId(int restaurantOrderId);
    Task<decimal> GetTaxAmount(int restaurantOrderId);
}