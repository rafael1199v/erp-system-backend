namespace Erp.Sales.Application.ContractDtos;

public class TicketItemContractResponse
{
    public string TicketItemCen { get; set; } = string.Empty;
    public string ProductCen { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Note { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? SentAt { get; set; }
    public int ResendCount { get; set; }
}
