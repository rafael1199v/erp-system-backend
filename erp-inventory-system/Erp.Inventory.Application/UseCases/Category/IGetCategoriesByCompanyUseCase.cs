using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Category;

public interface IGetCategoriesByCompanyUseCase
{
    Task<List<CategoryDto>> ExecuteAsync(int companyId);
}