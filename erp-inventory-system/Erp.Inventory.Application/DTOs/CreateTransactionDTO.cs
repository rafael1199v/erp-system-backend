namespace Erp.Inventory.Application.DTOs;

public class CreateTransactionDTO
{
    public int Quantity { get; set; }
    public string Reason { get; set; } =  string.Empty;
    public string TransactionDate { get; set; } = string.Empty;
    public int TransactionType { get; set; }
    public int ProductId { get; set; }
    public int WarehouseId { get; set; } 
}