namespace Erp.Purchasing.Domain.Entities;

public class PurchaseItem
{
    private PurchaseItem()
    {
    }

    public int Id { get; private set; }
    public string ProductCen { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public int PurchaseId { get; private set; }

    public static PurchaseItem Create(string productCen, int quantity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productCen);

        if (quantity <= 0)
        {
            throw new InvalidOperationException("La cantidad del item debe ser mayor a cero.");
        }

        return new PurchaseItem
        {
            ProductCen = productCen.Trim(),
            Quantity = quantity
        };
    }

    public static PurchaseItem Restore(int id, string productCen, int quantity, int purchaseId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productCen);

        return new PurchaseItem
        {
            Id = id,
            ProductCen = productCen.Trim(),
            Quantity = quantity,
            PurchaseId = purchaseId
        };
    }
}
