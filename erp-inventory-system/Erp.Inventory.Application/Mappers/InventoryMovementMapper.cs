using System.Globalization;
using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Domain.Entities;
using Erp.Inventory.Domain.Enums;

namespace Erp.Inventory.Application.Mappers;

public class InventoryMovementMapper : IInventoryMovementMapper
{
    public InventoryMovementEntity CreateInventoryMovementDtoToEntity(CreateInventoryMovementDTO createInventoryMovementDTO)
    {
        return new InventoryMovementEntity
        {
            Id = 0,
            Title = createInventoryMovementDTO.Title,
            ExternalReference = createInventoryMovementDTO.ExternalReference,
            CompanyId = createInventoryMovementDTO.CompanyId,
            MovementDate = DateOnly.ParseExact(createInventoryMovementDTO.MovementDate, "yyyy-MM-dd",
                CultureInfo.InvariantCulture),
            MovementStatus = (MovementStatusEnum)createInventoryMovementDTO.MovementStatus,
            MovementType = (MovementTypeEnum)createInventoryMovementDTO.MovementType,
            Transactions = createInventoryMovementDTO.Transactions.Select(createTransactionDto => new TransactionEntity
            {
                Id = 0,
                Quantity = createTransactionDto.Quantity,
                Reason = createTransactionDto.Reason,
                TransactionDate = DateOnly.ParseExact(createTransactionDto.TransactionDate, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture),
                TransactionType = (TransactionTypeEnum)createTransactionDto.TransactionType,
                WarehouseId =  createTransactionDto.WarehouseId,
                ProductId = createTransactionDto.ProductId
            }).ToList()
        };
    }

    public InventoryMovementDTO InventoryMovementEntityToDto(InventoryMovementEntity inventoryMovementEntity)
    {
        return new InventoryMovementDTO
        {
            Id = inventoryMovementEntity.Id,
            Cen = inventoryMovementEntity.Cen,
            Title = inventoryMovementEntity.Title,
            ExternalReference = inventoryMovementEntity.ExternalReference,
            MovementDate = inventoryMovementEntity.MovementDate.ToString(),
            MovementStatus = (int)inventoryMovementEntity.MovementStatus,
            MovementType = (int)inventoryMovementEntity.MovementType,
            Transactions = inventoryMovementEntity.Transactions.Select(t => new TransactionDTO
            {
                Id = t.Id,
                Cen = t.Cen,
                Quantity = t.Quantity,
                Reason = t.Reason,
                TransactionDate = t.TransactionDate.ToString(),
                TransactionType = (int)t.TransactionType,
                ProductId = t.ProductId,
                ProductCen = t.Product?.Cen ?? string.Empty,
                ProductName = t.Product?.ProductName ?? string.Empty,
                WarehouseId = t.WarehouseId,
                WarehouseCen = t.Warehouse?.Cen ?? string.Empty,
                WarehouseName = t.Warehouse?.Name ?? string.Empty
            }).ToList()
        };
    }
}
