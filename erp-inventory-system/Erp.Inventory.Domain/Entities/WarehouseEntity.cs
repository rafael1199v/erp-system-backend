namespace Erp.Inventory.Domain.Entities;

public class WarehouseEntity
{
    public required int Id { get; set; }
    public string Cen { get; set; } = string.Empty;
    public required string Name { get; set; }
    public int CompanyId { get; set; }
    
}
