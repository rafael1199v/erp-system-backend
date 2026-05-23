using Erp.Purchasing.Application.DTOs;
using Erp.Purchasing.Domain.Entities;

namespace Erp.Purchasing.Application.Repositories;

public interface IPurchaseRepository
{
    Task AddAsync(Purchase purchase, CancellationToken ct = default);

    Task<Purchase?> GetEditableByCompanyAndCenAsync(
        string companyCen,
        string orderCen,
        CancellationToken ct = default);

    Task UpdateAsync(Purchase purchase, CancellationToken ct = default);

    Task<PagedResultDto<PurchaseOrderListDto>> GetPagedAsync(
        string companyCen,
        PurchaseOrderQueryDto query,
        CancellationToken ct = default);

    Task<PurchaseOrderDetailDto?> GetDetailAsync(
        string companyCen,
        string orderCen,
        CancellationToken ct = default);
}
