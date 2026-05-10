using Erp.Purchasing.Domain.Enums;

namespace Erp.Purchasing.Domain.Entities;

public class Purchase
{
    private readonly List<PurchaseItem> _items = new();

    private Purchase()
    {
    }

    public int Id { get; private set; }
    public string Cen { get; private set; } = string.Empty;
    public string CompanyCen { get; private set; } = string.Empty;
    public int SupplierId { get; private set; }
    public PurchaseStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public IReadOnlyCollection<PurchaseItem> Items => _items.AsReadOnly();

    public static Purchase Create(string companyCen, int supplierId, IEnumerable<PurchaseItem> items)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(companyCen);

        var purchaseItems = items?.ToList() ?? throw new ArgumentNullException(nameof(items));
        if (purchaseItems.Count == 0)
        {
            throw new InvalidOperationException("La orden de compra debe tener al menos un item.");
        }

        var purchase = new Purchase
        {
            Cen = Guid.NewGuid().ToString(),
            CompanyCen = companyCen.Trim(),
            SupplierId = supplierId,
            Status = PurchaseStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        purchase._items.AddRange(purchaseItems);
        return purchase;
    }

    public static Purchase Restore(
        int id,
        string cen,
        string companyCen,
        int supplierId,
        PurchaseStatus status,
        DateTime createdAt,
        DateTime? confirmedAt,
        IEnumerable<PurchaseItem> items)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cen);
        ArgumentException.ThrowIfNullOrWhiteSpace(companyCen);

        var purchase = new Purchase
        {
            Id = id,
            Cen = cen.Trim(),
            CompanyCen = companyCen.Trim(),
            SupplierId = supplierId,
            Status = status,
            CreatedAt = createdAt,
            ConfirmedAt = confirmedAt
        };

        purchase._items.AddRange(items ?? Enumerable.Empty<PurchaseItem>());
        return purchase;
    }

    public void Confirm(DateTime confirmedAt)
    {
        if (Status != PurchaseStatus.Pending)
        {
            throw new InvalidOperationException("Solo se pueden confirmar ordenes de compra pendientes.");
        }

        Status = PurchaseStatus.Confirmed;
        ConfirmedAt = confirmedAt;
    }
}
