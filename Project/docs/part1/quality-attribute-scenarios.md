# Quality Attribute Scenarios

Five scenarios in **SEI 6-part format** (source / stimulus / artifact / environment / response / response measure). These drive the three ADD iterations in [architecture-checkpoint.md §6](architecture-checkpoint.md). The summary table in §4 of the checkpoint is the short view; this document is the testable view.

## QA-1 — Resilience: checkout under WMS unavailability

| Part            | Value                                                                                              |
|-----------------|----------------------------------------------------------------------------------------------------|
| Source          | WMS simulator returning HTTP 503                                                                   |
| Stimulus        | All fulfillment requests fail for 30 consecutive seconds                                           |
| Artifact        | Order placement path in nopCommerce + Omnichannel plugin outbox                                    |
| Environment     | Normal store load, RabbitMQ healthy, plugin scheduled task active                                  |
| Response        | Checkout completes; order persisted; outbox row written; fulfillment state = `pending`; worker retries with exponential backoff and trips circuit breaker |
| Response measure| Checkout P95 ≤ 1.5× normal-flow baseline; 0 checkout failures attributable to WMS; outbox row written ≤ 100 ms after `OrderPlacedEvent` |

Drives **Iteration 1**.

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
