namespace Erp.Sales.Presentation.ContractDtos;

public class PayTicketContractResponse
{
    public string SaleCen { get; set; } = string.Empty;
    public string TicketCen { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public string? InventoryDocumentCen { get; set; }
}
