using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Inventory.Infrastructure.Persistance.Models;

public class InventoryMovement
{
    [Column("id")]
    public int Id { get; set; }

    [Required, Column("title"), MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [Column("movement_date")]
    public DateOnly MovementDate { get; set; }
    
    [Column("movement_status_id")]
    public int MovementStatusId { get; set; }
    public MovementStatus MovementStatus { get; set; } = null!;
    
    [Column("movement_type_id")]
    public int MovementTypeId { get; set; }
    public MovementType MovementType { get; set; } = null!;
    
    [Column("company_id")]
    public int CompanyId { get; set;}
    public Company Company { get; set; } = null!;

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}