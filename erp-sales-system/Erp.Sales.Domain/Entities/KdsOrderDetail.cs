using Erp.Sales.Domain.Enums;

namespace Erp.Sales.Domain.Entities;

public class KdsOrderDetail
{
    public int ProductId { get; set; }
    public string? ProductCen { get; set; }
    public int RestaurantOrderDetailId { get; set; }
    public string TicketItemCen { get; set; } = string.Empty;
    public int RestaurantOrderId { get; set; }
    public string TicketCen { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public OrderDetailStatus RestaurantOrderDetailStatus { get; set; }
    public int ResendCount { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
