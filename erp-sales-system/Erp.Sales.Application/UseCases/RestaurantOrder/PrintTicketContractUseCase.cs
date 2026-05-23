using Erp.Inventory.Contracts;
using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Application.Services;
using Erp.Sales.Domain.Entities;
using Erp.Sales.Domain.Enums;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class PrintTicketContractUseCase(
    ISalesCenResolver salesCenResolver,
    IRestaurantOrderRepository restaurantOrderRepository,
    IRestaurantOrderDetailRepository restaurantOrderDetailRepository,
    IInventoryService inventoryService,
    IPdfService pdfService) : IPrintTicketContractUseCase
{
    public async Task<byte[]> ExecuteAsync(string companyCen, string ticketCen)
    {
        SalesCenLookup? ticketLookup = await salesCenResolver.ResolveTicketAsync(companyCen, ticketCen);
        if (ticketLookup is null)
        {
            throw new KeyNotFoundException("Ticket no encontrado");
        }

        var ticket = await restaurantOrderRepository.GetByIdAsync(ticketLookup.Id)
                     ?? throw new KeyNotFoundException("Ticket no encontrado");

        List<RestaurantOrderDetail> chargeableDetails = (await restaurantOrderDetailRepository.GetByRestaurantOrderIdAsync(ticket.Id))
            .Where(detail => detail.Status != OrderDetailStatus.Canceled)
            .ToList();

        if (chargeableDetails.Any(detail => string.IsNullOrWhiteSpace(detail.ProductCen)))
        {
            throw new InvalidOperationException("El ticket contiene items sin productCen y no puede imprimirse por contrato CEN");
        }

        List<string> productCens = chargeableDetails
            .Select(detail => detail.ProductCen!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        List<ProductContractDto> products = productCens.Count == 0
            ? []
            : await inventoryService.GetProductsByCensAsync(companyCen, productCens);

        Dictionary<string, ProductContractDto> productsByCen = BuildProductLookup(products);
        List<string> missingProductCens = productCens
            .Where(productCen => !productsByCen.ContainsKey(productCen))
            .ToList();

        if (missingProductCens.Count > 0)
        {
            throw new InvalidOperationException(
                $"No se pudo resolver informacion de productos para el PDF: {string.Join(", ", missingProductCens)}");
        }

        TicketPrintDto ticketPrint = new(
            TicketCen: ticket.Cen,
            DailyNumber: ticket.Order.DailyNumber,
            CreatedAt: ticket.OrderDatetime,
            TaxAmount: ticket.TaxPrice,
            Items: chargeableDetails.Select(detail =>
            {
                ProductContractDto product = productsByCen[detail.ProductCen!.Trim()];
                return new TicketPrintItemDto(
                    ProductCen: detail.ProductCen!.Trim(),
                    ProductName: product.Name,
                    Quantity: detail.Quantity,
                    UnitPrice: product.SalePrice,
                    Note: detail.Note);
            }).ToList());

        return pdfService.GenerateTicketContractPdf(ticketPrint);
    }

    private static Dictionary<string, ProductContractDto> BuildProductLookup(IEnumerable<ProductContractDto> products)
    {
        Dictionary<string, ProductContractDto> productsByCen = new(StringComparer.OrdinalIgnoreCase);
        foreach (ProductContractDto product in products)
        {
            if (!string.IsNullOrWhiteSpace(product.ProductCen))
            {
                productsByCen.TryAdd(product.ProductCen.Trim(), product);
            }

            if (!string.IsNullOrWhiteSpace(product.Sku))
            {
                productsByCen.TryAdd(product.Sku.Trim(), product);
            }
        }

        return productsByCen;
    }
}
