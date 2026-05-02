# ADD Iteration 2 — Step 4: Instantiate Elements and Allocate Responsibilities

## Schema Addition

New table **`Outbox`** in the nopCommerce database. Key columns: `AggregateId` (carries `OrderGuid` as idempotency key), `EventType`, `Payload` (serialised JSON), `Status` (`Pending / Sent / Failed`), `AttemptCount`, `CreatedAtUtc`. Indexed on `(Status, CreatedAtUtc)` for the dispatcher's polling query.

---

## Components

| Component | Type | Responsibility |
|---|---|---|
| `OutboxEntity` | Entity | Strongly-typed mapping of an `Outbox` row |
| `IOutboxWriter` / `OutboxWriter` | Scoped service | Inserts a `Pending` row into the `Outbox` table inside the ambient order transaction; throws on failure, rolling back the order |
| `IOutboxRepository` / `OutboxRepository` | Data access | Fetches pending rows with row-level locking (`FOR UPDATE SKIP LOCKED`); marks rows `Sent` or increments failure count |
| `OutboxDispatcherTask` | `IScheduleTask` | Polls `Outbox` every 1 s; publishes pending rows to RabbitMQ; marks each row `Sent` on success; logs failures without throwing |
| `OrderPlacedConsumer` (modified) | `IConsumer<OrderPlacedEvent>` | Replaces inline `BasicPublish` with a call to `IOutboxWriter`; loses `IRabbitMqConnectionFactory` dependency |
| `OutboxSettings` | `ISettings` | Poll interval (default 1 s), batch size (default 100), max attempts (default 10) |
| `PluginNopStartup` (modified) | `INopStartup` | Adds registrations for `IOutboxWriter`, `IOutboxRepository`, and `OutboxDispatcherTask`; existing registrations unchanged |

---

## Component Relationships

```
nopCommerce request thread (DB transaction open)
        │ PlaceOrderAsync
        ▼
OrderPlacedConsumer
        │ writes → OutboxWriter → INSERT Outbox (Pending)
        ▼
   DB COMMIT ──── request returns to user

──────── separate thread ────────

nopCommerce scheduler (every 1 s)
        ▼
OutboxDispatcherTask
        │ FetchPendingBatchAsync (row lock)
        ▼
OutboxRepository → rows
        │ for each: publish to RabbitMQ → MarkSentAsync
        ▼
RabbitMQ exchange: verdemart.orders / key: order.placed
        ▼
Queue: verdemart.orders.openboxes
```

---

## What Step 5 Will Do

Step 5 defines the interfaces between these components and confirms the RabbitMQ topology is unchanged from Iteration 1.
