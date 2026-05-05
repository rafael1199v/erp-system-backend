using Erp.Sales.Domain.Enums;

namespace Erp.Sales.Domain.Entities;

public class Order
{
    public required int Id { get; set; }
    public required int DailyNumber { get; set; }
    public required DateTime OrderDateTime { get; set; }
    public OrderStatus Status { get; set; }
    public required int CompanyId { get; set; }
    public int? CustomerId { get; set; }
    public required decimal TaxPrice { get; set; }
}