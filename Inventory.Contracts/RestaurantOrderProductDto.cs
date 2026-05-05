using System;

namespace Erp.Inventory.Contracts;

public class RestaurantOrderProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal SellPrice { get; set; }
    public int AvailableStock { get; set; }
    public bool IsAvailable { get; set; }
    public string ProductStatus { get; set; } = string.Empty;
}
