namespace Erp.Inventory.Application.DTOs;

public class GetProductCatalogDTO
{
    public int ProductId { get; set; }
    public string ProductCen { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string UnitCen { get; set; } = string.Empty;
    public decimal CurrentCost { get; set; }
    public decimal SellPrice { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string CategoryCen { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public int TotalStock { get; set; }
    public int ReorderLevel { get; set; }
    public string? StationCode { get; set; }
    
    public bool IsActive { get; set; }
}
