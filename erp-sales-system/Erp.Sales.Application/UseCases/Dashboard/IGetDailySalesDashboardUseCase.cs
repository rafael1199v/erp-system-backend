using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.Dashboard;

public interface IGetDailySalesDashboardUseCase
{
    Task<DailySalesDashboardDto> ExecuteAsync(int companyId);
}