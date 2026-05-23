namespace Erp.Inventory.Application.DTOs;

public class CreateInventoryMovementDTO
{
    public string Title { get; set; } = string.Empty;
    public string? ExternalReference { get; set; }
    public string MovementDate { get; set; } = string.Empty;
    public int MovementType { get; set; }
    public int MovementStatus { get; set; }
    public int CompanyId { get; set; }
    public List<CreateTransactionDTO> Transactions { get; set; } = new List<CreateTransactionDTO>();
}
