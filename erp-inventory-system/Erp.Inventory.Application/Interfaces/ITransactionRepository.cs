using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Interfaces;

public interface ITransactionRepository
{
    Task<List<TransactionEntity>> GetTransactionsByProduct(int productId);
}