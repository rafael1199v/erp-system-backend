using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Erp.Inventory.Application.Realtime;

namespace Erp.Inventory.Infrastructure.Realtime;

public sealed class RestockNotifier : IRestockNotifier
{
    private readonly ConcurrentDictionary<Guid, Channel<RestockEvent>> _subscribers = new();

    public ValueTask PublishAsync(RestockEvent restockEvent, CancellationToken cancellationToken = default)
    {
        foreach (var subscriber in _subscribers.Values)
        {
            subscriber.Writer.TryWrite(restockEvent);
        }

        return ValueTask.CompletedTask;
    }

    public async IAsyncEnumerable<RestockEvent> Subscribe(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var channel = Channel.CreateBounded<RestockEvent>(new BoundedChannelOptions(capacity: 100)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        _subscribers[id] = channel;

        try
        {
            await foreach (var restockEvent in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return restockEvent;
            }
        }
        finally
        {
            _subscribers.TryRemove(id, out _);
            channel.Writer.TryComplete();
        }
    }
}
