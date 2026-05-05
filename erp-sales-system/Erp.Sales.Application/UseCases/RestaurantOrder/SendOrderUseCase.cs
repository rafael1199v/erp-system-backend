using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class SendOrderUseCase(IRestaurantOrderDetailRepository restaurantOrderDetailRepository) : ISendOrderUseCase
{
    public async Task ExecuteAsync(int restaurantOrderId)
    {
        var restaurantOrderDetails = await restaurantOrderDetailRepository.GetByRestaurantOrderIdAsync(restaurantOrderId);
        var restaurantOrderDetailsWithoutSend = restaurantOrderDetails.Where(rod => rod.SentAt is null).ToList();
        
        foreach (var restaurantOrderDetail in restaurantOrderDetailsWithoutSend)
        {
            restaurantOrderDetail.SendDetail();
        }
        
        await restaurantOrderDetailRepository.UpdateRangeAsync(restaurantOrderDetailsWithoutSend);
    }
}