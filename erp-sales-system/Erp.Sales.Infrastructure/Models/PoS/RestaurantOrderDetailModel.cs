using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Sales.Infrastructure.Models.PoS;

[Table("restaurant_order_details")]
public class RestaurantOrderDetailModel
{
    public int Id { get; set; }
    public string Cen { get; set; } = Guid.NewGuid().ToString();
    
    public int RestaurantOrderId { get; set; }
    public RestaurantOrderModel RestaurantOrder { get; set; } = null!;
    
    public int ProductId { get; set; }
    public string ProductCen { get; set; } = string.Empty;
    
    public int RestaurantOrderDetailStatusId { get; set; }
    public RestaurantOrderDetailStatusModel RestaurantOrderDetailStatus { get; set; } = null!;
    
    [MaxLength(300)]
    public string? Note { get; set; }
    
    public int Quantity { get; set; }
    public DateTime? SentAt { get; set; }
    public int ResendCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}
