using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Movement;

public interface ICreateAdjustmentMovementUseCase
{
    Task<InventoryMovementDTO> ExecuteAsync(CreateInventoryMovementDTO createInventoryMovementDto);
}
