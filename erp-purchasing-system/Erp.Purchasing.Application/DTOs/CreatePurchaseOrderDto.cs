namespace Erp.Purchasing.Application.DTOs;

public sealed record CreatePurchaseOrderDto(
    string SupplierCen,
    IReadOnlyCollection<CreatePurchaseOrderItemDto> Items);

public sealed record CreatePurchaseOrderItemDto(
    string ProductCen,
    int Quantity);
