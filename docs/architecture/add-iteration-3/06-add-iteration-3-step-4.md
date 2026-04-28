# ADD Iteration 3 — Step 4: Instantiate Elements and Allocate Responsibilities

## What This Step Does

Step 4 turns the seven design concepts from Step 3 into concrete named components. Each gets a name, a type, a location in the codebase, and a defined set of responsibilities. The iteration produces two cohesive sets of components: one inside a new nopCommerce plugin (the synchronous allocation gate), and one inside a new independently-deployable bridge service (the asynchronous consumer).

---

## Part A — nopCommerce Plugin (Allocation Gate)

### Plugin Identity

| Property | Value |
|---|---|
| Plugin name | `Nop.Plugin.Inventory.AllocationGate` |
| Location | `src/Plugins/Nop.Plugin.Inventory.AllocationGate/` |
| Type | nopCommerce plugin (`IPlugin` via `BasePlugin`) |
| Naming convention | Matches existing convention (`Nop.Plugin.Misc.*`, `Nop.Plugin.Search.*`) |

---

### Database Schema Addition

#### Table: `ProductReservation`

| Column | Type | Notes |
|---|---|---|
| `Id` | `int`, PK, identity | inherited via `BaseEntity` |
| `ReservationKey` | `varchar(64)`, not null, unique | `OrderGuid` for web flow; opaque session key for POS |
| `ChannelKey` | `varchar(32)`, not null | `"web"` or `"pos"` |
| `ProductId` | `int`, not null, FK → `Product.Id` | |
| `WarehouseId` | `int`, not null, FK → `Warehouse.Id` | |
| `Quantity` | `int`, not null | Held quantity |
| `Status` | `tinyint`, not null, default 0 | `0=Reserved, 1=Committed, 2=Released, 3=Expired` |
| `ReservedAtUtc` | `datetime(6)`, not null | |
| `ReservedUntilUtc` | `datetime(6)`, not null | TTL — release if past this and still `Reserved` |
| `ConfirmedAtUtc` | `datetime(6)`, nullable | Set on transition to `Committed` |

Index: `(Status, ReservedUntilUtc)` for the release task; `(ProductId, WarehouseId, Status)` for the gate's availability query.

The legacy `ProductWarehouseInventory.ReservedQuantity` field is **not** modified — it has its own lifecycle in existing nopCommerce code paths (referenced in `02-current-state.md`) and the new gate uses `ProductReservation` so the new behaviour is reversible without touching legacy callers.

---

### Components

#### 1. `AllocationGatePlugin`

**Type:** `BasePlugin` implementation
**File:** `AllocationGatePlugin.cs`

**Responsibilities:**
- Plugin entry point — implements `IPlugin`
- On install: create the `ProductReservation` table via FluentMigrator migration; insert the schedule-task entry for `ReleaseExpiredReservationsTask`
- On uninstall: remove the schedule-task entry; the table is left in place to preserve audit history

---

#### 2. `ProductReservation`

**Type:** Entity mapped to the `ProductReservation` table
**File:** `Domain/ProductReservation.cs`

**Responsibilities:**
- Inherits `BaseEntity`
- Fields mirror the table schema
- `ReservationStatus` enum (`Reserved=0, Committed=1, Released=2, Expired=3`)

---

#### 3. `IProductReservationRepository` / `ProductReservationRepository`

**Type:** Data access service over `IRepository<ProductReservation>`
**File:** `Repositories/ProductReservationRepository.cs`

**Responsibilities:**
- `InsertReservationAsync(ProductReservation)` — inserts inside the ambient transaction
- `GetActiveSumByProductAsync(productId, warehouseId)` — returns `SUM(Quantity) WHERE Status=Reserved AND ReservedUntilUtc > now`, used by the gate to compute effective availability
- `MarkCommittedAsync(reservationKey)` / `MarkReleasedAsync(reservationKey)` — status transitions
- `FetchExpiredAsync(int batchSize)` — used by the schedule task

---

#### 4. `IAllocationGate` / `AllocationGate`

**Type:** Service that runs the actual gate logic
**File:** `Services/AllocationGate.cs`

**Responsibilities:**
- `ReserveAsync(reservationKey, channelKey, items, ttl)` — for each item:
  1. Acquire pessimistic row lock on `ProductWarehouseInventory` (DB row-level lock; exact mechanism settled in Step 5 — `SELECT ... FOR UPDATE` on MySQL/PostgreSQL, `WITH (UPDLOCK, HOLDLOCK)` on MSSQL)
  2. Compute effective available = `StockQuantity − ProductReservationRepository.GetActiveSumByProductAsync(...)`
  3. If insufficient, abort with structured failure indicating which item failed
  4. If sufficient, insert a `ProductReservation` row (`Status=Reserved`)
