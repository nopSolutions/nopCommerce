# ADR-009 — OpenBoxes Fulfillment State via Scheduled Polling

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-29 |
| Produced by | ADD Iteration 5 — Step 6 (driver: QAS-5 Visibility — warehouse half) |

## Context

QAS-5 requires that an OpenBoxes fulfillment order reaching `ISSUED` is reflected in nopCommerce within 30 seconds, with no operator action. The carrier half of QAS-5 was closed in Iteration 4 via inbound webhook (ADR-008). The warehouse half was deferred because it depends on how OpenBoxes can emit state changes outward.

OpenBoxes supports outbound webhooks with configurable endpoints per event type, full resource payloads, and retry logic for failed deliveries. However, webhook retry logic is bounded — exponential backoff with a fixed retry count or window before the delivery attempt is abandoned. QAS-4 requires self-healing after an outage of up to 30 minutes with no operator action. Whether that window is covered depends entirely on how OpenBoxes' retry is configured — a guarantee nopCommerce cannot own.

Polling inverts the dependency: nopCommerce reads OpenBoxes state on its own schedule. It reads state, not events. After any outage of any duration, the next poll tick reads the current state of all open fulfillment orders and reflects it — no dependency on OpenBoxes having retained a pending delivery. The reliability guarantee is unconditional and entirely within nopCommerce's control.

A further constraint shapes the hosting decision: `02-current-state.md` documents that `IScheduleTask` has no per-item progress tracking and no at-least-once delivery guarantee. However, the polling operation here is a read — not a write that can be lost. A missed poll tick means a delay of at most one additional interval, not a lost event. The reliability profile of `IScheduleTask` is therefore acceptable for this use case in a way it was not for the Outbox (ADR-004).

## Decision

Introduce `OpenBoxesStatusPollerTask`, an `IScheduleTask` running inside the existing `Nop.Plugin.Messaging.RabbitMq` plugin (or a dedicated OpenBoxes plugin), polling `GET /api/generic/shipment` every 30 seconds.

On each tick the task fetches fulfillment orders in a non-terminal state and compares their status against the last-known status recorded in nopCommerce. When a fulfillment order transitions to `ISSUED`:

1. The corresponding nopCommerce order status is updated.
2. A `carrier.booking.requested` row is written to the `OutboxMessage` table, triggering the carrier booking chain established in Iteration 4.

The 30-second interval is not arbitrary: it is the upper bound stated in QAS-5's response measure. Any shorter interval tightens the QAS-5 margin without changing the architecture; any longer interval violates it.

## Rejected Alternatives

**OpenBoxes push webhooks to a nopCommerce endpoint.**
OpenBoxes supports configurable outbound webhooks with retry logic for failed deliveries. *Rejected:* webhook retry is bounded — a finite attempt count with exponential backoff before the delivery is abandoned. QAS-4 requires self-healing after up to 30 minutes of outage; whether the retry window covers that is a function of OpenBoxes' configuration, not nopCommerce's. Satisfying your own QAS by relying on a third party's retry window is not a guarantee. Beyond reliability, webhooks deliver events — if any delivery in the sequence is dropped, nopCommerce and OpenBoxes permanently diverge on that order's status. Polling reads current state on every tick; divergence is structurally impossible. Webhooks would also require a new inbound HTTP endpoint on nopCommerce with bearer auth, an audit table, and an idempotency table — the same infrastructure ADR-008 built for the carrier. That cost is justified when the sender has no queryable API (the carrier). OpenBoxes does.

**Polling from inside the OpenBoxes Bridge (the Iteration 3 separate deployable).**
The bridge already has a connection to OpenBoxes and runs as an independent process. Adding the polling loop there would avoid introducing any new component. *Rejected:* the bridge's responsibility is translating `order.placed` messages into OpenBoxes fulfillment orders — it is a one-way writer. Giving it a read-and-reflect responsibility couples two unrelated concerns in the same deployable. It also creates a direct dependency from the bridge back into nopCommerce (to update order status), inverting the dependency direction ADR-007 established.

**A second separate polling deployable.**
A dedicated poller service, symmetric to the bridge, could poll OpenBoxes and publish a `fulfillment.status.updated` message to RabbitMQ for nopCommerce to consume. *Rejected:* the brief's "independently deployable subsystem" requirement is already satisfied by the bridge. A second deployable adds operational cost (deploy, monitor, restart) for a polling task whose only output is a queue message that nopCommerce then processes anyway. The `IScheduleTask` path removes one hop and one process boundary at no reliability cost, since the task is a read and a missed tick is recoverable on the next interval.

**Increasing the OpenBoxes Bridge poll frequency as a substitute.**
Rather than adding a new task, the bridge could be modified to re-read each fulfillment order it created and push status back via RabbitMQ. *Rejected:* same concern as the bridge hosting option above — it mixes outbound order creation with inbound state feedback in the same component. It also requires the bridge to maintain a local record of every fulfillment order it has ever created in order to query them on each tick, expanding its state surface significantly.

## Consequences

- QAS-5 warehouse half is structurally satisfied: the 30-second poll interval is the worst-case detection latency, and the status update and Outbox write happen within the same tick.
- The `ISSUED` detection triggers the Iteration 4 carrier booking chain via the Outbox, closing the full fulfillment loop without introducing a new message type or a new consumer.
- OpenBoxes API availability is on the poll tick path. A degraded OpenBoxes response delays the tick but does not lose state — the next tick retries the same read. This is acceptable under QAS-1's 30-minute outage budget.
- The polling task adds one outbound HTTP dependency inside the nopCommerce process. Circuit-breaker and timeout configuration for this call are recorded as a production-hardening residual.
- The `IScheduleTask` framework runs on a single node. In a multi-node nopCommerce deployment, multiple nodes would poll concurrently. The Outbox write is idempotent on `OrderGuid`, so duplicate writes are harmless; the order status update must be guarded by a last-write-wins check on the current status to avoid a redundant transition. This is a known gap at VerdeMart's current single-node scale and is documented in `08-risk-and-validation-plan.md`.
