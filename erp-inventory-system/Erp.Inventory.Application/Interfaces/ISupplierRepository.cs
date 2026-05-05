using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Interfaces;

public interface ISupplierRepository
{
    Task<List<SupplierEntity>> GetAllByCompany(int companyId);
}