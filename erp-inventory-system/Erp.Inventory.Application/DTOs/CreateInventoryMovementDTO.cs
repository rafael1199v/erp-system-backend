namespace Erp.Inventory.Application.DTOs;

public class CreateInventoryMovementDTO
{
    public string? Cen { get; set; }
    public string? ExternalReference { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MovementDate { get; set; } = string.Empty;
    public int MovementType { get; set; }
    public int MovementStatus { get; set; }
    public int CompanyId { get; set; }
    public List<CreateTransactionDTO> Transactions { get; set; } = new List<CreateTransactionDTO>();
}
