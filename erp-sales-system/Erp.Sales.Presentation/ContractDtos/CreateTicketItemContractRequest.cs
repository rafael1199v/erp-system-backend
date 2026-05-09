namespace Erp.Sales.Presentation.ContractDtos;

public class CreateTicketItemContractRequest
{
    public string ProductCen { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
}
