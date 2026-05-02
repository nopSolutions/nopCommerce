using RabbitMQ.Client.Exceptions;

namespace OutboxSpike;

public sealed class OutboxDispatcher
{
    private readonly OutboxRepository _repo;
    private readonly RabbitPublisher _publisher;
    private readonly TimeSpan _pollInterval;
    private readonly Action<string> _log;

    public OutboxDispatcher(OutboxRepository repo, RabbitPublisher publisher, Action<string> log, TimeSpan? pollInterval = null)
    {
        _repo = repo;
        _publisher = publisher;
        _log = log;
        _pollInterval = pollInterval ?? TimeSpan.FromSeconds(1);
    }

    public async Task RunAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await TickAsync();
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _log($"Dispatcher tick error: {ex.Message}");
            }

            try { await Task.Delay(_pollInterval, ct); }
            catch (OperationCanceledException) { break; }
        }
    }

    private async Task TickAsync()
    {
        var pending = (await _repo.GetPendingAsync(10)).ToList();
        if (pending.Count == 0)
            return;

        foreach (var msg in pending)
        {
            try
            {
                _publisher.Publish(msg.Id, msg.Payload);
                await _repo.MarkSentAsync(msg.Id);
                _log($"Published {msg.Id} → verdemart.orders | Status=Sent");
            }
            catch (BrokerUnreachableException)
            {
                _log($"Broker unreachable — {pending.Count} row(s) remain Pending");
                // reset publisher so next tick tries a fresh connection
                break;
            }
            catch (AlreadyClosedException)
            {
                _log($"Broker connection closed — {pending.Count} row(s) remain Pending");
                break;
            }
            catch (Exception ex)
            {
                _log($"Publish error for {msg.Id}: {ex.GetType().Name} — {ex.Message}");
                break;
            }
        }
    }
}
