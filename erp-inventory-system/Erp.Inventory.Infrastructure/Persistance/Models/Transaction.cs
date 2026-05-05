using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Inventory.Infrastructure.Persistance.Models;

public class Transaction
{
    [Column("id")]
    public int Id { get; set; }

    [Column("cen")]
    [MaxLength(64)]
    public string Cen { get; set; } = Guid.NewGuid().ToString();
    
    [Column("product_id")]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    [Column("warehouse_id")]
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    
    [Column("quantity")]
    public int Quantity { get; set; }
    
    [Column("reason")]
    [MaxLength(300)]
    public string? Reason { get; set; } = string.Empty;
    
    [Column("transaction_date")]
    public DateOnly TransactionDate { get; set; }
    
    [Column("inventory_movement_id")]
    public int InventoryMovementId { get; set; }
    public InventoryMovement InventoryMovement { get; set; } = null!;
    
    [Column("transaction_type_id")]
    public int TransactionTypeId { get; set; }
    public TransactionType TransactionType { get; set; } = null!;
    
    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;
}
