using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Sales.Infrastructure.Models.PoS;

[Table("restaurant_order_detail_statuses")]
public class RestaurantOrderDetailStatusModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}