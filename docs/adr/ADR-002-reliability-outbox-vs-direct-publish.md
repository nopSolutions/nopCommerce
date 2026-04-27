# ADR-002: Reliability Pattern — Outbox vs Direct Publish

**Status:** Accepted  
**Date:** 2026-04-26  
**Owner:** Sebastião  
**Deciders:** Full team

---

## Context

When an order is placed in nopCommerce, we need to reliably publish an `order.placed` event to RabbitMQ. The question is whether to publish directly from `OrderProcessingService` or to use the **Transactional Outbox Pattern**.

---

## Decision

**Use the Transactional Outbox Pattern.**

An `IntegrationEvent` row is written to the nopCommerce PostgreSQL database in the same transaction that saves the `Order`. A separate background service (`OutboxPublisherBackgroundService`) reads pending rows and publishes them to RabbitMQ, marking each as published after acknowledgement.

---

## Rationale

**Direct publish** (calling RabbitMQ inside `PlaceOrderAsync`) has a critical reliability gap:

```
BEGIN TRANSACTION
  INSERT Order
  INSERT OrderItems
  AdjustInventory
COMMIT TRANSACTION
  ↓ (transaction committed — order is saved)
PublishAsync(order.placed)   ← WHAT IF THIS FAILS?
```

If RabbitMQ is unavailable at commit time, or the process crashes between commit and publish, the order is saved but the event is **silently lost**. ERP and WMS never receive it. There is no recovery path short of manual intervention.

**Outbox pattern** removes this gap:

```
BEGIN TRANSACTION
  INSERT Order
  INSERT OrderItems
  AdjustInventory
  INSERT IntegrationEvent (status = Pending)   ← atomic with order
COMMIT TRANSACTION
  ↓
OutboxPublisherBackgroundService (polling loop)
  → reads Pending rows
  → publishes to RabbitMQ
  → marks as Published (on ACK)
  → retries on failure (row stays Pending)
```

The event is guaranteed to be published eventually as long as the nopCommerce process is running, regardless of broker availability at order time.

---

## Rejected Alternative: Direct Publish in PlaceOrderAsync

Rejected because:
- Introduces a dual-write problem with no atomicity guarantee
- Silent event loss on broker unavailability
- Makes order placement dependent on broker health (violates QA-1: WMS unavailability must not block orders)

---

## Rejected Alternative: nopCommerce IEventPublisher + persistent consumer

The existing `IEventPublisher` / `IConsumer<OrderPlacedEvent>` system publishes in-process. We could implement a persistent consumer that writes to RabbitMQ. However:
- Still a direct publish at consumer execution time — same reliability gap as above
- nopCommerce's event system has no retry or persistence mechanism

---

## Consequences

- New `IntegrationEvent` entity + FluentMigrator migration required
- `OutboxPublisherBackgroundService` must be registered as a hosted service via `INopStartup`
- Polling interval of the background service introduces latency (target: < 5 s)
- At-least-once delivery: Integration Service must be idempotent on `eventId` (see ADR-003)
- Outbox table must be periodically cleaned of old Published rows (background cleanup task)
