using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.Dashboard;

public class GetDailySalesDashboardUseCase(IDashboardRepository dashboardRepository) : IGetDailySalesDashboardUseCase
{
    public async Task<DailySalesDashboardDto> ExecuteAsync(int companyId)
    {
        var (startUtc, endUtc) = BoliviaDateRangeHelper.GetCurrentDayUtcRange();

        var (totalSales, ticketsCount) =
            await dashboardRepository.GetDailySalesSummaryAsync(companyId, startUtc, endUtc);

        if (ticketsCount == 0)
        {
            return new DailySalesDashboardDto(0, 0, 0);
        }

        decimal averageTicket = totalSales / ticketsCount;

        return new DailySalesDashboardDto(totalSales, ticketsCount, averageTicket);
    }
}