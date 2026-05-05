namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public interface ICreateRestaurantOrderDetailUseCase
{
	Task<int> ExecuteAsync(DTOs.CreateRestaurantOrderDetail createRestaurantOrderDetail);
}