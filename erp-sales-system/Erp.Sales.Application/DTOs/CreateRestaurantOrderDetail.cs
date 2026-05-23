namespace Erp.Sales.Application.DTOs;

public record CreateRestaurantOrderDetail(
    int RestaurantOrderId,
    string? Note,
    int Quantity,
    string? CreatedAt,
    string? ProductCen = null,
    int ProductId = 0
);
