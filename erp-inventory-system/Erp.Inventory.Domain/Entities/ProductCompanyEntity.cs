namespace Erp.Inventory.Domain.Entities;

public class ProductCompanyEntity
{
    public required int Id { get; init; }
    public string Cen { get; init; } = string.Empty;
    public string? Sku { get; init; }
    public string? Description { get; init; }
    public string? StationCode { get; init; }
    public required int CoreProductId { get; init; }
    public required int UnitId { get; init; }
    public required int CompanyId { get; init; }
    public required int ProductStatusId { get; init; }
    public required int SupplierId { get; init; }
    public required int CategoryId { get; init; }
    public required decimal CurrentCost { get; init; }
    public required int ReorderLevel { get; init; }
    public required decimal SellPrice { get; init; }
    public required CoreProductEntity? CoreProduct { get; init; }


    public static ProductCompanyEntity CreateOwnProduct(
        string name,
        string? imageUrl,
        int unitId,
        int companyId,
        int categoryId,
        int productStatusId,
        int supplierId,
        decimal currentCost,
        int reorderLevel,
        decimal sellPrice,
        string? sku = null,
        string? description = null,
        string? stationCode = null
    )
    {
        ValidatePricing(currentCost, sellPrice);

        return new ProductCompanyEntity
        {
            Id = 0,
            Cen = string.IsNullOrWhiteSpace(sku) ? Guid.NewGuid().ToString() : sku,
            Sku = sku,
            Description = description,
            StationCode = stationCode,
            CoreProductId = 0,
            UnitId = unitId,
            CompanyId = companyId,
            CategoryId = categoryId,
            ProductStatusId = productStatusId,
            SupplierId = supplierId,
            CurrentCost = currentCost,
            ReorderLevel = reorderLevel,
            SellPrice = sellPrice,
            CoreProduct = new CoreProductEntity
            {
                Id = 0,
                ImageUrl = imageUrl,
                Name = name,
            }
        };
    }

    public static ProductCompanyEntity UpdateOwnProduct(
        int productId,
        string name,
        string? imageUrl,
        int unitId,
        int companyId,
        int categoryId,
        int productStatusId,
        int supplierId,
        decimal currentCost,
        int reorderLevel,
        decimal sellPrice,
        string? sku = null,
        string? description = null,
        string? stationCode = null
    )
    {
        ValidatePricing(currentCost, sellPrice);

        return new ProductCompanyEntity
        {
            Id = productId,
            Sku = sku,
            Description = description,
            StationCode = stationCode,
            CoreProductId = 0,
            UnitId = unitId,
            CompanyId = companyId,
            CategoryId = categoryId,
            ProductStatusId = productStatusId,
            SupplierId = supplierId,
            CurrentCost = currentCost,
            ReorderLevel = reorderLevel,
            SellPrice = sellPrice,
            CoreProduct = new CoreProductEntity
            {
                Id = 0,
                ImageUrl = imageUrl,
                Name = name,
            }
        };
    }

    private static void ValidatePricing(decimal currentCost, decimal sellPrice)
    {
        if (sellPrice <= 0)
        {
            throw new Exception("El precio de venta no puede ser menor o igual a cero");
        }

        if (currentCost <= 0)
        {
            throw new Exception("El precio del producto no puede ser menor o igual a cero");
        }
    }
}
