using Nop.Data;
using Nop.Plugin.Misc.OmnichannelCore.Domains;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.OmnichannelCore.ScheduleTasks;

/// <summary>
/// Crash safety net (ADR-0011). The <see cref="OrderPlacedOutboxConsumer"/> writes the
/// outbox row inside the order-placed event handler; if that handler throws after the
/// order commits but before the row is written, the order would have no integration
/// trail. This task periodically scans recent orders that have no
/// <see cref="OmniOutboxMessage"/> and back-fills the missing row, so "every placed
/// order eventually produces an outbox message" holds even across crashes.
///
/// Verification gate (plan.md Phase 2): reconciler runs on schedule and reports 0
/// missing rows on a healthy run.
///
/// SCAFFOLD: the missing-row detection + back-fill is wired against recent orders;
/// the lookback window and payload shape mirror the consumer and must be kept in sync.
/// </summary>
public class OutboxReconcilerTask : IScheduleTask
{
    #region Fields

    private readonly IOrderService _orderService;
    private readonly IRepository<OmniOutboxMessage> _outboxMessageRepository;
    private readonly ILogger _logger;

    #endregion

    #region Ctor

    public OutboxReconcilerTask(IOrderService orderService,
        IRepository<OmniOutboxMessage> outboxMessageRepository,
        ILogger logger)
    {
        _orderService = orderService;
        _outboxMessageRepository = outboxMessageRepository;
        _logger = logger;
    }

    #endregion

    #region Methods

    public async Task ExecuteAsync()
    {
        // Look back over a recent window; orders older than this are assumed settled.
        var lookbackFromUtc = DateTime.UtcNow.AddHours(-24);

        var recentOrders = await _orderService.SearchOrdersAsync(
            createdFromUtc: lookbackFromUtc,
            createdToUtc: DateTime.UtcNow);

        var orderGuidsWithOutbox = (await _outboxMessageRepository.Table
                .Where(message => message.OrderGuid != null)
                .Select(message => message.OrderGuid!.Value)
                .ToListAsync())
            .ToHashSet();

        var missing = 0;
        foreach (var order in recentOrders)
        {
            if (orderGuidsWithOutbox.Contains(order.OrderGuid))
                continue;

            missing += 1;
            var now = DateTime.UtcNow;
            var messageId = Guid.NewGuid();

            await _outboxMessageRepository.InsertAsync(new OmniOutboxMessage
            {
                MessageId = messageId,
                EventType = OmnichannelCoreDefaults.OrderPlacedEventType,
                CorrelationId = order.OrderGuid.ToString("D"),
                OrderGuid = order.OrderGuid,
                OrderId = order.Id,
                Payload = string.Empty, // TODO Phase 2: serialize same shape as the consumer.
                Status = OmniOutboxMessageStatus.Pending,
                RetryCount = 0,
                CreatedOnUtc = now
            });
        }

        if (missing > 0)
            await _logger.WarningAsync($"OmnichannelCore reconciler back-filled {missing} missing outbox row(s)");
    }

    #endregion
}
