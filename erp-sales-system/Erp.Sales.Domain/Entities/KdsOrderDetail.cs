using Erp.Sales.Domain.Enums;

namespace Erp.Sales.Domain.Entities;

public class KdsOrderDetail
{
    public int ProductId { get; set; }
    public int RestaurantOrderDetailId { get; set; }
    public int RestaurantOrderId { get; set; }
    public int Quantity { get; set; }
    public OrderDetailStatus RestaurantOrderDetailStatus { get; set; }
    public int ResendCount { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
