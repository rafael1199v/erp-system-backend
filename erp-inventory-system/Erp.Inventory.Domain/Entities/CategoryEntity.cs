namespace Erp.Inventory.Domain.Entities;

public class CategoryEntity
{
    public required int Id { get; init; }
    public string Cen { get; init; } = string.Empty;
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required int CompanyId { get; init; }

    public static CategoryEntity Create(string name, int companyId, string? description = null)
    {
        return new CategoryEntity
        {
            Id = 0,
            Cen = Guid.NewGuid().ToString(),
            Name = name,
            Description = description,
            CompanyId = companyId
        };
    }
}
