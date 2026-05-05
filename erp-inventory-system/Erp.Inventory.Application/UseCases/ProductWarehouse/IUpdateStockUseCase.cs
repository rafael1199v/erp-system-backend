using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.UseCases.ProductWarehouse;

public interface IUpdateStockUseCase
{
    Task ExecuteAsync(List<TransactionEntity> transactions);
}