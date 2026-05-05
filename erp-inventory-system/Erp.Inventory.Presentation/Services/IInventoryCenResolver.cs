namespace Erp.Inventory.Presentation.Services;

public interface IInventoryCenResolver
{
    Task<int?> ResolveCompanyIdAsync(string companyCen);
    Task<int?> ResolveProductIdAsync(int companyId, string productCen);
    Task<int?> ResolveCategoryIdAsync(int companyId, string categoryCen);
    Task<int?> ResolveUnitIdAsync(int companyId, string unitCen);
    Task<int?> ResolveWarehouseIdAsync(int companyId, string warehouseCen);
}
