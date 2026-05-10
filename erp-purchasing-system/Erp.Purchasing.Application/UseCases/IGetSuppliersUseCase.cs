using Erp.Purchasing.Application.DTOs;

namespace Erp.Purchasing.Application.UseCases;

public interface IGetSuppliersUseCase
{
    Task<IReadOnlyCollection<SupplierDto>> ExecuteAsync(string companyCen, CancellationToken ct = default);
}
