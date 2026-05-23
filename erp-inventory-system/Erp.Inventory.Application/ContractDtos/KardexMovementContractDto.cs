namespace Erp.Inventory.Application.ContractDtos;

public class KardexMovementContractDto
{
    public string MovementCen { get; set; } = string.Empty;
    public string? DocumentCen { get; set; }
    public string ProductCen { get; set; } = string.Empty;
    public string WarehouseCen { get; set; } = string.Empty;
    public string MovementType { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}
