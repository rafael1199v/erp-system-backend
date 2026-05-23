using Erp.Purchasing.Application.DTOs;
using Erp.Purchasing.Domain.Entities;

namespace Erp.Purchasing.Application.Repositories;

public interface ISupplierRepository
{
    Task<IReadOnlyCollection<SupplierDto>> GetByCompanyAsync(
        string companyCen,
        CancellationToken ct = default);

    Task<Supplier?> GetByCompanyAndCenAsync(
        string companyCen,
        string supplierCen,
        CancellationToken ct = default);
}
