using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.UseCases.Category;

public class GetCategoriesByCompanyUseCase(ICategoryRepository categoryRepository) : IGetCategoriesByCompanyUseCase
{
    public async Task<List<CategoryDto>> ExecuteAsync(int companyId)
    {
        List<CategoryEntity> categoryEntities = await categoryRepository.GetAllByCompany(companyId);
        return categoryEntities.Select(c => new CategoryDto(
            Id: c.Id,
            Name: c.Name,
            CompanyId: c.CompanyId,
            Cen: c.Cen,
            Description: c.Description)).ToList();
    }
}
