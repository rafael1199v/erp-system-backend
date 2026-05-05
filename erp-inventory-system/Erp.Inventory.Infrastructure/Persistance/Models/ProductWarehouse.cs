using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Inventory.Infrastructure.Persistance.Models;

public class ProductWarehouse
{
    [Column("product_id")]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Column("warehouse_id")]
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    [Column("quantity")]
    public int Quantity { get; set; } = 0;
    
    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;
    
    [Column("last_movement")]
    public DateTime? LastMovement { get; set; }
}