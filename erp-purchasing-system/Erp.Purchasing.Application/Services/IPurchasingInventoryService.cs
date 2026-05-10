using Erp.Purchasing.Application.DTOs;

namespace Erp.Purchasing.Application.Services;

public interface IPurchasingInventoryService
{
    Task ConfirmStockIncreaseAsync(
        string companyCen,
        string warehouseCen,
        string purchaseOrderCen,
        IReadOnlyCollection<PurchaseOrderDetailItemDto> items,
        CancellationToken ct = default);
}
