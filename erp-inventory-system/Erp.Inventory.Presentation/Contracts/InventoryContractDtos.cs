namespace Erp.Inventory.Presentation.Contracts;

public sealed class CompanyContractDto
{
    public required string CompanyCen { get; init; }
    public required string Name { get; init; }
    public bool IsActive { get; init; }
}

public sealed class InventoryDashboardContractDto
{
    public required string CompanyCen { get; init; }
    public int TotalProducts { get; init; }
    public int TotalStockQuantity { get; init; }
    public int LowStockCount { get; init; }
    public int OutOfStockCount { get; init; }
}

public sealed class CategoryContractDto
{
    public required string CategoryCen { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CreateCategoryContractRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}

public sealed class UnitContractDto
{
    public required string UnitCen { get; init; }
    public required string Name { get; init; }
    public string? Abbreviation { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CreateUnitContractRequest
{
    public required string Name { get; init; }
    public string? Abbreviation { get; init; }
}

public sealed class WarehouseContractDto
{
    public required string WarehouseCen { get; init; }
    public required string Name { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CreateWarehouseContractRequest
{
    public required string Name { get; init; }
}

public sealed class ProductContractDto
{
    public required string ProductCen { get; init; }
    public required string Sku { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string CategoryCen { get; init; }
    public required string CategoryName { get; init; }
    public required string UnitCen { get; init; }
    public required string UnitName { get; init; }
    public decimal SalePrice { get; init; }
    public decimal? CostPrice { get; init; }
    public int ReorderLevel { get; init; }
    public required string Status { get; init; }
    public string? StationCode { get; init; }
}

public sealed class CreateProductContractRequest
{
    public required string Sku { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string CategoryCen { get; init; }
    public required string UnitCen { get; init; }
    public decimal SalePrice { get; init; }
    public decimal? CostPrice { get; init; }
    public int ReorderLevel { get; init; }
    public string? StationCode { get; init; }
}

public sealed class UpdateProductContractRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string CategoryCen { get; init; }
    public required string UnitCen { get; init; }
    public decimal SalePrice { get; init; }
    public decimal? CostPrice { get; init; }
    public int ReorderLevel { get; init; }
    public string? StationCode { get; init; }
}

public sealed class UpdateProductStatusContractRequest
{
    public required string Status { get; init; }
    public string? Reason { get; init; }
}

public sealed class ProductCreatedContractDto
{
    public required string ProductCen { get; init; }
    public required string Sku { get; init; }
    public required string Name { get; init; }
    public required string Status { get; init; }
    public int InitialStock { get; init; }
}

public sealed class StockItemContractDto
{
    public required string ProductCen { get; init; }
    public required string ProductName { get; init; }
    public required string WarehouseCen { get; init; }
    public required string WarehouseName { get; init; }
    public int AvailableQuantity { get; init; }
    public int ReservedQuantity { get; init; }
    public required string UnitName { get; init; }
    public int ReorderLevel { get; init; }
    public bool IsLowStock { get; init; }
}

public sealed class StockValidationContractRequest
{
    public required string WarehouseCen { get; init; }
    public required string Source { get; init; }
    public string? ReferenceCen { get; init; }
    public required IReadOnlyList<StockValidationItemContractDto> Items { get; init; }
}

public sealed class StockValidationItemContractDto
{
    public required string ProductCen { get; init; }
    public int Quantity { get; init; }
}

public sealed class StockValidationContractResponse
{
    public bool IsValid { get; init; }
    public required IReadOnlyList<StockRequirementContractDto> Requirements { get; init; }
}

public sealed class StockRequirementContractDto
{
    public required string ProductCen { get; init; }
    public required string ProductName { get; init; }
    public required string WarehouseCen { get; init; }
    public int RequestedQuantity { get; init; }
    public int AvailableQuantity { get; init; }
    public int MissingQuantity { get; init; }
    public string? UnitName { get; init; }
    public required string Reason { get; init; }
}

public sealed class StockConsumeContractRequest
{
    public required string WarehouseCen { get; init; }
    public required string Source { get; init; }
    public required string ReferenceCen { get; init; }
    public string? Reason { get; init; }
    public required IReadOnlyList<StockValidationItemContractDto> Items { get; init; }
}

public sealed class StockConsumeContractResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public string? DocumentCen { get; init; }
    public string? DocumentType { get; init; }
    public IReadOnlyList<string> GeneratedMovementCens { get; init; } = [];
    public IReadOnlyList<StockRequirementContractDto> Requirements { get; init; } = [];
}

public sealed class InventoryDocumentContractRequest
{
    public required string DocumentType { get; init; }
    public required string WarehouseCen { get; init; }
    public string? Reason { get; init; }
    public string? ExternalReference { get; init; }
    public required IReadOnlyList<InventoryDocumentLineContractRequest> Lines { get; init; }
}

public sealed class InventoryDocumentLineContractRequest
{
    public required string ProductCen { get; init; }
    public int Quantity { get; init; }
    public decimal? UnitCost { get; init; }
}

public sealed class InventoryDocumentContractDto
{
    public required string DocumentCen { get; init; }
    public required string DocumentType { get; init; }
    public required string Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public int TotalItems { get; init; }
    public IReadOnlyList<string> GeneratedMovementCens { get; init; } = [];
}

public sealed class StockAdjustmentContractRequest
{
    public required string WarehouseCen { get; init; }
    public string? Reason { get; init; }
    public required IReadOnlyList<StockAdjustmentLineContractRequest> Lines { get; init; }
}

public sealed class StockAdjustmentLineContractRequest
{
    public required string ProductCen { get; init; }
    public int Quantity { get; init; }
    public required string AdjustmentType { get; init; }
}

public sealed class KardexMovementContractDto
{
    public required string MovementCen { get; init; }
    public string? DocumentCen { get; init; }
    public required string ProductCen { get; init; }
    public required string WarehouseCen { get; init; }
    public required string MovementType { get; init; }
    public int Quantity { get; init; }
    public decimal? UnitCost { get; init; }
    public string? Reason { get; init; }
    public DateTime CreatedAt { get; init; }
}
