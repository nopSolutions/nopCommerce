# Feature: nopCommerce Real Order Events

**Branch:** `feature/nop-real-order-events`
**Owner:** Henrique
**Date:** 2026-05-19
**Status:** Local — unstaged, awaiting review

Replaces the Part-1 outbox spike (`AppStartedEvent` placeholder) with the production wiring of nopCommerce → RabbitMQ for the omnichannel commerce core (Scenario C). Implements TODO.md §"Week 3 — nopCommerce - Real Order Events / Stock Consumer / Health Endpoint" (lines 158–187).

## What this branch delivers

| # | Concern | Outcome |
|---|---|---|
| 1 | Real `order.placed` events | Hooked into `OrderProcessingService.PlaceOrderAsync` via `IConsumer<OrderPlacedEvent>` — no surgery on core code |
| 2 | Generic outbox writer | `SpikeOutboxService` → `OutboxService` with `WriteEventAsync(eventType, data)` |
| 3 | Production publisher task | `SpikeOutboxPublisherTask` → `OutboxPublisherTask`; RabbitMQ config moved to `appsettings.json` |
| 4 | Inbound stock updates | `StockUpdateConsumerBackgroundService` (HostedService) consuming `stock.updated` from RabbitMQ |
| 5 | Health endpoint | `GET /integration/health` returns pending count + last-publish age, 200/503 |
| 6 | Config | New `IntegrationConfig : IConfig`, auto-bound from `appsettings.json` |
| 7 | Migration | `OutboxPublisherTaskMigration` renames the `ScheduleTask` row from the spike type to the new type (or seeds it) |

## File-by-file changes

### Added

- **`src/Libraries/Nop.Core/Configuration/IntegrationConfig.cs`**
  Typed `IConfig` bound from `appsettings.json` via the existing IConfig auto-discovery.
  Fields: `RabbitMqHostname` (default `rabbitmq`), `RabbitMqPort` (5672), `RabbitMqUsername`/`Password` (guest/guest), `EventsExchange` (`verdemart.events`), `StockUpdatedQueue` (`nopcommerce.stock-updated`), `StockUpdatedRoutingKey` (`stock.updated`), `HealthDegradedAfterSeconds` (60).

- **`src/Libraries/Nop.Services/Integration/IOutboxService.cs`** + **`OutboxService.cs`**
  Replaces `ISpikeOutboxService` / `SpikeOutboxService`. Single method: `WriteEventAsync(string eventType, object data)` — serializes payload to JSON and inserts an `IntegrationEvent` row with `Published = false`.

- **`src/Libraries/Nop.Services/Integration/OrderPlacedEventConsumer.cs`**
  `IConsumer<OrderPlacedEvent>`. nopCommerce already publishes `OrderPlacedEvent` at `OrderProcessingService.cs:1617` and `:1993`, so this consumer plugs in via the existing `IEventPublisher` plumbing without touching `PlaceOrderAsync`. Loads order items via `IOrderService.GetOrderItemsAsync`, serializes `{ eventId, orderId, orderGuid, customerId, storeId, total, currency, createdOnUtc, items[] }` to the outbox as routing key `order.placed`.

- **`src/Libraries/Nop.Services/Integration/OutboxPublisherTask.cs`**
  Renamed from `SpikeOutboxPublisherTask`. Polls outbox for `Published == false` ordered by Id, publishes each to `IntegrationConfig.EventsExchange` with `IntegrationEvent.EventType` as the routing key. On success: marks `Published`, sets `PublishedOnUtc`, increments `PublishAttempts`, clears `LastError`. On failure: increments `PublishAttempts`, sets `LastError`, logs (row stays unpublished for retry).

- **`src/Presentation/Nop.Web.Framework/Infrastructure/StockUpdateConsumerBackgroundService.cs`**
  `BackgroundService` registered as `IHostedService`. On start: declares `verdemart.events` topic exchange, durable queue `nopcommerce.stock-updated` bound to routing key `stock.updated`, sets up an `AsyncEventingBasicConsumer`. Per message: creates a service scope, resolves `IProductService`, calls `AdjustInventoryAsync(product, delta, …)`. Manual ack on success; unknown productIds are ack'd and dropped (logged warning); deserialization/processing errors are nack'd with `requeue: false` (would route to DLX once configured). Placed in `Nop.Web.Framework` rather than `Nop.Services` to stay close to other HostedService precedent.

- **`src/Presentation/Nop.Web/Controllers/IntegrationHealthController.cs`**
  `GET /integration/health` — does not inherit `BasePublicController` (lightweight probe, no guest-account side effects, matching `KeepAliveController`). Counts unpublished outbox rows; reads most recent `PublishedOnUtc`. Returns JSON:
  ```json
  { "status": "healthy|degraded", "pendingOutboxCount": 0, "lastPublishedOnUtc": "...", "lastPublishedAgeSeconds": 12.4, "degradedAfterSeconds": 60 }
  ```
  Status code 503 when `pending > 0 && (no last-publish OR age > threshold)`, else 200.

- **`src/Libraries/Nop.Data/Migrations/UpgradeTo500/OutboxPublisherTaskMigration.cs`**
  `NopUpdateMigration` (`2026-05-19 00:00:00`). Idempotent: if a `ScheduleTask` row exists with the old or new `Type`, it's updated to the new fully-qualified type name and renamed; otherwise a new row is seeded with `Seconds = 10`, `Enabled = true`. Down is a no-op.

### Modified

