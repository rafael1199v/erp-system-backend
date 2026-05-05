using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.Waiters;

public interface IGetWaitersByCompanyUseCase
{
    Task<List<WaiterDto>> ExecuteAsync(int companyId);
}