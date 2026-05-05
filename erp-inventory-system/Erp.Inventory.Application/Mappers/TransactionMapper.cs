using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Mappers;

public class TransactionMapper : ITransactionMapper
{
    public TransactionDTO TransactionEntityToTransactionDto(TransactionEntity transactionEntity)
    {
        return new TransactionDTO
        {
            Id = transactionEntity.Id,
            Quantity = transactionEntity.Quantity,
            Reason = transactionEntity.Reason,
            TransactionDate = transactionEntity.TransactionDate.ToString("yyyy-MM-dd"),
            TransactionType = (int)transactionEntity.TransactionType,
            ProductId = transactionEntity.ProductId,
            WarehouseId = transactionEntity.WarehouseId
        };
    }
}