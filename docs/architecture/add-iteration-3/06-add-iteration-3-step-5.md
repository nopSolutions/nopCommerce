# ADD Iteration 3 — Step 5: Define Interfaces

## Part A — `Nop.Plugin.Inventory.AllocationGate`

**`IProductReservationRepository`** — key methods: `InsertReservationAsync`, `GetActiveSumByProductAsync` (used by the gate for availability), `MarkCommittedAsync`, `MarkReleasedAsync`, `FetchExpiredAsync`.

**`IAllocationGate`** — three operations:
- `ReserveAsync(reservationKey, channelKey, items, ttl)` — acquires pessimistic row lock, checks effective availability, inserts `ProductReservation` rows; returns a typed `AllocationResult` (success or failure-with-item-list)
- `ConfirmAsync(reservationKey)` — transitions reservations to `Committed`
- `ReleaseAsync(reservationKey)` — transitions to `Released`

**`AllocationGateProductServiceDecorator`** — wraps `IProductService`; intercepts `AdjustInventoryAsync` for decrements only; all other methods delegate unchanged.

**`/api/inventory/*` HTTP contract:**

| Endpoint | Request body | Success | Failure |
|---|---|---|---|
| `POST /reserve` | `{ reservationKey, items: [{ productId, warehouseId, quantity }] }` | `200 OK` + reservation details | `409 Conflict` + failed-item list |
| `POST /confirm` | `{ reservationKey }` | `200 OK` | `404` if key unknown |
| `POST /release` | `{ reservationKey }` | `200 OK` | `404` if key unknown |

**`AllocationSettings`** (`ISettings`) — `WebReservationTtlSeconds` (30), `PosReservationTtlSeconds` (300), `ReleaseTaskBatchSize` (200), `ReleaseTaskIntervalSeconds` (30).

---

## Part B — `VerdeMart.OpenBoxesBridge`

**`IOpenBoxesClient`** — `CreateFulfillmentAsync(OrderPlacedMessage)` returns a `CreateFulfillmentResult` (success with id, duplicate, or failure); bounded retry-with-backoff for transient HTTP errors inside the call; broader retry owned by message redelivery.

**`IDedupRepository`** — `HasProcessedAsync(orderGuid)` and `RecordProcessedAsync(orderGuid, fulfillmentId)`; the insert uses `ON CONFLICT DO NOTHING` semantics to handle concurrent races safely.

**`BridgeSettings`** — `RabbitMqHost`, `OrderQueueName` (`verdemart.orders.openboxes`), `MaxRedeliveryAttempts` (5), `OpenBoxesBaseUrl`, `OpenBoxesApiKey`, `DedupConnectionString` — all env-driven in production.

---

## Dependency Map

```
[ nopCommerce plugin ]

AllocationGateProductServiceDecorator
    depends on → IAllocationGate
    wraps      → IProductService (inner)

IAllocationGate / AllocationGate
    depends on → IProductReservationRepository
    locks      → ProductWarehouseInventory (DB row lock)

AllocationApiController
    depends on → IAllocationGate

ReleaseExpiredReservationsTask
    depends on → IProductReservationRepository

[ Bridge service ]

OrderPlacedMessageConsumer
    depends on → IDedupRepository
    depends on → IOpenBoxesClient

IOpenBoxesClient / OpenBoxesClient
    depends on → BridgeSettings
    calls      → OpenBoxes REST API
```

---

## What Step 6 Will Do

Step 6 sketches the updated component view and sequence diagrams, and records the architectural decisions this iteration produced.
