namespace Erp.Inventory.Presentation.ContractDtos;

public class UpdateProductStatusContractRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
