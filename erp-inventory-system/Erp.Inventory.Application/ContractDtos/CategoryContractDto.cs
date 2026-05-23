namespace Erp.Inventory.Application.ContractDtos;

public class CategoryContractDto
{
    public string CategoryCen { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
