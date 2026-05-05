using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.Mappers;
using Erp.Inventory.Application.UseCases.ProductWarehouse;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.UseCases.Movement;

public class CreateAdjustmentMovementUseCase : ICreateAdjustmentMovementUseCase
{
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IInventoryMovementMapper _inventoryMovementMapper;
    private readonly IProductRepository _productRepository;
    private readonly IUpdateStockUseCase _updateStockUseCase;

    public CreateAdjustmentMovementUseCase(
        IInventoryMovementRepository inventoryMovementRepository, 
        IInventoryMovementMapper inventoryMovementMapper,
        IProductRepository productRepository,
        IUpdateStockUseCase updateStockUseCase)
    {
        this._inventoryMovementRepository = inventoryMovementRepository;
        this._inventoryMovementMapper = inventoryMovementMapper;
        this._productRepository = productRepository;
        this._updateStockUseCase = updateStockUseCase;
    }
    
    public async Task ExecuteAsync(CreateInventoryMovementDTO createInventoryMovementDto)
    {
        InventoryMovementEntity inventoryMovementEntity =
            this._inventoryMovementMapper.CreateInventoryMovementDtoToEntity(createInventoryMovementDto);
        
        inventoryMovementEntity.MakeAdjustment();
        inventoryMovementEntity.ApplyOperation();
        
        await this._updateStockUseCase.ExecuteAsync(inventoryMovementEntity.Transactions);
        await this._inventoryMovementRepository.CreateMovementAsync(inventoryMovementEntity);
    }

   
}