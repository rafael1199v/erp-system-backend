namespace Erp.Inventory.Contracts;

public class StockIncreaseContractRequest
{
    public string WarehouseCen { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string ReferenceCen { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public List<StockValidationItemContractDto> Items { get; set; } = new();
}
