namespace Erp.Sales.Domain.Entities;

public class Sale
{
    public required decimal SubtotalPrice { get; init; }
    public required decimal TaxPrice { get; init; }
    public required decimal DiscountPercentage { get; init; }
    public required DateTime SaleDatetime { get; init; }
    public int? CustomerId { get; init; }
    public required int PaymentTypeId { get; init; }
    public required int CompanyId { get; init; }
    public List<SaleDetail> SaleDetails { get; init; } = new();
}
