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
            ProductCen = productStock.ProductCen,
            Sku = productStock.Sku,
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
            ProductCen = productEntity.Cen,
            Sku = productEntity.Sku,
            ProductName = productEntity.ProductName,
            Description = productEntity.Description,
            Unit = productEntity.Unit,
            UnitCen = productEntity.UnitCen,
            CurrentCost = productEntity.CurrentCost,
            SellPrice = productEntity.SellPrice,
            ImageUrl = productEntity.ImageUrl,
            CategoryId = productEntity?.Category?.Id ?? 0,
            CategoryCen = productEntity?.Category?.Cen ?? string.Empty,
            CategoryName = productEntity?.Category?.Name ?? "Uncategorized",
            StatusCode = (int)productEntity?.Status!,
            TotalStock = productEntity.GetTotalStock(),
            ReorderLevel = productEntity.ReorderLevel,
            StationCode = productEntity.StationCode,
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
                Cen = warehouseWithStock.WarehouseCen,
                Name = warehouseWithStock.WarehouseName,
                Stock = warehouseWithStock.Stock
            }).ToList()
        };
    }
}
