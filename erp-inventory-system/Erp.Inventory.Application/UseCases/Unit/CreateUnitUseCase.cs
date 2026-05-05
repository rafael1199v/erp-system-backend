using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.UseCases.Unit;

public class CreateUnitUseCase(IUnitRepository unitRepository) : ICreateUnitUseCase
{
    public async Task<int> ExecuteAsync(CreateUnitDto createUnitDto)
    {
        UnitEntity unit = UnitEntity.Create(name: createUnitDto.Name, companyId: createUnitDto.CompanyId);

        return await unitRepository.CreateAsync(unit);
    }
}