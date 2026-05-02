# ADD Iteration 2 — Step 5: Define Interfaces

## Key Interfaces

**`IOutboxWriter`** — `WriteAsync(aggregateType, aggregateId, eventType, payload)`; must be called inside an open order transaction and enlist in it; throws on failure, rolling back the order.

**`IOutboxRepository`** — three methods: `FetchPendingBatchAsync(batchSize)` (row-locked poll, safe for future multi-instance deployments), `MarkSentAsync(id, publishedAtUtc)`, `MarkAttemptFailedAsync(id, error)`; the latter two are independent transactions — a failure to mark causes safe republish on the next tick.

**`OutboxEntity`** — entity inheriting `BaseEntity`; fields: `AggregateType`, `AggregateId`, `EventType`, `Payload`, `Status` (`Pending/Sent/Failed`), `AttemptCount`, `CreatedAtUtc`, `LastAttemptAtUtc`, `PublishedAtUtc`, `LastError`.

**`OutboxDispatcherTask`** (`IScheduleTask`) — `ExecuteAsync()`; depends on `IOutboxRepository`, `IRabbitMqConnectionFactory`, `OutboxSettings`, `RabbitMqSettings`; never throws — all exceptions are caught and logged so the scheduler can continue.

**`OutboxSettings`** (`ISettings`) — `PollIntervalSeconds` (default 1), `BatchSize` (default 100), `MaxAttempts` (default 10).

---

## Topology — Unchanged

This iteration only changes what writes to the broker and when, not what is written.

| Element | Name | Properties |
|---|---|---|
| Exchange | `verdemart.orders` | direct, durable, no auto-delete |
| Queue | `verdemart.orders.openboxes` | durable, manual ack |
| Binding | queue → exchange | routing key: `order.placed` |
| Message | `order.placed` | persistent, JSON — wire contract identical to Iteration 1 |

---

## Dependency Map

```
PluginNopStartup
    registers → IRabbitMqConnectionFactory  (singleton)
    registers → IConsumer<OrderPlacedEvent> (scoped)
    registers → IOutboxWriter               (scoped)
    registers → IOutboxRepository           (scoped)
    registers → OutboxDispatcherTask        (scoped, resolved by scheduler)

OrderPlacedConsumer
    depends on → IOutboxWriter
    produces   → OutboxEntity rows (Pending)

OutboxDispatcherTask
    depends on → IOutboxRepository
    depends on → IRabbitMqConnectionFactory
    depends on → OutboxSettings, RabbitMqSettings
    publishes  → exchange: verdemart.orders / key: order.placed

OutboxWriter
    enlists in → ambient order transaction
```

---

## What Step 6 Will Do

Step 6 produces the updated component view and sequence diagram, and records the architectural decision this iteration produced.
