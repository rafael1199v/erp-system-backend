namespace Erp.Sales.Application.UseCases.RestaurantOrderDetails;

public interface IResendOrderDetailUseCase
{
    Task ExecuteAsync(int restaurantOrderDetailId); 
}