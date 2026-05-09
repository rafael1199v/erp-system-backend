using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;
using Erp.Sales.Domain.Enums;
using Erp.Sales.Infrastructure.Models;
using Erp.Sales.Infrastructure.Models.PoS;
using Microsoft.EntityFrameworkCore;

namespace Erp.Sales.Infrastructure.Repositories;

public class RestaurantOrderRepository(SalesDbContext salesDbContext) : IRestaurantOrderRepository
{
    private static readonly TimeZoneInfo BoliviaTimeZone = ResolveBoliviaTimeZone();

    public async Task CancelOrderAsync(int restaurantOrderId)
    {
        await using var transaction = await salesDbContext.Database.BeginTransactionAsync();

        try
        {
            var restaurantOrder = await Queryable
                .Where<RestaurantOrderModel>(salesDbContext.RestaurantOrders
                    .Include(ro => ro.Order), ro => ro.Id == restaurantOrderId && !ro.IsDeleted && !ro.Order.IsDeleted)
                .FirstOrDefaultAsync() ?? throw new KeyNotFoundException("La orden del restaurante no existe");

            if (restaurantOrder.Order.OrderStatusId == (int)OrderStatus.Paid)
            {
                throw new InvalidOperationException("No se puede cancelar una orden pagada");
            }

            if (restaurantOrder.Order.OrderStatusId == (int)OrderStatus.Cancelled)
            {
                await transaction.CommitAsync();
                return;
            }

            restaurantOrder.Order.OrderStatusId = (int)OrderStatus.Cancelled;

            await salesDbContext.RestaurantOrderDetails
                .Where(rod => rod.RestaurantOrderId == restaurantOrderId
                              && !rod.IsDeleted
                              && rod.RestaurantOrderDetailStatusId != (int)OrderDetailStatus.Canceled)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(rod => rod.RestaurantOrderDetailStatusId, (int)OrderDetailStatus.Canceled));

            await salesDbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<RestaurantOrder?> GetByIdAsync(int restaurantOrderId)
    {
        var restaurantOrderModel = await Queryable
            .Where<RestaurantOrderModel>(salesDbContext.RestaurantOrders
                .Include(ro => ro.Order), ro => ro.Id == restaurantOrderId && !ro.IsDeleted && !ro.Order.IsDeleted)
            .FirstOrDefaultAsync();

        return restaurantOrderModel is null ? null : ToDomain(restaurantOrderModel);
    }

    public async Task<int> CreateAsync(RestaurantOrder restaurantOrder)
    {       
        await using var transaction = await salesDbContext.Database.BeginTransactionAsync();

        try
        {
            var (dayStartUtc, nextDayStartUtc) = GetCurrentLocalDayUtcBounds();

            var maxTicketNumber = await salesDbContext.Orders
                .Where(o => o.CompanyId == restaurantOrder.CompanyId 
                            && o.CreatedAt >= dayStartUtc
                            && o.CreatedAt < nextDayStartUtc)
                .MaxAsync(o => (int?)o.DailyNumber) ?? 0;
            
            var orderModel = new OrderModel
            {
                CompanyId = restaurantOrder.CompanyId,
                CompanyCen = restaurantOrder.CompanyCen,
                TaxPrice = restaurantOrder.TaxPrice,
                CustomerId = restaurantOrder.CustomerId,
                DailyNumber = maxTicketNumber + 1,
                OrderStatusId = (int)OrderStatus.Open
            };
            
            await salesDbContext.Orders.AddAsync(orderModel);
            await salesDbContext.SaveChangesAsync();
            
            var restaurantOrderModel = ToModel(restaurantOrder);
            restaurantOrderModel.OrderId = orderModel.Id;
            
            await salesDbContext.RestaurantOrders.AddAsync(restaurantOrderModel);
            await salesDbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return restaurantOrderModel.Id;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
        
    }

    public async Task<List<RestaurantOrder>> GetCurrentDayOrders(int companyId)
    {
        var (dayStartUtc, nextDayStartUtc) = GetCurrentLocalDayUtcBounds();

        var restaurantOrders = await Queryable
            .Where<RestaurantOrderModel>(salesDbContext.RestaurantOrders
                .Include(or => or.Order), or =>
                or.Order.CompanyId == companyId
                && or.Order.OrderDatetime >= dayStartUtc
                && or.Order.OrderDatetime < nextDayStartUtc)
            .ToListAsync();
        
        return Enumerable.ToList(restaurantOrders.Select(ToDomain));
    }

    public async Task AssignWaiter(int restaurantOrderId, int waiterId)
    {
        var restaurantOrder = await Queryable
            .Where<RestaurantOrderModel>(salesDbContext.RestaurantOrders, ro => ro.Id == restaurantOrderId && !ro.IsDeleted).FirstOrDefaultAsync();

        if (restaurantOrder is not null)
        {
            restaurantOrder?.WaiterId = waiterId;
            await salesDbContext.SaveChangesAsync();
        }
    }

    private static RestaurantOrderModel ToModel(RestaurantOrder entity)
    {
        return new RestaurantOrderModel
        {
            Id = entity.Id,
            Cen = string.IsNullOrWhiteSpace(entity.Cen) ? Guid.NewGuid().ToString() : entity.Cen,
            OrderId = entity.OrderId,
            WaiterId = entity.WaiterId,
        };
    }

    private static RestaurantOrder ToDomain(RestaurantOrderModel model)
    {
        //TODO: Create an composition relationship between order and restaurant order, so we don't need to repeat data in both of them.
        return new RestaurantOrder
        {
            Id = model.Id,
            Cen = model.Cen,
            OrderId = model.Id,
            TaxPrice = model.Order.TaxPrice,
            CompanyId = model.Order.CompanyId,
            CompanyCen = model.Order.CompanyCen,
            CustomerId = model.Order.CustomerId,
            WaiterId = model.WaiterId,
            OrderDatetime = model.Order.OrderDatetime,
            Order = new Order
            {
                Id = model.Order.Id,
                DailyNumber = model.Order.DailyNumber,
                OrderDateTime = model.Order.OrderDatetime,
                Status = (OrderStatus)model.Order.OrderStatusId,
                CompanyId = model.Order.CompanyId,
                CustomerId = model.Order.CustomerId,
                TaxPrice = model.Order.TaxPrice
            }
        };
    }

    public async Task<int?> GetCompanyId(int restaurantOrderId)
    {
        var resturantOrder = await Queryable
        .Where<RestaurantOrderModel>(salesDbContext.RestaurantOrders
            .Include(ro => ro.Order), ro => ro.Id == restaurantOrderId && !ro.IsDeleted)
        .FirstOrDefaultAsync();

        return resturantOrder?.Order.CompanyId;
    }

    public async Task<decimal> GetTaxAmount(int restaurantOrderId)
    {
        var taxAmount = await Queryable
            .Where<RestaurantOrderModel>(salesDbContext.RestaurantOrders, ro => ro.Id == restaurantOrderId && !ro.IsDeleted)
            .Select(ro => ro.Order.TaxPrice).FirstOrDefaultAsync();
        
        return taxAmount;
    }

    private static (DateTime dayStartUtc, DateTime nextDayStartUtc) GetCurrentLocalDayUtcBounds()
    {
        var utcNow = DateTime.UtcNow;
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, BoliviaTimeZone);
        var localDayStart = localNow.Date;

        var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(localDayStart, BoliviaTimeZone);
        var nextDayStartUtc = TimeZoneInfo.ConvertTimeToUtc(localDayStart.AddDays(1), BoliviaTimeZone);

        return (dayStartUtc, nextDayStartUtc);
    }

    private static TimeZoneInfo ResolveBoliviaTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/La_Paz");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
        }
    }
}
