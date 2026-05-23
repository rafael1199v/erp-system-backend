namespace Erp.Purchasing.Domain.Entities;

public class Supplier
{
    private Supplier()
    {
    }

    public int Id { get; private set; }
    public string Cen { get; private set; } = string.Empty;
    public string CompanyCen { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;

    public static Supplier Restore(int id, string cen, string companyCen, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cen);
        ArgumentException.ThrowIfNullOrWhiteSpace(companyCen);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Supplier
        {
            Id = id,
            Cen = cen.Trim(),
            CompanyCen = companyCen.Trim(),
            Name = name.Trim()
        };
    }
}
