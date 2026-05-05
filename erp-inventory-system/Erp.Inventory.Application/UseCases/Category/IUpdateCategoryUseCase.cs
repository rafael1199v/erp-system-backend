using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Category;

public interface IUpdateCategoryUseCase
{
    Task ExecuteAsync(CategoryDto category);
}