- **`src/Libraries/Nop.Services/Integration/RabbitMQ/RabbitMqPublisher.cs`**
  Constructor now takes `IntegrationConfig` and reads connection params from it. Removed the `TODO: Move to appsettings.json` block. Removed "Spike" log lines (the publisher task already logs at the higher level).

- **`src/Presentation/Nop.Web.Framework/Infrastructure/IntegrationStartup.cs`**
  - Registers `IntegrationConfig` as a singleton resolved from `Singleton<AppSettings>.Instance.Get<IntegrationConfig>()` so other services can inject it directly.
  - `IOutboxService` → `OutboxService` (scoped).
  - `IRabbitMqPublisher` → `RabbitMqPublisher` (scoped, unchanged).
  - `AddHostedService<StockUpdateConsumerBackgroundService>()`.

### Deleted

- `src/Libraries/Nop.Services/Events/AppStartedEventConsumer.cs` — spike placeholder, no longer fires anything.
- `src/Libraries/Nop.Services/Integration/ISpikeOutboxService.cs`
- `src/Libraries/Nop.Services/Integration/SpikeOutboxService.cs`
- `src/Libraries/Nop.Services/Integration/SpikeOutboxPublisherTask.cs`

## Event contracts

### `order.placed` (outbound, nopCommerce → Integration Service)

```json
{
  "eventId": "GUID",
  "orderId": 1234,
  "orderGuid": "GUID",
  "customerId": 42,
  "storeId": 1,
  "total": 99.50,
  "currency": "EUR",
  "createdOnUtc": "2026-05-19T10:00:00Z",
  "items": [
    { "productId": 17, "quantity": 2, "unitPrice": 49.75 }
  ]
}
```
Exchange: `verdemart.events` (topic), routing key: `order.placed`.

### `stock.updated` (inbound, Integration Service → nopCommerce)

```json
{
  "eventId": "GUID",
  "productId": 17,
  "delta": -2,
  "source": "wms|pos",
  "timestampUtc": "2026-05-19T10:00:05Z"
}
```
Queue: `nopcommerce.stock-updated` (durable), bound to `verdemart.events` with routing key `stock.updated`. `delta` is the signed quantity change passed to `IProductService.AdjustInventoryAsync`.

## Configuration

`appsettings.json` will gain (on first run after the IConfig binding) an `IntegrationConfig` section. Override per environment to point at the real RabbitMQ host. Example for docker-compose:

```json
"IntegrationConfig": {
  "RabbitMqHostname": "rabbitmq",
  "RabbitMqPort": 5672,
  "RabbitMqUsername": "guest",
  "RabbitMqPassword": "guest",
  "EventsExchange": "verdemart.events",
  "StockUpdatedQueue": "nopcommerce.stock-updated",
  "StockUpdatedRoutingKey": "stock.updated",
  "HealthDegradedAfterSeconds": 60
}
```

## Architectural notes

- **Outbox transactional guarantee**: `OrderPlacedEventConsumer` writes the event row in the same scope as the order-placed event handling. The nopCommerce `OrderPlacedEvent` is published *after* the order has been saved (`SaveOrderDetailsAsync` runs at `OrderProcessingService.cs:1589`), so the outbox row is committed alongside the order in the same logical request. If we want strict same-transaction guarantees we'd need to enroll in the active LinqToDB transaction explicitly — current scope is good enough for the assignment but worth flagging.
- **Idempotency**: the consumer is not yet dedup'ing by `eventId`. Acceptable for the demo where the Integration Service is the only producer; tightens up with the Part-2 test suite (TODO.md §"Integration Tests / Test: Idempotency").
- **Cross-channel conflict resolution** (cancel recent web order on negative stock per QA-4) is **not** in this branch — it has its own design surface (which orders to pick, how to surface to customer service) and belongs to a follow-up branch.
- **Polly / circuit breaker**: lives in the Order Integration Service (Martim's lane), not here. The nopCommerce stock consumer just nacks without requeue; a DLX/DLQ binding on the consumer queue is the cleanest hardening step.

## Open follow-ups

1. **Build not verified locally** — `global.json` pins SDK `10.0.100`; only `9.0.312` is installed on this machine. CI / a teammate with .NET 10 should `dotnet build src` before merge.
2. **EventId-based dedup** in `StockUpdateConsumerBackgroundService` — small table or `IShortTermCacheManager` key per `eventId`.
3. **DLX/DLQ binding** for `nopcommerce.stock-updated` so nacks land somewhere observable.
4. **Cross-channel conflict resolution** (QA-4) — its own branch.
5. **Migration timestamp** (`2026-05-19 00:00:00`) — bump if anyone else has landed a `UpgradeTo500` migration in between when this is opened.

## How to verify locally

1. Bring up RabbitMQ (`docker-compose -f docker-compose.yml up rabbitmq`).
2. Start nopCommerce. The `OutboxPublisherTaskMigration` runs, the scheduled task ticks every 10s.
3. Place a test order through the storefront. Check the DB `IntegrationEvent` table for a row with `EventType = "order.placed"`. Within 10s `Published = true`.
4. In RabbitMQ Management UI (15672) confirm the message reached `verdemart.events` with routing key `order.placed`.
5. Hit `GET /integration/health` — should return `healthy` with `pendingOutboxCount = 0`.
6. Stop RabbitMQ and place another order. Within 60s `/integration/health` should flip to `503 degraded`.
7. Publish a test `stock.updated` message manually (Management UI) — the `StockUpdateConsumerBackgroundService` log line + `Product.StockQuantity` change confirm the consumer path.
