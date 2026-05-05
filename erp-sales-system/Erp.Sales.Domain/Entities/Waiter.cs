namespace Erp.Sales.Domain.Entities;

public class Waiter
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int CompanyId { get; set; }
}