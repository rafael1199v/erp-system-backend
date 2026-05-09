using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;
using Erp.Sales.Domain.Enums;
using Erp.Sales.Infrastructure.Models.PoS;
using Microsoft.EntityFrameworkCore;

namespace Erp.Sales.Infrastructure.Repositories;

public class RestaurantOrderDetailRepository(SalesDbContext salesDbContext) : IRestaurantOrderDetailRepository
{
	public async Task<bool> RestaurantOrderExistsAndOpenAsync(int restaurantOrderId)
	{
		return await salesDbContext.RestaurantOrders
			.Include(ro => ro.Order)
			.AnyAsync(ro =>
				ro.Id == restaurantOrderId &&
				!ro.IsDeleted &&
				!ro.Order.IsDeleted &&
				ro.Order.OrderStatusId == (int)OrderStatus.Open);
	}

	public async Task<int> CreateAsync(RestaurantOrderDetail restaurantOrderDetail)
	{
		var model = ToModel(restaurantOrderDetail);

		await salesDbContext.RestaurantOrderDetails.AddAsync(model);
		await salesDbContext.SaveChangesAsync();

		return model.Id;
	}

	public async Task<RestaurantOrderDetail?> GetByIdAsync(int restaurantOrderDetailId)
	{
		var model = await Queryable
			.Where<RestaurantOrderDetailModel>(salesDbContext.RestaurantOrderDetails, rod => rod.Id == restaurantOrderDetailId && !rod.IsDeleted)
			.FirstOrDefaultAsync();

		return model is null ? null : ToDomain(model);
	}

	public async Task<RestaurantOrderDetail?> GetByCompanyAndCenAsync(string companyCen, string ticketItemCen)
	{
		var normalizedCompanyCen = companyCen.Trim();
		var normalizedTicketItemCen = ticketItemCen.Trim();

		var model = await salesDbContext.RestaurantOrderDetails
			.AsNoTracking()
			.Include(rod => rod.RestaurantOrder)
			.ThenInclude(restaurantOrder => restaurantOrder.Order)
			.FirstOrDefaultAsync(rod =>
				rod.Cen == normalizedTicketItemCen &&
				!rod.IsDeleted &&
				!rod.RestaurantOrder.IsDeleted &&
				!rod.RestaurantOrder.Order.IsDeleted &&
				rod.RestaurantOrder.Order.CompanyCen == normalizedCompanyCen);

		return model is null ? null : ToDomain(model);
	}

	public async Task UpdateQuantityAsync(int restaurantOrderDetailId, int quantity, string? note)
	{
        var model = await Queryable
            .Where<RestaurantOrderDetailModel>(salesDbContext.RestaurantOrderDetails, rod => rod.Id == restaurantOrderDetailId && !rod.IsDeleted)
            .FirstOrDefaultAsync() ?? throw new KeyNotFoundException("El detalle de la orden no existe.");

        model.Quantity = quantity;
        model.Note = note;
		await salesDbContext.SaveChangesAsync();
	}

	public async Task<List<RestaurantOrderDetail>> GetByRestaurantOrderIdAsync(int restaurantOrderId)
	{
		var restaurantOrderDetails = await Queryable
			.Where<RestaurantOrderDetailModel>(salesDbContext.RestaurantOrderDetails
				.AsNoTracking(), rod => rod.RestaurantOrderId == restaurantOrderId && !rod.IsDeleted)
			.ToListAsync();

		return Enumerable.ToList(restaurantOrderDetails.Select(ToDomain));
	}

	public async Task UpdateRangeAsync(List<RestaurantOrderDetail> restaurantOrderDetails)
	{
		var restaurantOrderDetailModels = restaurantOrderDetails.Select(ToModel).ToList();
		salesDbContext.RestaurantOrderDetails.UpdateRange(restaurantOrderDetailModels);
		
		await salesDbContext.SaveChangesAsync();
	}

	public async Task ResendOrderDetailAsync(int restaurantOrderDetailId)
	{
		var restaurantOrderDetailModel = await Queryable
			.Where<RestaurantOrderDetailModel>(salesDbContext.RestaurantOrderDetails, rod => rod.Id == restaurantOrderDetailId && !rod.IsDeleted)
			.FirstOrDefaultAsync() ?? throw new KeyNotFoundException("El detalle de la orden no existe.");
		
		if(restaurantOrderDetailModel.ResendCount >= 1000)
			throw new Exception("No puedes exceder el limite de 1000 reenvios");
		
		restaurantOrderDetailModel.ResendCount += 1;
		await salesDbContext.SaveChangesAsync();
	}

	private static RestaurantOrderDetailModel ToModel(RestaurantOrderDetail restaurantOrderDetail)
	{
		return new RestaurantOrderDetailModel
		{
			Id = restaurantOrderDetail.Id,
			Cen = string.IsNullOrWhiteSpace(restaurantOrderDetail.Cen)
				? Guid.NewGuid().ToString()
				: restaurantOrderDetail.Cen,
			RestaurantOrderId = restaurantOrderDetail.RestaurantOrderId,
			ProductId = restaurantOrderDetail.ProductId,
			ProductCen = restaurantOrderDetail.ProductCen,
			RestaurantOrderDetailStatusId = (int)restaurantOrderDetail.Status,
			Note = restaurantOrderDetail.Note,
			Quantity = restaurantOrderDetail.Quantity,
			SentAt = restaurantOrderDetail.SentAt,
			CreatedAt = restaurantOrderDetail.CreatedAt
		};
	}

	private static RestaurantOrderDetail ToDomain(RestaurantOrderDetailModel model)
	{
		return new RestaurantOrderDetail
		{
			Id = model.Id,
			Cen = model.Cen,
			RestaurantOrderId = model.RestaurantOrderId,
			TicketCen = model.RestaurantOrder?.Cen ?? string.Empty,
			ProductId = model.ProductId,
			ProductCen = model.ProductCen,
			Status = (OrderDetailStatus)model.RestaurantOrderDetailStatusId,
			Note = model.Note,
			Quantity = model.Quantity,
			SentAt = model.SentAt,
			CreatedAt = model.CreatedAt,
			ResendCount = model.ResendCount
		};
	}
}
