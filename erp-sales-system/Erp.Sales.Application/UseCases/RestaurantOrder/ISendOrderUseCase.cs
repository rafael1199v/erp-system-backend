namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public interface ISendOrderUseCase
{
    Task ExecuteAsync(int restaurantOrderId);
}