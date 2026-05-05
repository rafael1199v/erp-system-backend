namespace Erp.Inventory.Application.DTOs;

public record LowStockDashboardDto(
    int ProductId,
    string ProductName,
    int TotalStock,
    int ReorderLevel,
    string StockState
);