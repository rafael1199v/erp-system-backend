using Erp.Inventory.Domain.Enums;

namespace Erp.Inventory.Domain.Entities;

public class ProductEntity
{
    public int ProductId { get; set; }
    public string Cen { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? StationCode { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal CurrentCost { get; set; }
    public string? ImageUrl { get; set; }
    public CategoryEntity? Category { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Unavailable;
    public int ReorderLevel { get; set; }
    public required bool IsActive { get; set; }

    public List<WarehouseStockEntity> Warehouses { get; set; } = new List<WarehouseStockEntity>();

    public int GetTotalStock()
    {
        return Warehouses.Sum(warehouse => warehouse.Stock);
    }
}
