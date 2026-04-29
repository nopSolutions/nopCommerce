# ADR-012 — External Shipment Correlation via `ExternalShipmentId` on `Shipment`

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-29 |
| Produced by | ADD Iteration 4 — Step 6 (drivers: QAS-5; CON-17; `04-qas.md:83`) |

## Context

QAS-5 explicitly states the design constraint in its design-decision-forced clause (`04-qas.md:83`): *"The `Shipment` entity needs an `ExternalShipmentId` field to correlate the incoming webhook to the correct order."* When WireMock POSTs `shipment.status.updated` with `{ carrierTrackingId: "WM-ABC-123", … }`, nopCommerce needs a way to map that to the local `Shipment` row.

Two related concerns shape the choice:
- CON-17: webhook arrival can race the dispatch event. The carrier may emit `shipment.status.updated` before nopCommerce has committed the local dispatch record, leaving the webhook with no correlation target.
- A single shipment may be split, re-routed, or carried by multiple carriers in real-world logistics, raising the question of whether a single `ExternalShipmentId` column is sufficient or a 1-to-many side table is needed.

## Decision

**Storage:** four new nullable columns are added to the existing `Shipment` entity (in `Nop.Core.Domain.Shipping`):
- `ExternalShipmentId` (`varchar(128)`) — the carrier-issued tracking identifier
- `ExternalCarrierCode` (`varchar(32)`) — e.g. `"WIREMOCK"` — to disambiguate when multi-carrier is added later
- `ExternalShippingStatus` (`varchar(64)`) — most-recent carrier vocabulary (see ADR-013)
- `LastStatusOccurredAtUtc` (`datetime(6)`) — out-of-order guard timestamp

A composite index `(ExternalCarrierCode, ExternalShipmentId)` supports the inbound consumer's lookup. All columns are nullable so existing `Shipment` rows are unaffected.

**Population:** `ExternalShipmentId` is populated by the outbound `CarrierBookingConsumer` after a successful WireMock booking call (see ADR-014). A `Shipment` row exists from the moment the admin creates the shipment; it acquires `ExternalShipmentId` once the booking call returns.

**Race handling (CON-17):** when an inbound webhook arrives but no `Shipment` matches the `(ExternalCarrierCode, ExternalShipmentId)` lookup, the consumer NACKs with requeue. After `MaxStatusRedeliveries` (default 5), the message routes to the DLQ. By the time the threshold is reached, either the booking has committed, or operators have a queue-tooling-visible signal that something is wrong upstream.

## Rejected Alternatives

**Separate 1-to-many side table (`ShipmentExternalRef`).**
*Rejected:* VerdeMart has one carrier per shipment and no current scenario requires multiple correlation IDs per shipment. The side table doubles join cost on a hot read path (every webhook does this lookup) for a feature nobody is asking for. Adding the side table later, if multi-carrier or split shipments arrive, is a non-breaking schema migration.

**Store `ExternalShipmentId` in a JSON metadata blob on `Shipment`.**
*Rejected:* the field is hot — every inbound webhook does a `WHERE ExternalShipmentId = ?` lookup. Indexing JSON paths is awkward across MSSQL/MySQL/PostgreSQL and ties the design to engine-specific JSON support.

**Reject unmatched webhooks immediately (return 404, no requeue).**
*Rejected:* this defeats CON-17 — a webhook that arrives a few hundred milliseconds before the booking commits would be permanently lost. The NACK-with-requeue pattern absorbs the race window without operator action.

**Generate `ExternalShipmentId` locally and tell the carrier what value to use.**
*Rejected:* carriers issue their own tracking identifiers and refuse to accept caller-supplied values for them. The integration must accept the carrier's identifier, not impose one.

## Consequences

- Inbound webhook correlation is a single indexed lookup (`(ExternalCarrierCode, ExternalShipmentId)`) — sub-millisecond at any plausible scale.
- The schema change is additive: existing `Shipment` rows acquire NULL values; existing nopCommerce code paths that don't know about external correlation continue to work unchanged.
- The four columns live on the entity itself (not a side table) so the data layer's `IRepository<Shipment>` returns shipments fully populated without joins.
- The `ExternalCarrierCode` column reserves the multi-carrier door without forcing the design now. If a second carrier is added, the mapping table in `IExternalStatusMapper` becomes carrier-aware and the lookup remains structurally identical.
- The race window (CON-17) is bounded by `MaxStatusRedeliveries × redelivery interval`. With the default 5 redeliveries and RabbitMQ's default redelivery semantics, the bound is well inside human-operator response time.
- Once `ExternalShipmentId` is set, it is never changed in normal operation; if the carrier reissues a tracking id (rare), the inbound flow's "unmatched → DLQ" path catches it for operator review.
- The new columns add ~250 bytes per `Shipment` row (most rows will populate them); operationally negligible.
