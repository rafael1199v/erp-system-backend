namespace Erp.Inventory.Application.ContractDtos;

public class UpdateProductStatusContractRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
