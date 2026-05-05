using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Inventory.Infrastructure.Persistance.Models;

public class Warehouse
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name")]
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("company_id")]
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    public ICollection<ProductWarehouse> ProductWarehouses { get; set; } = new List<ProductWarehouse>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}