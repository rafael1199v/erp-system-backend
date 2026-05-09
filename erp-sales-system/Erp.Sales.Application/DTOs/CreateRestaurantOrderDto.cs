namespace Erp.Sales.Application.DTOs;

public record CreateRestaurantOrderDto(
    int? CustomerId,
    int? WaiterId,
    int CompanyId,
    string? CompanyCen = null
);
