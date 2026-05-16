using System.Collections.Concurrent;

namespace VerdeMart.OpenBoxesBridge.Messaging;

// Singleton — survives across per-message scopes so retry counts accumulate correctly
// across redeliveries of the same OrderGuid.
public class RetryCounter
{
    private readonly ConcurrentDictionary<Guid, int> _counts = new();

    public int Increment(Guid orderGuid) =>
        _counts.AddOrUpdate(orderGuid, 1, (_, c) => c + 1);

    public void Remove(Guid orderGuid) =>
        _counts.TryRemove(orderGuid, out _);
}
