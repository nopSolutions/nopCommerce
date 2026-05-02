# ADR-008 — Webhook Ingestion via Plugin with Async Internal Queue Handoff

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-29 |
| Produced by | ADD Iteration 4 — Step 6 (drivers: QAS-5; CON-18; CON-20; CON-23; CON-24) |

## Context

QAS-5 requires that a carrier-issued `shipment.status.updated` webhook produces a customer-visible state change and a queued notification email **within 10 seconds** of receipt. Pressure point #7 from `02-current-state.md:84-90` named the gap explicitly: nopCommerce has no framework-level scaffold for inbound webhooks — no signature verification, no idempotent ingestion, no retry-handling pattern. Every plugin would otherwise reinvent these.

Three operational concerns shape the design:
- CON-18: carriers retry on 5xx and on timeout. The handler must be idempotent under repeated delivery of the same event.
- CON-20: an inbound HTTP endpoint exposed to the public internet must authenticate.
- CON-24: any synchronous external work inside the controller eats the 10 s budget and risks triggering carrier-side retries with opaque timing.

Iteration 3 already established the dedup-table + DLQ pattern for at-least-once consumers (via the OpenBoxes bridge); the question for this ADR is **where** the work happens once a webhook is received.

## Decision

**Hosting:** the webhook endpoint lives inside a new nopCommerce plugin (`Nop.Plugin.Shipping.CarrierWebhook`), consistent with ADR-002. No new deployable is introduced. Background services (`CarrierStatusConsumer`, `CarrierBookingConsumer`) are registered as `IHostedService` and run in the same process as the nopCommerce web app.

**Async handoff:** the controller has three responsibilities only — authenticate, write the raw payload to the audit table, publish to a local RabbitMQ queue (`verdemart.carrier.status`). It returns 200 to the carrier *before* any business processing happens. A separate `CarrierStatusConsumer` drains the queue and performs dedup, correlation, the out-of-order guard, the status update, and the email enqueue.

**Authentication:** bearer token on the `Authorization` header, value from configuration. Constant-time comparison; mismatches return 401 and are still recorded in the audit table with `Outcome=Rejected, Notes="auth"`.

**Idempotency:** dedup table `ProcessedCarrierEvent` keyed by carrier-supplied `EventId` (UUID in the payload). If the event id is already present, the consumer acks and skips. Same defence-in-depth shape as the OpenBoxes bridge dedup table.

**Out-of-order guard:** the consumer applies an update only when the payload's `OccurredAtUtc` is strictly greater than `Shipment.LastStatusOccurredAtUtc`. Ordering signal is the carrier's authoritative timestamp; arrival order is irrelevant.

**Poison handling:** the inbound queue carries `x-dead-letter-exchange = verdemart.carrier.status.dlx`. Per-message redelivery counted from RabbitMQ's `x-death` header; once the configured `MaxStatusRedeliveries` is exceeded (default 5), the consumer NACKs without requeue and the broker routes the message to `verdemart.carrier.status.dlq` for operator review. Same DLQ pattern as the OpenBoxes bridge.

**Audit:** every receipt is written to `CarrierWebhookEvent` *before* any other work — including malformed payloads (`Outcome=Rejected, Notes="malformed"`) and auth failures. The audit table is queryable ground truth for dispute resolution.

## Rejected Alternatives

**Synchronous in-controller processing.**
*Rejected:* internal slowness (DB contention, slow email infrastructure) would translate directly into slow carrier responses, triggering carrier retries with opaque timing. The 10 s budget would be split between the carrier's HTTP timeout and nopCommerce's processing — half is gone before anything starts. The controller would also own retry logic that the broker provides for free.

**Reuse the existing publisher Outbox + `OutboxDispatcherTask` for inbound webhooks.**
*Rejected:* the existing Outbox is publisher-side (writes that need to *leave* nopCommerce), and the dispatcher's polling cadence is tuned for that. An inbound queue with a push consumer is lower-latency and structurally clearer; reusing the outbox here would conflate two semantics in one table.

**HMAC payload signing instead of bearer token.**
*Rejected for this iteration:* HMAC is the production-grade choice and offers payload-integrity protection that bearer does not, but for the WireMock demo the carrier sends what we tell it, so the threat model bearer guards against (token theft from logs/headers) is the dominant residual risk. HMAC is recorded as a production-hardening residual.

**No webhook auth (demo only).**
*Rejected:* exposing an unauthenticated endpoint that mutates customer-visible state is the kind of footgun the architecture exercise is supposed to *avoid*, not normalise.

**Audit only successful receipts.**
*Rejected:* rejected receipts (auth failures, malformed payloads) are exactly the ones operators most need to see — they indicate misconfiguration or attack. Auditing every receipt is cheap (one INSERT) and operationally honest.

## Consequences

- The webhook controller's response time is bounded by audit-row INSERT + queue publish — typically tens of milliseconds. The carrier never sees nopCommerce-internal slowness.
- Internal retries are deterministic: NACK with requeue inside the broker, redelivery threshold, then DLQ. Operators have a queue-tooling-visible signal for stuck messages.
- Webhook auth is configuration-driven; rotating the token requires only a settings change and a corresponding update to WireMock's outbound config.
- The `CarrierWebhookEvent` audit table grows linearly with carrier traffic; retention is an operational concern (recorded as a residual).
- The 10 s QAS-5 budget is met by construction with substantial margin; empirical timing remains owed by the bundled spike.
- Two new DLQs join the OpenBoxes one from Iteration 3; the operational replay tooling residual broadens but the pattern is uniform across all three.
- The plugin produced here is a candidate template for future inbound integrations (payment-status callbacks, supplier ASN feeds). No framework generalisation is performed in this iteration; doing so would be premature.
