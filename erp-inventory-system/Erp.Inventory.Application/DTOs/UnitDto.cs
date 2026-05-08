namespace Erp.Inventory.Application.DTOs;

public record UnitDto(
    int Id,
    string Name,
    int CompanyId,
    string Cen = "",
    string? Abbreviation = null
);
