using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace Erp.Inventory.Infrastructure.Persistance.Services;

public class InventoryCenResolver(AppDbContext context) : IInventoryCenResolver
{
    public async Task<CenLookup?> ResolveCompanyAsync(string companyCen)
    {
        string? normalizedCen = NormalizeCen(companyCen);
        if (normalizedCen is null)
        {
            return null;
        }

        return await context.Companies
            .AsNoTracking()
            .Where(company => !company.IsDeleted && company.Cen == normalizedCen)
            .Select(company => new CenLookup(company.Id, company.Cen, company.Name))
            .FirstOrDefaultAsync();
    }

    public async Task<CenLookup?> ResolveProductAsync(int companyId, string productCen)
    {
        string? normalizedCen = NormalizeCen(productCen);
        if (normalizedCen is null)
        {
            return null;
        }

        return await context.Products
            .AsNoTracking()
            .Where(product => product.CompanyId == companyId
                              && !product.IsDeleted
                              && (product.Cen == normalizedCen || product.Sku == normalizedCen))
            .Select(product => new CenLookup(product.Id, product.Cen, product.CoreProduct.Name))
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyDictionary<string, CenLookup>> ResolveProductsAsync(int companyId, IEnumerable<string> productCens)
    {
        List<string> normalizedCens = productCens
            .Select(NormalizeCen)
            .OfType<string>()
            .Distinct()
            .ToList();

        if (normalizedCens.Count == 0)
        {
            return new Dictionary<string, CenLookup>();
        }

        var products = await context.Products
            .AsNoTracking()
            .Where(product => product.CompanyId == companyId
                              && !product.IsDeleted
                              && (normalizedCens.Contains(product.Cen)
                                  || (product.Sku != null && normalizedCens.Contains(product.Sku))))
            .Select(product => new
            {
                product.Id,
                product.Cen,
                product.Sku,
                Name = product.CoreProduct.Name
            })
            .ToListAsync();

        var resolvedProducts = new Dictionary<string, CenLookup>();
        foreach (string requestedCen in normalizedCens)
        {
            var product = products.FirstOrDefault(product => product.Cen == requestedCen || product.Sku == requestedCen);
            if (product is null)
            {
                continue;
            }

            resolvedProducts[requestedCen] = new CenLookup(product.Id, product.Cen, product.Name);
        }

        return resolvedProducts;
    }

    public async Task<CenLookup?> ResolveWarehouseAsync(int companyId, string warehouseCen)
    {
        string? normalizedCen = NormalizeCen(warehouseCen);
        if (normalizedCen is null)
        {
            return null;
        }

        return await context.Warehouses
            .AsNoTracking()
            .Where(warehouse => warehouse.CompanyId == companyId
                                && !warehouse.IsDeleted
                                && warehouse.Cen == normalizedCen)
            .Select(warehouse => new CenLookup(warehouse.Id, warehouse.Cen, warehouse.Name))
            .FirstOrDefaultAsync();
    }

    public async Task<CenLookup?> ResolveCategoryAsync(int companyId, string categoryCen)
    {
        string? normalizedCen = NormalizeCen(categoryCen);
        if (normalizedCen is null)
        {
            return null;
        }

        return await context.Categories
            .AsNoTracking()
            .Where(category => category.CompanyId == companyId
                               && !category.IsDeleted
                               && category.Cen == normalizedCen)
            .Select(category => new CenLookup(category.Id, category.Cen, category.Name))
            .FirstOrDefaultAsync();
    }

    public async Task<CenLookup?> ResolveUnitAsync(int companyId, string unitCen)
    {
        string? normalizedCen = NormalizeCen(unitCen);
        if (normalizedCen is null)
        {
            return null;
        }

        return await context.Units
            .AsNoTracking()
            .Where(unit => unit.CompanyId == companyId
                           && !unit.IsDeleted
                           && unit.Cen == normalizedCen)
            .Select(unit => new CenLookup(unit.Id, unit.Cen, unit.Name))
            .FirstOrDefaultAsync();
    }

    private static string? NormalizeCen(string cen)
    {
        string normalizedCen = cen.Trim();
        return string.IsNullOrWhiteSpace(normalizedCen) ? null : normalizedCen;
    }
}
