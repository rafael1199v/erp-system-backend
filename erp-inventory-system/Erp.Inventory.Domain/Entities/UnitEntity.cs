namespace Erp.Inventory.Domain.Entities;

public class UnitEntity
{
    public required int Id { get; init; }
    public string Cen { get; init; } = string.Empty;
    public required string Name { get; init; }
    public string? Abbreviation { get; init; }
    public required int CompanyId { get; init; }
    
    public static UnitEntity Create(string name, int companyId)
    {
        return new UnitEntity
        {
            Id = 0,
            Cen = Guid.NewGuid().ToString(),
            Name = name,
            CompanyId = companyId
        };
    }
}
