namespace Erp.Inventory.Domain.Entities;

public class CategoryEntity
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required int CompanyId { get; init; }

    public static CategoryEntity Create(string name, int companyId)
    {
        return new CategoryEntity
        {
            Id = 0,
            Name = name,
            CompanyId = companyId
        };
    }
}