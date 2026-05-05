namespace Erp.Sales.Application.DTOs;

public record AssignWaiterDto(
    int RestaurantOrderId,
    int WaiterId
);