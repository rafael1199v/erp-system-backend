using Erp.Inventory.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace Erp.Inventory.Presentation.Services;

public class InventoryCenResolver(AppDbContext context) : IInventoryCenResolver
{
    public async Task<int?> ResolveCompanyIdAsync(string companyCen)
    {
        return await context.Companies
            .Where(c => c.Cen == companyCen && !c.IsDeleted)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<int?> ResolveProductIdAsync(int companyId, string productCen)
    {
        return await context.Products
            .Where(p => p.CompanyId == companyId
                        && !p.IsDeleted
                        && (p.Cen == productCen || p.Sku == productCen))
            .Select(p => (int?)p.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<int?> ResolveCategoryIdAsync(int companyId, string categoryCen)
    {
        return await context.Categories
            .Where(c => c.CompanyId == companyId && c.Cen == categoryCen && !c.IsDeleted)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<int?> ResolveUnitIdAsync(int companyId, string unitCen)
    {
        return await context.Units
            .Where(u => u.CompanyId == companyId && u.Cen == unitCen && !u.IsDeleted)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<int?> ResolveWarehouseIdAsync(int companyId, string warehouseCen)
    {
        return await context.Warehouses
            .Where(w => w.CompanyId == companyId && w.Cen == warehouseCen && !w.IsDeleted)
            .Select(w => (int?)w.Id)
            .FirstOrDefaultAsync();
    }
}
