using Erp.Inventory.Contracts;

namespace Erp.Inventory.Presentation.ContractAdapters;

public interface IInventoryStockContractAdapter
{
    Task<InventoryContractResult<List<StockItemContractDto>>> GetStockAsync(
        string companyCen,
        string? productCen = null,
        string? warehouseCen = null);
    Task<InventoryContractResult<StockValidationContractResponse>> ValidateStockAsync(string companyCen, StockValidationContractRequest request);
    Task<InventoryContractResult<StockConsumeContractResponse>> ConsumeStockAsync(string companyCen, StockConsumeContractRequest request);
    Task<InventoryContractResult<string>> IncreaseStockAsync(string companyCen, StockIncreaseContractRequest request);
}