- `ConfirmAsync(reservationKey)` — transitions all rows for the key to `Committed`. Called after the actual stock decrement succeeds (web flow) or after POS commits its sale (POS flow)
- `ReleaseAsync(reservationKey)` — transitions to `Released`. Called on POS cancel or on web checkout failure paths

Returns a strongly-typed `AllocationResult` (success or failure-with-reason) — never throws for "unavailable", reserves exceptions for system errors.

---

#### 5. `AllocationGateProductServiceDecorator`

**Type:** Decorator that wraps the default `IProductService`
**File:** `Services/AllocationGateProductServiceDecorator.cs`

**Responsibilities:**
- Delegates every method to the inner `IProductService` except `AdjustInventoryAsync`
- For `AdjustInventoryAsync` with **negative** `quantityToChange` (a stock decrement), runs the gate logic before delegating:
  1. Computes effective availability under row lock
  2. If insufficient, throws `NopException("Stock unavailable for product {id}")` — caught by `OrderProcessingService.PlaceOrderAsync`'s existing try/catch (verified at line 1634), which converts it into a `PlaceOrderResult` failure with the error message
  3. If sufficient, calls `inner.AdjustInventoryAsync(...)` and lets the inner service do the actual decrement
- For positive `quantityToChange` (stock returns, e.g. on cancel) and other methods, passes through unchanged

This is the integration point that intercepts the existing seam in `OrderProcessingService.MoveShoppingCartItemsToOrderItemsAsync` without modifying core code.

---

#### 6. `AllocationApiController`

**Type:** ASP.NET Core controller exposing the gate to POS
**File:** `Controllers/AllocationApiController.cs`
**Route:** `/api/inventory/...`

**Responsibilities:**
- `POST /api/inventory/reserve` — body: `{ reservationKey, items: [{ productId, warehouseId, quantity }] }`. Calls `IAllocationGate.ReserveAsync(...)`. Returns `200 OK` with reservation details, or `409 Conflict` with the failed-item list. The response body is structured so POS can show "out of stock" at the till.
- `POST /api/inventory/confirm` — body: `{ reservationKey }`. Calls `ConfirmAsync`, transitions reservations to committed, **also** decrements `StockQuantity` via the inner `IProductService` since POS does not flow through `OrderProcessingService`. Returns `200 OK`.
- `POST /api/inventory/release` — body: `{ reservationKey }`. Calls `ReleaseAsync`. Returns `200 OK`.
- Authentication: API token bearer (POS must authenticate; mechanism deferred to operational config — out of scope for QAS-2 itself)

---

#### 7. `ReleaseExpiredReservationsTask`

**Type:** `IScheduleTask` implementation
**File:** `ScheduleTasks/ReleaseExpiredReservationsTask.cs`

**Responsibilities:**
- Runs on a configurable interval (default 30 s)
- Calls `ProductReservationRepository.FetchExpiredAsync(batchSize)` — `WHERE Status=Reserved AND ReservedUntilUtc < now`
- For each expired row: marks `Status=Expired`, the row's effect on availability vanishes from the next gate evaluation
- Does not throw on transient errors — logs and lets the next tick retry, consistent with the pattern from `OutboxDispatcherTask` in Iter 2

---

#### 8. `AllocationSettings`

**Type:** `ISettings` implementation
**File:** `AllocationSettings.cs`

**Fields:**

| Field | Default | Notes |
|---|---|---|
| `WebReservationTtlSeconds` | `30` | Web reservations are short-lived — released by transaction lifecycle, but a TTL bound is held |
| `PosReservationTtlSeconds` | `300` | POS reserves for up to 5 min between reserve and confirm/release |
| `ReleaseTaskBatchSize` | `200` | Rows per release-task tick |
| `ReleaseTaskIntervalSeconds` | `30` | Schedule cadence |

---

#### 9. `PluginNopStartup`

**Type:** `INopStartup` implementation
**File:** `Infrastructure/PluginNopStartup.cs`

**Responsibilities:**
- Registers `IAllocationGate` → `AllocationGate` (scoped)
- Registers `IProductReservationRepository` → `ProductReservationRepository` (scoped)
- Registers `ReleaseExpiredReservationsTask` (scoped, resolved by the scheduler)
- **Decorates** the existing `IProductService` registration with `AllocationGateProductServiceDecorator`. nopCommerce wires Autofac as the underlying container (`Nop.Web/Program.cs` calls `UseServiceProviderFactory(new AutofacServiceProviderFactory())`), so native Autofac `RegisterDecorator` applies. Step 5 finalises the call
- Does not call `MapControllers` itself — controllers under `Controllers/` are picked up by the existing Nop.Web routing convention for plugins

