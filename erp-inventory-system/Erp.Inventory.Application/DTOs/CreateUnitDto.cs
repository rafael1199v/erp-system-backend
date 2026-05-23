namespace Erp.Inventory.Application.DTOs;

public record CreateUnitDto(
    string Name,
    int CompanyId,
    string? Abbreviation = null
);
