using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Movement;

public interface ICreateMovementUseCase
{
    Task ExecuteAsync(CreateInventoryMovementDTO createInventoryMovementDto);
}
