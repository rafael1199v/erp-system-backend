namespace Erp.Inventory.Application.DTOs;

public record ProductDto(
    int Id,
    string Name,
    string? ImageUrl,
    int UnitId,
    int CompanyId,
    int ProductStatusId,
    int SupplierId,
    int CategoryId,
    decimal CurrentCost,
    int ReorderLevel,
    decimal SellPrice
);