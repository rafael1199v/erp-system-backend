using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Sales.Infrastructure.Models;

[Table("order_statuses")]
public class OrderStatusModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}