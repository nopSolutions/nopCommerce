# 7-Minute Presentation Script - Part 1 (10 slides)

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

## Slide 8 - Risks and Spike (5:45-6:20)

The main risk is whether we can integrate without blocking checkout or rewriting order processing.

We ran a concrete spike on this and confirmed the feasible path:

- `OrderPlacedEvent` is raised after persistence — a plugin can consume it without touching checkout.
- A plugin can own its own migration tables for outbox, inbox and projections.
- A scheduled task can publish pending messages after checkout completes.

This is not a guess. We have located the exact source points and the event shape is defined.

## Slide 9 - Scope and Commitment (6:20-6:45)

**In scope for the final delivery:**

- `Nop.Plugin.Misc.OmnichannelCore` — outbox, inbox, projections, callback API
- Omnichannel Worker — RabbitMQ consumer, WMS coordination, retry, circuit breaker, DLQ
- WMS simulator — normal, slow, unavailable and contradictory modes
- POS simulator — stock events with duplicate and stale variants
- Docker Compose — single command to run the full environment
- Architecture report, updated ADRs and evidence pack

**Out of scope:**

- Rewriting or extracting nopCommerce core services
- Real ERP, WMS or POS systems
- Keycloak, real payments or real shipping carriers
- Distributed exactly-once guarantees

## Slide 10 - Demo Plan (6:45-7:00)

The final demo will show five scenarios:

1. Normal order placed → WMS accepts → fulfillment confirmed.
2. WMS unavailable → checkout still succeeds → order queued.
3. WMS recovers → pending order processed → fulfillment confirmed.
4. POS stock change → visible in nopCommerce.
5. Duplicate or stale POS event → silently ignored with evidence.

