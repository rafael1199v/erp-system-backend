using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;
using Erp.Inventory.Domain.Enums;
using Erp.Inventory.Infrastructure.Persistance.Context;
using Erp.Inventory.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Inventory.Infrastructure.Persistance.Repositories;

public class InventoryMovementRepository : IInventoryMovementRepository
{

    private readonly AppDbContext _context;

    public InventoryMovementRepository(AppDbContext context)
    {
        this._context = context;
    }
    
    public async Task<InventoryMovementEntity> CreateMovementAsync(InventoryMovementEntity inventoryMovementEntity)
    {
        InventoryMovement inventoryMovement = this.ToModel(inventoryMovementEntity);
        await _context.InventoryMovements.AddAsync(inventoryMovement);
        await _context.SaveChangesAsync();

        return ToDomain(inventoryMovement);
    }

    public async Task<List<InventoryMovementEntity>> GetMovementsByTypeAsync(MovementTypeEnum movementType, int companyId)
    {
        var movements = await _context.InventoryMovements
            .Include(m => m.Transactions)
            .ThenInclude(t => t.Product)
            .ThenInclude(p => p.CoreProduct)
            .Include(m => m.Transactions)
            .ThenInclude(t => t.Warehouse)
            .Where(m => m.MovementTypeId == (int)movementType && m.CompanyId == companyId).ToListAsync();
        
        return Enumerable.ToList(movements.Select(ToDomain));
    }

    public async Task<List<InventoryMovementEntity>> GetAll(int companyId)
    {
        var movements = await Queryable
            .Where<InventoryMovement>(_context.InventoryMovements
                .Include(m => m.Transactions)
                .ThenInclude(t => t.Product)
                .ThenInclude(p => p.CoreProduct)
                .Include(m => m.Transactions)
                .ThenInclude(t => t.Warehouse), m => m.CompanyId == companyId)
            .ToListAsync();
        
        return  Enumerable.ToList(movements.Select(ToDomain));
    }


    private InventoryMovement ToModel(InventoryMovementEntity inventoryMovementEntity)
    {
        return new InventoryMovement
        {
            Id = inventoryMovementEntity.Id,
            Cen = string.IsNullOrWhiteSpace(inventoryMovementEntity.Cen) ? Guid.NewGuid().ToString() : inventoryMovementEntity.Cen,
            Title = inventoryMovementEntity.Title,
            ExternalReference = inventoryMovementEntity.ExternalReference,
            CompanyId = inventoryMovementEntity.CompanyId,
            MovementDate = inventoryMovementEntity.MovementDate,
            MovementTypeId = (int)inventoryMovementEntity.MovementType,
            MovementStatusId = (int)inventoryMovementEntity.MovementStatus,
            Transactions = inventoryMovementEntity.Transactions.Select(transactionEntity => new Transaction
            {
                Id = transactionEntity.Id,
                Cen = string.IsNullOrWhiteSpace(transactionEntity.Cen) ? Guid.NewGuid().ToString() : transactionEntity.Cen,
                ProductId = transactionEntity.ProductId,
                WarehouseId = transactionEntity.WarehouseId,
                Quantity = transactionEntity.Quantity,
                Reason = transactionEntity.Reason,
                TransactionDate = transactionEntity.TransactionDate,
                TransactionTypeId = (int)transactionEntity.TransactionType,
            }).ToList()
        };
    }


    private InventoryMovementEntity ToDomain(InventoryMovement inventoryMovement)
    {
        return new InventoryMovementEntity
        {
            Id = inventoryMovement.Id,
            Cen = inventoryMovement.Cen,
            Title = inventoryMovement.Title,
            ExternalReference = inventoryMovement.ExternalReference,
            CompanyId = inventoryMovement.CompanyId,
            MovementDate = inventoryMovement.MovementDate,
            MovementType = (MovementTypeEnum)inventoryMovement.MovementTypeId,
            MovementStatus = (MovementStatusEnum)inventoryMovement.MovementStatusId,
            Transactions = inventoryMovement.Transactions.Select(t => new TransactionEntity
            {
                Id = t.Id,
                Cen = t.Cen,
                Quantity = t.Quantity,
                Reason = t.Reason ?? "No reason",
                TransactionDate = t.TransactionDate,
                TransactionType = (TransactionTypeEnum)t.TransactionTypeId,
                WarehouseId = t.WarehouseId,
                ProductId = t.ProductId,
                Product = t.Product is null
                    ? null
                    : new ProductEntity
                    {
                        ProductId = t.Product.Id,
                        Cen = t.Product.Cen,
                        Sku = t.Product.Sku,
                        ProductName = t.Product.CoreProduct?.Name ?? string.Empty,
                        Description = t.Product.Description,
                        StationCode = t.Product.StationCode,
                        Unit = string.Empty,
                        CurrentCost = t.Product.CurrentCost,
                        SellPrice = t.Product.SellPrice,
                        ReorderLevel = t.Product.ReorderLevel,
                        IsActive = t.Product.IsActive
                    },
                Warehouse = t.Warehouse is null
                    ? null
                    : new WarehouseEntity
                    {
                        Id = t.Warehouse.Id,
                        Cen = t.Warehouse.Cen,
                        Name = t.Warehouse.Name,
                        CompanyId = t.Warehouse.CompanyId
                    }
            }).ToList()
        };
    }
}
