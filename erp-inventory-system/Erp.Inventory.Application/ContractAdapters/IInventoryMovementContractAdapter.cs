using Erp.Inventory.Application.ContractDtos;

namespace Erp.Inventory.Application.ContractAdapters;

public interface IInventoryMovementContractAdapter
{
    Task<InventoryContractResult<InventoryDocumentContractDto>> CreateDocumentAsync(string companyCen, InventoryDocumentContractRequest request);
    Task<InventoryContractResult<List<InventoryDocumentContractDto>>> GetDocumentsAsync(
        string companyCen,
        string? documentType = null,
        DateTime? from = null,
        DateTime? to = null);
    Task<InventoryContractResult<InventoryAdjustmentContractResponse>> CreateAdjustmentAsync(string companyCen, InventoryAdjustmentContractRequest request);
    Task<InventoryContractResult<List<KardexMovementContractDto>>> GetKardexAsync(
        string companyCen,
        string productCen,
        string? warehouseCen = null,
        DateTime? from = null,
        DateTime? to = null);
}
