# ADR-0003 - Use Outbox and RabbitMQ for Fulfillment Integration

## Status

Accepted.

## 1. Context

Scenario C requires nopCommerce checkout to remain useful when the external WMS is slow, unavailable, or returns contradictory state (QA-1 Resilience, QA-3 Traceability). Today the integration boundary has no buffer: when an order is placed, the system must decide between calling WMS inline during checkout, polling for new orders elsewhere, or handing the order to a buffered asynchronous path before WMS is touched.

The final demo must show normal flow, induced WMS degradation, and observable recovery — with **no lost or duplicated fulfillment**. A direct WMS call during checkout couples the checkout latency and availability budget to WMS health, which the assignment explicitly warns against.

Prior decisions in scope:

- ADR-0002 keeps the commerce core inside nopCommerce; integration lives in a plugin + worker.
- ADR-0005 forbids a shared database across service boundaries.
- ADR-0009 fixes the plugin/worker boundary as the integration surface.

The decision pressure is therefore: **how does an order placed in nopCommerce reach WMS without making checkout depend on WMS, and without violating boundary discipline?**

## 2. Proposed solutions

| Option                                       | Basic move                                                    |
|----------------------------------------------|---------------------------------------------------------------|
| A. Synchronous HTTP from checkout to WMS     | Call WMS inline on `OrderPlacedEvent` (the boring option)     |
| B. Worker polls nopCommerce DB for new orders| Background worker reads `Order` rows and pushes to WMS        |
| C. Outbox in plugin + RabbitMQ to worker     | Durable hand-off; broker absorbs the WMS dependency           |
| D. Fire-and-forget MQ publish from web thread| Publish to RabbitMQ inline; no outbox table                   |

### Option A — Synchronous HTTP from checkout

**Mechanism:** the plugin calls WMS directly in the `OrderPlacedEvent` handler, on the web request thread.

**Works because:** simplest path; the system instantly knows whether WMS accepted the order; no new infrastructure.

**May fail because:** checkout latency and availability inherit WMS's worst case; WMS outage breaks checkout; retry/backoff has to live on the web request thread; an order can be created in nopCommerce but never reach WMS if the HTTP call fails after commit.

### Option B — Worker polls nopCommerce DB for new orders

**Mechanism:** a separate worker process queries the nopCommerce `Order` table for newly placed orders and pushes them to WMS.

**Works because:** no broker dependency.

**May fail because:** the worker reads nopCommerce internals, violating ADR-0005; ordering, ack, and at-most-once semantics must be reinvented on the DB; back-pressure and retry policy become hidden in SQL state; cross-boundary coupling is now invisible at code review.

### Option C — Outbox in plugin + RabbitMQ to worker

**Mechanism:** on `OrderPlacedEvent` the plugin writes `OmniOutboxMessage` in the same transaction as nopCommerce's own order side effects. A scheduled publisher drains the outbox to RabbitMQ. The worker consumes, calls WMS, and posts results back to the plugin via HTTP.

**Works because:** checkout completes as soon as the outbox row is durable; transport and retry live outside the web request thread; broker provides ack, redelivery, and DLQ; every fulfillment intent has a replayable record.

**May fail because:** delivery is at-least-once, so consumers must be idempotent (covered by ADR-0006); the outbox publisher is a scheduled-task with its own health signal; RabbitMQ becomes a new operational dependency.

### Option D — Fire-and-forget MQ publish from web thread

**Mechanism:** the plugin publishes directly to RabbitMQ inside the `OrderPlacedEvent` handler, without an outbox table.

**Works because:** lowest infrastructure surface that still removes WMS from checkout.

**May fail because:** a crash or broker outage between DB commit and publish silently drops the event; no durable record that "this order was supposed to fulfil"; recovery from a broker outage cannot be replayed from nopCommerce state alone.

## 3. Technical analysis

### Trade-off table

| Option                  | Works because                                          | May fail because                                          |
|-------------------------|--------------------------------------------------------|-----------------------------------------------------------|
| A. Sync HTTP            | Immediate confirmation; no new infra                   | Checkout inherits WMS availability/latency                |
| B. DB polling           | No broker dependency                                   | Violates ADR-0005; hides coupling; reinvents ack/retry    |
| C. Outbox + MQ          | Durable hand-off; replayable; decouples checkout       | Broker + outbox health to monitor; at-least-once          |
| D. Fire-and-forget MQ   | Lowest surface that still removes WMS from checkout    | Silently loses events on crash between commit and publish |