---

## Part B — OpenBoxes Bridge (Independent Service)

### Service Identity

| Property | Value |
|---|---|
| Service name | `VerdeMart.OpenBoxesBridge` |
| Location | `openboxes-bridge/` (sibling to `nopCommerce/` in the project root) |
| Type | .NET 8 Worker Service (`Microsoft.Extensions.Hosting.IHostedService`) |
| Hosting | Single Docker container |
| Justification for .NET | Consistency with the main nopCommerce stack reduces tooling spread; the choice is reversible — the only contract is RabbitMQ + OpenBoxes HTTP |

---

### Database Schema (Bridge-Local)

#### Table: `processed_orders`

| Column | Type | Notes |
|---|---|---|
| `OrderGuid` | `char(36)`, PK | Idempotency key (per ADR-003) |
| `ProcessedAtUtc` | `datetime(6)`, not null | When the bridge created the OpenBoxes fulfillment |
| `OpenBoxesFulfillmentId` | `varchar(128)`, nullable | The identifier OpenBoxes returned (or null if the row predates a successful create) |

Stored in a small bridge-local SQLite or PostgreSQL DB; the choice is incidental and decided at deployment.

---

### Components

#### 10. `BridgeWorker`

**Type:** `BackgroundService` (extends `IHostedService`)
**File:** `Worker.cs`

**Responsibilities:**
- Composition root for the worker — wires the consumer, the OpenBoxes client, and the dedup repository through DI
- Establishes the RabbitMQ connection on startup and tears it down on shutdown
- Hosts the message-handling loop on a managed thread

---

#### 11. `OrderPlacedMessageConsumer`

**Type:** RabbitMQ consumer for `verdemart.orders.openboxes`
**File:** `Consumers/OrderPlacedMessageConsumer.cs`

**Responsibilities:**
- Subscribes to the queue with `autoAck: false` (per ADR-003)
- For each message:
  1. Deserialize `OrderPlacedMessage`; if version > supported, NACK (no requeue) — routes to DLQ
  2. Call `IDedupRepository.HasProcessedAsync(OrderGuid)` — if true, ack and skip
  3. Call `IOpenBoxesClient.CreateFulfillmentAsync(message)` — on success, insert dedup row, ack
  4. On OpenBoxes "duplicate" response, treat as success, insert dedup row with returned id, ack
  5. On other errors, increment a per-message redelivery count (carried via `x-death` header by RabbitMQ); if past threshold, NACK without requeue → DLQ; otherwise NACK with requeue

---

#### 12. `IOpenBoxesClient` / `OpenBoxesClient`

**Type:** Typed HTTP client for OpenBoxes
**File:** `OpenBoxes/OpenBoxesClient.cs`

**Responsibilities:**
- `CreateFulfillmentAsync(OrderPlacedMessage)` — calls the OpenBoxes REST endpoint to create a fulfillment order; returns `CreateFulfillmentResult` (success with id, or duplicate, or failure)
- Owns retry-with-backoff for transient HTTP errors (5xx, timeouts) — bounded inside the call; broader retry is handled by message redelivery
- Reads `OpenBoxesBaseUrl` and credentials from configuration

The exact OpenBoxes endpoint and request shape depends on the spike outcome flagged in Step 1 (OpenBoxes API capability). Step 5 finalises the contract; if the spike reveals OpenBoxes does not natively support fulfillment-order creation by API, the client adapts to whatever entry point exists.

---

#### 13. `IDedupRepository` / `DedupRepository`

**Type:** Repository over the bridge-local `processed_orders` table
**File:** `Persistence/DedupRepository.cs`

**Responsibilities:**
- `HasProcessedAsync(orderGuid)` — `SELECT 1 FROM processed_orders WHERE OrderGuid = @g`
- `RecordProcessedAsync(orderGuid, openBoxesFulfillmentId)` — `INSERT IGNORE` semantics (or `ON CONFLICT DO NOTHING`) so a redelivered message that races itself does not double-insert

---

#### 14. `BridgeSettings`

**Type:** Strongly-typed config (`IOptions<BridgeSettings>`)
**File:** `Configuration/BridgeSettings.cs`

**Fields:**

