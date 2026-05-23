namespace Erp.Inventory.Application.ContractDtos;

public class CreateCategoryContractRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
