using Erp.Inventory.Contracts;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Application.UseCases.KDS;
using Erp.Sales.Domain.Entities;
using Erp.Sales.Domain.Enums;
using Erp.Sales.Presentation.ContractDtos;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers.Contract;

[ApiController]
[Route("api/sales/companies/{companyCen}/kds")]
public class KdsContractController(
    ISalesCenResolver salesCenResolver,
    IKdsRepository kdsRepository,
    IRestaurantOrderDetailRepository restaurantOrderDetailRepository,
    IInventoryService inventoryService,
    IChangeKdsItemStatusUseCase changeKdsItemStatusUseCase)
    : ControllerBase
{
    [HttpGet("teams")]
    public async Task<IActionResult> GetTeams(string companyCen)
    {
        int? companyId = await salesCenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada" });
        }

        List<KdsTeam> teams = await kdsRepository.GetTeamsByCompanyIdAsync(companyId.Value);
        return Ok(teams.Select(team => new KdsTeamContractResponse
        {
            TeamCen = team.Cen,
            Name = team.Name,
            CategoryCens = team.CategoryCens
        }).ToList());
    }

    [HttpGet("teams/{teamCen}/items")]
    public async Task<IActionResult> GetTeamItems(string companyCen, string teamCen)
    {
        int? companyId = await salesCenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada" });
        }

        SalesCenLookup? teamLookup = await salesCenResolver.ResolveTeamAsync(companyCen, teamCen);
        if (teamLookup is null)
        {
            return NotFound(new { message = "Equipo KDS no encontrado" });
        }

        KdsTeam? team = await kdsRepository.GetTeamByIdAsync(companyId.Value, teamLookup.Id);
        if (team is null)
        {
            return NotFound(new { message = "Equipo KDS no encontrado" });
        }

        var fromUtc = DateTime.UtcNow.AddHours(-48);
        List<KdsOrderDetail> orderItems = await kdsRepository.GetOrderItemsByCompanyIdAsync(companyId.Value, fromUtc);
        if (orderItems.Count == 0)
        {
            return Ok(new List<KdsItemContractResponse>());
        }

        List<KdsItemContractResponse> items = await ToKdsItemResponsesAsync(companyCen, team, orderItems);
        return Ok(items);
    }

    [HttpPatch("items/{ticketItemCen}/status")]
    public async Task<IActionResult> UpdateItemStatus(
        string companyCen,
        string ticketItemCen,
        [FromBody] UpdateKdsItemStatusContractRequest request)
    {
        RestaurantOrderDetail? item = await restaurantOrderDetailRepository.GetByCompanyAndCenAsync(companyCen, ticketItemCen);
        if (item is null)
        {
            return NotFound(new { message = "Item KDS no encontrado" });
        }

        if (!TryParseStatus(request.Status, out OrderDetailStatus status))
        {
            return BadRequest(new { message = "Estado de item KDS no valido" });
        }

        await changeKdsItemStatusUseCase.ExecuteAsync(item.Id, (int)status);

        RestaurantOrderDetail updatedItem = await restaurantOrderDetailRepository.GetByIdAsync(item.Id)
                                            ?? throw new InvalidOperationException("No se pudo recuperar el item actualizado");

        return Ok(new
        {
            ticketItemCen = updatedItem.Cen,
            status = ToContractStatus(updatedItem.Status)
        });
    }

    private async Task<List<KdsItemContractResponse>> ToKdsItemResponsesAsync(
        string companyCen,
        KdsTeam team,
        List<KdsOrderDetail> orderItems)
    {
        List<ProductContractDto> contractProducts = await inventoryService.GetProductsAsync(companyCen);
        var productsByCen = contractProducts.ToDictionary(product => product.ProductCen, StringComparer.OrdinalIgnoreCase);

        HashSet<string> teamCategoryCens = team.CategoryCens.ToHashSet(StringComparer.OrdinalIgnoreCase);

        return orderItems
            .Select(item =>
            {
                productsByCen.TryGetValue(item.ProductCen ?? string.Empty, out ProductContractDto? product);

                bool belongsToTeam = ProductBelongsToTeam(product, teamCategoryCens);
                if (!belongsToTeam)
                {
                    return null;
                }

                return new KdsItemContractResponse
                {
                    TicketItemCen = item.TicketItemCen,
                    TicketCen = item.TicketCen,
                    ProductCen = item.ProductCen ?? product?.ProductCen ?? string.Empty,
                    ProductName = product?.Name ?? "Producto sin CEN",
                    Quantity = item.Quantity,
                    Status = ToContractStatus(item.RestaurantOrderDetailStatus),
                    Note = item.Note,
                    ResendCount = item.ResendCount,
                    CreatedAt = item.CreatedAt.ToString("o")
                };
            })
            .Where(item => item is not null)
            .Select(item => item!)
            .ToList();
    }

    private static bool ProductBelongsToTeam(
        ProductContractDto? product,
        HashSet<string> teamCategoryCens)
    {
        if (teamCategoryCens.Count > 0)
        {
            return product is not null && teamCategoryCens.Contains(product.CategoryCen);
        }

        return false;
    }

    private static bool TryParseStatus(string value, out OrderDetailStatus status)
    {
        string normalizedStatus = value.Trim().Replace("-", "_").ToUpperInvariant();
        status = normalizedStatus switch
        {
            "CREATED" => OrderDetailStatus.Created,
            "PREPARING" => OrderDetailStatus.Preparing,
            "DELIVERED" => OrderDetailStatus.Delivered,
            "CANCELED" => OrderDetailStatus.Canceled,
            "CANCELLED" => OrderDetailStatus.Canceled,
            _ => default
        };

        return status != default;
    }

    private static string ToContractStatus(OrderDetailStatus status)
    {
        return status switch
        {
            OrderDetailStatus.Created => "created",
            OrderDetailStatus.Preparing => "preparing",
            OrderDetailStatus.Delivered => "delivered",
            OrderDetailStatus.Canceled => "canceled",
            _ => status.ToString().ToLowerInvariant()
        };
    }
}
