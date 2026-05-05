using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.Mappers;
using Erp.Inventory.Application.UseCases.ProductWarehouse;
using Erp.Inventory.Domain.Entities;
using Erp.Inventory.Domain.Enums;

namespace Erp.Inventory.Application.UseCases.Movement;

public class CreateMovementUseCase : ICreateMovementUseCase
{

    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IInventoryMovementMapper _inventoryMapper;
    private readonly IUpdateStockUseCase _updateStockUseCase;
    private readonly IProductRepository _productRepository;


    public CreateMovementUseCase(
        IInventoryMovementRepository inventoryMovementRepository,
        IInventoryMovementMapper inventoryMovementMapper,
        IUpdateStockUseCase updateStockUseCase,
        IProductRepository productRepository
    )
    {
        _inventoryMovementRepository = inventoryMovementRepository;
        _inventoryMapper = inventoryMovementMapper;
        _updateStockUseCase = updateStockUseCase;
        _productRepository = productRepository;
    }

    public async Task ExecuteAsync(CreateInventoryMovementDTO createInventoryMovementDto)
    {
        InventoryMovementEntity inventoryMovementEntity = this._inventoryMapper.CreateInventoryMovementDtoToEntity(createInventoryMovementDto);

        if (inventoryMovementEntity.MovementType == MovementTypeEnum.Issue)
        {
            List<int> productIds = inventoryMovementEntity.Transactions
                .Select(transaction => transaction.ProductId)
                .Distinct()
                .ToList();

            bool allProductsAreActive = await _productRepository.AreAllActiveAsync(
                inventoryMovementEntity.CompanyId,
                productIds
            );

            if (!allProductsAreActive)
            {
                throw new Exception("No se puede registrar salidas con productos desactivados");
            }
        }

        inventoryMovementEntity.ApplyOperation();

        await this._updateStockUseCase.ExecuteAsync(inventoryMovementEntity.Transactions);

        await this._inventoryMovementRepository.CreateMovementAsync(inventoryMovementEntity);

    }
}
