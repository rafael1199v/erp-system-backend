namespace Erp.Inventory.Contracts;

public class StockValidationContractResponse
{
    public bool IsValid { get; set; }
    public List<StockRequirementContractDto> Requirements { get; set; } = new();
}
