using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Contracts;
using Erp.Inventory.Domain.Enums;
using Erp.Inventory.Presentation.ContractDtos;

namespace Erp.Inventory.Presentation.ContractAdapters;

public class InventoryContractMapper : IInventoryContractMapper
{
    public CompanyContractDto ToCompanyContract(GetCompanyDTO company)
    {
        return new CompanyContractDto
        {
            CompanyCen = company.Cen,
            Name = company.Name,
            IsActive = true
        };
    }

    public CategoryContractDto ToCategoryContract(CategoryDto category)
    {
        return new CategoryContractDto
        {
            CategoryCen = category.Cen,
            Name = category.Name,
            Description = category.Description,
            IsActive = true
        };
    }

    public UnitContractDto ToUnitContract(UnitDto unit)
    {
        return new UnitContractDto
        {
            UnitCen = unit.Cen,
            Name = unit.Name,
            Abbreviation = unit.Abbreviation,
            IsActive = true
        };
    }

    public WarehouseContractDto ToWarehouseContract(WarehouseDTO warehouse)
    {
        return new WarehouseContractDto
        {
            WarehouseCen = warehouse.Cen,
            Name = warehouse.Name,
            IsActive = true
        };
    }

    public ProductContractDto ToProductContract(GetProductCatalogDTO product)
    {
        return new ProductContractDto
        {
            ProductCen = product.ProductCen,
            Sku = product.Sku ?? product.ProductCen,
            Name = product.ProductName,
            Description = product.Description,
            CategoryCen = product.CategoryCen,
            CategoryName = product.CategoryName,
            UnitCen = product.UnitCen,
            UnitName = product.Unit,
            SalePrice = product.SellPrice,
            CostPrice = product.CurrentCost,
            ReorderLevel = product.ReorderLevel,
            Status = ToContractProductStatus(product),
            StationCode = product.StationCode
        };
    }

    public StockItemContractDto ToStockItemContract(GetProductCatalogDTO product, GetWarehouseWithStockDTO warehouse)
    {
        return new StockItemContractDto
        {
            ProductCen = product.ProductCen,
            ProductName = product.ProductName,
            WarehouseCen = warehouse.Cen,
            WarehouseName = warehouse.Name,
            AvailableQuantity = warehouse.Stock,
            ReservedQuantity = 0,
            UnitName = product.Unit,
            ReorderLevel = product.ReorderLevel,
            IsLowStock = warehouse.Stock <= product.ReorderLevel
        };
    }

    public StockValidationContractResponse ToStockValidationContract(
        StockValidationResultDto validationResult,
        IReadOnlyDictionary<int, string> productCensById,
        IReadOnlyDictionary<int, string> warehouseCensById)
    {
        return new StockValidationContractResponse
        {
            IsValid = validationResult.AllAvailable,
            Requirements = validationResult.Insufficiencies.Select(insufficiency => new StockRequirementContractDto
            {
                ProductCen = productCensById.GetValueOrDefault(insufficiency.ProductId, string.Empty),
                ProductName = insufficiency.ProductName,
                WarehouseCen = warehouseCensById.GetValueOrDefault(insufficiency.WarehouseId, string.Empty),
                RequestedQuantity = insufficiency.RequestedQuantity,
                AvailableQuantity = insufficiency.AvailableQuantity,
                MissingQuantity = insufficiency.RequestedQuantity - insufficiency.AvailableQuantity,
                UnitName = string.Empty,
                Reason = InventoryContractAdapterDefaults.InsufficientStockReason
            }).ToList()
        };
    }

    public InventoryDocumentContractDto ToInventoryDocumentContract(InventoryMovementDTO movement, string? documentType = null)
    {
        return new InventoryDocumentContractDto
        {
            DocumentCen = movement.Cen,
            DocumentType = documentType ?? ToContractDocumentType(movement.MovementType, movement.ExternalReference),
            Status = ToContractMovementStatus(movement.MovementStatus),
            Title = movement.Title,
            CreatedAt = ToDateTime(movement.MovementDate),
            TotalItems = movement.Transactions.Count,
            GeneratedMovementCens = movement.Transactions
                .Select(transaction => transaction.Cen)
                .Where(cen => !string.IsNullOrWhiteSpace(cen))
                .ToList()
        };
    }

    public KardexMovementContractDto ToKardexMovementContract(InventoryMovementDTO movement, TransactionDTO transaction)
    {
        return new KardexMovementContractDto
        {
            MovementCen = transaction.Cen,
            DocumentCen = movement.Cen,
            ProductCen = transaction.ProductCen,
            WarehouseCen = transaction.WarehouseCen,
            MovementType = ToContractTransactionType(transaction.TransactionType, movement.MovementType, movement.ExternalReference),
            Quantity = Math.Abs(transaction.Quantity),
            UnitCost = null,
            Reason = transaction.Reason,
            CreatedAt = ToDateTime(transaction.TransactionDate)
        };
    }

    private static string ToContractProductStatus(GetProductCatalogDTO product)
    {
        if (!product.IsActive)
        {
            return InventoryContractAdapterDefaults.InactiveStatus;
        }

        return (ProductStatus)product.StatusCode switch
        {
            ProductStatus.Available => InventoryContractAdapterDefaults.ActiveStatus,
            ProductStatus.OutOfStock => InventoryContractAdapterDefaults.OutOfStockStatus,
            ProductStatus.Discontinued => InventoryContractAdapterDefaults.InactiveStatus,
            _ => InventoryContractAdapterDefaults.InactiveStatus
        };
    }

    private static string ToContractDocumentType(int movementType, string? externalReference)
    {
        return (MovementTypeEnum)movementType switch
        {
            MovementTypeEnum.Receipt => "ENTRY",
            //MovementTypeEnum.Issue when IsSaleReference(externalReference) => "SALE_EXIT",
            MovementTypeEnum.Issue => "EXIT",
            MovementTypeEnum.Adjustment => "ADJUSTMENT",
            _ => "UNKNOWN"
        };
    }

    private static string ToContractTransactionType(int transactionType, int movementType, string? externalReference)
    {
        if ((MovementTypeEnum)movementType == MovementTypeEnum.Adjustment)
        {
            return transactionType == (int)TransactionTypeEnum.Out ? "ADJUSTMENT_OUT" : "ADJUSTMENT_IN";
        }

        return ToContractDocumentType(movementType, externalReference);
    }

    private static string ToContractMovementStatus(int movementStatus)
    {
        return (MovementStatusEnum)movementStatus == MovementStatusEnum.Completed
            ? InventoryContractAdapterDefaults.RegisteredStatus
            : "DRAFT";
    }

    private static DateTime ToDateTime(string value)
    {
        return DateTime.TryParse(value, out DateTime parsedDate)
            ? parsedDate
            : DateTime.UtcNow;
    }

    private static bool IsSaleReference(string? externalReference)
    {
        return externalReference?.StartsWith("SALES_PAYMENT:", StringComparison.OrdinalIgnoreCase) == true;
    }
}
