namespace Erp.Sales.Application.Interfaces;

public interface IDashboardRepository
{
    Task<(decimal TotalSales, int TicketsCount)> GetDailySalesSummaryAsync(int companyId, DateTime fromUtc, DateTime toUtc);
    Task<List<(int ProductId, int TotalQuantity)>> GetTopSoldProductsAsync(int companyId, DateTime fromUtc, DateTime toUtc,
        int topN);
}