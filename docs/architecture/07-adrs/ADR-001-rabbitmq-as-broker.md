# ADR-001 — RabbitMQ as Message Broker

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-27 |
| Produced by | ADD Iteration 1 — Step 6 (driver: QAS-1 Reliability) |

## Context

nopCommerce must notify surrounding systems (OpenBoxes, carrier, and others) when an order is placed. A direct HTTP call from the checkout thread creates a hard dependency on those systems being available and fast. QAS-1 and QAS-3 explicitly forbid this.

## Decision

Use RabbitMQ as the message broker for all cross-context event delivery. All bounded contexts communicate via RabbitMQ exchanges and queues, not via direct HTTP calls in the synchronous request path.

## Rejected Alternatives

**Direct synchronous HTTP call from the checkout thread.** Each downstream system (OpenBoxes, carrier) would be called synchronously at the point of order placement. If any system is slow or unavailable, checkout either blocks or fails. *Rejected:* QAS-1 and QAS-3 explicitly forbid placing a downstream dependency on the checkout critical path — events must not be lost during a downstream outage and checkout must complete regardless of downstream availability. Both are impossible under a synchronous call model.

## Consequences

- Checkout is decoupled from downstream system availability
- Messages are durable — an OpenBoxes outage does not lose orders
- Adds RabbitMQ as a required infrastructure component
- Bridge services are needed for systems that do not natively consume AMQP
