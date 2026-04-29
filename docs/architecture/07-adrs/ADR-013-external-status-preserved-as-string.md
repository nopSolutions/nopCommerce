# ADR-013 — External Status Preserved as String; Internal Enum Untouched

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-29 |
| Produced by | ADD Iteration 4 — Step 6 (drivers: QAS-5; CON-19; CON-21; ADR-002) |

## Context

Carrier vocabularies are richer than nopCommerce's internal `ShippingStatus` enum:

| nopCommerce `ShippingStatus` (in `Nop.Core.Domain.Shipping`) | Carrier vocabulary (WireMock) |
|---|---|
| `NotYetShipped` | `BOOKED` |
| `Shipped` | `PICKED_UP`, `IN_TRANSIT`, `OUT_FOR_DELIVERY` |
| `Delivered` | `DELIVERED` |
| `ShippingNotRequired` | (no equivalent) |
| (no equivalent) | `EXCEPTION`, `RETURNED` |

QAS-5's response measure says "tracking status visible to the customer." A many-to-few mapping into the existing enum collapses `IN_TRANSIT` and `OUT_FOR_DELIVERY` into `Shipped` and loses precisely the visibility the customer is supposed to gain. Extending the enum is invasive: `ShippingStatus` lives in `Nop.Core` and is consumed by every shipping-related service in the framework — adding members forces existing `switch` statements to handle them or break, which is out of plugin scope per ADR-002.

CON-19 (out-of-order delivery) also needs an authoritative ordering signal that does not depend on whether a status change crosses an internal-enum threshold.

## Decision

**Two-vocabulary model:** the carrier's status vocabulary is preserved verbatim alongside the existing internal enum, not folded into it.

- `Shipment.ExternalShippingStatus` (varchar) — most recent carrier value verbatim, e.g. `"OUT_FOR_DELIVERY"`. Customer-facing render path uses this when present.
- `Shipment.ShippingStatus` (existing enum) — left untouched in the framework. Internal nopCommerce code paths that already consume it continue to work at their existing granularity.
- `Shipment.LastStatusOccurredAtUtc` — carrier-supplied timestamp of the most recent applied event. Used by the out-of-order guard.

**Mapping:** a hard-coded `IExternalStatusMapper` inside the plugin returns a `ShippingStatus?`. The consumer applies the internal-enum transition only when `MapToInternal(externalStatus)` returns non-null. Carrier values that don't imply an internal transition (`PICKED_UP`, `IN_TRANSIT`, `OUT_FOR_DELIVERY` — all of which the existing enum represents as `Shipped`; `EXCEPTION`, `RETURNED` — no internal equivalent) leave the internal enum unchanged.

**Out-of-order guard:** the consumer applies an update only when the payload's `OccurredAtUtc` is strictly greater than `Shipment.LastStatusOccurredAtUtc`. Older events are recorded in the audit table but do not move state. Carrier-supplied timestamps are the authoritative ordering signal, regardless of arrival order on the wire.

**Render path:** the customer-facing order-detail page renders `ExternalShippingStatus` when present, falling back to the internal enum otherwise. A new `ShipmentStatusUpdated.CustomerNotification` email template uses two new tokens (`%Shipment.ExternalStatus%`, `%Shipment.ExternalStatusDescription%`) to surface the richer vocabulary verbatim.

## Rejected Alternatives

**Extend `ShippingStatus` with new members (`InTransit`, `OutForDelivery`, `Exception`, `Returned`).**
*Rejected:* `ShippingStatus` lives in `Nop.Core` and is consumed by every shipping-related service in the framework. Adding members forces existing `switch` statements throughout the codebase to handle the new cases or break. The change is invasive and out of plugin scope per ADR-002.

**Many-to-few mapping only (no separate external string field).**
*Rejected:* the customer loses visibility of `OUT_FOR_DELIVERY` and `IN_TRANSIT` — exactly the visibility QAS-5 set out to give them. Nothing in the QAS asks the architecture to coarsen the carrier's view; it asks the architecture to surface it.

**Plugin-local enum stored as integer.**
*Rejected:* a plugin enum is more brittle than a string under carrier-vocabulary evolution. Adding a new carrier value requires a code change and plugin redeploy with a new enum member; storing the string survives most carrier-side schema additions without code change. Strings also serialise more honestly across the audit log.

**Use arrival order rather than carrier `OccurredAtUtc` for ordering.**
*Rejected:* CON-19's worst case is real — webhook A (`IN_TRANSIT`) arriving after webhook B (`DELIVERED`) because of carrier retry windows, NAT timeouts, or transient routing issues. Trusting the wire arrival order would corrupt the customer-visible state in this case. Carriers don't edit past events; they emit new events with new timestamps, and the timestamp guard handles corrections cleanly: a corrective event has a newer `OccurredAtUtc` than the bogus one and wins.

**Make the mapping admin-configurable (DB-backed mapping table, CRUD UI).**
*Rejected:* the mapping is stable for a given carrier and changes maybe once a year. A CRUD UI, a settings surface, and an admin-procedure document are disproportionate to the change rate. The hard-coded design can be promoted to admin-configurable later if operational reality demands it; the inverse refactor is harder.

## Consequences

- The customer sees the carrier's actual vocabulary on the order page and in the notification email. QAS-5's visibility intent is honoured precisely.
- `Nop.Core` is not modified. ADR-002's plugin boundary is preserved.
- Internal nopCommerce code paths that consume `ShippingStatus` continue to work unchanged at their existing granularity. `Delivered` still triggers downstream logic that watches that transition.
- The carrier-vocabulary string survives carrier-side additive changes without code change. New carrier values that lack an internal mapping are recorded but produce no internal transition — operators see the new value in the audit log and can decide whether a code change is warranted.
- Out-of-order delivery is handled correctly: timestamp wins, not arrival order. The user's worry about a stale `DELIVERED` is resolved by the carrier's authoritative timestamp.
- Repeated identical events (same status, same `OccurredAtUtc`, same `EventId`) are dedup'd at the `EventId` layer (ADR-011) before the timestamp guard runs.
- Adding a second carrier later requires `IExternalStatusMapper` to become carrier-aware (`MapToInternal(carrierCode, externalStatus)`); the data model already supports it via `ExternalCarrierCode` (ADR-012).
- An "external status" that has no internal-enum implication does not trigger downstream nopCommerce logic that watches `ShippingStatus` transitions. If a future requirement demands action on `EXCEPTION` or `RETURNED`, the mapper can return a new internal status only if/when the framework's enum gains the corresponding member — an intentionally-deferred decision.
