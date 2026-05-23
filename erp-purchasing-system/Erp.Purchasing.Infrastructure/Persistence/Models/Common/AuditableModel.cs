using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Purchasing.Infrastructure.Persistence.Models.Common;

public class AuditableModel
{
    [Column("is_active")]
    public bool IsActive { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}