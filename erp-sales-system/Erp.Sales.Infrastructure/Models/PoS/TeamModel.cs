using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Sales.Infrastructure.Models.PoS;

[Table("teams")]
public class TeamModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}