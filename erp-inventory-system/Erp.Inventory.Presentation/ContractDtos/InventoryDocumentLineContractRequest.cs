namespace Erp.Inventory.Presentation.ContractDtos;

public class InventoryDocumentLineContractRequest
{
    public string ProductCen { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal? UnitCost { get; set; }
}
