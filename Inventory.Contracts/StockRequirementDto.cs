namespace Erp.Inventory.Contracts;

public class StockRequirementDto
{
    public int ProductId { get; set; }
    public int RequestedQuantity { get; set; }
    public int WarehouseId { get; set; }
}
