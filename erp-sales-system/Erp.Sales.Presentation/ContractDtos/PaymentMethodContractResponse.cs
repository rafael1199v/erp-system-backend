namespace Erp.Sales.Presentation.ContractDtos;

public class PaymentMethodContractResponse
{
    public string PaymentMethodCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
