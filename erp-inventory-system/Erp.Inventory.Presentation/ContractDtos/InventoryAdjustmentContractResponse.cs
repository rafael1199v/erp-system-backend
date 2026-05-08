namespace Erp.Inventory.Presentation.ContractDtos;

public class InventoryAdjustmentContractResponse
{
    public string AdjustmentCen { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<GeneratedMovementContractDto> GeneratedMovements { get; set; } = new List<GeneratedMovementContractDto>();
}
