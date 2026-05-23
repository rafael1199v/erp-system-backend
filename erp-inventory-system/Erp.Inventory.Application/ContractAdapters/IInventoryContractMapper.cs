using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Contracts;
using Erp.Inventory.Application.ContractDtos;

namespace Erp.Inventory.Application.ContractAdapters;

public interface IInventoryContractMapper
{
    CompanyContractDto ToCompanyContract(GetCompanyDTO company);
    CategoryContractDto ToCategoryContract(CategoryDto category);
    UnitContractDto ToUnitContract(UnitDto unit);
    WarehouseContractDto ToWarehouseContract(WarehouseDTO warehouse);
    ProductContractDto ToProductContract(GetProductCatalogDTO product);
    StockItemContractDto ToStockItemContract(GetProductCatalogDTO product, GetWarehouseWithStockDTO warehouse);
    StockValidationContractResponse ToStockValidationContract(
        StockValidationResultDto validationResult,
        IReadOnlyDictionary<int, string> productCensById,
        IReadOnlyDictionary<int, string> warehouseCensById);
    InventoryDocumentContractDto ToInventoryDocumentContract(InventoryMovementDTO movement, string? documentType = null);
    KardexMovementContractDto ToKardexMovementContract(InventoryMovementDTO movement, TransactionDTO transaction);
}
