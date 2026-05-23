using Erp.Purchasing.Application.DTOs;

namespace Erp.Purchasing.Application.UseCases;

public interface IConfirmPurchaseUseCase
{
    Task<PurchaseOrderConfirmationDto> ExecuteAsync(
        string companyCen,
        string orderCen,
        CancellationToken ct = default);
}
