namespace Erp.Sales.Presentation.ContractDtos;

public class TicketTotalsContractResponse
{
    public string TicketCen { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
}
