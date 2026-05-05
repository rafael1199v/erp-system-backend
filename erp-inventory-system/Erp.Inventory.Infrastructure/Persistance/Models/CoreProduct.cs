using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Inventory.Infrastructure.Persistance.Models;

public class CoreProduct
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name")]
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Column("image_url")]
    [MaxLength(300)]
    public string? ImageUrl { get; set; }
    
    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;
    
    [Column("is_global_product")]
    public bool IsGlobalProduct { get; set; } = false;
    
    public ICollection<Product> Products { get; set; } = new List<Product>();
}