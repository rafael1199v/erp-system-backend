namespace Erp.Sales.Application.ContractDtos;

public class CreateTicketItemContractRequest
{
    public string ProductCen { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Note { get; set; }
}
