using Nop.Data;
using Nop.Plugin.Misc.OmnichannelCore.Domains;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.OmnichannelCore.ScheduleTasks;

/// <summary>
/// Phase 2 scheduled task: ships <see cref="OmniOutboxMessageStatus.Pending"/> outbox
/// rows to RabbitMQ (exchange <c>commerce</c>, routing key
/// <c>commerce.order.placed.v1</c>) with publisher confirms, then marks each row
/// <see cref="OmniOutboxMessageStatus.Published"/>. On failure it increments
/// <c>RetryCount</c>/<c>LastError</c> and leaves the row pending for the next run.
///
/// Registered as a nopCommerce ScheduleTask (Type =
/// "Nop.Plugin.Misc.OmnichannelCore.ScheduleTasks.OutboxPublisherTask,
/// Nop.Plugin.Misc.OmnichannelCore") in the plugin Install step.
///
/// SCAFFOLD: the outbox scan + status transitions are wired; the actual RabbitMQ
/// publish (with publisher confirms) is stubbed — see the PublishAsync TODO. Wire
/// it against services/contracts Topology in Phase 2 and measure publish lag
/// (target ≤ 60 s from row-created to MQ-published).
/// </summary>
public class OutboxPublisherTask : IScheduleTask
{
    #region Fields

    private readonly IRepository<OmniOutboxMessage> _outboxMessageRepository;
    private readonly ILogger _logger;

    #endregion

    #region Ctor

    public OutboxPublisherTask(IRepository<OmniOutboxMessage> outboxMessageRepository, ILogger logger)
    {
        _outboxMessageRepository = outboxMessageRepository;
        _logger = logger;
    }

    #endregion

    #region Methods

    public async Task ExecuteAsync()
    {
        var pending = await _outboxMessageRepository.Table
            .Where(message => message.StatusId == (int)OmniOutboxMessageStatus.Pending)
            .OrderBy(message => message.Id)
            .Take(100)
            .ToListAsync();

        foreach (var message in pending)
        {
            try
            {
                await PublishAsync(message);

                message.Status = OmniOutboxMessageStatus.Published;
                message.PublishedOnUtc = DateTime.UtcNow;
                message.UpdatedOnUtc = message.PublishedOnUtc;
                await _outboxMessageRepository.UpdateAsync(message);
            }
            catch (Exception exception)
            {
                message.RetryCount += 1;
                message.LastError = exception.Message;
                message.UpdatedOnUtc = DateTime.UtcNow;
                await _outboxMessageRepository.UpdateAsync(message);

                await _logger.ErrorAsync(
                    $"OmnichannelCore outbox publish failed for messageId {message.MessageId}", exception);
            }
        }
    }

    #endregion

    #region Utilities

    private static Task PublishAsync(OmniOutboxMessage message)
    {
        // TODO Phase 2: publish message.Payload to RabbitMQ using the shared
        // Topology (exchange "commerce", routing key "commerce.order.placed.v1")
        // with publisher confirms before returning. Throw on nack/timeout so the
        // catch block retries.
        _ = message;
        return Task.CompletedTask;
    }

    #endregion
}
