using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Sales.Infrastructure.Models;

[Table("warehouse_configurations")]
public class WarehouseConfigurationModel
{
    public int CompanyId { get; set; }
    public string? CompanyCen { get; set; }
    public int MainWarehouseId { get; set; }
    public string? MainWarehouseCen { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}
