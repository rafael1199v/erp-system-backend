namespace Erp.Inventory.Application.Interfaces;

public sealed record CenLookup(int Id, string Cen, string Name);

public interface IInventoryCenResolver
{
    Task<CenLookup?> ResolveCompanyAsync(string companyCen);
    Task<CenLookup?> ResolveProductAsync(int companyId, string productCen);
    Task<IReadOnlyDictionary<string, CenLookup>> ResolveProductsAsync(int companyId, IEnumerable<string> productCens);
    Task<CenLookup?> ResolveWarehouseAsync(int companyId, string warehouseCen);
    Task<CenLookup?> ResolveCategoryAsync(int companyId, string categoryCen);
    Task<CenLookup?> ResolveUnitAsync(int companyId, string unitCen);
}
