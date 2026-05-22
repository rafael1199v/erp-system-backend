namespace Erp.Inventory.Application.ContractDtos;

public class InventoryAdjustmentContractRequest
{
    public string WarehouseCen { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public List<InventoryAdjustmentLineContractRequest> Lines { get; set; } = new List<InventoryAdjustmentLineContractRequest>();
}
