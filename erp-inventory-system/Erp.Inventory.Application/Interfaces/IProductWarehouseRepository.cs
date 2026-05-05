using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Interfaces;

public interface IProductWarehouseRepository
{
    Task UpdateStockAsync(List<ProductWarehouseStockEntity> productWarehouseStockEntities);
    Task<List<ProductWarehouseStockEntity>> GetProductWarehouseStockAsync(List<TransactionEntity> transactions);
    Task<bool> ProductHasAvailableStock(int productId, int requestedQuantity, int companyId, int warehouseId);
    Task<List<CurrentStockDto>> GetAvailableStockAsync(int companyId, List<int> productIds, List<int> warehouseIds);
}