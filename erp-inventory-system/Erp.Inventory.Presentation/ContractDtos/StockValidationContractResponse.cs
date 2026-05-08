namespace Erp.Inventory.Presentation.ContractDtos;

public class StockValidationContractResponse
{
    public bool IsValid { get; set; }
    public List<StockRequirementContractDto> Requirements { get; set; } = new List<StockRequirementContractDto>();
}
