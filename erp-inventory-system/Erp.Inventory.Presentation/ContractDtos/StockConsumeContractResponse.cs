namespace Erp.Inventory.Presentation.ContractDtos;

public class StockConsumeContractResponse
{
    public bool Success { get; set; }
    public string? DocumentCen { get; set; }
    public string? DocumentType { get; set; }
    public List<string> GeneratedMovementCens { get; set; } = new List<string>();
    public List<StockRequirementContractDto> Requirements { get; set; } = new List<StockRequirementContractDto>();
}
