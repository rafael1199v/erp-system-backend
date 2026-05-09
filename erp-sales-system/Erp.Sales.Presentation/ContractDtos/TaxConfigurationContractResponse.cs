namespace Erp.Sales.Presentation.ContractDtos;

public class TaxConfigurationContractResponse
{
    public string CompanyCen { get; set; } = string.Empty;
    public decimal GlobalTaxPercentage { get; set; }
}
