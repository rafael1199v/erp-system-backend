using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Inventory.Infrastructure.Persistance.Models;

public class Product
{
    [Column("id")]
    public int Id { get; set; }

    [Column("cen")]
    public string Cen { get; set; } = Guid.NewGuid().ToString();

    [Column("sku")]
    public string? Sku { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("station_code")]
    public string? StationCode { get; set; }
    
    [Column("core_product_id")]
    public int CoreProductId { get; set; }
    public CoreProduct CoreProduct { get; set; } = null!;
    
    [Column("unit_id")]
    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;
    
    [Column("category_id")]
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    [Column("company_id")]
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    
    [Column("product_status_id")]
    public int ProductStatusId { get; set; }
    public ProductStatus ProductStatus { get; set; } = null!;
    
    [Column("supplier_id")]
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    [Column("current_cost",TypeName = "decimal(12, 2)")]
    public decimal CurrentCost { get; set; } = 0.00m;
    
    [Column("reorder_level")]
    public int ReorderLevel { get; set; }
    
    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;
    
    [Column("sell_price", TypeName = "decimal(12, 2)")]
    public decimal SellPrice { get; set; } = 0.00m;
    
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    public ICollection<ProductWarehouse> ProductWarehouses { get; set; } = new List<ProductWarehouse>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
