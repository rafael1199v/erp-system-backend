using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.RestaurantOrderDetails;

public class ResendOrderDetailUseCase(IRestaurantOrderDetailRepository restaurantOrderDetailRepository) : IResendOrderDetailUseCase
{
    public async Task ExecuteAsync(int restaurantOrderDetailId)
    {
        await restaurantOrderDetailRepository.ResendOrderDetailAsync(restaurantOrderDetailId);
    }
}