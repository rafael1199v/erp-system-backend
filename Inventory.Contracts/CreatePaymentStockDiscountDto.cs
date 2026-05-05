namespace Erp.Inventory.Contracts;

public class CreatePaymentStockDiscountDto
{
    public int RestaurantOrderId { get; set; }
    public int CompanyId { get; set; }
    public int WarehouseId { get; set; }
    public DateTime PaymentDateUtc { get; set; }
    public List<PaymentStockDiscountItemDto> Items { get; set; } = new();
}
