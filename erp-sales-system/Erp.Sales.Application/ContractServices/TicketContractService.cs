using Erp.Inventory.Contracts;
using Erp.Sales.Application.ContractDtos;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;

namespace Erp.Sales.Application.ContractServices;

public class TicketContractService(IInventoryService inventoryService) : ITicketContractService
{
    public TicketContractResponse ToTicketResponse(RestaurantOrder ticket)
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

    public async Task<List<TicketItemContractResponse>> ToTicketItemResponsesAsync(
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
