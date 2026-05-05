using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Inventory.Infrastructure.Persistance.Models;

public class Company
{
    [Column("id")]
    public int Id { get; set; }

    [Column("cen")]
    public string Cen { get; set; } = Guid.NewGuid().ToString();
    
    [Column("name")]
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Column("image_url")]
    [MaxLength(300)]
    public string? ImageUrl { get; set; }
    
    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    [Column("global_tax", TypeName = "decimal(12, 2)")]
    public decimal GlobalTax { get; set; } = (decimal)0.0;
    
    public ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
}
