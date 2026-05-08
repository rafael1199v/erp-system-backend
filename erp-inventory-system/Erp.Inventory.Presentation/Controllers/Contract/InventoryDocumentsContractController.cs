using Erp.Inventory.Presentation.ContractAdapters;
using Erp.Inventory.Presentation.ContractDtos;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers.Contract;

[Route("api/inventory/companies/{companyCen}")]
public class InventoryDocumentsContractController(IInventoryMovementContractAdapter movementAdapter)
    : InventoryContractControllerBase
{
    [HttpPost("documents")]
    public async Task<IActionResult> CreateDocument(string companyCen, [FromBody] InventoryDocumentContractRequest request)
    {
        return ToCreatedResult(await movementAdapter.CreateDocumentAsync(companyCen, request));
    }

    [HttpGet("documents")]
    public async Task<IActionResult> GetDocuments(
        string companyCen,
        [FromQuery] string? documentType,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        return ToActionResult(await movementAdapter.GetDocumentsAsync(companyCen, documentType, from, to));
    }

    [HttpGet("products/{productCen}/kardex")]
    public async Task<IActionResult> GetKardex(
        string companyCen,
        string productCen,
        [FromQuery] string? warehouseCen,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        return ToActionResult(await movementAdapter.GetKardexAsync(companyCen, productCen, warehouseCen, from, to));
    }
}
