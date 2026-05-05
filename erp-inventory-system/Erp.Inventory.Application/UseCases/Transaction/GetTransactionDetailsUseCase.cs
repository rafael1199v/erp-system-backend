using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.Mappers;

namespace Erp.Inventory.Application.UseCases.Transaction;

public class GetTransactionDetailsUseCase : IGetTransactionDetailsUseCase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ITransactionMapper _transactionMapper;
    
    public GetTransactionDetailsUseCase(ITransactionRepository transactionRepository, ITransactionMapper transactionMapper)
    {
        _transactionRepository = transactionRepository;
        _transactionMapper = transactionMapper;
    }
    
    public async Task<TransactionDetailsDTO> ExecuteAsync(int productId)
    {
        var transactionEntities = await _transactionRepository.GetTransactionsByProduct(productId);
        
        var transactionDetails = new TransactionDetailsDTO
        {
            ProductId = productId,
            Transactions = transactionEntities.Select(_transactionMapper.TransactionEntityToTransactionDto).OrderBy(t => t.TransactionDate).ThenBy(t => t.Id).ToList(),
            ProductName = transactionEntities.FirstOrDefault()?.Product!.ProductName!
        };
        
        return transactionDetails;
    }
}