namespace Erp.Inventory.Contracts;

public class StockConsumeContractResponse
{
    public bool Success { get; set; }
    public string? DocumentCen { get; set; }
    public string? DocumentType { get; set; }
    public List<string> GeneratedMovementCens { get; set; } = new();
    public List<StockRequirementContractDto> Requirements { get; set; } = new();
}
