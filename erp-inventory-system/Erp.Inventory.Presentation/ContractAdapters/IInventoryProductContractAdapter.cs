using Erp.Inventory.Contracts;
using Erp.Inventory.Presentation.ContractDtos;

namespace Erp.Inventory.Presentation.ContractAdapters;

public interface IInventoryProductContractAdapter
{
    Task<InventoryContractResult<List<ProductContractDto>>> GetProductsAsync(
        string companyCen,
        string? search = null,
        string? categoryCen = null,
        string? status = null);
    Task<InventoryContractResult<CreateProductContractResponse>> CreateProductAsync(string companyCen, CreateProductContractRequest request);
    Task<InventoryContractResult<ProductContractDto>> UpdateProductAsync(string companyCen, string productCen, UpdateProductContractRequest request);
    Task<InventoryContractResult<ProductContractDto>> UpdateProductStatusAsync(string companyCen, string productCen, UpdateProductStatusContractRequest request);
    Task<InventoryContractResult<List<SellableProductContractDto>>> GetSellableProductsAsync(
        string companyCen,
        string? search,
        string? categoryCen,
        string? warehouseCen,
        bool onlyAvailable,
        int page,
        int pageSize);
}
