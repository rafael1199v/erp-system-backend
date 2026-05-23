using Erp.Purchasing.Application.DTOs;

namespace Erp.Purchasing.Application.UseCases;

public interface IGetPurchaseUseCase
{
    Task<PagedResultDto<PurchaseOrderListDto>> GetPagedAsync(
        string companyCen,
        PurchaseOrderQueryDto query,
        CancellationToken ct = default);

    Task<PurchaseOrderDetailDto> GetDetailAsync(
        string companyCen,
        string orderCen,
        CancellationToken ct = default);
}
