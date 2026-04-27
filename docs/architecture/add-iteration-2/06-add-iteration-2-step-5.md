# ADD Iteration 2 — Step 5: Define Interfaces

## What This Step Does

Step 5 defines how the new components communicate. After this step the design is precise enough to implement.

---

## 1. `IOutboxWriter`

```csharp
public interface IOutboxWriter
{
    Task WriteAsync(
        string aggregateType,
        string aggregateId,
        string eventType,
        string payload,
        CancellationToken cancellationToken = default);
}
```

- Called from inside an open `OrderProcessingService` transaction
- Must enlist in the ambient transaction — implementation uses the same `INopDataProvider` / repository as the Order insert
- `aggregateId` is `OrderGuid.ToString()` — used by downstream consumers as the idempotency key
- Throws on failure; the order transaction rolls back

---

## 2. `IOutboxRepository`

```csharp
public interface IOutboxRepository
{
    Task<IReadOnlyList<OutboxEntity>> FetchPendingBatchAsync(
        int batchSize,
        CancellationToken cancellationToken = default);

    Task MarkSentAsync(
        long id,
        DateTime publishedAtUtc,
        CancellationToken cancellationToken = default);

    Task MarkAttemptFailedAsync(
        long id,
        string error,
        CancellationToken cancellationToken = default);
}
```

- `FetchPendingBatchAsync` issues `SELECT … WHERE Status=0 ORDER BY CreatedAtUtc LIMIT @batchSize FOR UPDATE SKIP LOCKED` so concurrent dispatcher instances (future) cannot pick the same rows
- `MarkSentAsync` and `MarkAttemptFailedAsync` are independent transactions; failure to mark sent will cause republish on the next tick (at-least-once)

---

## 3. `OutboxEntity`

```csharp
public class OutboxEntity : BaseEntity
{
    public string AggregateType { get; set; }
    public string AggregateId { get; set; }
    public string EventType { get; set; }
    public string Payload { get; set; }
    public OutboxStatus Status { get; set; }
    public int AttemptCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastAttemptAtUtc { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public string LastError { get; set; }
}

public enum OutboxStatus : byte
{
    Pending = 0,
    Sent = 1,
    Failed = 2
}
```

- Inherits `BaseEntity` to integrate with nopCommerce's repository layer
- `Payload` carries the serialised `OrderPlacedMessage` from Iteration 1 — wire contract unchanged

---

## 4. `OutboxDispatcherTask`

```csharp
public class OutboxDispatcherTask : IScheduleTask
{
    public OutboxDispatcherTask(
        IOutboxRepository repository,
        IRabbitMqConnectionFactory connectionFactory,
        OutboxSettings settings,
        RabbitMqSettings rabbitSettings,
        ILogger logger);

    public Task ExecuteAsync();
}
```

- `IScheduleTask` is the existing nopCommerce contract; the scheduler invokes `ExecuteAsync()` on the configured cadence
- All exceptions are caught and logged inside `ExecuteAsync` — the task never throws, because throwing would prevent the next scheduled run

---

## 5. `OutboxSettings`

```csharp
public class OutboxSettings : ISettings
{
    public int PollIntervalSeconds { get; set; } = 1;
    public int BatchSize { get; set; } = 100;
    public int MaxAttempts { get; set; } = 10;
}
```

---

## 6. Updated `OrderPlacedConsumer`

```csharp
public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    public OrderPlacedConsumer(
        IOutboxWriter outboxWriter,
        RabbitMqSettings settings);

    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        var message = BuildMessage(eventMessage.Order);
        var json = JsonSerializer.Serialize(message);

        await _outboxWriter.WriteAsync(
            aggregateType: "Order",
            aggregateId: eventMessage.Order.OrderGuid.ToString(),
            eventType:   "order.placed",
            payload:     json);
    }
}
```

- No more `IRabbitMqConnectionFactory` dependency — the consumer no longer talks to the broker directly
- The serialised payload is identical to Iteration 1's `OrderPlacedMessage` — wire compatibility is preserved

---

## 7. Updated `PluginNopStartup`

```csharp
public class PluginNopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
        services.AddScoped<IConsumer<OrderPlacedEvent>, OrderPlacedConsumer>();

        services.AddScoped<IOutboxWriter, OutboxWriter>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<OutboxDispatcherTask>();
    }

    public int Order => 3000;
}
```

The dispatcher schedule entry is registered through the plugin's install hook (`InstallAsync`) by inserting a row into the `ScheduleTask` table:

```csharp
await _scheduleTaskRepository.InsertAsync(new ScheduleTask
{
    Name        = "VerdeMart Outbox Dispatcher",
    Type        = "Nop.Plugin.Messaging.RabbitMq.ScheduleTasks.OutboxDispatcherTask",
    Seconds     = 1,
    Enabled     = true,
    StopOnError = false
});
```

---

## Topology — Unchanged

The RabbitMQ exchange, queue, binding, message contract, and routing key from Iteration 1 are reused as-is. This iteration only changes **what writes to the broker and when**, not **what is written**.

| Element | Name | Properties |
|---|---|---|
| Exchange | `verdemart.orders` | direct, durable, no auto-delete |
| Queue | `verdemart.orders.openboxes` | durable, manual ack |
| Binding | queue → exchange | routing key: `order.placed` |
| Message | `order.placed` | persistent (delivery mode 2), JSON |

---

## Interface Dependency Map

```
PluginNopStartup
    registers → IRabbitMqConnectionFactory  (singleton)
    registers → IConsumer<OrderPlacedEvent> (scoped)
    registers → IOutboxWriter               (scoped)
    registers → IOutboxRepository           (scoped)
    registers → OutboxDispatcherTask        (scoped, resolved by scheduler)

OrderPlacedConsumer
    depends on → IOutboxWriter
    depends on → RabbitMqSettings (for serialisation context)
    produces   → OutboxEntity rows (Pending)

OutboxDispatcherTask
    depends on → IOutboxRepository
    depends on → IRabbitMqConnectionFactory
    depends on → OutboxSettings
    depends on → RabbitMqSettings
    consumes   → OutboxEntity rows (Pending → Sent)
    publishes  → exchange: verdemart.orders / key: order.placed

OutboxWriter
    depends on → INopDataProvider (or repository wrapper)
    enlists in → ambient order transaction
```

---

## What Step 6 Will Do

Step 6 produces the updated component view, the updated runtime sequence diagram, and records the two ADRs that this iteration produced.
