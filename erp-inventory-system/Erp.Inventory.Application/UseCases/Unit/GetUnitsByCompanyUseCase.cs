using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;

namespace Erp.Inventory.Application.UseCases.Unit;

public class GetUnitsByCompanyUseCase(IUnitRepository unitRepository) : IGetUnitsByCompanyUseCase
{
    public async Task<List<UnitDto>> ExecuteAsync(int companyId)
    {
        var unitEntities = await unitRepository.GetAllByCompanyIdAsync(companyId);
        return unitEntities.Select(u => new UnitDto(
            Id: u.Id,
            Name: u.Name,
            CompanyId: u.CompanyId,
            Cen: u.Cen,
            Abbreviation: u.Abbreviation)).ToList();
    }
}
