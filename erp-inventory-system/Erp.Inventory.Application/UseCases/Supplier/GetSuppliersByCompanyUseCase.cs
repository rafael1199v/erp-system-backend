using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;

namespace Erp.Inventory.Application.UseCases.Supplier;

public class GetSuppliersByCompanyUseCase(ISupplierRepository supplierRepository) : IGetSuppliersByCompanyUseCase
{
    public async Task<List<SupplierDto>> ExecuteAsync(int companyId)
    {
        var supplierEntities = await supplierRepository.GetAllByCompany(companyId);
        return supplierEntities.Select(s => new SupplierDto(Id: s.Id, Name: s.Name)).ToList();
    }
}