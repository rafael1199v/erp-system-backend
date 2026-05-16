using Erp.Inventory.Contracts;
using Erp.Sales.Application.ContractDtos;
using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.ContractServices;

public class SalesDashboardContractService(
    IDashboardRepository dashboardRepository,
    IInventoryService inventoryService)
    : ISalesDashboardContractService
{
    private const int DefaultTopN = 10;
    private const int MaxTopN = 100;
    private const string BoliviaIanaTimeZoneId = "America/La_Paz";
    private const string BoliviaWindowsTimeZoneId = "SA Western Standard Time";

    public async Task<List<TopProductDashboardContractResponse>> GetTopProductsAsync(
        int companyId,
        string companyCen,
        int topN)
    {
        var (startUtc, endUtc) = GetCurrentBoliviaDayUtcRange();
        int normalizedTopN = NormalizeTopN(topN);

        var topProducts = await dashboardRepository.GetTopSoldProductReferencesAsync(
            companyId,
            startUtc,
            endUtc,
            normalizedTopN);

        if (topProducts.Count == 0)
        {
            return [];
        }

        Dictionary<string, ProductContractDto> productsByCen = await GetProductsByCenAsync(companyCen);

        return topProducts.Select(product =>
        {
            ProductContractDto? productContract = null;
            if (!string.IsNullOrWhiteSpace(product.ProductCen))
            {
                productsByCen.TryGetValue(product.ProductCen, out productContract);
            }

            return new TopProductDashboardContractResponse
            {
                ProductCen = product.ProductCen,
                ProductName = productContract?.Name ?? "Producto sin CEN",
                TotalQuantity = product.TotalQuantity,
                CategoryCen = productContract?.CategoryCen,
                CategoryName = productContract?.CategoryName,
                SalePrice = productContract?.SalePrice ?? 0
            };
        }).ToList();
    }

    private async Task<Dictionary<string, ProductContractDto>> GetProductsByCenAsync(string companyCen)
    {
        List<ProductContractDto> products = await inventoryService.GetProductsAsync(companyCen);
        return products.ToDictionary(product => product.ProductCen, StringComparer.OrdinalIgnoreCase);
    }

    private static int NormalizeTopN(int topN)
    {
        if (topN <= 0)
        {
            return DefaultTopN;
        }

        return topN > MaxTopN ? MaxTopN : topN;
    }

    private static (DateTime StartUtc, DateTime EndUtc) GetCurrentBoliviaDayUtcRange()
    {
        TimeZoneInfo boliviaTimeZone = ResolveBoliviaTimeZone();
        DateTime boliviaNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, boliviaTimeZone);
        DateTime boliviaDayStart = boliviaNow.Date;
        DateTime boliviaNextDayStart = boliviaDayStart.AddDays(1);

        DateTime dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(boliviaDayStart, boliviaTimeZone);
        DateTime nextDayStartUtc = TimeZoneInfo.ConvertTimeToUtc(boliviaNextDayStart, boliviaTimeZone);

        return (dayStartUtc, nextDayStartUtc);
    }

    private static TimeZoneInfo ResolveBoliviaTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(BoliviaIanaTimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(BoliviaWindowsTimeZoneId);
        }
    }
}
