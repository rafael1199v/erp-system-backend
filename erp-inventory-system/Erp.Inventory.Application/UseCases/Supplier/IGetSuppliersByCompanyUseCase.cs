using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Supplier;

public interface IGetSuppliersByCompanyUseCase
{
    Task<List<SupplierDto>> ExecuteAsync(int companyId);
}