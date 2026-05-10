using Erp.Purchasing.Application.DTOs;

namespace Erp.Purchasing.Application.UseCases;

public interface ICreatePurchaseUseCase
{
    Task<PurchaseOrderSummaryDto> ExecuteAsync(
        string companyCen,
        CreatePurchaseOrderDto request,
        CancellationToken ct = default);
}
