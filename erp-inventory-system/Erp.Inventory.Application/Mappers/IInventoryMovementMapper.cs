using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Mappers;

public interface IInventoryMovementMapper
{ 
    InventoryMovementEntity CreateInventoryMovementDtoToEntity(CreateInventoryMovementDTO createInventoryMovementDTO);
    InventoryMovementDTO InventoryMovementEntityToDto(InventoryMovementEntity inventoryMovementEntity);
}