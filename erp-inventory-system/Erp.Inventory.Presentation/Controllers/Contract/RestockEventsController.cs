using System.Text.Json;
using Erp.Inventory.Application.Realtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers.Contract;

[ApiController]
public class RestockEventsController(IRestockNotifier restockNotifier) : ControllerBase
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    [EndpointSummary("Stream de eventos de restock (SSE)")]
    [EndpointDescription("""
                         Server-Sent Events. Emite un evento cada vez que entra stock para la empresa indicada
                         (por ejemplo, al confirmar una compra). El cliente (Ventas) se conecta con EventSource.
                         """)]
    [HttpGet("/api/inventory/companies/{companyCen}/restock-events")]
    public async Task StreamRestockEvents(string companyCen, CancellationToken cancellationToken)
    {
        Response.Headers.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers["X-Accel-Buffering"] = "no";

        await foreach (var restockEvent in restockNotifier.Subscribe(cancellationToken))
        {
            if (!string.Equals(restockEvent.CompanyCen, companyCen, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var json = JsonSerializer.Serialize(restockEvent, SerializerOptions);
            await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}
