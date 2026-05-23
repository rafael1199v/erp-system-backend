using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.Waiters;

public interface IGetWaiterOptionsByCompanyUseCase
{
    Task<List<WaiterOptionDto>> ExecuteAsync(int companyId);
}
