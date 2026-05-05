using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Unit;

public interface IUpdateUnitUseCase
{
    Task ExecuteAsync(UnitDto unit);
}