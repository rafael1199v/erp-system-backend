namespace Erp.Inventory.Contracts;

public class StockIncreaseContractRequest
{
    public List<StockValidationItemContractDto> Items { get; set; } = new();
}
