namespace Erp.Inventory.Contracts;

public class RestaurantOrderDetailProductDto
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double SellPrice { get; set; }
}