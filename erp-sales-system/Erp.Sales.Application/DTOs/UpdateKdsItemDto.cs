namespace Erp.Sales.Application.DTOs;

public record UpdateKdsItemDto(
    int RestaurantOrderDetailId,
    int NewStatusId
);