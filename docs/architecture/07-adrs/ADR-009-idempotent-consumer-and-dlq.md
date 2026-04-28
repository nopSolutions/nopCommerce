# ADR-009 — Idempotent Consumer with Bridge-Local Dedup and Dead-Letter Queue

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-28 |
| Produced by | ADD Iteration 3 — Step 6 (drivers: ADR-003 idempotency mandate; CON-10; CON-12) |

## Context

ADR-003 mandates idempotent consumers under at-least-once delivery, with `OrderGuid` as the idempotency key. ADR-004 makes at-least-once delivery a hard property of the publisher path. Iteration 3 introduces the first real consumer — the OpenBoxes bridge — so two operational concerns become real for the first time:

- CON-10: where the idempotency check lives. Bridge-local dedup table, OpenBoxes natural dedup on `OrderGuid`, or both.
- CON-12: poison messages (malformed payload, persistent OpenBoxes rejection) must not block the queue.

OpenBoxes' API behaviour around duplicate detection is uncertain — open at Iter 3 Step 1 until a feasibility spike confirms.

## Decision

**Idempotency:** the bridge maintains a small local dedup table (`processed_orders`, primary key `OrderGuid`, plus `ProcessedAtUtc` and `OpenBoxesFulfillmentId`). On each message, the bridge first checks the table; if the order is already present, the message is acked and skipped. Otherwise, the bridge calls OpenBoxes' fulfillment-creation API; on success, it inserts the dedup row and acks. If OpenBoxes returns a duplicate-detection error, the bridge treats it as success and inserts the dedup row with whatever identifier OpenBoxes returns.

This is **defence in depth**: the bridge does not assume OpenBoxes deduplicates, but co-operates with native dedup if present.

**Dead-letter queue:** the existing `verdemart.orders.openboxes` queue gains the argument `x-dead-letter-exchange = verdemart.orders.dlx`. A new direct, durable exchange `verdemart.orders.dlx` is declared, with a durable queue `verdemart.orders.openboxes.dlq` bound to it on routing key `order.placed`. The bridge reads the `x-death` header that RabbitMQ adds to redelivered messages and counts redelivery attempts; once the count exceeds a configurable threshold (default 5), the bridge NACKs without requeue. The broker routes the message to the DLQ for operator review.

## Rejected Alternatives

**Trust OpenBoxes' natural idempotency only; no bridge dedup table.**
*Rejected:* OpenBoxes' API behaviour around duplicate detection is uncertain. Building correctness on an unverified property risks silent duplicates in the demo.

**No dedup; rely on OpenBoxes erroring on duplicate creation, retry indefinitely.**
*Rejected:* some duplicate-creation paths return ambiguous errors that the bridge cannot distinguish from genuine failures, leading to message loops.

**Quarantine table inside the bridge instead of a DLQ.**
*Rejected:* RabbitMQ's native dead-letter mechanism is tested, idiomatic, and visible in standard RabbitMQ tooling. A custom table reinvents the wheel and hides poison messages from operators.

**Infinite retry without DLQ.**
*Rejected:* a single poison message blocks the queue indefinitely, behind which every order stalls.

## Consequences

- The bridge requires a small local store for dedup state. Operationally minor (SQLite or PostgreSQL) and isolated to the bridge's deployment.
- Manual ack semantics from ADR-003 are realised: messages remain on the queue until OpenBoxes confirms creation, satisfying CON-11.
- An operator workflow exists for poison messages: inspect the DLQ, decide to replay, repair, or discard.
- At-least-once delivery (ADR-004 consequence) is concretely safe: redelivered messages produce no duplicate fulfillment.
- The bridge's correctness now depends on the local dedup store's availability. If that store is down, the bridge cannot safely process; documented as a residual risk for Step 7.
