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
    
    public async Task CreateMovementAsync(InventoryMovementEntity inventoryMovementEntity)
    {
        InventoryMovement inventoryMovement = this.ToModel(inventoryMovementEntity);
        await _context.InventoryMovements.AddAsync(inventoryMovement);
        await _context.SaveChangesAsync();
    }

    public async Task<List<InventoryMovementEntity>> GetMovementsByTypeAsync(MovementTypeEnum movementType, int companyId)
    {
        var movements = await _context.InventoryMovements
            .Include(m => m.Transactions)    
            .Where(m => m.MovementTypeId == (int)movementType && m.CompanyId == companyId).ToListAsync();
        
        return Enumerable.ToList(movements.Select(ToDomain));
    }

    public async Task<List<InventoryMovementEntity>> GetAll(int companyId)
    {
        var movements = await Queryable
            .Where<InventoryMovement>(_context.InventoryMovements
                .Include(m => m.Transactions), m => m.CompanyId == companyId)
            .ToListAsync();
        
        return  Enumerable.ToList(movements.Select(ToDomain));
    }


    private InventoryMovement ToModel(InventoryMovementEntity inventoryMovementEntity)
    {
        return new InventoryMovement
        {
            Id = inventoryMovementEntity.Id,
            Title = inventoryMovementEntity.Title,
            CompanyId = inventoryMovementEntity.CompanyId,
            MovementDate = inventoryMovementEntity.MovementDate,
            MovementTypeId = (int)inventoryMovementEntity.MovementType,
            MovementStatusId = (int)inventoryMovementEntity.MovementStatus,
            Transactions = inventoryMovementEntity.Transactions.Select(transactionEntity => new Transaction
            {
                Id = transactionEntity.Id,
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
            Title = inventoryMovement.Title,
            CompanyId = inventoryMovement.CompanyId,
            MovementDate = inventoryMovement.MovementDate,
            MovementType = (MovementTypeEnum)inventoryMovement.MovementTypeId,
            MovementStatus = (MovementStatusEnum)inventoryMovement.MovementStatusId,
            Transactions = inventoryMovement.Transactions.Select(t => new TransactionEntity
            {
                Id = t.Id,
                Quantity = t.Quantity,
                Reason = t.Reason ?? "No reason",
                TransactionDate = t.TransactionDate,
                TransactionType = (TransactionTypeEnum)t.TransactionTypeId,
                WarehouseId = t.WarehouseId,
                ProductId = t.ProductId
            }).ToList()
        };
    }
}