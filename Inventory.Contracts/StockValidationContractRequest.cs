namespace Erp.Inventory.Contracts;

public class StockValidationContractRequest
{
    public string WarehouseCen { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string? ReferenceCen { get; set; }
    public List<StockValidationItemContractDto> Items { get; set; } = new();
}
