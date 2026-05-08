namespace Erp.Inventory.Presentation.ContractDtos;

public class InventoryDocumentContractDto
{
    public string DocumentCen { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int TotalItems { get; set; }
    public List<string> GeneratedMovementCens { get; set; } = new List<string>();
}
