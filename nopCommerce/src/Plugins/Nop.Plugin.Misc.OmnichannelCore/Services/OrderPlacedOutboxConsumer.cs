using System.Text.Json;
using Nop.Core.Events;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.OmnichannelCore.Domains;
using Nop.Services.Events;
using Nop.Services.Orders;

namespace Nop.Plugin.Misc.OmnichannelCore.Services;

/// <summary>
/// Phase 2 consumer: when nopCommerce raises <see cref="OrderPlacedEvent"/>, write a
/// durable <see cref="OmniOutboxMessage"/> row in the SAME request, instead of calling
/// the warehouse synchronously on the checkout thread. The scheduled
/// <c>OutboxPublisherTask</c> later ships the row to RabbitMQ.
///
/// This is the outbox half of the assignment's async workflow + reliability decision
/// (QA-5: outbox row written ≤ 100 ms after OrderPlacedEvent; 0 synchronous external
/// HTTP calls in checkout). The reconciler (<c>OutboxReconcilerTask</c>) is the crash
/// safety net described in ADR-0011.
///
/// SCAFFOLD: the row is written with a serialized payload, but the exact payload shape
/// should be aligned with services/contracts CommerceOrderPlaced + the sample in
/// docs/evidence/sample-commerce-order-placed-v1.json (order lines are not yet
/// expanded here — done in Phase 2 with IOrderService.GetOrderItemsAsync).
/// </summary>
public class OrderPlacedOutboxConsumer : IConsumer<OrderPlacedEvent>
{
    #region Fields

    private readonly IRepository<OmniOutboxMessage> _outboxMessageRepository;

    #endregion

    #region Ctor

    public OrderPlacedOutboxConsumer(IRepository<OmniOutboxMessage> outboxMessageRepository)
    {
        _outboxMessageRepository = outboxMessageRepository;
    }

    #endregion

    #region Methods

    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        var order = eventMessage.Order;
        var now = DateTime.UtcNow;
        var messageId = Guid.NewGuid();

        var payload = JsonSerializer.Serialize(new
        {
            messageId,
            correlationId = order.OrderGuid.ToString("D"),
            eventType = OmnichannelCoreDefaults.OrderPlacedEventType,
            occurredOnUtc = now,
            orderGuid = order.OrderGuid,
            orderId = order.Id,
            storeId = order.StoreId
            // TODO Phase 2: include line items (productId, sku, quantity, warehouseId).
        });

        await _outboxMessageRepository.InsertAsync(new OmniOutboxMessage
        {
            MessageId = messageId,
            EventType = OmnichannelCoreDefaults.OrderPlacedEventType,
            CorrelationId = order.OrderGuid.ToString("D"),
            OrderGuid = order.OrderGuid,
            OrderId = order.Id,
            Payload = payload,
            Status = OmniOutboxMessageStatus.Pending,
            RetryCount = 0,
            CreatedOnUtc = now
        });
    }

    #endregion
}
