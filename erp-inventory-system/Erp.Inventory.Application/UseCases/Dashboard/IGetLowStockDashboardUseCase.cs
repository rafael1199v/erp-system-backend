using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Dashboard;

public interface IGetLowStockDashboardUseCase
{
    Task<List<LowStockDashboardDto>> ExecuteAsync(int companyId);
}