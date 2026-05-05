using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Inventory.Infrastructure.Persistance.Models;

public class MovementStatus
{
    [Column("id")]
    public int Id { get; set; }
    
    [Required, Column("name"), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("is_deleted")] 
    public bool IsDeleted { get; set; } = false;

    public ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
}