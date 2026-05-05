using Erp.Sales.Domain.Entities;

namespace Erp.Sales.Application.Interfaces;

public interface ITaxConfigurationRepository
{
    Task CreateGlobalTaxAsync(GlobalTaxConfiguration globalTaxConfiguration);
    Task UpdateGlobalTaxAsync(GlobalTaxConfiguration globalTaxConfiguration);
    Task<GlobalTaxConfiguration> GetGlobalTaxAsync(int companyId);
}