| Field | Default | Notes |
|---|---|---|
| `RabbitMqHost` | env-driven | host, port, user, password |
| `OrderQueueName` | `verdemart.orders.openboxes` | Matches Iter 1 topology |
| `MaxRedeliveryAttempts` | `5` | Above this → DLQ |
| `OpenBoxesBaseUrl` | env-driven | |
| `OpenBoxesApiKey` | env-driven | |
| `DedupConnectionString` | env-driven | |

---

## Part C — RabbitMQ Topology Additions

| Element | Name | Properties |
|---|---|---|
| Exchange | `verdemart.orders.dlx` | type: direct, durable, auto-delete: false |
| Queue | `verdemart.orders.openboxes.dlq` | durable, no auto-delete |
| Binding | DLQ ← DLX | routing key: `order.placed` |
| Update to existing queue | `verdemart.orders.openboxes` | argument: `x-dead-letter-exchange = verdemart.orders.dlx` |

The dead-letter exchange and queue are declared by the bridge service on startup (so the bridge owns its own consumption topology). Adding `x-dead-letter-exchange` to the existing queue is a backward-compatible argument addition; messages already in the queue at deploy time are unaffected.

---

## Part D — Wire Contract Update

### `OrderPlacedMessage` — extended

The Iter 1 message gains a single field:

```csharp
public record OrderPlacedMessage(
    int OrderId,
    Guid OrderGuid,
    int CustomerId,
    decimal OrderTotal,
    DateTime CreatedOnUtc,
    IReadOnlyList<OrderItemMessage> Items,
    int Version = 1   // NEW
);
```

The `Version` field defaults to `1` so existing publish code compiles without change. Consumers read tolerantly: unknown fields ignored, missing optional fields default. The publisher in Iter 1's plugin is updated to set the field explicitly when serialising — the change is additive.

---

## Component Relationships

```
WEB CHECKOUT                                        POS
   │                                                  │
   │ PlaceOrderAsync                                  │ HTTP POST
   │  → MoveShoppingCartItemsToOrderItemsAsync        │ /api/inventory/reserve
   │   → IProductService.AdjustInventoryAsync         │
   │     (decorated)                                  │
   ▼                                                  ▼
┌────────────────────────────────────────────────────┐
│  AllocationGateProductServiceDecorator             │   │ AllocationApiController
│   ├── if decrement: call IAllocationGate           │   │  POST /reserve
│   └── delegate to inner IProductService            │   │  POST /confirm
└────────────────┬───────────────────────────────────┘   │  POST /release
                 │                                       │
                 ▼                                       ▼
        ┌──────────────────────────────────────────────────┐
        │              IAllocationGate                     │
        │   ReserveAsync / ConfirmAsync / ReleaseAsync     │
        └────────────┬───────────────────────────┬─────────┘
                     │                           │
                     ▼                           ▼
        ProductWarehouseInventory      ProductReservation
        (existing — row-lock target)   (new — TTL-bounded holds)
                                                ▲
                                                │ ReleaseExpiredReservationsTask
                                                │  (IScheduleTask, every 30 s)

[ Existing publish path from Iter 1 + Iter 2 — unchanged ]
                     │
                     ▼
              RabbitMQ ─── verdemart.orders ─── verdemart.orders.openboxes (durable)
                                                         │  x-dead-letter-exchange ─→ DLX → DLQ
                                                         ▼
                                          ┌─────────────────────────────────┐
                                          │ VerdeMart.OpenBoxesBridge       │
                                          │  (separate Docker container)    │
                                          │                                 │
                                          │  BridgeWorker                   │
                                          │   └── OrderPlacedMessageConsumer│
                                          │         ├── IDedupRepository    │
                                          │         │   (processed_orders)  │
                                          │         └── IOpenBoxesClient    │
                                          └────────────────┬────────────────┘
                                                           │ HTTP
                                                           ▼
                                                     OpenBoxes
```

The components inside nopCommerce stay inside `Nop.Plugin.Inventory.AllocationGate`. The bridge components live in their own repository tree at `openboxes-bridge/`. The only shared contract is the RabbitMQ topology and the `OrderPlacedMessage` JSON shape.

---

## What Step 5 Will Do

Step 5 defines the precise interfaces — method signatures, data types, and configuration surfaces — between these components. Specific contracts to settle:

- The exact `IAllocationGate` method shape and `AllocationResult` type
- The wire shape of the `/api/inventory/*` endpoints (request and response JSON)
- The exact Autofac decorator-registration call
- The OpenBoxes API contract once the spike resolves Step 1's open question
- The DLQ message envelope (additional headers for operational triage)
