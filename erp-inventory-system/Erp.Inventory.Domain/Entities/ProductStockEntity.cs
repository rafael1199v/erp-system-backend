namespace Erp.Inventory.Domain.Entities;

public class ProductStockEntity
{
    public int ProductId { get; set; }
    public string ProductCen { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal CurrentCost { get; set; }
    public int TotalStock { get; set; }
    public string? ImageUrl { get; set; }
}
