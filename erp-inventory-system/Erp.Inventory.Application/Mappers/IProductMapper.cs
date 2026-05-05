using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Mappers;

public interface IProductMapper
{
    GetProductStockDTO ProductStockToDto(ProductStockEntity productStock);
    GetProductCatalogDTO ProductEntityToCatalogDto(ProductEntity productEntity);
    ProductWarehouseDTO ProductEntityToProductWarehouseDto(ProductEntity productEntity);

}