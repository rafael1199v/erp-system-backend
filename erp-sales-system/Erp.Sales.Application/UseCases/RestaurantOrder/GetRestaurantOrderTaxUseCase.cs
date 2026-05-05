using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class GetRestaurantOrderTaxUseCase(IRestaurantOrderRepository restaurantOrderRepository)
    : IGetRestaurantOrderTaxUseCase
{
    public async Task<decimal> ExecuteAsync(int restaurantOrderDetailId)
    {
        return await restaurantOrderRepository.GetTaxAmount(restaurantOrderDetailId);
    }
}