namespace Erp.Sales.Application.DTOs;

public class StockInsufficiencyResponseDto
{
    public int ProductId { get; set; }
    public string? ProductCen { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? WarehouseCen { get; set; }
    public int RequestedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public int MissingQuantity { get; set; }
}
