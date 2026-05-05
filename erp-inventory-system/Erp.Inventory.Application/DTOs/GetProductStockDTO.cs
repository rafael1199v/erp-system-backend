namespace Erp.Inventory.Application.DTOs;

public class GetProductStockDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal CurrentCost { get; set; }
    public int TotalStock { get; set; }
    public string? ImageUrl { get; set; }
}