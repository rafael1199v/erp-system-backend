namespace Erp.Inventory.Domain.Entities;

public class WarehouseStockEntity
{
    public required int WarehouseId { get; set; }
    public string WarehouseCen { get; set; } = string.Empty;
    public required string WarehouseName { get; set; }
    public required int Stock { get; set; }
}
