using Erp.Purchasing.Domain.Enums;

namespace Erp.Purchasing.Application.DTOs;

public sealed record PurchaseOrderSummaryDto(
    string OrderCen,
    PurchaseStatus Status);
