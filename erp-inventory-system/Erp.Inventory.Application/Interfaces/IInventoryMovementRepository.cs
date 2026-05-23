using Erp.Inventory.Domain.Entities;
using Erp.Inventory.Domain.Enums;

namespace Erp.Inventory.Application.Interfaces;

public interface IInventoryMovementRepository
{
    Task<InventoryMovementEntity> CreateMovementAsync(InventoryMovementEntity inventoryMovementEntity);
    Task<List<InventoryMovementEntity>> GetMovementsByTypeAsync(MovementTypeEnum movementType, int companyId);
    Task<List<InventoryMovementEntity>> GetAll(int companyId);
}
