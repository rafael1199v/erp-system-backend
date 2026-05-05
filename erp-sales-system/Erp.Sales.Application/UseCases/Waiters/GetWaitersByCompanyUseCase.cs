using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.Waiters;

public class GetWaitersByCompanyUseCase(IWaiterRepository waiterRepository) : IGetWaitersByCompanyUseCase
{
    public async Task<List<WaiterDto>> ExecuteAsync(int companyId)
    {
        var waiterEntities = await waiterRepository.GetWaitersByCompanyAsync(companyId);

        return waiterEntities.Select(w => new WaiterDto(Id: w.Id, Name: w.Name)).ToList();
    }
}