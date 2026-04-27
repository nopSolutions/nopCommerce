# ADR-003 — Durable Queues with Persistent Delivery and Manual Acknowledgement

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-27 |
| Produced by | ADD Iteration 1 — Step 6 (drivers: QAS-1 Reliability, QAS-4 Recoverability) |

## Context

QAS-1 requires zero message loss during an OpenBoxes outage of up to 30 minutes. QAS-4 requires that all missed events self-heal after recovery with no operator action.

## Decision

All queues are declared durable. All messages are published with delivery mode 2 (persistent). All consumers use manual acknowledgement — a message is acknowledged only after the downstream action succeeds.

## Consequences

- Messages survive a RabbitMQ broker restart
- Messages survive a consumer outage of any duration (bounded only by disk)
- Consumer logic must be idempotent — redelivered messages must not create duplicates
- `OrderGuid` is the idempotency key for all order-related messages
