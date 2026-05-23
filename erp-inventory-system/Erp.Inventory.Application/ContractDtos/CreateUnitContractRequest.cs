namespace Erp.Inventory.Application.ContractDtos;

public class CreateUnitContractRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Abbreviation { get; set; }
}
