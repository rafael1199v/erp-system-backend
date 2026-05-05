namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public interface IGetRestaurantOrderTaxUseCase
{
    Task<decimal> ExecuteAsync(int restaurantOrderDetailId);
}