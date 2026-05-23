using Erp.Purchasing.Application.DTOs;
using Erp.Purchasing.Application.UseCases;
using Erp.Purchasing.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Purchasing.Presentation.Controllers;

[ApiController]
[Route("api/purchases/companies/{companyCen}/orders")]
public class PurchaseOrderController(
    IGetPurchaseUseCase getPurchaseUseCase,
    ICreatePurchaseUseCase createPurchaseUseCase,
    IConfirmPurchaseUseCase confirmPurchaseUseCase) : ControllerBase
{
    [EndpointSummary("Lista ordenes de compra")]
    [EndpointDescription("""
                         Devuelve una lista paginada de ordenes de compra filtradas por estado.
                         Usar para consultar el historial reciente o pendientes.
                         """)]
    [ProducesResponseType(typeof(PagedResultDto<PurchaseOrderListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<IActionResult> GetOrders(
        string companyCen,
        [FromQuery] PurchaseStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool sortDescending = true,
        CancellationToken ct = default)
    {
        var query = new PurchaseOrderQueryDto(status, page, pageSize, sortDescending);
        return Ok(await getPurchaseUseCase.GetPagedAsync(companyCen, query, ct));
    }

    [EndpointSummary("Obtiene el detalle de una orden de compra")]
    [EndpointDescription("""
                         Devuelve el detalle completo de una orden segun su CEN.
                         Usar para ver productos, cantidades y totales antes de confirmar.
                         """)]
    [ProducesResponseType(typeof(PurchaseOrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpGet("{orderCen}")]
    public async Task<IActionResult> GetOrder(string companyCen, string orderCen, CancellationToken ct = default)
    {
        return Ok(await getPurchaseUseCase.GetDetailAsync(companyCen, orderCen, ct));
    }

    [EndpointSummary("Crea una orden de compra")]
    [EndpointDescription("""
                         Registra una nueva orden de compra con sus items.
                         Usar antes de confirmar para que el inventario no se actualice todavia.
                         """)]
    [ProducesResponseType(typeof(PurchaseOrderSummaryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        string companyCen,
        [FromBody] CreatePurchaseOrderDto request,
        CancellationToken ct = default)
    {
        var result = await createPurchaseUseCase.ExecuteAsync(companyCen, request, ct);
        return CreatedAtAction(
            nameof(GetOrder),
            new { companyCen, orderCen = result.OrderCen },
            result);
    }

    [EndpointSummary("Confirma una orden de compra")]
    [EndpointDescription("""
                         Confirma la orden y registra el ingreso de stock.
                         Usar cuando la compra se recepcione para actualizar existencias.
                         Integra con el API de Inventario para incrementar stock.
                         """)]
    [ProducesResponseType(typeof(PurchaseOrderConfirmationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpPost("{orderCen}/confirm")]
    public async Task<IActionResult> ConfirmOrder(string companyCen, string orderCen, CancellationToken ct = default)
    {
        return Ok(await confirmPurchaseUseCase.ExecuteAsync(companyCen, orderCen, ct));
    }
}
