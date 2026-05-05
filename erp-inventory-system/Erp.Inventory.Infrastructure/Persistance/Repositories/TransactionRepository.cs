using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;
using Erp.Inventory.Domain.Enums;
using Erp.Inventory.Infrastructure.Persistance.Context;
using Erp.Inventory.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Inventory.Infrastructure.Persistance.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        this._context = context;
    }
    
    public async Task<List<TransactionEntity>> GetTransactionsByProduct(int productId)
    {
        var transactions = await Queryable
            .Where<Transaction>(_context.Transactions
                .Include(t => t.Product)
                .ThenInclude(p => p.CoreProduct)
            
                .Include(t => t.Product)
                .ThenInclude(p => p.Unit)
            
                .Include(t => t.TransactionType)
                .Include(t => t.Warehouse), t => t.ProductId == productId)
            .ToListAsync();
        
        return Enumerable.ToList(transactions.Select(ToDomain));
    }

    private TransactionEntity ToDomain(Transaction transactionModel)
    {
        return new TransactionEntity
        {
            Id = transactionModel.Id,
            Quantity = transactionModel.Quantity,
            Reason = transactionModel.Reason ?? "No reason provided",
            TransactionDate = transactionModel.TransactionDate,
            TransactionType = (TransactionTypeEnum)transactionModel.TransactionType.Id,
            ProductId = transactionModel.ProductId,
            WarehouseId = transactionModel.WarehouseId,
            Product = new ProductEntity
            {
                ProductId = transactionModel.Product.Id,
                ProductName = transactionModel.Product.CoreProduct.Name,
                Unit = transactionModel.Product.Unit.Name,
                CurrentCost = transactionModel.Product.CurrentCost,
                ImageUrl = transactionModel.Product.CoreProduct.ImageUrl,
                ReorderLevel = transactionModel.Product.ReorderLevel,
                IsActive = transactionModel.Product.IsActive
            },
            Warehouse = new WarehouseEntity
            {
                Id = transactionModel.Warehouse.Id,
                Name = transactionModel.Warehouse.Name
            }
        };
    }
}