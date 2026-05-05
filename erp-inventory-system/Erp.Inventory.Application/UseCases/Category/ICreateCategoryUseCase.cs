using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Category;

public interface ICreateCategoryUseCase
{
    Task<int> ExecuteAsync(CreateCategoryDto createCategoryDto);
}