namespace Erp.Inventory.Domain.Entities;

public class UnitEntity
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required int CompanyId { get; init; }
    
    public static UnitEntity Create(string name, int companyId)
    {
        return new UnitEntity
        {
            Id = 0,
            Name = name,
            CompanyId = companyId
        };
    }
}