using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;
using Erp.Inventory.Infrastructure.Persistance.Context;
using Erp.Inventory.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Inventory.Infrastructure.Persistance.Repositories;

public class ProductWarehouseRepository : IProductWarehouseRepository
{
    private readonly AppDbContext _context;
 
    public ProductWarehouseRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task UpdateStockAsync(List<ProductWarehouseStockEntity> productWarehouseStockEntities)
    {
        foreach (var productWarehouseStockEntity in productWarehouseStockEntities)
        {
            await _context.ProductWarehouses.Where(pw =>
                    pw.ProductId == productWarehouseStockEntity.ProductId &&
                    pw.WarehouseId == productWarehouseStockEntity.WarehouseId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(pw => pw.Quantity, productWarehouseStockEntity.Stock));
        }   
    }

    public async Task<List<ProductWarehouseStockEntity>> GetProductWarehouseStockAsync(List<TransactionEntity> transactions)
    {
        var productIds = transactions.Select(t => t.ProductId).Distinct().ToList();
        var warehouseIds = transactions.Select(t => t.WarehouseId).Distinct().ToList();

        var validPairs = transactions
            .Select(t => new { t.ProductId, t.WarehouseId })
            .ToHashSet();
        
        var rawData = await Queryable
            .Where<ProductWarehouse>(this._context.ProductWarehouses
                .Include(t => t.Product)
                .ThenInclude(p => p.CoreProduct)
                .Include(t => t.Warehouse), pw => productIds.Contains(pw.ProductId) && warehouseIds.Contains(pw.WarehouseId))
            .ToListAsync();
        
        var result = Enumerable
            .Where<ProductWarehouse>(rawData, pw => validPairs.Contains(new { pw.ProductId, pw.WarehouseId }))
            .Select(ToDomain)
            .ToList();

        return result;
    }

    public async Task<bool> ProductHasAvailableStock(int productId, int requestedQuantity, int companyId, int warehouseId)
    {
        return await _context.ProductWarehouses
            .AnyAsync(pw =>
                pw.ProductId == productId && pw.WarehouseId == warehouseId && pw.Product.CompanyId == companyId &&
                pw.Quantity >= requestedQuantity && !pw.Product.IsDeleted);
    }

    public async Task<List<CurrentStockDto>> GetAvailableStockAsync(int companyId, List<int> productIds, List<int> warehouseIds)
    {
        return await _context.ProductWarehouses
            .Where(pw => pw.Product.CompanyId == companyId
                         && productIds.Contains(pw.ProductId)
                         && warehouseIds.Contains(pw.WarehouseId))
            .Select(pw => new CurrentStockDto
            (
                ProductId: pw.ProductId,
                WarehouseId: pw.WarehouseId,
                Quantity: pw.Quantity
            ))
            .ToListAsync();
    }


    private ProductWarehouseStockEntity ToDomain(ProductWarehouse productWarehouseModel)
    {
        return new ProductWarehouseStockEntity
        {
            ProductId = productWarehouseModel.ProductId,
            WarehouseId = productWarehouseModel.WarehouseId,
            Stock = productWarehouseModel.Quantity,
            ProductName = productWarehouseModel.Product.CoreProduct.Name,
            WarehouseName = productWarehouseModel.Warehouse.Name
        };
    }
}