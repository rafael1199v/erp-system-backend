using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;

namespace Erp.Sales.Application.UseCases.TaxConfiguration;

public class GetTaxConfigurationUseCase(ITaxConfigurationRepository taxConfigurationRepository)
    : IGetTaxConfigurationUseCase
{
    public async Task<decimal> ExecuteAsync(int companyId)
    {
        GlobalTaxConfiguration? taxConfiguration = await taxConfigurationRepository.FindGlobalTaxAsync(companyId);
        return taxConfiguration?.GlobalTaxPercentage ?? 0;
    }
}
