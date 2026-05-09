using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;
using Erp.Sales.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Sales.Infrastructure.Repositories;

public class TaxConfigurationRepository(SalesDbContext salesDbContext) : ITaxConfigurationRepository
{
    public async Task CreateGlobalTaxAsync(GlobalTaxConfiguration globalTaxConfiguration)
    {
        TaxConfigurationModel taxConfigurationModel = ToModel(globalTaxConfiguration);
        await salesDbContext.TaxConfigurations.AddAsync(taxConfigurationModel);

        await salesDbContext.SaveChangesAsync();
    }

    public async Task UpdateGlobalTaxAsync(GlobalTaxConfiguration globalTaxConfiguration)
    {
        TaxConfigurationModel? taxConfigurationModel =
            await salesDbContext.TaxConfigurations.FindAsync(globalTaxConfiguration.CompanyId);

        if (taxConfigurationModel is null)
        {
            throw new Exception("El impuesto global no pudo ser actualizado");
        }

        taxConfigurationModel.GlobalTaxPercentage = globalTaxConfiguration.GlobalTaxPercentage;
        if (!string.IsNullOrWhiteSpace(globalTaxConfiguration.CompanyCen))
        {
            taxConfigurationModel.CompanyCen = globalTaxConfiguration.CompanyCen;
        }

        await salesDbContext.SaveChangesAsync();
    }

    public async Task<GlobalTaxConfiguration> GetGlobalTaxAsync(int companyId)
    {
        TaxConfigurationModel? taxConfigurationModel = await salesDbContext.TaxConfigurations.FindAsync(companyId);
        return taxConfigurationModel is null
            ? throw new Exception("No se encontro la configuracion de impuesto global para la empresa especificada")
            : ToDomain(taxConfigurationModel);
    }

    public async Task<GlobalTaxConfiguration?> FindGlobalTaxAsync(int companyId)
    {
        TaxConfigurationModel? taxConfigurationModel = await salesDbContext.TaxConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(tax => tax.CompanyId == companyId && !tax.IsDeleted);

        return taxConfigurationModel is null ? null : ToDomain(taxConfigurationModel);
    }

    public async Task UpsertGlobalTaxAsync(GlobalTaxConfiguration globalTaxConfiguration)
    {
        TaxConfigurationModel? taxConfigurationModel = await salesDbContext.TaxConfigurations
            .FirstOrDefaultAsync(tax => tax.CompanyId == globalTaxConfiguration.CompanyId);

        if (taxConfigurationModel is null)
        {
            await salesDbContext.TaxConfigurations.AddAsync(ToModel(globalTaxConfiguration));
        }
        else
        {
            taxConfigurationModel.GlobalTaxPercentage = globalTaxConfiguration.GlobalTaxPercentage;
            taxConfigurationModel.CompanyCen = globalTaxConfiguration.CompanyCen;
            taxConfigurationModel.IsDeleted = false;
        }

        await salesDbContext.SaveChangesAsync();
    }

    private static TaxConfigurationModel ToModel(GlobalTaxConfiguration globalTaxConfiguration)
    {
        return new TaxConfigurationModel
        {
            CompanyId = globalTaxConfiguration.CompanyId,
            CompanyCen = globalTaxConfiguration.CompanyCen,
            GlobalTaxPercentage = globalTaxConfiguration.GlobalTaxPercentage
        };
    }

    public static GlobalTaxConfiguration ToDomain(TaxConfigurationModel model)
    {
        return new GlobalTaxConfiguration
        {
            CompanyId = model.CompanyId,
            CompanyCen = model.CompanyCen,
            GlobalTaxPercentage = model.GlobalTaxPercentage
        };
    }
}
