using Erp.Inventory.Contracts;
using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Application.UseCases.RestaurantOrder;
using Erp.Sales.Domain.Entities;
using Erp.Sales.Presentation.ContractDtos;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers.Contract;

[ApiController]
[Route("api/sales/companies/{companyCen}/tickets")]
public class TicketsContractController(
    ISalesCenResolver salesCenResolver,
    ICreateRestaurantOrderUseCase createRestaurantOrderUseCase,
    ICreateRestaurantOrderDetailUseCase createRestaurantOrderDetailUseCase,
    IAssignWaiterUseCase assignWaiterUseCase,
    IRestaurantOrderRepository restaurantOrderRepository,
    IRestaurantOrderDetailRepository restaurantOrderDetailRepository,
    IInventoryService inventoryService)
    : ControllerBase
{
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

        if (request.ProductId <= 0)
        {
            return BadRequest(new { message = "productId es requerido temporalmente para crear items desde la ruta CEN" });
        }

        if (string.IsNullOrWhiteSpace(request.ProductCen))
        {
            return BadRequest(new { message = "productCen es requerido" });
        }

        int ticketItemId = await createRestaurantOrderDetailUseCase.ExecuteAsync(new CreateRestaurantOrderDetail(
            RestaurantOrderId: ticket.Id,
            ProductId: request.ProductId,
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

    private static TicketContractResponse ToTicketResponse(RestaurantOrder ticket)
    {
        return new TicketContractResponse
        {
            TicketCen = ticket.Cen,
            DailyNumber = ticket.Order.DailyNumber,
            Status = ticket.Order.Status.ToString(),
            CreatedAt = ticket.OrderDatetime.ToString("o"),
            CompanyCen = ticket.CompanyCen,
            TaxAmount = ticket.TaxPrice
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
                ProductName = product?.Name ?? $"Producto {detail.ProductId}",
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
