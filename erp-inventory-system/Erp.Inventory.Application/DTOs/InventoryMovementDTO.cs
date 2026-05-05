namespace Erp.Inventory.Application.DTOs;

public class InventoryMovementDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MovementDate { get; set; } = string.Empty;
    public int MovementType { get; set; }
    public int MovementStatus { get; set; }
    public List<TransactionDTO> Transactions { get; set; } = new List<TransactionDTO>();
}