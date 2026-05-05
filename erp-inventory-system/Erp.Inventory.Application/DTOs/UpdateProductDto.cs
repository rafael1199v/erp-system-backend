namespace Erp.Inventory.Application.DTOs;

public record UpdateProductDto(
    int ProductId,
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

