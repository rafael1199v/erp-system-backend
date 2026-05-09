using Erp.Inventory.Contracts;
using Erp.Inventory.Presentation.ContractDtos;

namespace Erp.Inventory.Presentation.ContractAdapters;

public interface IInventoryCatalogContractAdapter
{
    Task<List<CompanyContractDto>> GetCompaniesAsync();
    Task<InventoryContractResult<CompanyLookupContractDto>> GetCompanyByCenAsync(string companyCen);
    Task<InventoryContractResult<List<CategoryContractDto>>> GetCategoriesAsync(string companyCen);
    Task<InventoryContractResult<CategoryContractDto>> CreateCategoryAsync(string companyCen, CreateCategoryContractRequest request);
    Task<InventoryContractResult<CategoryContractDto>> UpdateCategoryAsync(string companyCen, string categoryCen, CreateCategoryContractRequest request);
    Task<InventoryContractResult<List<UnitContractDto>>> GetUnitsAsync(string companyCen);
    Task<InventoryContractResult<UnitContractDto>> CreateUnitAsync(string companyCen, CreateUnitContractRequest request);
    Task<InventoryContractResult<UnitContractDto>> UpdateUnitAsync(string companyCen, string unitCen, CreateUnitContractRequest request);
    Task<InventoryContractResult<List<WarehouseContractDto>>> GetWarehousesAsync(string companyCen);
}
