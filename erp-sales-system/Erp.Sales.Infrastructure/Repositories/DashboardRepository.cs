using Erp.Sales.Application.Interfaces;
using Erp.Sales.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Sales.Infrastructure.Repositories;

public class DashboardRepository(SalesDbContext salesDbContext) : IDashboardRepository
{
    public async Task<(decimal TotalSales, int TicketsCount)> GetDailySalesSummaryAsync(int companyId, DateTime fromUtc,
        DateTime toUtc)
    {
        var filteredSalesQuery = salesDbContext.Sales
            .AsNoTracking()
            .Where<SaleModel>(s =>
                !s.IsDeleted &&
                s.CompanyId == companyId &&
                s.SaleDatetime >= fromUtc &&
                s.SaleDatetime < toUtc);

        int ticketsCount = await filteredSalesQuery.CountAsync();
        if (ticketsCount == 0)
        {
            return (0, 0);
        }

        decimal totalSales = await filteredSalesQuery.SumAsync(s => s.SubtotalPrice * (s.TaxPrice / 100 + 1));
        return (totalSales, ticketsCount);
    }

    public async Task<List<(int ProductId, int TotalQuantity)>> GetTopSoldProductsAsync(int companyId, DateTime fromUtc,
        DateTime toUtc, int topN)
    {
        var topProducts = await Queryable
            .Where<SaleModel>(salesDbContext.Sales
                .AsNoTracking(), s =>
                !s.IsDeleted &&
                s.CompanyId == companyId &&
                s.SaleDatetime >= fromUtc &&
                s.SaleDatetime < toUtc)
            .SelectMany(s => s.SaleDetails
                .Where(sd => !sd.IsDeleted)
                .Select(sd => new
                {
                    sd.ProductId,
                    sd.Quantity
                }))
            .GroupBy(sd => sd.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalQuantity = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.TotalQuantity)
            .Take(topN)
            .ToListAsync();

        return [.. Enumerable.Select(topProducts, tp => (tp.ProductId, tp.TotalQuantity))];
    }

    public async Task<List<(string ProductCen, int TotalQuantity)>> GetTopSoldProductReferencesAsync(
        int companyId,
        DateTime fromUtc,
        DateTime toUtc,
        int topN)
    {
        var topProducts = await Queryable
            .Where<SaleModel>(salesDbContext.Sales
                .AsNoTracking(), s =>
                !s.IsDeleted &&
                s.CompanyId == companyId &&
                s.SaleDatetime >= fromUtc &&
                s.SaleDatetime < toUtc)
            .SelectMany(s => s.SaleDetails
                .Where(sd => !sd.IsDeleted)
                .Select(sd => new
                {
                    sd.ProductCen,
                    sd.Quantity
                }))
            .Where(sd => sd.ProductCen != string.Empty)
            .GroupBy(sd => sd.ProductCen)
            .Select(g => new
            {
                ProductCen = g.Key,
                TotalQuantity = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.TotalQuantity)
            .Take(topN)
            .ToListAsync();

        return [.. topProducts.Select(tp => (tp.ProductCen, tp.TotalQuantity))];
    }
}
