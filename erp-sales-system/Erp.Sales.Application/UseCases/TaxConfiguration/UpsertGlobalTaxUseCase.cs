using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;

namespace Erp.Sales.Application.UseCases.TaxConfiguration;

public class UpsertGlobalTaxUseCase(ITaxConfigurationRepository taxConfigurationRepository)
    : IUpsertGlobalTaxUseCase
{
    public async Task<decimal> ExecuteAsync(UpsertGlobalTaxDto upsertGlobalTaxDto)
    {
        GlobalTaxConfiguration taxConfiguration = GlobalTaxConfiguration.Create(
            companyId: upsertGlobalTaxDto.CompanyId,
            globalTaxPercentage: upsertGlobalTaxDto.GlobalTaxPercentage,
            companyCen: upsertGlobalTaxDto.CompanyCen);

        await taxConfigurationRepository.UpsertGlobalTaxAsync(taxConfiguration);
        return taxConfiguration.GlobalTaxPercentage;
    }
}
