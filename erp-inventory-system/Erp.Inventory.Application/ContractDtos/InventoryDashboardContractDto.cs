namespace Erp.Inventory.Application.ContractDtos;

public class InventoryDashboardContractDto
{
    public string CompanyCen { get; set; } = string.Empty;
    public int TotalProducts { get; set; }
    public decimal TotalStockQuantity { get; set; }
    public int LowStockCount { get; set; }
    public int OutOfStockCount { get; set; }
}
