using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;

namespace Erp.Sales.Application.UseCases.TaxConfiguration;

public class CreateGlobalTaxUseCase(ITaxConfigurationRepository taxConfigurationRepository) : ICreateGlobalTaxUseCase
{
    public async Task ExecuteAsync(CreateGlobalTaxDto createGlobalTaxDto)
    {
        var taxConfiguration = GlobalTaxConfiguration.Create(
            companyId: createGlobalTaxDto.CompanyId, globalTaxPercentage: createGlobalTaxDto.GlobalTaxPercentage);
        
        await taxConfigurationRepository.CreateGlobalTaxAsync(taxConfiguration);
    }
}