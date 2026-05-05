namespace Erp.Sales.Domain.Entities;

public class SaleDetail
{
    public required int ProductId { get; init; }
    public required decimal Price { get; init; }
    public required int Quantity { get; init; }
}
