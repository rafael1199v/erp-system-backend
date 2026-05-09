namespace Erp.Sales.Application.DTOs;

public record CreateRestaurantOrderDetail(
    int RestaurantOrderId,
    int ProductId,
    string? Note,
    int Quantity,
    string? CreatedAt,
    string? ProductCen = null
);
