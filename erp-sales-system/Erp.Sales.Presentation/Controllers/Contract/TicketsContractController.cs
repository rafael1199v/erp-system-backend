using Erp.Inventory.Contracts;
using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Application.UseCases.RestaurantOrder;
using Erp.Sales.Application.UseCases.RestaurantOrderDetails;
using Erp.Sales.Domain.Entities;
using Erp.Sales.Presentation.ContractDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers.Contract;

[ApiController]
[Route("api/sales/companies/{companyCen}/tickets")]
public class TicketsContractController(
    ISalesCenResolver salesCenResolver,
    ICreateRestaurantOrderUseCase createRestaurantOrderUseCase,
    ICreateRestaurantOrderDetailUseCase createRestaurantOrderDetailUseCase,
    IUpdateRestaurantOrderDetailQuantityUseCase updateRestaurantOrderDetailQuantityUseCase,
    IResendOrderDetailUseCase resendOrderDetailUseCase,
    ISendOrderUseCase sendOrderUseCase,
    IAssignWaiterUseCase assignWaiterUseCase,
    ICancelRestaurantOrderUseCase cancelRestaurantOrderUseCase,
    IPrintTicketContractUseCase printTicketContractUseCase,
    IGetTicketTotalsUseCase getTicketTotalsUseCase,
    IRestaurantOrderRepository restaurantOrderRepository,
    IRestaurantOrderDetailRepository restaurantOrderDetailRepository,
    IInventoryService inventoryService)
    : ControllerBase
{
    [EndpointSummary("Lista tickets del dia")]
    [EndpointDescription("""
                         Devuelve los tickets activos del dia actual.
                         Usar para paneles de operacion o historico corto.
                         """)]
    [ProducesResponseType(typeof(List<TicketContractResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<IActionResult> GetTickets(string companyCen)
    {
        int? companyId = await salesCenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada" });
        }

        List<RestaurantOrder> tickets = await restaurantOrderRepository.GetCurrentDayOrders(companyId.Value);
        return Ok(tickets.Select(ToTicketResponse).ToList());
    }

    [EndpointSummary("Crea un ticket")]
    [EndpointDescription("""
                         Abre un ticket para una nueva orden en la empresa.
                         Usar al iniciar una atencion de mesa o pedido.
                         """)]
    [ProducesResponseType(typeof(TicketContractResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpPost]
    public async Task<IActionResult> CreateTicket(
        string companyCen,
        [FromBody] CreateTicketContractRequest request)
    {
        int? companyId = await salesCenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada" });
        }

        SalesCenLookup? waiter = null;
        if (!string.IsNullOrWhiteSpace(request.WaiterCen))
        {
            waiter = await salesCenResolver.ResolveWaiterAsync(companyCen, request.WaiterCen);
            if (waiter is null)
            {
                return NotFound(new { message = "Mesero no encontrado" });
            }
        }

        int ticketId = await createRestaurantOrderUseCase.ExecuteAsync(new CreateRestaurantOrderDto(
            CustomerId: null,
            WaiterId: waiter?.Id,
            CompanyId: companyId.Value,
            CompanyCen: companyCen));

        if (waiter is not null)
        {
            await assignWaiterUseCase.ExecuteAsync(new AssignWaiterDto(ticketId, waiter.Id));
        }

        RestaurantOrder ticket = await restaurantOrderRepository.GetByIdAsync(ticketId)
                                 ?? throw new InvalidOperationException("No se pudo recuperar el ticket creado");

        TicketContractResponse response = ToTicketResponse(ticket);
        response.WaiterCen = waiter?.Cen;

        return Created(
            $"/api/sales/companies/{Uri.EscapeDataString(companyCen)}/tickets/{Uri.EscapeDataString(ticket.Cen)}",
            response);
    }

    [EndpointSummary("Lista items de un ticket")]
    [EndpointDescription("""
                         Devuelve los items asociados a un ticket.
                         Usar para ver detalle de productos y cantidades.
                         Integra con el API de Inventario para enriquecer datos de producto.
                         """)]
    [ProducesResponseType(typeof(List<TicketItemContractResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpGet("{ticketCen}/items")]
    public async Task<IActionResult> GetTicketItems(string companyCen, string ticketCen)
    {
        SalesCenLookup? ticket = await salesCenResolver.ResolveTicketAsync(companyCen, ticketCen);
        if (ticket is null)
        {
            return NotFound(new { message = "Ticket no encontrado" });
        }

        List<RestaurantOrderDetail> details = await restaurantOrderDetailRepository.GetByRestaurantOrderIdAsync(ticket.Id);
        return Ok(await ToTicketItemResponsesAsync(companyCen, details));
    }

    [EndpointSummary("Agrega un item a un ticket")]
    [EndpointDescription("""
                         Crea un nuevo item dentro del ticket con producto y cantidad.
                         Usar para registrar pedidos de clientes.
                         Integra con el API de Inventario para enriquecer datos de producto.
                         """)]
    [ProducesResponseType(typeof(TicketItemContractResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpPost("{ticketCen}/items")]
    public async Task<IActionResult> CreateTicketItem(
        string companyCen,
        string ticketCen,
        [FromBody] CreateTicketItemContractRequest request)
    {
        SalesCenLookup? ticket = await salesCenResolver.ResolveTicketAsync(companyCen, ticketCen);
        if (ticket is null)
        {
            return NotFound(new { message = "Ticket no encontrado" });
        }

        if (string.IsNullOrWhiteSpace(request.ProductCen))
        {
            return BadRequest(new { message = "productCen es requerido" });
        }

        int ticketItemId = await createRestaurantOrderDetailUseCase.ExecuteAsync(new CreateRestaurantOrderDetail(
            RestaurantOrderId: ticket.Id,
            Note: request.Note,
            Quantity: request.Quantity,
            CreatedAt: null,
            ProductCen: request.ProductCen));

        RestaurantOrderDetail item = await restaurantOrderDetailRepository.GetByIdAsync(ticketItemId)
                                     ?? throw new InvalidOperationException("No se pudo recuperar el item creado");

        List<TicketItemContractResponse> response = await ToTicketItemResponsesAsync(companyCen, [item]);
        string itemCen = response.First().TicketItemCen;

        return Created(
            $"/api/sales/companies/{Uri.EscapeDataString(companyCen)}/tickets/{Uri.EscapeDataString(ticketCen)}/items/{Uri.EscapeDataString(itemCen)}",
            response.First());
    }

    [EndpointSummary("Actualiza un item de ticket")]
    [EndpointDescription("""
                         Modifica cantidad o nota del item en el ticket.
                         Usar para ajustes solicitados por el cliente.
                         Integra con el API de Inventario para enriquecer datos de producto.
                         """)]
    [ProducesResponseType(typeof(TicketItemContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpPatch("{ticketCen}/items/{ticketItemCen}")]
    public async Task<IActionResult> UpdateTicketItem(
        string companyCen,
        string ticketCen,
        string ticketItemCen,
        [FromBody] UpdateTicketItemContractRequest request)
    {
        SalesCenLookup? item = await salesCenResolver.ResolveTicketItemAsync(companyCen, ticketCen, ticketItemCen);
        if (item is null)
        {
            return NotFound(new { message = "Item de ticket no encontrado" });
        }

        await updateRestaurantOrderDetailQuantityUseCase.ExecuteAsync(new UpdateRestaurantOrderDetailQuantityDto(
            RestaurantOrderDetailId: item.Id,
            Quantity: request.Quantity,
            Note: request.Note));

        RestaurantOrderDetail updatedItem = await restaurantOrderDetailRepository.GetByIdAsync(item.Id)
                                            ?? throw new InvalidOperationException("No se pudo recuperar el item actualizado");

        List<TicketItemContractResponse> response = await ToTicketItemResponsesAsync(companyCen, [updatedItem]);
        return Ok(response.First());
    }

    [EndpointSummary("Reenvia un item a cocina")]
    [EndpointDescription("""
                         Marca un item para reenvio en el flujo de cocina.
                         Usar cuando un item debe prepararse nuevamente.
                         Integra con el API de Inventario para enriquecer datos de producto.
                         """)]
    [ProducesResponseType(typeof(TicketItemContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpPost("{ticketCen}/items/{ticketItemCen}/resend")]
    public async Task<IActionResult> ResendTicketItem(string companyCen, string ticketCen, string ticketItemCen)
    {
        SalesCenLookup? item = await salesCenResolver.ResolveTicketItemAsync(companyCen, ticketCen, ticketItemCen);
        if (item is null)
        {
            return NotFound(new { message = "Item de ticket no encontrado" });
        }

        await resendOrderDetailUseCase.ExecuteAsync(item.Id);

        RestaurantOrderDetail updatedItem = await restaurantOrderDetailRepository.GetByIdAsync(item.Id)
                                            ?? throw new InvalidOperationException("No se pudo recuperar el item reenviado");

        List<TicketItemContractResponse> response = await ToTicketItemResponsesAsync(companyCen, [updatedItem]);
        return Ok(response.First());
    }

    [EndpointSummary("Envia un ticket a cocina")]
    [EndpointDescription("""
                         Cambia el estado del ticket para iniciar preparacion.
                         Usar cuando el pedido esta listo para cocina.
                         Integra con el API de Inventario para enriquecer datos de producto.
                         """)]
    [ProducesResponseType(typeof(List<TicketItemContractResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpPost("{ticketCen}/send")]
    public async Task<IActionResult> SendTicket(string companyCen, string ticketCen)
    {
        SalesCenLookup? ticket = await salesCenResolver.ResolveTicketAsync(companyCen, ticketCen);
        if (ticket is null)
        {
            return NotFound(new { message = "Ticket no encontrado" });
        }

        await sendOrderUseCase.ExecuteAsync(ticket.Id);

        List<RestaurantOrderDetail> details = await restaurantOrderDetailRepository.GetByRestaurantOrderIdAsync(ticket.Id);
        return Ok(await ToTicketItemResponsesAsync(companyCen, details));
    }

    [EndpointSummary("Asigna mesero a un ticket")]
    [EndpointDescription("""
                         Asocia un mesero al ticket abierto.
                         Usar cuando se reasigna la atencion de la mesa.
                         """)]
    [ProducesResponseType(typeof(AssignTicketWaiterContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpPut("{ticketCen}/waiter")]
    public async Task<IActionResult> AssignTicketWaiter(
        string companyCen,
        string ticketCen,
        [FromBody] AssignTicketWaiterContractRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.WaiterCen))
        {
            return BadRequest(new { message = "waiterCen es requerido" });
        }

        SalesCenLookup? ticket = await salesCenResolver.ResolveTicketAsync(companyCen, ticketCen);
        if (ticket is null)
        {
            return NotFound(new { message = "Ticket no encontrado" });
        }

        SalesCenLookup? waiter = await salesCenResolver.ResolveWaiterAsync(companyCen, request.WaiterCen);
        if (waiter is null)
        {
            return NotFound(new { message = "Mesero no encontrado" });
        }

        await assignWaiterUseCase.ExecuteAsync(new AssignWaiterDto(ticket.Id, waiter.Id));

        return Ok(new AssignTicketWaiterContractResponse
        {
            TicketCen = ticket.Cen,
            WaiterCen = waiter.Cen,
            WaiterName = waiter.Name ?? string.Empty
        });
    }

    [EndpointSummary("Cancela un ticket")]
    [EndpointDescription("""
                         Cancela un ticket activo por solicitud del cliente o error.
                         Usar antes del pago si el pedido no debe continuar.
                         """)]
    [ProducesResponseType(typeof(CancelTicketContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpPost("{ticketCen}/cancel")]
    public async Task<IActionResult> CancelTicket(
        string companyCen,
        string ticketCen,
        [FromBody] CancelTicketContractRequest? request)
    {
        SalesCenLookup? ticket = await salesCenResolver.ResolveTicketAsync(companyCen, ticketCen);
        if (ticket is null)
        {
            return NotFound(new { message = "Ticket no encontrado" });
        }

        try
        {
            await cancelRestaurantOrderUseCase.ExecuteAsync(new CancelRestaurantOrderDto(ticket.Id));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }

        return Ok(new CancelTicketContractResponse
        {
            TicketCen = ticket.Cen
        });
    }

    [EndpointSummary("Imprime un ticket")]
    [EndpointDescription("""
                         Genera el PDF del ticket para impresion o envio.
                         Usar al cerrar la cuenta o para comprobantes.
                         """)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpGet("{ticketCen}/print")]
    public async Task<IActionResult> PrintTicket(string companyCen, string ticketCen)
    {
        try
        {
            byte[] pdfBytes = await printTicketContractUseCase.ExecuteAsync(companyCen, ticketCen);
            return File(pdfBytes, "application/pdf", $"ticket-{ticketCen}.pdf");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [EndpointSummary("Obtiene totales de un ticket")]
    [EndpointDescription("""
                         Devuelve subtotal, impuesto y total del ticket.
                         Usar para mostrar resumen antes de cobrar.
                         """)]
    [ProducesResponseType(typeof(TicketTotalsContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpGet("{ticketCen}/totals")]
    public async Task<IActionResult> GetTicketTotals(string companyCen, string ticketCen)
    {
        SalesCenLookup? ticket = await salesCenResolver.ResolveTicketAsync(companyCen, ticketCen);
        if (ticket is null)
        {
            return NotFound(new { message = "Ticket no encontrado" });
        }

        try
        {
            TicketTotalsDto totals = await getTicketTotalsUseCase.ExecuteAsync(ticket.Id);
            return Ok(new TicketTotalsContractResponse
            {
                TicketCen = ticket.Cen,
                Subtotal = totals.Subtotal,
                TaxAmount = totals.TaxAmount,
                Total = totals.Total
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    private static TicketContractResponse ToTicketResponse(RestaurantOrder ticket)
    {
        return new TicketContractResponse
        {
            TicketCen = ticket.Cen,
            DailyNumber = ticket.Order.DailyNumber,
            Status = ticket.Order.Status.ToString(),
            CreatedAt = ticket.OrderDatetime.ToString("o"),
            CompanyCen = ticket.CompanyCen,
            TaxAmount = ticket.TaxPrice,
            WaiterCen = ticket.WaiterCen,
        };
    }

    private async Task<List<TicketItemContractResponse>> ToTicketItemResponsesAsync(
        string companyCen,
        List<RestaurantOrderDetail> details)
    {
        if (details.Count == 0)
        {
            return [];
        }

        Dictionary<string, ProductContractDto> productsByCen = await GetProductsByCenAsync(companyCen, details);

        return details.Select(detail =>
        {
            ProductContractDto? product = null;
            if (!string.IsNullOrWhiteSpace(detail.ProductCen))
            {
                productsByCen.TryGetValue(detail.ProductCen, out product);
            }

            return new TicketItemContractResponse
            {
                TicketItemCen = detail.Cen,
                ProductCen = detail.ProductCen ?? string.Empty,
                ProductName = product?.Name ?? "Producto sin CEN",
                Quantity = detail.Quantity,
                UnitPrice = product?.SalePrice ?? 0,
                Note = detail.Note,
                Status = detail.Status.ToString(),
                SentAt = detail.SentAt?.ToString("o"),
                ResendCount = detail.ResendCount
            };
        }).ToList();
    }

    private async Task<Dictionary<string, ProductContractDto>> GetProductsByCenAsync(
        string companyCen,
        List<RestaurantOrderDetail> details)
    {
        if (details.All(detail => string.IsNullOrWhiteSpace(detail.ProductCen)))
        {
            return new Dictionary<string, ProductContractDto>(StringComparer.OrdinalIgnoreCase);
        }

        List<ProductContractDto> products = await inventoryService.GetProductsAsync(companyCen);
        return products.ToDictionary(product => product.ProductCen, StringComparer.OrdinalIgnoreCase);
    }
}
