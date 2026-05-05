# ADR-008 — Carrier Status via Scheduled Polling

| | |
|---|---|
| Status | Accepted |
| Date | 2026-05-04 |
| Produced by | ADD Iteration 4 — Step 6 (drivers: QAS-5; CON-18; CON-23; CON-24) |

## Context

QAS-5 requires that a carrier-issued shipment status change produces a customer-visible state change and a queued notification email within 30 seconds. The original design for this path used an inbound webhook endpoint on nopCommerce, with the carrier (WireMock) pushing status updates on state change.

WireMock, the carrier stand-in used throughout this project, does not implement webhook retry logic. A push-based design whose sender cannot retry is unverifiable under test: a single missed delivery leaves nopCommerce silently diverged from carrier state with no recovery path. The guarantee QAS-5 requires cannot be owned by nopCommerce if the delivery mechanism is controlled entirely by the sender.

Polling inverts the dependency: nopCommerce reads carrier state on its own schedule. After any outage of any duration, the next poll tick reads the current status of all open shipments and reflects it — no dependency on the carrier having retained a pending notification. The reliability guarantee is unconditional and entirely within nopCommerce's control.

This is the same reasoning that drove ADR-009 for OpenBoxes, and the same `IScheduleTask` pattern applies here.

## Decision

Introduce `CarrierStatusPollerTask`, an `IScheduleTask` running inside the `Nop.Plugin.Shipping.CarrierWebhook` plugin, polling `GET /api/shipments/{ExternalShipmentId}/status` against WireMock every 30 seconds for each open shipment.

On each tick the task fetches the current status for all shipments in a non-terminal state and compares against the last-known status recorded in nopCommerce. When a status transition is detected:

1. `Shipment.ExternalShippingStatus` is updated.
2. A customer notification email is queued.

The `Shipment` entity requires an `ExternalShipmentId` field, populated when the carrier booking response is processed by `CarrierBookingConsumer` (Iteration 4 outbound path), to correlate poll responses to the correct order.

The 30-second interval satisfies QAS-5's response measure and is consistent with the OpenBoxes polling interval established in ADR-009.

## Rejected Alternatives

**Inbound webhook from WireMock (original design).**
WireMock does not implement retry logic on webhook delivery. A single failed delivery leaves nopCommerce diverged from carrier state with no self-healing path. QAS-5's guarantee cannot be satisfied by a mechanism the sender can abandon. *Rejected.*

**Inbound webhook with nopCommerce-side retry request.**
nopCommerce could detect a missed delivery by comparing expected transitions against actual state and requesting a re-delivery from the carrier. *Rejected:* this requires nopCommerce to know what it doesn't know — the transition it missed — which requires polling anyway. The workaround reimplements polling at higher complexity.

**Polling from inside the OpenBoxes Bridge.**
The bridge runs as an independent process and could be extended to also poll the carrier API. *Rejected:* the bridge's responsibility is translating `order.placed` messages into OpenBoxes fulfillment orders. Adding a carrier polling loop couples two unrelated concerns and inverts the dependency direction ADR-007 established.

## Consequences

- QAS-5 carrier half is structurally satisfied: the 30-second poll interval is the worst-case detection latency, and the status update and email enqueue happen within the same tick.
- The polling task runs one outbound HTTP call per open shipment per tick. At VerdeMart's current scale this is negligible; at larger scale a batch endpoint would reduce request count.
- No inbound HTTP endpoint is exposed to the public internet for carrier callbacks, removing the bearer-auth and audit-table infrastructure the webhook design required.
- The `IScheduleTask` framework runs on a single node. In a multi-node deployment, multiple nodes would poll concurrently; the status update must be guarded by a last-write-wins check on the current status to avoid a redundant transition. This is a known gap at VerdeMart's current single-node scale.
- Circuit-breaker and timeout configuration for the outbound HTTP call to WireMock are recorded as a production-hardening residual.
