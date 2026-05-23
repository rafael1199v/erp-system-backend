namespace Erp.Inventory.Application.DTOs;

public record CategoryDto(
    int Id,
    string Name,
    int CompanyId,
    string Cen = "",
    string? Description = null
);
