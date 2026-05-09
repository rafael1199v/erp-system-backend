using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Sales.Infrastructure.Models;

[Table("orders")]
public class OrderModel
{
    public int Id { get; set; }
    
    public int DailyNumber { get; set; }
    
    public DateTime OrderDatetime { get; set; } = DateTime.UtcNow;

    public int OrderStatusId { get; set; }
    public OrderStatusModel OrderStatus { get; set; } = null!;

    public int? CustomerId { get; set; }
    public CustomerModel? Customer { get; set; }

    public int CompanyId { get; set; }
    public string? CompanyCen { get; set; }
    public decimal TaxPrice { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    public ICollection<OrderDetailModel> OrderDetails { get; set; } = new List<OrderDetailModel>();
}
