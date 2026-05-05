namespace Erp.Inventory.Contracts;

public class PaymentStockDiscountItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
}
