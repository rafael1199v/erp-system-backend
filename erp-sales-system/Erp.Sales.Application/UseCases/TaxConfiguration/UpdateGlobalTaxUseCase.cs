using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;

namespace Erp.Sales.Application.UseCases.TaxConfiguration;

public class UpdateGlobalTaxUseCase(ITaxConfigurationRepository taxConfigurationRepository) : IUpdateGlobalTaxUseCase
{
    public async Task ExecuteAsync(UpdateGlobalTaxDto updateGlobalTaxDto)
    {
        var taxConfiguration = GlobalTaxConfiguration.Create(
            companyId: updateGlobalTaxDto.CompanyId, globalTaxPercentage: updateGlobalTaxDto.GlobalTaxPercentage);
        
        await taxConfigurationRepository.UpdateGlobalTaxAsync(taxConfiguration);
    }
}