using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;
using Erp.Sales.Domain.Enums;
using Erp.Sales.Infrastructure.Models;
using Erp.Sales.Infrastructure.Models.PoS;
using Microsoft.EntityFrameworkCore;

namespace Erp.Sales.Infrastructure.Repositories;

public class PaymentProcessRepository(SalesDbContext salesDbContext) : IPaymentProcessRepository
{
    public async Task<SalesCenLookup> CreateSaleAndCloseOrderAsync(int restaurantOrderId, Sale sale)
    {
        await using var transaction = await salesDbContext.Database.BeginTransactionAsync();

        try
        {
            var restaurantOrder = await Queryable
                .Where<RestaurantOrderModel>(salesDbContext.RestaurantOrders
                    .Include(ro => ro.Order), ro => ro.Id == restaurantOrderId && !ro.IsDeleted && !ro.Order.IsDeleted)
                .FirstOrDefaultAsync() ?? throw new KeyNotFoundException("La orden del restaurante no existe");

            var saleModel = new SaleModel
            {
                Cen = string.IsNullOrWhiteSpace(sale.Cen) ? Guid.NewGuid().ToString() : sale.Cen,
                SubtotalPrice = sale.SubtotalPrice,
                TaxPrice = sale.TaxPrice,
                DiscountPercentage = sale.DiscountPercentage,
                SaleDatetime = sale.SaleDatetime,
                CustomerId = sale.CustomerId,
                PaymentTypeId = sale.PaymentTypeId,
                CompanyId = sale.CompanyId,
                CompanyCen = sale.CompanyCen ?? string.Empty,
                SaleDetails = sale.SaleDetails.Select(detail => new SaleDetailModel
                {
                    ProductId = detail.ProductId,
                    ProductCen = detail.ProductCen ?? string.Empty,
                    Price = detail.Price,
                    Quantity = detail.Quantity
                }).ToList()
            };

            await salesDbContext.Sales.AddAsync(saleModel);
            restaurantOrder.Order.OrderStatusId = (int)OrderStatus.Paid;

            await salesDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new SalesCenLookup(saleModel.Id, saleModel.Cen);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
