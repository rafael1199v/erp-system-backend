namespace Erp.Inventory.Presentation.ContractDtos;

public class StockRequirementContractDto
{
    public string ProductCen { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string WarehouseCen { get; set; } = string.Empty;
    public decimal RequestedQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal MissingQuantity { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
