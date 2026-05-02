# 7-Minute Presentation Script - Part 1

## Slide 1 - Scenario and Problem (0:00-0:45)

We selected **Scenario C - Omnichannel Commerce Core**. The business problem is that nopCommerce is no longer only the web shop. It must coordinate orders, stock and fulfillment across warehouse and store channels.

Our architectural problem is not "connect more systems". It is: **how does the commerce core remain useful when surrounding operational systems are delayed, stale, unavailable or contradictory?**

## Slide 2 - Current State (0:45-1:30)

nopCommerce is a modular monolith with a plugin model. Orders, catalog, stock and shipping already exist inside the monolith.

The good news is that `OrderPlacedEvent`, warehouses, stock reservation and plugin infrastructure give us extension points.

The gap is that nopCommerce events are in-process and not durable integration events. There is no outbox, no inbox and no explicit model for delayed external fulfillment.

## Slide 3 - Domain Boundaries (1:30-2:05)

We separate four contexts:

- nopCommerce Core owns orders, products, stock and shipments.
- Omnichannel Integration owns outbox, inbox and projections.
- WMS Simulator owns external fulfillment behavior.
- POS Simulator owns store-originated stock changes.

The rule is: no external component reads or writes the nopCommerce database directly.

## Slide 4 - Quality Attributes (2:05-2:50)

The design is driven by five quality attributes:

- Resilience: checkout succeeds even when WMS is unavailable.
- Consistency: duplicates and stale stock updates are detected.
- Traceability: order, message and fulfillment attempt can be correlated.
- Operability: queues, retries and degraded state are visible in the demo.
- Performance: WMS latency is removed from the checkout critical path.

## Slide 5 - Framework and Tactics (2:50-3:35)

We use **ADD - Attribute-Driven Design** because the assignment is explicitly quality-attribute driven.

The selected tactics are asynchronous messaging, outbox, inbox/idempotency, retry with backoff, circuit breaker, DLQ and local projections.

The architecture is intentionally selective: we do not decompose nopCommerce into microservices.

## Slide 6 - Target Architecture (3:35-4:45)

The target has nopCommerce plus an Omnichannel Core plugin, RabbitMQ, an Omnichannel Worker, WMS simulator and POS simulator.

Flow one: order placed in nopCommerce becomes an outbox message, then a RabbitMQ event, then a WMS fulfillment request handled by the worker.

Flow two: POS emits a stock change, the worker consumes it and calls the plugin, which updates stock visibility or records stale updates.

## Slide 7 - Decisions and Rejected Alternatives (4:45-5:45)

Main ADRs:

- Select Scenario C.
- Keep commerce core inside nopCommerce.
- Use outbox plus RabbitMQ instead of synchronous WMS calls.
- Use simulators instead of full ERP/WMS/POS products.
- Do not share databases across boundaries.

Rejected alternatives include full microservice extraction, direct WMS calls during checkout and direct database integration.

## Slide 8 - Risks and Spike (5:45-6:35)

The main risk is whether we can integrate without blocking checkout or rewriting order processing.

Our spike inspected the extension points and confirms the feasible path:

- `OrderPlacedEvent` is raised after persistence.
- A plugin can consume this event.
- A plugin can own tables for outbox/inbox/projections.
- A scheduled task can publish messages after checkout.

This gives us a concrete implementation path for Part 2.

## Slide 9 - Roadmap and Demo Plan (6:35-7:00)

Next we implement the plugin, worker, RabbitMQ and simulators.

Final demo will show:

1. Normal order-to-WMS fulfillment.
2. WMS unavailable while checkout still succeeds.
3. Recovery after WMS comes back.
4. POS stock update becoming visible.
5. Duplicate or stale update handled safely.

