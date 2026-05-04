# ADR-0003 - Use Outbox and RabbitMQ for Fulfillment Integration

## Status

Accepted.

## Context

The final demo must show the system remains useful when an operational system is slow, unavailable or contradictory. A direct WMS call during checkout would make checkout depend on WMS health.

## Decision

Use a durable outbox in the nopCommerce plugin and RabbitMQ for asynchronous fulfillment and stock integration.

The plugin will write `OmniOutboxMessage` when it observes `OrderPlacedEvent`. A scheduled publisher will send pending messages to RabbitMQ. The worker consumes messages and handles WMS/POS coordination.

## Consequences

- Checkout does not wait for WMS.
- Delivery is at-least-once, so consumers must be idempotent.
- Retry, DLQ and recovery become visible and demonstrable.

## Tradeoffs

- At-least-once shifts complexity to consumers (`messageId` deduplication, see ADR-0006).
- Outbox publisher adds operational surface: scheduled-task health and queue lag must be monitored.
- Recovery time after RabbitMQ failure depends on backlog drain rate; not bounded by the architecture itself.

## Rejected Alternatives

- Synchronous HTTP from checkout to WMS was rejected because WMS degradation would break checkout or force long user waits.
- Database polling by the worker against nopCommerce tables was rejected because it creates hidden shared-database coupling.

