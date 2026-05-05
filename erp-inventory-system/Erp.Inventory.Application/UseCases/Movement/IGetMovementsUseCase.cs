using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Movement;

public interface IGetMovementsUseCase
{
    Task<List<InventoryMovementDTO>> GetMovementsByTypeAsync(int movementType, int companyId); 
    Task<List<InventoryMovementDTO>> GetMovementsAsync(int companyId);
}