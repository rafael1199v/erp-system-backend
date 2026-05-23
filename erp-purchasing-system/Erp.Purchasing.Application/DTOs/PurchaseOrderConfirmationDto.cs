using Erp.Purchasing.Domain.Enums;

namespace Erp.Purchasing.Application.DTOs;

public sealed record PurchaseOrderConfirmationDto(
    string OrderCen,
    PurchaseStatus Status,
    DateTime ConfirmedAt);
