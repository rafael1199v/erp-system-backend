using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.Dashboard;

public interface IGetTopProductsDashboardUseCase
{
    Task<List<TopProductDashboardDto>> ExecuteAsync(int companyId, int topN);
}