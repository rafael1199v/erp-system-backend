namespace Erp.Inventory.Application.Realtime;

public sealed record RestockEvent(
    string CompanyCen,
    string WarehouseCen,
    string ReferenceCen,
    DateTime OccurredAt,
    IReadOnlyList<RestockItem> Items);

public sealed record RestockItem(string ProductCen, decimal Quantity);
