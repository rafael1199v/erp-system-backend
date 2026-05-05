using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Transaction;

public interface IGetTransactionDetailsUseCase
{
    Task<TransactionDetailsDTO> ExecuteAsync(int productId);
}