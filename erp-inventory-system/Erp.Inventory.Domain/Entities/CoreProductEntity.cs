namespace Erp.Inventory.Domain.Entities;

public class CoreProductEntity
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? ImageUrl { get; init; }
}