using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Application.UseCases.RestaurantOrder;
using Erp.Sales.Application.ContractDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Erp.Sales.Application.Constants;

namespace Erp.Sales.Presentation.Controllers.Contract;

[ApiController]
[Route("api/sales/companies/{companyCen}/tickets/{ticketCen}/payment")]
public class TicketPaymentsContractController(
    ISalesCenResolver salesCenResolver,
    ISalesPaymentResolver paymentResolver,
    IProcessRestaurantOrderPaymentUseCase processRestaurantOrderPaymentUseCase)
    : ControllerBase
{
    [EndpointSummary("Procesa el pago de un ticket")]
    [EndpointDescription("""
                         Registra el pago de un ticket usando el metodo indicado.
                         Usar cuando el cliente finaliza la compra.
                         """)]
    [ProducesResponseType(typeof(PayTicketContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProcessRestaurantOrderPaymentResultDto), StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> PayTicket(
        string companyCen,
        string ticketCen,
        [FromBody] PayTicketContractRequest request)
    {
        SalesCenLookup? ticket = await salesCenResolver.ResolveTicketAsync(companyCen, ticketCen);
        if (ticket is null)
        {
            return NotFound(new { message = "Ticket no encontrado" });
        }

        int? paymentTypeId = await paymentResolver.ResolvePaymentIdByCode(request.PaymentMethodCode);

        if (paymentTypeId is null)
        {
            return BadRequest(new { message = "Metodo de pago no valido" });
        }

        ProcessRestaurantOrderPaymentResultDto result = await processRestaurantOrderPaymentUseCase.ExecuteAsync(
            new ProcessRestaurantOrderPaymentDto
            {
                RestaurantOrderId = ticket.Id,
                PaymentTypeId = paymentTypeId.Value
            });

        if (!result.IsSuccess)
        {
            return Conflict(result);
        }

        return Ok(new PayTicketContractResponse
        {
            SaleCen = result.SaleCen ?? string.Empty,
            TicketCen = ticket.Cen,
            Status = PaymentStatus.Paid,
            Subtotal = result.Subtotal,
            TaxAmount = result.TaxAmount,
            Total = result.Total,
            InventoryDocumentCen = result.InventoryDocumentCen
        });
    }
}
