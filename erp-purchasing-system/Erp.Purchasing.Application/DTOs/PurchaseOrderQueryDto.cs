using Erp.Purchasing.Domain.Enums;

namespace Erp.Purchasing.Application.DTOs;

public sealed record PurchaseOrderQueryDto(
    PurchaseStatus? Status = null,
    int Page = 1,
    int PageSize = 20,
    bool SortDescending = true);
