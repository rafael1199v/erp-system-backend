namespace Erp.Inventory.Application.Realtime;

public interface IRestockNotifier
{
    ValueTask PublishAsync(RestockEvent restockEvent, CancellationToken cancellationToken = default);

    IAsyncEnumerable<RestockEvent> Subscribe(CancellationToken cancellationToken);
}