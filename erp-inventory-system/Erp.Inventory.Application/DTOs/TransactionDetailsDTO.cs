namespace Erp.Inventory.Application.DTOs;

public class TransactionDetailsDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = String.Empty;
    public List<TransactionDTO> Transactions { get; set; } = new List<TransactionDTO>();
}