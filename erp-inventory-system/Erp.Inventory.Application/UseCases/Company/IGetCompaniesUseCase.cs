using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Company;

public interface IGetCompaniesUseCase
{
    Task<List<GetCompanyDTO>> ExecuteAsync();
}