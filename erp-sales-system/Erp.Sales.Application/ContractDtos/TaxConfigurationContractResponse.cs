namespace Erp.Sales.Application.ContractDtos;

public class TaxConfigurationContractResponse
{
    public string CompanyCen { get; set; } = string.Empty;
    public decimal GlobalTaxPercentage { get; set; }
}
