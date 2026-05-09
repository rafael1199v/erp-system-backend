using Erp.Inventory.Contracts;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Application.UseCases.Dashboard;
using Erp.Sales.Presentation.ContractDtos;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers.Contract;

[ApiController]
[Route("api/sales/companies/{companyCen}/dashboard")]
public class DashboardContractController(
    ISalesCenResolver salesCenResolver,
    IDashboardRepository dashboardRepository,
    IInventoryService inventoryService,
    IGetDailySalesDashboardUseCase getDailySalesDashboardUseCase,
    IGetKdsStatusDashboardUseCase getKdsStatusDashboardUseCase)
    : ControllerBase
{
    private const int DefaultTopN = 10;
    private const int MaxTopN = 100;
    private const string BoliviaIanaTimeZoneId = "America/La_Paz";
    private const string BoliviaWindowsTimeZoneId = "SA Western Standard Time";

    [HttpGet("daily-sales")]
    public async Task<IActionResult> GetDailySales(string companyCen)
    {
        int? companyId = await ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada" });
        }

        return Ok(await getDailySalesDashboardUseCase.ExecuteAsync(companyId.Value));
    }

    [HttpGet("top-products")]
    public async Task<IActionResult> GetTopProducts(string companyCen, [FromQuery] int topN = DefaultTopN)
    {
        int? companyId = await ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada" });
        }

        var (startUtc, endUtc) = GetCurrentBoliviaDayUtcRange();
        int normalizedTopN = NormalizeTopN(topN);

        var topProducts = await dashboardRepository.GetTopSoldProductReferencesAsync(
            companyId.Value,
            startUtc,
            endUtc,
            normalizedTopN);

        if (topProducts.Count == 0)
        {
            return Ok(new List<TopProductDashboardContractResponse>());
        }

        Dictionary<string, ProductContractDto> productsByCen = await GetProductsByCenAsync(companyCen);

        return Ok(topProducts.Select(product =>
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
        }).ToList());
    }

    [HttpGet("kds-status")]
    public async Task<IActionResult> GetKdsStatus(string companyCen)
    {
        int? companyId = await ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada" });
        }

        return Ok(await getKdsStatusDashboardUseCase.ExecuteAsync(companyId.Value));
    }

    private async Task<int?> ResolveCompanyIdAsync(string companyCen)
    {
        return await salesCenResolver.ResolveCompanyIdAsync(companyCen);
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
