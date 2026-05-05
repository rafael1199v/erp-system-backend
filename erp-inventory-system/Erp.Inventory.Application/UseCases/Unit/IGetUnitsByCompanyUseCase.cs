using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Unit;

public interface IGetUnitsByCompanyUseCase
{
    Task<List<UnitDto>> ExecuteAsync(int companyId);
}