using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Sales.Infrastructure.Models.PoS;

[Table("team_configurations")]
public class TeamConfigurationModel
{
    public int CompanyId { get; set; }
    public string CompanyCen { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryCen { get; set; } = string.Empty;

    public int TeamId { get; set; }
    public TeamModel Team { get; set; } = null!;

}
