# ADR-015 — OpenBoxes Fulfillment State via Scheduled Polling

| | |
| --- | --- |
| Status | Accepted |
| Date | 2026-05-01 |
| Produced by | ADD Iteration 5 — Step 6 (driver: QAS-5 warehouse visibility half; CON-25; CON-26; CON-27) |

## Context

QAS-5's warehouse visibility clause requires that when OpenBoxes marks a fulfillment order as `ISSUED`, that state becomes visible in nopCommerce within a bounded interval without operator intervention. The OpenBoxes feasibility spike confirmed that OpenBoxes provides no outbound webhook or event-push mechanism — its API is purely pull-based REST CRUD. A push-based design is architecturally preferable but not available.

The only automated option is polling.

## Decision

A new `IScheduleTask` (`OpenBoxesStatusPollerTask`) is added to `Nop.Plugin.Inventory.AllocationGate`. It runs on a configurable interval (default 30 seconds) and calls `GET /api/generic/shipment?status=ISSUED` on the OpenBoxes REST API. For each fulfillment order in `ISSUED` state, it correlates back to the nopCommerce order via `OrderGuid` (stored in OpenBoxes as `referenceNumber` at bridge creation time), updates the order status, and writes an outbox row to trigger the carrier booking chain — reusing the existing `OutboxDispatcherTask` from Iter 2 and the `CarrierBookingConsumer` from Iter 4.

The task is idempotent: if the order is already `Complete`, the detection is a no-op. All exceptions are caught internally; the next tick retries, consistent with `OutboxDispatcherTask` and `ReleaseExpiredReservationsTask`.

## Rejected Alternatives

**Outbound webhook from OpenBoxes.**
*Rejected:* OpenBoxes has no webhook registration or event-push capability. Not an architectural choice — an external constraint confirmed by the feasibility spike.

**Manual admin trigger.**
*Rejected:* violates QAS-5's "no operator action required" clause. Introduces unbounded latency dependent on staff availability. A 30-second polling cadence costs one HTTP call per tick; a manual step costs operator attention on every order.

**Continuous HTTP long-polling or SSE.**
*Rejected:* OpenBoxes does not support SSE or long-polling. A persistent connection from a background task to an external system adds connection lifecycle complexity for no benefit over periodic polling at this scale.

**Separate deployable poller service (mirroring ADR-008).**
*Rejected:* ADR-008's separation was justified because the OpenBoxes bridge has no writeback into nopCommerce. The polling task does have writeback (order status update); a separate service would need its own path back into nopCommerce — another HTTP API, another auth surface, another deploy artefact. The brief's "≥1 independently deployable subsystem" requirement is already satisfied by ADR-008.

## Consequences

- OpenBoxes `ISSUED` state is visible in nopCommerce within one polling interval (≤30 s by default) — QAS-5 warehouse clause satisfied without operator intervention.
- Carrier booking becomes fully automatic: `ISSUED` detection → outbox row → `CarrierBookingConsumer` → WireMock booking → `ExternalShipmentId` populated. No admin step required anywhere in the fulfillment-to-shipping chain.
- The polling interval is configurable via `AllocationSettings` without redeployment. Increasing it reduces OpenBoxes API load at the cost of higher visibility latency.
- The poller adds one HTTP call to OpenBoxes every 30 seconds. At VerdeMart's scale this is negligible; monitor in production.
- The `IOpenBoxesClient` interface gains one method (`GetIssuedFulfillmentOrdersAsync`); the bridge and the poller share the same client contract, keeping OpenBoxes API details in one place.
- `CANCELED` and sub-`ISSUED` states (`PICKED`, `VERIFYING`) are observed but not acted upon in this iteration. The `IOpenBoxesStatusMapper` interface isolates the mapping policy for future extension.
