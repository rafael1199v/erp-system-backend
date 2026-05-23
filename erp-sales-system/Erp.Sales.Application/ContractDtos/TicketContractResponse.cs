namespace Erp.Sales.Application.ContractDtos;

public class TicketContractResponse
{
    public string TicketCen { get; set; } = string.Empty;
    public int DailyNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public string? WaiterCen { get; set; }
    public string? CompanyCen { get; set; }
    public decimal TaxAmount { get; set; }
}
