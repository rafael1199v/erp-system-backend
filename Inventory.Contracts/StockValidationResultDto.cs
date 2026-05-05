namespace Erp.Inventory.Contracts;

public class StockValidationResultDto
{
    public bool AllAvailable { get; set; }
    public List<StockInsufficiencyDto> Insufficiencies { get; set; } = new();
}
