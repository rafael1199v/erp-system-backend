using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Interfaces;

public interface IUnitRepository
{
    Task<int> CreateAsync(UnitEntity unit);
    Task<List<UnitEntity>> GetAllByCompanyIdAsync(int companyId);
    Task UpdateAsync(UnitEntity unit);
}