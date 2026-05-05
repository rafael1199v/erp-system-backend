using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.UseCases.ProductWarehouse;

public class UpdateStockUseCase : IUpdateStockUseCase
{
    private readonly IProductWarehouseRepository _productWarehouseRepository;

    public UpdateStockUseCase(IProductWarehouseRepository productWarehouseRepository)
    {
        this._productWarehouseRepository = productWarehouseRepository;
    }
    
    public async Task ExecuteAsync(List<TransactionEntity> transactions)
    {
        List<ProductWarehouseStockEntity> productWarehouseStockEntities = await _productWarehouseRepository.GetProductWarehouseStockAsync(transactions);
    
        transactions = transactions
            .OrderBy(t => t.ProductId)
            .ThenBy(t => t.WarehouseId)
            .ToList();

        productWarehouseStockEntities = productWarehouseStockEntities
            .OrderBy(pw => pw.ProductId)
            .ThenBy(pw => pw.WarehouseId)
            .ToList();

        for (int i = 0; i < transactions.Count; i++)
        {
            productWarehouseStockEntities[i].UpdateStock(transactions[i].Quantity);
        }
        
        await this._productWarehouseRepository.UpdateStockAsync(productWarehouseStockEntities);
    }
}