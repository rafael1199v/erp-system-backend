using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.UseCases.Unit;

public class UpdateUnitUseCase(IUnitRepository unitRepository) : IUpdateUnitUseCase
{
    public async Task ExecuteAsync(UnitDto unit)
    {
        var unitEntities = await unitRepository.GetAllByCompanyIdAsync(unit.CompanyId);
        var existsSameName = unitEntities.Any(u => u.Id != unit.Id
                                                   && string.Equals(u.Name.Trim(), unit.Name.Trim(), StringComparison.CurrentCultureIgnoreCase));

        if (existsSameName)
        {
            throw new Exception("Una unidad con ese nombre ya existe"); 
        }

        UnitEntity unitEntity = new()
        {
            Id = unit.Id,
            Cen = unit.Cen,
            Name = unit.Name,
            Abbreviation = unit.Abbreviation,
            CompanyId = unit.CompanyId
        };
        
        await unitRepository.UpdateAsync(unitEntity);
    }
}
