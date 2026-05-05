namespace Erp.Sales.Application.DTOs;

public record UpdateRestaurantOrderDetailQuantityDto(
    int RestaurantOrderDetailId,
    int Quantity,
    string? Note
);