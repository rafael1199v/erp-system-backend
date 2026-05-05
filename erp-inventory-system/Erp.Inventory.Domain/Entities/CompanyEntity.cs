namespace Erp.Inventory.Domain.Entities;

public class CompanyEntity
{
    public required int Id { get; set; }
    public string Cen { get; set; } = string.Empty;
    public required string Name { get; set; }
}
