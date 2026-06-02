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

## Rejected Alternatives

**Transient, auto-delete, or non-persistent queues and messages.** A non-durable queue and its in-flight messages are destroyed when the RabbitMQ broker restarts; non-persistent (delivery mode 1) messages are not written to disk and are lost on restart. *Rejected:* all three configurations are architectural violations of QAS-1 — a broker restart during a 30-minute downstream outage would permanently lose every enqueued order event.

**Auto-acknowledgement (`autoAck: true`).** With auto-ack, RabbitMQ marks a message as delivered as soon as it is written to the consumer's TCP socket. If the consumer process crashes before the downstream action completes, the message is silently discarded. *Rejected:* at-least-once delivery (required by QAS-1 and QAS-4) is impossible under auto-ack; there is no requeue path on consumer failure.

**Fanout exchange instead of a direct exchange with routing keys.** A fanout exchange broadcasts every message to every bound queue regardless of message type. *Rejected:* once the topology grows to include additional event types (order paid, order cancelled), a fanout sends all messages to all consumers — impractical and wasteful. A direct exchange with a routing key (`order.placed`) allows each consumer queue to bind selectively and supports adding new event types without any publisher change.

## Consequences

- Messages survive a RabbitMQ broker restart
- Messages survive a consumer outage of any duration (bounded only by disk)
- Consumer logic must be idempotent — redelivered messages must not create duplicates
- `OrderGuid` is the idempotency key for all order-related messages
