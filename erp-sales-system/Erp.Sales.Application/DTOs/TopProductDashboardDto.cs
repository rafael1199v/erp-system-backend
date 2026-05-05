namespace Erp.Sales.Application.DTOs;

public record TopProductDashboardDto(
    int ProductId,
    string ProductName,
    int TotalQuantity,
    int CategoryId,
    double SellPrice
);