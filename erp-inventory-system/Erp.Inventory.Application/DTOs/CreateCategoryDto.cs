namespace Erp.Inventory.Application.DTOs;

public record CreateCategoryDto(
    string Name,
    int CompanyId,
    string? Description = null
);
