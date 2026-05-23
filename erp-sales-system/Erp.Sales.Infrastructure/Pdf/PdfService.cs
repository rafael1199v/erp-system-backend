using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Services;
using Erp.Sales.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Erp.Sales.Infrastructure.Pdf;

public class PdfService : IPdfService
{
    public byte[] GenerateRestaurantOrderPdf(int restaurantOrderId, List<RestaurantOrderDetailDto> restaurantOrderDetails)
    {
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A6);
                page.Margin(1, Unit.Centimetre);
                page.Content().Column(col =>
                {
                    col.Item().Text($"Orden #{restaurantOrderId}")
                        .FontSize(18).Bold().AlignCenter();

                    col.Item().Text("REIMPRESIÓN")
                        .FontSize(10).AlignCenter();

                    col.Item().LineHorizontal(1);
                    
                    foreach (var detail in restaurantOrderDetails.Where(detail => detail.RestaurantOrderStatusId != (int)OrderDetailStatus.Canceled))
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"{detail.Quantity}x {detail.Name}");
                            row.AutoItem().Text($"Bs. {detail.UnitPrice * detail.Quantity:F2}");
                        });

                        if (!string.IsNullOrEmpty(detail.Note))
                            col.Item().Text($"{detail.Note}").FontSize(9);
                    }

                    col.Item().LineHorizontal(1);
                    col.Item().Text("— Fin de la orden —")
                        .FontSize(9).AlignCenter();
                });
            });
        }).GeneratePdf();


        return pdf;
    }

    public byte[] GenerateTicketContractPdf(TicketPrintDto ticket)
    {
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A6);
                page.Margin(1, Unit.Centimetre);
                page.Content().Column(col =>
                {
                    col.Item().Text($"Ticket #{ticket.DailyNumber}")
                        .FontSize(18).Bold().AlignCenter();

                    col.Item().Text(ticket.TicketCen)
                        .FontSize(8).AlignCenter();

                    col.Item().Text(ticket.CreatedAt.ToString("yyyy-MM-dd HH:mm"))
                        .FontSize(8).AlignCenter();

                    col.Item().Text("REIMPRESION")
                        .FontSize(10).AlignCenter();

                    col.Item().LineHorizontal(1);

                    foreach (var item in ticket.Items)
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"{item.Quantity}x {item.ProductName}");
                            row.AutoItem().Text($"Bs. {item.UnitPrice * item.Quantity:F2}");
                        });

                        if (!string.IsNullOrWhiteSpace(item.Note))
                        {
                            col.Item().Text(item.Note).FontSize(9);
                        }
                    }

                    col.Item().LineHorizontal(1);
                    col.Item().Text("Fin de la orden")
                        .FontSize(9).AlignCenter();
                });
            });
        }).GeneratePdf();

        return pdf;
    }
}
