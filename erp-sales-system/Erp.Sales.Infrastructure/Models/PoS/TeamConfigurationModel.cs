using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Sales.Infrastructure.Models.PoS;

[Table("team_configurations")]
public class TeamConfigurationModel
{
    public int CompanyId { get; set; }
    public string? CompanyCen { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryCen { get; set; }

    public int TeamId { get; set; }
    public TeamModel Team { get; set; } = null!;

}
