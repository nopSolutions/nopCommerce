# ADD Iteration 3 — Step 4: Instantiate Elements and Allocate Responsibilities

This iteration produces two cohesive component sets: a nopCommerce plugin (the allocation gate) and an independently-deployable bridge service.

---

## Part A — `Nop.Plugin.Inventory.AllocationGate`

**Location:** `src/Plugins/Nop.Plugin.Inventory.AllocationGate/`

**Schema addition:** new table `ProductReservation` — key columns: `ReservationKey` (idempotency key, unique), `ChannelKey` (`"web"` / `"pos"`), `ProductId`, `WarehouseId`, `Quantity`, `Status` (`Reserved / Committed / Released / Expired`), `ReservedUntilUtc` (TTL). The legacy `ProductWarehouseInventory.ReservedQuantity` field is not modified — the new behaviour is reversible.

| Component | Type | Responsibility |
|---|---|---|
| `AllocationGatePlugin` | `BasePlugin` | Plugin entry; creates `ProductReservation` table and registers `ReleaseExpiredReservationsTask` on install |
| `ProductReservation` | Entity | TTL-bounded reservation row; status transitions: `Reserved → Committed / Released / Expired` |
| `IProductReservationRepository` | Data access | Insert reservations; sum active quantity for availability check; status transitions; fetch expired rows |
| `IAllocationGate` / `AllocationGate` | Scoped service | `Reserve / Confirm / Release` with pessimistic row lock on `ProductWarehouseInventory`; computes effective availability as `StockQuantity − active reservations`; returns structured result, never throws for "unavailable" |
| `AllocationGateProductServiceDecorator` | `IProductService` decorator | Intercepts `AdjustInventoryAsync` for negative quantities (decrements); runs the gate; throws `NopException` on insufficient stock (caught at `OrderProcessingService.cs:1634`) |
| `AllocationApiController` | ASP.NET Core controller | Exposes `POST /api/inventory/{reserve\|confirm\|release}` for POS; bearer-token auth |
| `ReleaseExpiredReservationsTask` | `IScheduleTask` | Marks `ProductReservation` rows `Expired` where TTL has passed; runs every 30 s; never throws |
| `AllocationSettings` | `ISettings` | Web and POS TTL values, release task batch size and interval |
| `PluginNopStartup` | `INopStartup` | Registers gate services; decorates `IProductService` with `AllocationGateProductServiceDecorator` via DI descriptor swap |

---

## Part B — `VerdeMart.OpenBoxesBridge`

**Location:** `openboxes-bridge/` (separate repository tree)  
**Hosting:** independent .NET worker service in its own Docker container  
**Schema addition (bridge-local):** table `processed_orders` — columns: `OrderGuid` (PK), `ProcessedAtUtc`, `OpenBoxesFulfillmentId`.

| Component | Type | Responsibility |
|---|---|---|
| `BridgeWorker` | `BackgroundService` | Composition root; manages RabbitMQ connection lifecycle |
| `OrderPlacedMessageConsumer` | RabbitMQ consumer | Deduplicates on `OrderGuid`; calls `IOpenBoxesClient`; NACKs to DLQ after configurable retry threshold |
| `IOpenBoxesClient` / `OpenBoxesClient` | Typed HTTP client | `CreateFulfillmentAsync` — calls the OpenBoxes REST API; bounded retry-with-backoff for transient errors |
| `IDedupRepository` / `DedupRepository` | Data access | `HasProcessedAsync` / `RecordProcessedAsync` on the bridge-local `processed_orders` table |
| `BridgeSettings` | Typed config | RabbitMQ connection, queue name, max redelivery attempts, OpenBoxes URL and credentials |

---

## Part C — RabbitMQ Topology Additions

| Element | Name | Properties |
|---|---|---|
| Exchange | `verdemart.orders.dlx` | direct, durable |
| Queue | `verdemart.orders.openboxes.dlq` | durable |
| Binding | DLQ ← DLX | routing key: `order.placed` |
| Existing queue update | `verdemart.orders.openboxes` | adds `x-dead-letter-exchange` argument |

---

## Part D — Wire Contract Update

`OrderPlacedMessage` gains a `Version` field (default `1`). Consumers read tolerantly — unknown fields ignored, missing optional fields default. Breaking changes use a new routing key and parallel queue binding.

---

## Component Relationships

```
WEB CHECKOUT                              POS
    │ AdjustInventoryAsync                 │ POST /api/inventory/reserve
    ▼ (decorated)                          ▼
AllocationGateProductServiceDecorator  AllocationApiController
    └── IAllocationGate ────────────────────┘
            │ pessimistic lock on ProductWarehouseInventory
            │ INSERT ProductReservation
            ▼
       DB (nopCommerce)
            ▲ ReleaseExpiredReservationsTask (every 30 s)

[ Existing publish path Iter 1 + 2 — unchanged ]
    ▼
RabbitMQ: verdemart.orders.openboxes (+ DLX/DLQ)
    ▼
VerdeMart.OpenBoxesBridge (separate container)
    ├── IDedupRepository (processed_orders)
    └── IOpenBoxesClient → OpenBoxes REST API
```

---

## What Step 5 Will Do

Step 5 defines the key interfaces — gate method shapes, HTTP API contracts, and bridge client contracts.
