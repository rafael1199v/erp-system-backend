namespace Erp.Sales.Application.DTOs;

public record TicketTotalsDto(
    decimal Subtotal,
    decimal TaxAmount,
    decimal Total
);
