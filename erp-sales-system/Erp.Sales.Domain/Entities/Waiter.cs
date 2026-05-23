namespace Erp.Sales.Domain.Entities;

public class Waiter
{
    public required int Id { get; set; }
    public string Cen { get; set; } = string.Empty;
    public required string Name { get; set; }
    public required int CompanyId { get; set; }
    public string? CompanyCen { get; set; }
}
