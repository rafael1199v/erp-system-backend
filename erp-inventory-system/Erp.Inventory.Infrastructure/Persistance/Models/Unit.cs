using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Inventory.Infrastructure.Persistance.Models;

public class Unit
{
    [Column("id")]
    public int Id { get; set; }

    [Column("cen")]
    public string Cen { get; set; } = Guid.NewGuid().ToString();

    [Column("name"), Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Column("abbreviation")]
    public string? Abbreviation { get; set; }
    
    [Column("company_id")]
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    
    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;
}
