namespace Erp.Inventory.Domain.Entities;

public class ProductWarehouseStockEntity
{
    public required int ProductId { get; set; }
    public string ProductCen { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public required int WarehouseId { get; set; }
    public string WarehouseCen { get; set; } = string.Empty;
    public int Stock { get; set; }
    
    public String ProductName { get; set; } = string.Empty;
    public String WarehouseName { get; set; } = string.Empty;

    public void UpdateStock(int addedValue)
    {
        if(addedValue < 0 && Stock + addedValue < 0)
        {
            throw new InvalidOperationException($"El stock del producto {this.ProductName} en el almacen {this.WarehouseName} no puede ser negativo. Maximo {this.Stock}");
        }
        
        Stock += addedValue;
    }
}
