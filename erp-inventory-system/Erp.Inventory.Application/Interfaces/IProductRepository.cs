using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Contracts;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Interfaces;

public interface IProductRepository
{
    Task<List<ProductStockEntity>> GetCompanyProductsWithStockAsync(int companyId);
    Task<List<ProductEntity>> GetAll(int companyId);
    Task<ProductEntity> GetById(int productId);
    Task<List<ProductWarehouseStockEntity>> GetStockByIds(List<int> productIds);
    Task<int> CreateOwnProduct(ProductCompanyEntity productCompanyEntity);
    Task<bool> UpdateOwnProductAsync(ProductCompanyEntity productCompanyEntity);
    Task<bool> DeactivateAsync(int productId, int companyId);
    Task<bool> AreAllActiveAsync(int companyId, IEnumerable<int> productIds);

    Task<bool> ActiveAsync(int productId, int companyId);
    
    Task<ProductCompanyEntity?> GetProductWithCompanyAsync(int productId);
    
    Task<bool> IsProductActiveAsync(int productId, int companyId);
    
    Task<List<RestaurantOrderProductDto>> GetRestaurantOrderProductsAsync(int companyId, int warehouseId);
    Task<List<RestaurantOrderDetailProductDto>> GetRestaurantOrderDetailProductsAsync(List<int> productIds);

    Task<List<ProductInformationDto>> GetProductInformationAsync(int companyId, List<int> productIds);

    Task<List<SellableProductContractDto>> GetSellableProductsAsync(
        int companyId,
        string? search,
        string? categoryCen,
        string? warehouseCen,
        bool onlyAvailable,
        int page,
        int pageSize);
}
