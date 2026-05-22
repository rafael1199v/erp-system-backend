using Erp.Inventory.Application.ContractAdapters;
using Erp.Inventory.Application.ContractDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers.Contract;

[Route("api/inventory/companies/{companyCen}")]
public class InventoryDocumentsContractController(IInventoryMovementContractAdapter movementAdapter)
    : InventoryContractControllerBase
{
    [EndpointSummary("Crea un documento de inventario")]
    [EndpointDescription("""
                         Registra un documento de movimiento (ingreso, salida o ajuste) para una empresa.
                         Usar cuando se requiera respaldar movimientos con un documento formal.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(InventoryDocumentContractDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpPost("documents")]
    public async Task<IActionResult> CreateDocument(string companyCen, [FromBody] InventoryDocumentContractRequest request)
    {
        return ToCreatedResult(await movementAdapter.CreateDocumentAsync(companyCen, request));
    }

    [EndpointSummary("Lista documentos de inventario")]
    [EndpointDescription("""
                         Devuelve documentos por tipo y rango de fechas.
                         Usar para auditoria y conciliacion de movimientos.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(List<InventoryDocumentContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpGet("documents")]
    public async Task<IActionResult> GetDocuments(
        string companyCen,
        [FromQuery] string? documentType,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        return ToActionResult(await movementAdapter.GetDocumentsAsync(companyCen, documentType, from, to));
    }

    [EndpointSummary("Obtiene kardex de un producto")]
    [EndpointDescription("""
                         Devuelve los movimientos de un producto en una bodega y rango de fechas.
                         Usar para seguimiento de existencias y costos.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(List<KardexMovementContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
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
