namespace Erp.Sales.Presentation.ContractDtos;

public class KdsItemContractResponse
{
    public string TicketItemCen { get; set; } = string.Empty;
    public string TicketCen { get; set; } = string.Empty;
    public string ProductCen { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
    public int ResendCount { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}
