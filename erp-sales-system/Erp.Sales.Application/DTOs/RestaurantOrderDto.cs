namespace Erp.Sales.Application.DTOs;

public record RestaurantOrderDto(
    int Id,
    int DailyNumber,
    string OrderDatetime,
    int OrderStatusId,
    int? CustomerId,
    decimal TaxPrice,
    int RestaurantOrderId,
    int? WaiterId
);