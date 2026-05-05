using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Interfaces;

public interface ICategoryRepository
{
    Task<int> Create(CategoryEntity categoryEntity);
    Task<List<CategoryEntity>> GetAllByCompany(int companyId);
    Task UpdateAsync(CategoryEntity categoryEntity);
}