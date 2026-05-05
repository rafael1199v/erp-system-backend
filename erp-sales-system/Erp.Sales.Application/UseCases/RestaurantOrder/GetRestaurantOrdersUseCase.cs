using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class GetRestaurantOrdersUseCase(IRestaurantOrderRepository restaurantOrderRepository) : IGetRestaurantOrdersUseCase
{
    public async Task<List<RestaurantOrderDto>> ExecuteAsync(int companyId)
    {
        List<Domain.Entities.RestaurantOrder> restaurantOrders =
            await restaurantOrderRepository.GetCurrentDayOrders(companyId);

        return restaurantOrders.Select(ro => new RestaurantOrderDto(
            Id: ro.Order.Id,
            DailyNumber: ro.Order.DailyNumber,
            OrderDatetime: ro.Order.OrderDateTime.ToString("o"),
            OrderStatusId: (int)ro.Order.Status,
            CustomerId: ro.Order.CustomerId,
            TaxPrice: ro.Order.TaxPrice,
            RestaurantOrderId: ro.Id,
            WaiterId: ro.WaiterId
        )).ToList();
    }
}