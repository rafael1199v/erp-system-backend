using Erp.Purchasing.Application.Exceptions;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace Erp.Purchasing.Infrastructure.Http;

public sealed class InventoryFailureTranslatingHandler : DelegatingHandler
{
    private const string Message =
        "El módulo de Inventario no está disponible en este momento. Intenta nuevamente en unos segundos.";

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException
                                      or BrokenCircuitException
                                      or TimeoutRejectedException
                                      or TaskCanceledException)
        {
            throw new InventoryUnavailableException(Message, ex);
        }
    }
}
