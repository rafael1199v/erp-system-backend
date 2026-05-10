using Erp.Purchasing.Application.DTOs;
using Erp.Purchasing.Application.Exceptions;
using Erp.Purchasing.Application.Repositories;

namespace Erp.Purchasing.Application.UseCases;

public class GetSuppliersUseCase(ISupplierRepository supplierRepository) : IGetSuppliersUseCase
{
    public Task<IReadOnlyCollection<SupplierDto>> ExecuteAsync(string companyCen, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(companyCen))
        {
            throw new PurchasingBusinessException("El CEN de la empresa es requerido.");
        }

        return supplierRepository.GetByCompanyAsync(companyCen, ct);
    }
}
