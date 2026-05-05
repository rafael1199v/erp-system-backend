using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;
using Erp.Sales.Infrastructure.Models;

namespace Erp.Sales.Infrastructure.Repositories;

public class TaxConfigurationRepository(SalesDbContext salesDbContext) : ITaxConfigurationRepository
{
    public async Task CreateGlobalTaxAsync(GlobalTaxConfiguration globalTaxConfiguration)
    {
        var taxConfigurationModel = ToModel(globalTaxConfiguration);
        await salesDbContext.TaxConfigurations.AddAsync(taxConfigurationModel);
        
        await salesDbContext.SaveChangesAsync();
    }

    public async Task UpdateGlobalTaxAsync(GlobalTaxConfiguration globalTaxConfiguration)
    {
        var taxConfigurationModel = await salesDbContext.TaxConfigurations.FindAsync(globalTaxConfiguration.CompanyId);

        if (taxConfigurationModel == null)
        {
            throw new Exception("El impuesto global no pudo ser actualizado");
        }
        
        taxConfigurationModel.GlobalTaxPercentage = globalTaxConfiguration.GlobalTaxPercentage;
        await salesDbContext.SaveChangesAsync();
    }

    public async Task<GlobalTaxConfiguration> GetGlobalTaxAsync(int companyId)
    {
        var taxConfigurationModel = await salesDbContext.TaxConfigurations.FindAsync(companyId);
        return taxConfigurationModel == null ? throw new Exception("No se encontró la configuración de impuesto global para la empresa especificada") : ToDomain(taxConfigurationModel);
    }

    private static TaxConfigurationModel ToModel(GlobalTaxConfiguration globalTaxConfiguration)
    {
        return new TaxConfigurationModel
        {
            CompanyId = globalTaxConfiguration.CompanyId,
            GlobalTaxPercentage = globalTaxConfiguration.GlobalTaxPercentage
        };
    }

    public static GlobalTaxConfiguration ToDomain(TaxConfigurationModel model)
    {
        return new GlobalTaxConfiguration
        {
            CompanyId = model.CompanyId,
            GlobalTaxPercentage = model.GlobalTaxPercentage
        };
    }
}