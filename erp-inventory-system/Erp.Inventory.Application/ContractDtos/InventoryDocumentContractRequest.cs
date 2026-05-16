namespace Erp.Inventory.Application.ContractDtos;

public class InventoryDocumentContractRequest
{
    public string DocumentType { get; set; } = string.Empty;
    public string WarehouseCen { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? ExternalReference { get; set; }
    public List<InventoryDocumentLineContractRequest> Lines { get; set; } = new List<InventoryDocumentLineContractRequest>();
}
