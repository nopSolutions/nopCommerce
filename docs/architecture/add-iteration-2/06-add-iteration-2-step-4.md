# ADD Iteration 2 — Step 4: Instantiate Elements and Allocate Responsibilities

## What This Step Does

Step 4 turns the design concepts from Step 3 into concrete named components. Each gets a name, a type, a location in the codebase, and a defined set of responsibilities.

---

## Database Schema Addition

### Table: `Outbox`

| Column | Type | Notes |
|---|---|---|
| `Id` | `bigint`, PK, identity | |
| `AggregateType` | `varchar(64)`, not null | e.g. `"Order"` |
| `AggregateId` | `varchar(64)`, not null | `OrderGuid` — idempotency key downstream |
| `EventType` | `varchar(128)`, not null | e.g. `"order.placed"` |
| `Payload` | `text`, not null | Serialised JSON — the existing `OrderPlacedMessage` |
| `Status` | `tinyint`, not null, default 0 | `0=Pending, 1=Sent, 2=Failed` |
| `AttemptCount` | `int`, not null, default 0 | |
| `CreatedAtUtc` | `datetime(6)`, not null | |
| `LastAttemptAtUtc` | `datetime(6)`, nullable | |
| `PublishedAtUtc` | `datetime(6)`, nullable | |
| `LastError` | `varchar(1024)`, nullable | |

---

## Components

### 1. `OutboxEntity`

**Type:** Plain entity mapped to the `Outbox` table
**File:** `Domain/OutboxEntity.cs`

**Responsibilities:**
- Strongly-typed representation of an Outbox row
- Fields mirror the table schema exactly

---

### 2. `IOutboxWriter` / `OutboxWriter`

**Type:** Service called by `OrderPlacedConsumer`
**File:** `Outbox/OutboxWriter.cs`

**Responsibilities:**
- Single method `WriteAsync(string aggregateType, string aggregateId, string eventType, string payload)`
- Inserts a row into the `Outbox` table inside the **ambient database transaction** — uses the same `INopDataProvider` / repository as the order write so the insert enlists in the open transaction
- Sets `Status = Pending`, `CreatedAtUtc = DateTime.UtcNow`, `AttemptCount = 0`
- Returns nothing; failure throws and rolls back the order transaction

---

### 3. `IOutboxRepository` / `OutboxRepository`

**Type:** Data access layer for the dispatcher
**File:** `Outbox/OutboxRepository.cs`

**Responsibilities:**
- `FetchPendingBatchAsync(int batchSize)` — `SELECT … FROM Outbox WHERE Status=0 ORDER BY CreatedAtUtc LIMIT @batchSize FOR UPDATE SKIP LOCKED`
- `MarkSentAsync(long id, DateTime publishedAtUtc)` — sets `Status=Sent`, `PublishedAtUtc=…`
- `MarkAttemptFailedAsync(long id, string error)` — increments `AttemptCount`, sets `LastAttemptAtUtc` and `LastError`
- The polling query uses row-level locking so multiple dispatcher instances (if ever introduced) cannot publish the same row

---

### 4. `OutboxDispatcherTask`

**Type:** `IScheduleTask` implementation
**File:** `ScheduleTasks/OutboxDispatcherTask.cs`

**Responsibilities:**
- Implements `IScheduleTask.ExecuteAsync()`
- Per execution:
  1. Open a connection
  2. Call `_outboxRepository.FetchPendingBatchAsync(batchSize)`
  3. For each row: parse the message, call `_connectionFactory.CreateChannel().BasicPublish(...)`, then `MarkSentAsync(...)`
  4. On any publish exception: `MarkAttemptFailedAsync(...)` and continue with the next row
  5. Return without throwing — the next scheduled execution retries failed rows
- Registered with the nopCommerce scheduler at plugin install with a 1-second interval (configurable)

---

### 5. `OrderPlacedConsumer` (modified)

**Type:** Existing component — body of `HandleEventAsync` changes
**File:** `Consumers/OrderPlacedConsumer.cs`

**Change:**
- Old body: `BasicPublish(...)` directly on the broker
- New body: build the same `OrderPlacedMessage`, serialise it, call `_outboxWriter.WriteAsync("Order", order.OrderGuid.ToString(), "order.placed", json)`
- The DI graph for `OrderPlacedConsumer` gains `IOutboxWriter` as a dependency and loses `IRabbitMqConnectionFactory`

---

### 6. `OutboxSettings`

**Type:** `ISettings` implementation
**File:** `Outbox/OutboxSettings.cs`

**Fields:**

| Field | Default | Notes |
|---|---|---|
| `PollIntervalSeconds` | `1` | Scheduler interval |
| `BatchSize` | `100` | Rows per dispatcher tick |
| `MaxAttempts` | `10` | After which the row is logged but kept retried (no DLQ in this iteration) |

---

### 7. `PluginNopStartup` (modified)

**File:** `Infrastructure/PluginNopStartup.cs`

**New registrations:**

```csharp
services.AddScoped<IOutboxWriter, OutboxWriter>();
services.AddScoped<IOutboxRepository, OutboxRepository>();
services.AddScoped<OutboxDispatcherTask>();
```

`IRabbitMqConnectionFactory` registration unchanged — the dispatcher uses it.

---

## Component Relationships

```
nopCommerce request thread
        │
        │ PlaceOrderAsync (DB transaction open)
        ▼
OrderProcessingService
        │ commits Order + StockQuantity
        │ fires OrderPlacedEvent
        ▼
OrderPlacedConsumer  (in same TX)
        │ builds OrderPlacedMessage
        ▼
OutboxWriter
        │ INSERT into Outbox (Pending)
        ▼
   DB COMMIT  ────────────────  request returns to user

──────────── time passes, separate thread ─────────────

nopCommerce scheduler
        │ tick
        ▼
OutboxDispatcherTask
        │ FetchPendingBatchAsync()
        ▼
OutboxRepository
        │ rows
        ▼
OutboxDispatcherTask
        │ for each row:
        │   RabbitMqConnectionFactory.CreateChannel()
        │   BasicPublish to verdemart.orders / order.placed
        │   MarkSentAsync(id)
        ▼
RabbitMQ exchange: verdemart.orders
        │ existing topology — unchanged
        ▼
Queue: verdemart.orders.openboxes
```

---

## What Step 5 Will Do

Step 5 defines the precise interfaces — method signatures, data types, and configuration shape — between these components.
