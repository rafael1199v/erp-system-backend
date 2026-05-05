using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Unit;

public interface ICreateUnitUseCase
{
    Task<int> ExecuteAsync(CreateUnitDto createUnitDto);
}