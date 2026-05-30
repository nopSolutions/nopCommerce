# ADR-0011: Order Outbox Insertion Strategy (Consumer + Reconciler)

- **Status**: Accepted
- **Date**: 2026-05-15
- **Deciders**: Team G02
- **Drivers (QA/Constraint)**: QA-5 (outbox row ≤ 100 ms after OrderPlacedEvent; 0 synchronous external HTTP in checkout), Constraint 1 (async workflow), Constraint 2 (explicit reliability decision: outbox)

## Context

Scenario C requires nopCommerce to notify the warehouse when an order is placed, without calling the WMS synchronously on the checkout thread (that would couple checkout latency and availability to an external system). The standard pattern is the **transactional outbox**: write a durable row in the same unit of work as the business event, then publish it asynchronously.

nopCommerce raises `OrderPlacedEvent` (`Nop.Services.Orders.OrderPlacedEvent`) after an order is placed. Our plugin consumes it (`OrderPlacedOutboxConsumer`) and inserts an `OmniOutboxMessage`. The risk: nopCommerce's event publisher catches consumer exceptions, so if the consumer throws (or the process crashes) after the order commits but before the outbox row is written, the order would exist with **no integration trail** and the WMS would never hear about it.

## Decision

We will write the outbox row in the `OrderPlacedEvent` consumer (fast path), **and** run a scheduled `OutboxReconcilerTask` (safety net) that periodically scans recent orders with no corresponding `OmniOutboxMessage` and back-fills the missing row. Publishing is done by a separate `OutboxPublisherTask` with publisher confirms. Together: the consumer gives low latency (QA-5), the reconciler guarantees eventual completeness across crashes.

## Consequences

- **Positive**: no synchronous external HTTP in checkout; every placed order eventually has an outbox row even if the consumer fails; clean separation of "record intent" (consumer) from "ship it" (publisher).
- **Negative**: two code paths can create the same logical row, so the reconciler must be idempotent (dedupe on `OrderGuid`); the reconciler adds a periodic scan cost.

## Tradeoffs

We trade a small amount of duplicated logic and a periodic reconcile scan for crash-safety. The alternative (consumer only) is simpler but can silently drop integration for an order; the alternative (reconciler only) is robust but adds latency that risks QA-5.

## Rejected Alternatives

- **Consumer only, no reconciler**: rejected — `EventPublisher` swallows consumer exceptions, so a failure leaves an order with no outbox row and no alarm.
- **Synchronous publish inside the consumer**: rejected — puts external MQ/HTTP on the checkout path, breaking the async-workflow constraint and QA-5.
- **Polling orders only (no event consumer)**: rejected — adds up to one scan-interval of latency before the WMS is told, and wastes work re-scanning settled orders.