### Evaluation forces matrix

| Force                | A. Sync HTTP        | B. DB poll          | C. Outbox + MQ                 | D. Fire-and-forget    |
|----------------------|---------------------|---------------------|--------------------------------|-----------------------|
| Deployability        | neutral             | neutral             | adds broker + scheduled task   | adds broker           |
| Data consistency     | strong but fragile  | weak (no ack)       | at-least-once + idempotency    | weak (drops possible) |
| Operational complexity| low                | medium              | medium-high                    | low-medium            |
| Fault isolation      | poor (checkout breaks) | medium           | strong (broker absorbs WMS)    | poor (no replay)      |
| Team ownership       | one team            | unclear (DB shared) | plugin owns outbox, worker owns transport | unclear     |
| Supportability       | poor (failures in user path) | poor (DB-side reasoning) | strong (queue/DLQ + outbox rows visible) | very poor (events disappear) |
| Reversibility        | medium              | low                 | medium (transport swappable)   | low (no recovery record) |

### Dominant forces for this ADR

- **Fault isolation** between checkout and WMS — QA-1 cannot tolerate WMS taking down checkout.
- **Supportability / observability** of in-flight fulfillment — QA-3 requires "why is order X pending?" to be answerable from durable artefacts.
- **Data consistency** under redelivery — accepted because ADR-0006 makes idempotency a first-class invariant.

### Accepted damage

- RabbitMQ becomes a runtime dependency to deploy, monitor, and recover.
- A scheduled outbox publisher becomes a code path with its own health signal (lag, run frequency, error rate).
- At-least-once delivery pushes the idempotency burden onto every consumer (covered by ADR-0006).
- Recovery time after a RabbitMQ outage depends on backlog drain rate, not bounded by the architecture itself.

## 4. Final solution

Adopt **Option C — durable outbox in the plugin + RabbitMQ to the worker**.

The plugin writes `OmniOutboxMessage` in the same DB transaction as the nopCommerce order side effects. A scheduled publisher drains pending rows to RabbitMQ. The worker consumes, calls WMS, and posts back to the plugin via HTTP for projection updates.

This option wins despite its operational cost because:

- it removes WMS health from the checkout availability budget (QA-1);
- it produces a durable, replayable trail of every fulfillment intent (QA-3);
- the at-least-once consequence is already absorbed by ADR-0006 (idempotency strategy);
- alternative D would have been cheaper in infra but loses the durable record that makes the demo's recovery scenario inspectable.

This decision is not "async is better"; it is "the dominant forces are fault isolation and supportability, and Option C is the only one that satisfies both without breaking ADR-0005".

## Triggers to revisit

Reopen this decision if any of the following becomes true:

- Checkout latency or error rate becomes dominated by the outbox publisher rather than by nopCommerce itself.
- Broker operational cost (queue lag, DLQ size, redelivery rate) becomes the top support burden — a transactional-outbox CDC approach (e.g., Debezium) or a different transport may be warranted.
- A second consumer of `commerce.order.placed.v1` (e.g., analytics) emerges and forces revisiting the fan-out shape.
- WMS gains a synchronous SLA strong enough to weaken the case for buffering.

## Evidence to watch

- **Outbox lag** (rows pending > N seconds) during normal and degraded runs in the demo.
- **Worker retry / DLQ counts** during the induced WMS outage scenario.
- **End-to-end time** from `OrderPlacedEvent` to `fulfillment.accepted` projection, under nominal and degraded WMS.
- **Duplicated fulfillment side effects** observed — must remain 0; non-zero indicates ADR-0006 has regressed and this ADR's accepted-damage assumption no longer holds.

## Rejected Alternatives

- **A. Synchronous HTTP from checkout to WMS** — rejected: WMS degradation would break checkout or force long user waits; fails QA-1.
- **B. Worker polls nopCommerce DB for new orders** — rejected: violates ADR-0005 (shared-DB coupling) and reinvents ack/retry semantics on top of SQL.
- **D. Fire-and-forget MQ publish from web thread** — rejected: a crash between DB commit and publish silently loses the fulfillment intent; the demo cannot show recovery from state that does not exist.
