using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Mappers;

public class ProductMapper : IProductMapper
{
    public GetProductStockDTO ProductStockToDto(ProductStockEntity productStock)
    {
        return new GetProductStockDTO
        {
            ProductId = productStock.ProductId,
            ProductName = productStock.ProductName,
            Unit = productStock.Unit,
            CurrentCost = productStock.CurrentCost,
            TotalStock = productStock.TotalStock,
            ImageUrl = productStock.ImageUrl
        };
    }

    public GetProductCatalogDTO ProductEntityToCatalogDto(ProductEntity productEntity)
    {
        return new GetProductCatalogDTO
        {
            ProductId = productEntity.ProductId,
            ProductName = productEntity.ProductName,
            Unit = productEntity.Unit,
            CurrentCost = productEntity.CurrentCost,
            ImageUrl = productEntity.ImageUrl,
            CategoryId = productEntity?.Category?.Id ?? 0,
            CategoryName = productEntity?.Category?.Name ?? "Uncategorized",
            StatusCode = (int)productEntity?.Status!,
            TotalStock = productEntity.GetTotalStock(),
            ReorderLevel = productEntity.ReorderLevel,
            IsActive = productEntity.IsActive
        };
    }

    public ProductWarehouseDTO ProductEntityToProductWarehouseDto(ProductEntity productEntity)
    {
        return new ProductWarehouseDTO
        {
            Product = this.ProductEntityToCatalogDto(productEntity),
            Warehouses = productEntity.Warehouses.Select(warehouseWithStock => new GetWarehouseWithStockDTO
            {
                Id = warehouseWithStock.WarehouseId,
                Name = warehouseWithStock.WarehouseName,
                Stock = warehouseWithStock.Stock
            }).ToList()
        };
    }
}