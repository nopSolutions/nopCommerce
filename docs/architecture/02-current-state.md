# Current-State Analysis of nopCommerce

## What Already Works in Our Favour

**Multi-warehouse inventory model**
`ProductWarehouseInventory` tracks `StockQuantity` and `ReservedQuantity` per warehouse per product. `AdjustInventoryAsync` and `BookReservedInventoryAsync` in `ProductService` handle the reservation → sold lifecycle. A `StockQuantityHistory` audit table logs every adjustment with a message tag (e.g. "PlaceOrder", "Ship"), giving an audit foundation.

**Rich order state model**
Orders carry three independent status axes: `OrderStatus`, `PaymentStatus`, and `ShippingStatus`. `OrderProcessingService` exposes explicit transition methods - `MarkOrderAsPaidAsync`, `ShipAsync`, `ReadyForPickupAsync`, `DeliverAsync`, `CancelOrderAsync` - each one publishing a typed domain event.

**Internal pub/sub event system**
`IEventPublisher` / `IConsumer<T>` provides a type-safe in-process event bus. Domain events exist for every major state change: `OrderPlacedEvent`, `OrderPaidEvent`, `ShipmentSentEvent`, `ShipmentDeliveredEvent`, `ShipmentReadyForPickupEvent`. Any plugin can implement `IConsumer<T>` and react to these - this is the natural integration hook.

**Plugin extension points**
`IPaymentMethod`, `IShippingRateComputationMethod`, and `IShipmentTracker` are designed for third-party integration. The `IScheduleTask` framework supports polling loops.

**GenericAttribute as escape hatch**
Any entity can carry arbitrary key-value data via `GenericAttribute`. `Order.OrderGuid` is a stable UUID for cross-system correlation.

---

## Architectural Seams and Pressure Points

### 1. The event system is synchronous and in-process only

**Seam:** `EventPublisher.cs`

All `IConsumer<T>` handlers execute inline, in the same request thread, with no persistence. If a handler fails, the exception is swallowed (logged only). There is no retry, no dead-letter queue, no replay. An order is placed, `OrderPlacedEvent` fires, and if the downstream consumer crashes, the event is simply lost.

**Conflict with scenario:** The scenario requires reliable delivery of order state to external systems (ERP, warehouse). In-process synchronous events cannot provide that guarantee.

---

### 2. Inventory is adjusted immediately, not truly reserved

**Seam:** `OrderProcessingService` → `AdjustInventoryAsync` called at order placement

When an order is placed, `StockQuantity` is decremented immediately. There is no atomic hold mechanism and no enforcement at checkout: two concurrent orders for the last unit can both succeed, driving stock negative.

**Conflict with scenario:** Cross-channel stock visibility requires a true allocation model - the system showing a customer "1 in stock" must honour that across web, POS, and warehouse simultaneously.

---

### 3. The data model has no external identity or sync-state fields

**Seam:** `Order`, `Shipment`, `Warehouse` entities

None of the core entities carry `ExternalOrderId`, `SyncStatus`, `SyncDateUtc`, or `LastSyncError`. The only workarounds are `GenericAttribute` (extra join per lookup) or `CustomValuesXml` (string parsing).

**Conflict with scenario:** When the ERP reports an order as fulfilled, nopCommerce has nowhere native to record "this order was acknowledged by ERP at T."

---

### 4. The shipment state model is too thin for fulfillment workflows

**Seam:** `Shipment.cs`

Shipment status is encoded as three nullable date fields: `ShippedDateUtc`, `ReadyForPickupDateUtc`, `DeliveryDateUtc`. There are no intermediate states - picked, packed, dispatched, in-transit, exception, returned. There is no `ExternalShipmentId` for warehouse or 3PL reference.

**Conflict with scenario:** The buy-online / fulfill-through-another-channel flow requires tracking a shipment as it moves through warehouse pick → pack → carrier handoff → delivery.

---

### 5. Scheduled tasks are single-instance with no distributed coordination

**Seam:** `IScheduleTask`, `ScheduleTaskRunner`

The task framework runs within a single app process with no distributed locking and no per-item progress tracking. If a sync task processes 50 orders and crashes on the 51st, it restarts from scratch. There is no outbox pattern, no at-least-once delivery guarantee.

**Conflict with scenario:** Any polling task that syncs orders to ERP or pulls inventory from warehouse systems cannot be made reliably idempotent without adding that logic entirely from scratch.

---

### 6. The order-placement mutex is process-local

**Seam:** `OrderProcessingService.PlaceOrderAsync`

The duplicate-order guard uses a `Mutex` - which is per-process. In a horizontally-scaled deployment, two nodes can each accept an order for the same customer simultaneously and both succeed.

**Conflict with scenario:** An omnichannel core accepting orders from web and POS simultaneously must use a distributed lock or an idempotency key at the database level.

---

### 7. No webhook ingestion or outbound integration pattern in the framework

**Seam:** `BasePlugin`

The plugin base provides no scaffold for outbound HTTP with retry, inbound webhook verification, or message queue consumers. Each integration plugin must implement its own `HttpClient`, retry policy, and webhook controller from scratch.

**Conflict with scenario:** Every external system integration (ERP, warehouse, POS, carrier) becomes a bespoke HTTP client with no shared circuit-breaker, backoff, or dead-letter handling.

---

## Summary

| Scenario Requirement | nopCommerce Support | Gap Severity |
|---|---|---|
| Order placed → reflected in ERP/warehouse | `OrderPlacedEvent` exists | High - event is in-process, no guaranteed delivery |
| Cross-channel stock visibility | Per-warehouse `StockQuantity` exists | High - no atomic allocation, no external sync state |
| System useful when external systems lag | Plugin can implement fallback logic | High - no framework support for circuit breaker |
| Fulfillment state back-propagation | `ShipmentSentEvent`, `DeliveryDateUtc` exist | Medium - states too coarse, no external ID field |
| Distributed multi-node safety | Single `Mutex` | High - process-local only |
| Sync recovery after outage | `ScheduleTask` framework exists | Medium - no per-item progress, no outbox |

nopCommerce provides the domain model and the event vocabulary, but the delivery guarantees, external identity fields, and resilience patterns required for a true omnichannel core must all be built on top of it. The `IConsumer<T>` + `IScheduleTask` pair is where the integration layer will live.
