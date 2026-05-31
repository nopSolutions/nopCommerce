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

Introduce `OpenBoxesStatusPollerTask`, an `IScheduleTask` running inside `Nop.Plugin.Fulfillment.OpenBoxes`, polling `GET /api/generic/shipment?status=ISSUED` every 30 seconds.

When a fulfillment order in `SHIPPED` state (the value OpenBoxes returns for issued shipments) is detected, the task creates a `Shipment`, sets the order to `Complete`, and writes a `carrier.booking.requested` outbox row — all within a single `TransactionScope`. After the transaction commits, nopCommerce calls back to OpenBoxes to confirm receipt via the partial-receiving API, closing the fulfillment loop on the OpenBoxes side.

Idempotency is flag-based: an `OpenBoxesReceiveConfirmed` attribute is stored on the `Shipment` entity after a successful receive confirmation. On subsequent ticks the poller checks this flag rather than comparing statuses. If the transaction committed but the receive confirmation failed, the next tick detects the missing flag and retries only the confirmation, not the full flow.

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

**Outbox for the receive confirmation call.**
The call to confirm receipt in OpenBoxes runs outside the `TransactionScope`. An outbox would protect against a crash in the millisecond window between the transaction commit and the confirmation call. *Rejected:* the polling interval itself is the retry mechanism — a failed confirmation leaves the fulfillment as `ISSUED` in OpenBoxes, which causes it to reappear in the next tick where only the confirmation is retried. The outbox is the right answer for a webhook design where a missed event is gone forever; for a polling design the next tick recovers unconditionally, making the outbox over-engineering.

**Redis cache and distributed lock (ADR-010).**
A status cache and distributed lock were designed to handle multi-node deployments and reduce DB reads. *Superseded:* no QA requires multi-instance deployment, and each fulfillment is processed exactly once — after confirmation it leaves the `ISSUED` state and never reappears in the poll batch, so the cache hit rate would be zero. See ADR-010.

## Consequences

- QAS-5 warehouse half is structurally satisfied: the 30-second poll interval is the worst-case detection latency, and the shipment creation, order status update, and outbox write happen within the same tick.
- The `SHIPPED` detection triggers the carrier booking chain via the outbox, closing the full fulfillment loop without introducing a new message type or a new consumer.
- nopCommerce now has a bidirectional relationship with OpenBoxes: the bridge sends orders outbound, and the poller confirms receipt inbound. This is a deliberate scope extension of the OpenBoxes integration.
- A partial tick failure (transaction committed, receive confirmation failed) self-heals on the next tick via the `OpenBoxesReceiveConfirmed` flag. No operator action is required.
- OpenBoxes API availability is on the poll tick path. A degraded response delays the tick but does not lose state — the next tick retries unconditionally.
- The `IScheduleTask` framework runs on a single node. In a multi-node deployment, multiple nodes would poll and process concurrently. The `TransactionScope` write is idempotent on `OrderGuid` but the receive confirmation could be duplicated. This is a known gap at VerdeMart's current single-node scale and is documented in `08-risk-and-validation-plan.md`.
