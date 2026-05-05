namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public interface IPrintRestaurantOrderUseCase
{
    Task<byte[]> ExecuteAsync(int restaurantOrderId);
}