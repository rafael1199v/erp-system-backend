namespace Erp.Inventory.Application.ContractDtos;

public class InventoryAdjustmentLineContractRequest
{
    public string ProductCen { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string AdjustmentType { get; set; } = string.Empty;
}
