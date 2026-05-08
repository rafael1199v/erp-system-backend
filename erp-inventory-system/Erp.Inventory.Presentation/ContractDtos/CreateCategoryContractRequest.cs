namespace Erp.Inventory.Presentation.ContractDtos;

public class CreateCategoryContractRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
