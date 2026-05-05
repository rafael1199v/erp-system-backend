namespace Erp.Sales.Application.DTOs;

public record DailySalesDashboardDto(
    decimal TotalSales,
    int TicketsCount,
    decimal AverageTicket
);