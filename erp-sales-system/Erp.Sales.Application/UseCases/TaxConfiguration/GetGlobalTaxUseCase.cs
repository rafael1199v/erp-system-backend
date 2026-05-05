using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.TaxConfiguration;

public class GetGlobalTaxUseCase(ITaxConfigurationRepository taxConfigurationRepository) : IGetGlobalTaxUseCase
{
    public async Task<decimal> ExecuteAsync(int companyId)
    {
        var globalTax = await taxConfigurationRepository.GetGlobalTaxAsync(companyId);
        return globalTax.GlobalTaxPercentage;
    }
}