namespace Erp.Inventory.Domain.Entities;

public class WarehouseStockEntity
{
    public required int WarehouseId { get; set; }
    public required string WarehouseName { get; set; }
    public required int Stock { get; set; }
}