namespace Erp.Inventory.Application.DTOs;

public record CreateProductDto(
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
    string? Sku = null
);
