using Erp.Purchasing.Domain.Enums;

namespace Erp.Purchasing.Application.DTOs;

public sealed record PurchaseOrderDetailDto(
    string OrderCen,
    PurchaseStatus Status,
    DateTime CreatedAt,
    DateTime? ConfirmedAt,
    string SupplierCen,
    IReadOnlyCollection<PurchaseOrderDetailItemDto> Items);

public sealed record PurchaseOrderDetailItemDto(
    string ProductCen,
    int Quantity);
