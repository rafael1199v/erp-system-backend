namespace Erp.Sales.Application.DTOs;

public record TicketPrintDto(
    string TicketCen,
    int DailyNumber,
    DateTime CreatedAt,
    decimal TaxAmount,
    List<TicketPrintItemDto> Items);

public record TicketPrintItemDto(
    string ProductCen,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    string? Note);
