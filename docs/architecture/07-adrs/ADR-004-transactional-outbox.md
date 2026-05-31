# ADR-004 — Transactional Outbox for Reliable Publish

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-27 |
| Produced by | ADD Iteration 2 — Step 6 (driver: QAS-1, full coverage) |

## Context

Iteration 1 satisfied QAS-1 under the assumption that RabbitMQ is reachable at the moment `OrderPlacedConsumer.HandleEventAsync` runs. If the broker is unreachable or the application crashes between the order's database commit and the inline `BasicPublish` call, the order is committed but no message is sent. This is the dual-write problem.

QAS-1's full statement requires zero orders lost during a 30-minute outage and recovery within 60 seconds. To honour that without the assumption of broker availability, the order commit and the publish intent must be atomic.

## Decision

Adopt the **transactional outbox** pattern. Inside the same database transaction that commits the `Order` and decrements `StockQuantity`, insert a row into a new `Outbox` table carrying the serialised message and a `Pending` status. A separate `OutboxDispatcherTask` polls the table on a 1-second cadence, publishes pending rows to RabbitMQ, and updates their status.

The publish is therefore decoupled from the request thread entirely. RabbitMQ never appears in the synchronous checkout path.

## Rejected Alternatives

**Publish-with-retry off-thread.** Move the inline `BasicPublish` to a background queue (`Channel<T>` or `Task.Run`) and retry on failure.
*Rejected:* off-thread retry is not atomic with the order commit. If the application process dies between commit and successful publish, the in-memory queue dies with it and the message is lost — exactly the failure mode this decision exists to close.

**Change Data Capture (Debezium / binlog tailing).** Tail the MySQL binlog and turn inserts into events.
*Rejected:* disproportionate infrastructure for VerdeMart's scope (Debezium plus a Kafka-class transport); couples the wire contract to the database schema, so every column rename becomes a breaking change for consumers; the brief explicitly warns against "too many technologies with shallow purpose".

**Two-phase commit between MySQL and RabbitMQ (XA).** Distributed transaction across both systems.
*Rejected:* RabbitMQ does not provide production-grade XA support; even where 2PC is available it is a known operational hazard (blocking failures, recovery complexity); the standard tradeoff is at-least-once + idempotent consumers.

## Consequences

- The order commit and the publish intent are atomic; the broker-down hole from Iteration 1 is closed.
- Checkout latency is bounded by database performance only.
- At-least-once delivery becomes mandatory; consumers must be idempotent (already mandated by ADR-003, with `OrderGuid` as the idempotency key).
- A new database table (`Outbox`) is added to the nopCommerce schema and grows over time; a retention strategy is needed (deferred to a later iteration).
- A new component (`OutboxDispatcherTask`) is introduced; it runs as an `IScheduleTask` inside the nopCommerce process on a 10-second poll interval, using `FOR UPDATE SKIP LOCKED` so future multi-node deployments remain safe without a distributed lock.
- Per-message latency between order commit and broker arrival increases by at most one polling interval (10 s default), well under QAS-1's 60 s recovery clause.
