using Erp.Sales.Application.Interfaces;
using Erp.Sales.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Sales.Infrastructure.Repositories;

public class WarehouseConfigurationRepository(SalesDbContext salesDbContext) : IWarehouseConfigurationRepository
{
    public async Task<int?> GetWarehouseIdByCompanyIdAsync(int companyId)
    {
        return await salesDbContext.WarehouseConfigurations
            .Where<WarehouseConfigurationModel>(wc => wc.CompanyId == companyId && !wc.IsDeleted)
            .Select(wc => (int?)wc.MainWarehouseId)
            .FirstOrDefaultAsync();
    }

    public async Task<string?> GetWarehouseCenByCompanyIdAsync(int companyId)
    {
        return await salesDbContext.WarehouseConfigurations
            .Where<WarehouseConfigurationModel>(wc => wc.CompanyId == companyId && !wc.IsDeleted)
            .Select(wc => wc.MainWarehouseCen)
            .FirstOrDefaultAsync();
    }
}
