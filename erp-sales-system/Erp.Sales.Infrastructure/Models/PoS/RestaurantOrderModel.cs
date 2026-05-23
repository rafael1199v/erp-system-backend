using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Sales.Infrastructure.Models.PoS;

[Table("restaurant_orders")]
public class RestaurantOrderModel
{
    public int Id { get; set; }
    public string Cen { get; set; } = Guid.NewGuid().ToString();
    
    public int OrderId { get; set; }
    public OrderModel Order { get; set; } = null!;
    
    public int? WaiterId { get; set; }
    public WaiterModel? Waiter { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}
