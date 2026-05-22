using Erp.Inventory.Contracts;
using Erp.Inventory.Application.ContractAdapters;
using Erp.Inventory.Application.ContractDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers.Contract;

[Route("api/inventory/companies/{companyCen}/stock")]
public class InventoryStockContractController(
    IInventoryStockContractAdapter stockAdapter,
    IInventoryMovementContractAdapter movementAdapter)
    : InventoryContractControllerBase
{
    [EndpointSummary("Consulta stock por empresa")]
    [EndpointDescription("""
                         Devuelve existencias por producto y/o bodega.
                         Usar para consultas de disponibilidad y auditoria.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(List<StockItemContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpGet]
    public async Task<IActionResult> GetStock(
        string companyCen,
        [FromQuery] string? productCen,
        [FromQuery] string? warehouseCen)
    {
        return ToActionResult(await stockAdapter.GetStockAsync(companyCen, productCen, warehouseCen));
    }

    [EndpointSummary("Valida disponibilidad de stock")]
    [EndpointDescription("""
                         Evalua si los productos requeridos tienen stock suficiente.
                         Usar antes de consumos o ventas para prevenir faltantes.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(StockValidationContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateStock(string companyCen, [FromBody] StockValidationContractRequest request)
    {
        return ToActionResult(await stockAdapter.ValidateStockAsync(companyCen, request));
    }

    [EndpointSummary("Consume stock")]
    [EndpointDescription("""
                         Registra la salida de stock segun un requerimiento.
                         Usar para consumos desde ventas o procesos internos.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(StockConsumeContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpPost("consume")]
    public async Task<IActionResult> ConsumeStock(string companyCen, [FromBody] StockConsumeContractRequest request)
    {
        return ToActionResult(await stockAdapter.ConsumeStockAsync(companyCen, request));
    }
    
    [EndpointSummary("Incrementa stock")]
    [EndpointDescription("""
                         Registra ingreso de stock por compras u otras fuentes.
                         Usar cuando se recepcionen productos en bodega.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpPost("increase")]
    public async Task<IActionResult> IncreaseStock(string companyCen, [FromBody] StockIncreaseContractRequest request)
    {
        return ToActionResult(await stockAdapter.IncreaseStockAsync(companyCen, request));
    } 
    
    [EndpointSummary("Crea un ajuste de inventario")]
    [EndpointDescription("""
                         Registra un ajuste manual para corregir diferencias de stock.
                         Usar cuando se requiere cuadrar inventario por conteo fisico.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(InventoryAdjustmentContractResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpPost("adjustments")]
    public async Task<IActionResult> CreateAdjustment(string companyCen,
        [FromBody] InventoryAdjustmentContractRequest request)
    {
        return ToCreatedResult(await movementAdapter.CreateAdjustmentAsync(companyCen, request));
    }
}
