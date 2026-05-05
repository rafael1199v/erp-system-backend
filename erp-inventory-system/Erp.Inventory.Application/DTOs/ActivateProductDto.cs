namespace Erp.Inventory.Application.DTOs;

public record ActivateProductDto(
    int ProductId,
    int CompanyId
);