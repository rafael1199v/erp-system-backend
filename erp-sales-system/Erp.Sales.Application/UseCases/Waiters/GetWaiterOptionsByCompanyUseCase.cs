using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;

namespace Erp.Sales.Application.UseCases.Waiters;

public class GetWaiterOptionsByCompanyUseCase(IWaiterRepository waiterRepository)
    : IGetWaiterOptionsByCompanyUseCase
{
    public async Task<List<WaiterOptionDto>> ExecuteAsync(int companyId)
    {
        List<Waiter> waiters = await waiterRepository.GetWaitersByCompanyAsync(companyId);
        return waiters
            .Where(waiter => !string.IsNullOrWhiteSpace(waiter.Cen))
            .Select(waiter => new WaiterOptionDto(waiter.Cen, waiter.Name))
            .ToList();
    }
}
