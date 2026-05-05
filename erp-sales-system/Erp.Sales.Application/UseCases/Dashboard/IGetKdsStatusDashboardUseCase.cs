using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.Dashboard;

public interface IGetKdsStatusDashboardUseCase
{
    Task<KdsStatusDashboardDto> ExecuteAsync(int companyId);
}