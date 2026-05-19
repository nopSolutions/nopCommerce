# Quality Attribute Scenarios

Five scenarios in **SEI 6-part format** (source / stimulus / artifact / environment / response / response measure). These drive the three ADD iterations in [architecture-checkpoint.md §6](architecture-checkpoint.md). The summary table in §4 of the checkpoint is the short view; this document is the testable view.

## Why these five

The lecturer's QA deck names four "core" attributes — **Performance, Availability, Security, Modifiability** — and a set of extension attributes (Deployability, Integrability, Auditability, Operability, Energy efficiency, Portability). Scenario C exercises **Availability** (under WMS degradation), **Performance** (off the checkout critical path), **Integrability** (POS integration without polluting core stock — recorded under "Consistency" below for clarity), **Auditability** (recorded as "Traceability"), and **Operability**. **Security** is satisfied structurally (internal endpoints behind a demo token, ADR-0005 boundary) but is not the architectural pressure of this scenario. **Modifiability** is satisfied structurally by the plugin/worker boundary (ADR-0002, ADR-0005) — adding a new external system means adding a worker handler, not changing core code — and so does not warrant its own runtime scenario. We deliberately keep the count at five: the rubric caps it there, and adding a sixth would dilute focus.

## QA-1 — Resilience and Recovery: checkout under WMS unavailability

| Part            | Value                                                                                              |
|-----------------|----------------------------------------------------------------------------------------------------|
| Source          | WMS simulator returning HTTP 503                                                                   |
| Stimulus        | All fulfillment requests fail for 30 consecutive seconds, then WMS returns to HTTP 200             |
| Artifact        | Order placement path in nopCommerce + Omnichannel plugin outbox + worker retry/circuit-breaker     |
| Environment     | Normal store load, RabbitMQ healthy, plugin scheduled task active                                  |
| Response (degraded) | Checkout completes; order persisted; outbox row written; fulfillment state = `pending`; worker retries with exponential backoff and trips circuit breaker after threshold |
| Response (recovery) | After WMS returns to 200, worker drains the queued backlog and transitions affected fulfillments from `pending` → `accepted`; circuit breaker re-closes after success window |
| Response measure| **Degraded**: checkout P95 ≤ 1.5× normal-flow baseline; 0 checkout failures attributable to WMS; outbox row written ≤ 100 ms after `OrderPlacedEvent`. **Recovery**: backlog drain ≤ 60 s after WMS returns 200; 0 orders left in `pending` for > 5 min after recovery |

Drives **Iteration 1**. The recovery half of this scenario is the assignment's mandatory "how the affected path recovers or reconciles" demo line.

## QA-2 — Consistency: duplicate or stale POS stock event

| Part            | Value                                                                                              |
|-----------------|----------------------------------------------------------------------------------------------------|
| Source          | POS simulator                                                                                      |
| Stimulus        | Same `pos.stock.changed.v1` message redelivered, OR a message with `sourceVersion` lower than the last applied version |
| Artifact        | Plugin inbox + `OmniStockSyncState` projection                                                     |
| Environment     | Normal load, at-least-once delivery semantics                                                      |
| Response        | Duplicate `messageId` rejected; older `sourceVersion` ignored; projection state unchanged          |
| Response measure| Duplicate detection ≤ 50 ms; 0 duplicate fulfillment side effects; stale-update count visible in admin view |

Drives **Iteration 2**.

## QA-3 — Traceability: explain a pending order without code reading

| Part            | Value                                                                                              |
|-----------------|----------------------------------------------------------------------------------------------------|
| Source          | Support agent investigating customer complaint                                                     |
| Stimulus        | Question: "Why is order `OrderGuid` still pending fulfillment?"                                    |
| Artifact        | Plugin admin view + structured logs + RabbitMQ management UI                                       |
| Environment     | Production-like; order placed within last 24 h                                                     |
| Response        | Agent locates outbox row, MQ message ID, worker attempt log, fulfillment state — all linked by `OrderGuid` and `messageId` |
| Response measure| 100% of orders link outbox row → MQ message → worker attempt → projection row; resolution path visible in ≤ 3 admin clicks |

Drives **Iteration 3**.

## QA-4 — Operability: degraded WMS visible to operators

| Part            | Value                                                                                              |
|-----------------|----------------------------------------------------------------------------------------------------|
| Source          | Operations on-call                                                                                 |
| Stimulus        | WMS simulator switched to `slow` or `unavailable` mode                                             |
| Artifact        | RabbitMQ management UI + plugin admin view + worker logs                                           |
| Environment     | Live demo                                                                                          |
| Response        | Operator observes: queue depth rising, retry count > 0, circuit breaker open, DLQ size if persistent |
| Response measure| Queue depth, retry count, DLQ size, fulfillment-pending count visible in one dashboard view; refresh latency ≤ 5 s |

Cross-cutting; supports **Iteration 1** and **Iteration 3**.

## QA-5 — Performance: integration off the checkout critical path

| Part            | Value                                                                                              |
|-----------------|----------------------------------------------------------------------------------------------------|
| Source          | Customer placing an order                                                                          |
| Stimulus        | `OrderPlacedEvent` fired                                                                           |
| Artifact        | Plugin `OrderPlacedEvent` consumer + outbox table                                                  |
| Environment     | Normal load                                                                                        |
| Response        | Plugin writes outbox row and returns; no synchronous WMS or worker call on the checkout thread     |
| Response measure| Outbox row written ≤ 100 ms after `OrderPlacedEvent`; 0 synchronous external HTTP calls in checkout trace |

Cross-cutting; supports **Iteration 1**.

---

**Baseline definition.** "Normal-flow baseline" = checkout latency measured against the unmodified nopCommerce flow before omnichannel plugin is installed. Captured in Part 2 evidence pack.

**Common writing failures avoided** (per ADD slide on scenario writing): no adjective-only measures; environment named on every scenario; artifact names a specific component, not "the system"; no architectural decisions written into the scenario itself.

## Prioritization

Mapping each scenario onto the lecturer's two-axis grid (business importance × technical risk). "Attack first" = upper-right quadrant.

| ID  | Business importance | Technical risk | Quadrant       | Why                                                                       |
|-----|---------------------|----------------|----------------|---------------------------------------------------------------------------|
| QA-1| High                | High           | **Attack first** | Mandatory pressure point of the scenario; nopCommerce has no native async path |
| QA-2| High                | Medium         | **Attack first** | At-least-once is unavoidable; idempotency is well-known but must be applied correctly |
| QA-3| Medium              | Low            | Cheap win      | Correlation IDs are cheap to thread through; payoff is high               |
| QA-4| Medium              | Low            | Cheap win      | RabbitMQ management UI + simple admin view; mostly off-the-shelf           |
| QA-5| High                | Low            | Cheap win      | Trivially satisfied by writing the outbox row inside the consumer; no sync HTTP in checkout is enforceable by code review |

Iteration order in §6 reflects this: Iteration 1 attacks both upper-right scenarios, Iteration 2 closes QA-2's residual risk, Iteration 3 collects the cheap wins.
