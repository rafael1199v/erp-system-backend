namespace Erp.Inventory.Application.DTOs;

public record ProductDto(
    int Id,
    string Cen,
    string? Sku,
    string Name,
    string? ImageUrl,
    int UnitId,
    int CompanyId,
    int ProductStatusId,
    int SupplierId,
    int CategoryId,
    decimal CurrentCost,
    int ReorderLevel,
    decimal SellPrice,
    string? Description = null,
    string? StationCode = null
);
