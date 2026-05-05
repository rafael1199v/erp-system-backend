using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.Mappers;
using Erp.Inventory.Domain.Enums;

namespace Erp.Inventory.Application.UseCases.Movement;

public class GetMovementsUseCase : IGetMovementsUseCase
{
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IInventoryMovementMapper _inventoryMovementMapper;

    public GetMovementsUseCase(IInventoryMovementRepository inventoryMovementRepository, IInventoryMovementMapper inventoryMovementMapper)
    {
        this._inventoryMovementRepository = inventoryMovementRepository;
        this._inventoryMovementMapper = inventoryMovementMapper;
    }


    public async Task<List<InventoryMovementDTO>> GetMovementsByTypeAsync(int movementType, int companyId)
    {
        var movementEntities = await _inventoryMovementRepository.GetMovementsByTypeAsync((MovementTypeEnum)movementType, companyId);
        return movementEntities.Select(_inventoryMovementMapper.InventoryMovementEntityToDto).ToList();
    }

    public async Task<List<InventoryMovementDTO>> GetMovementsAsync(int companyId)
    {
        var movementEntities = await _inventoryMovementRepository.GetAll(companyId);
        return movementEntities.Select(_inventoryMovementMapper.InventoryMovementEntityToDto).ToList();
    }
}