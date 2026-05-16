namespace Erp.Sales.Application.ContractDtos;

public class KdsTeamContractResponse
{
    public string TeamCen { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> CategoryCens { get; set; } = [];
}
