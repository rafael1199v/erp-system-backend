namespace Erp.Sales.Application.DTOs;

public record KdsOrderItem(
    int ProductId,
    int CategoryId,
    int RestaurantOrderDetailId,
    int RestaurantOrderId,
    string ProductName,
    int Quantity,
    string OrderItemStatus,
    int OrderItemStatusId,
    string? Note,
    int ResendCount
);