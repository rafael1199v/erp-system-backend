using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.UseCases.Category;

public class CreateCategoryUseCase(ICategoryRepository categoryRepository) : ICreateCategoryUseCase
{
    public async Task<int> ExecuteAsync(CreateCategoryDto createCategoryDto)
    {
        CategoryEntity category =
            CategoryEntity.Create(
                name: createCategoryDto.Name,
                companyId: createCategoryDto.CompanyId,
                description: createCategoryDto.Description);

        return await categoryRepository.Create(category);
    }
}
