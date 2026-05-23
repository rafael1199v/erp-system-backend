using Erp.Sales.Domain.Entities;

namespace Erp.Sales.Application.Interfaces;

public interface IRestaurantOrderDetailRepository
{
	Task<bool> RestaurantOrderExistsAndOpenAsync(int restaurantOrderId);
	Task<int> CreateAsync(RestaurantOrderDetail restaurantOrderDetail);
	Task<RestaurantOrderDetail?> GetByIdAsync(int restaurantOrderDetailId);
	Task<RestaurantOrderDetail?> GetByCompanyAndCenAsync(string companyCen, string ticketItemCen);
	Task UpdateQuantityAsync(int restaurantOrderDetailId, int quantity, string? note);
	Task<List<RestaurantOrderDetail>> GetByRestaurantOrderIdAsync(int restaurantOrderId);
	Task UpdateRangeAsync(List<RestaurantOrderDetail> restaurantOrderDetails);
	Task ResendOrderDetailAsync(int restaurantOrderDetailId);
}
