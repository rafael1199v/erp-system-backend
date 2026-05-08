namespace Erp.Inventory.Presentation.ContractDtos;

public class StockConsumeContractRequest
{
    public string WarehouseCen { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string ReferenceCen { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public List<StockConsumeItemContractDto> Items { get; set; } = new List<StockConsumeItemContractDto>();
}
