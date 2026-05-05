using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Inventory.Infrastructure.Persistance.Models;

public class TransactionType
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;
    
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}