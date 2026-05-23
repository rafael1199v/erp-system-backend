using Erp.Purchasing.Application.DTOs;
using Erp.Purchasing.Application.Exceptions;
using Erp.Purchasing.Application.Repositories;

namespace Erp.Purchasing.Application.UseCases;

public class GetPurchaseUseCase(IPurchaseRepository purchaseRepository) : IGetPurchaseUseCase
{
    public Task<PagedResultDto<PurchaseOrderListDto>> GetPagedAsync(
        string companyCen,
        PurchaseOrderQueryDto query,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(companyCen))
        {
            throw new PurchasingBusinessException("El CEN de la empresa es requerido.");
        }

        return purchaseRepository.GetPagedAsync(companyCen, Normalize(query), ct);
    }

    public async Task<PurchaseOrderDetailDto> GetDetailAsync(
        string companyCen,
        string orderCen,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(companyCen))
        {
            throw new PurchasingBusinessException("El CEN de la empresa es requerido.");
        }

        if (string.IsNullOrWhiteSpace(orderCen))
        {
            throw new PurchasingBusinessException("El CEN de la orden es requerido.");
        }

        return await purchaseRepository.GetDetailAsync(companyCen, orderCen, ct)
               ?? throw new PurchasingNotFoundException("La orden de compra no existe.");
    }

    private static PurchaseOrderQueryDto Normalize(PurchaseOrderQueryDto? query)
    {
        const int maxPageSize = 100;

        var requested = query ?? new PurchaseOrderQueryDto();
        var page = Math.Max(1, requested.Page);
        var pageSize = Math.Clamp(requested.PageSize, 1, maxPageSize);

        return requested with
        {
            Page = page,
            PageSize = pageSize
        };
    }
}
