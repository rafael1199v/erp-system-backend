namespace Erp.Inventory.Application.DTOs;

public class GetWarehouseWithStockDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Stock { get; set; }
}