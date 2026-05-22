namespace Erp.Inventory.Application.ContractDtos;

public class UnitContractDto
{
    public string UnitCen { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Abbreviation { get; set; }
    public bool IsActive { get; set; }
}
