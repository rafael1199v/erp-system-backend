using Erp.Purchasing.Domain.Enums;

namespace Erp.Purchasing.Application.DTOs;

public sealed record PurchaseOrderListDto(
    string OrderCen,
    PurchaseStatus Status,
    DateTime CreatedAt,
    DateTime? ConfirmedAt,
    string SupplierCen,
    int ItemCount);
