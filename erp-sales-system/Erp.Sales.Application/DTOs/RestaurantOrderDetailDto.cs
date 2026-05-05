namespace Erp.Sales.Application.DTOs;

public record RestaurantOrderDetailDto(
    int ProductId,
    string Name,
    double UnitPrice,
    int Quantity,
    string? Note,
    int RestaurantOrderDetailId,
    string? SentAt,
    int RestaurantOrderStatusId,
    string RestaurantOrderStatus,
    int ResendCount
);