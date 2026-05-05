using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.UseCases.Category;

public class UpdateCategoryUseCase(ICategoryRepository categoryRepository) : IUpdateCategoryUseCase
{
    public async Task ExecuteAsync(CategoryDto category)
    {
        var categoryEntities = await categoryRepository.GetAllByCompany(category.CompanyId);
        var existsName = categoryEntities.Any(c => string.Equals(c.Name.Trim(), category.Name.Trim(), StringComparison.CurrentCultureIgnoreCase));

        if (existsName)
        {
            throw new Exception($"La categoria con nombre: {category.Name} ya existe");
        }
        
        var categoryEntity = new CategoryEntity
        {
            Id = category.Id,
            Name = category.Name,
            CompanyId =  category.CompanyId
        };
        
        await categoryRepository.UpdateAsync(categoryEntity);
    }
}