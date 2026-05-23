namespace Erp.Inventory.Application.DTOs;

public class TransactionDTO
{
    public int Id { get; set; }
    public string Cen { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string TransactionDate { get; set; } = string.Empty;
    public int TransactionType { get; set; }
    public int ProductId { get; set; }
    public string ProductCen { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseCen { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
}
