namespace Erp.Inventory.Presentation.ContractDtos;

public class StockItemContractDto
{
    public string ProductCen { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string WarehouseCen { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public decimal AvailableQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal ReorderLevel { get; set; }
    public bool IsLowStock { get; set; }
}
