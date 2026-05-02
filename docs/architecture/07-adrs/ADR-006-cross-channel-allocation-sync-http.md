# ADR-006 — Cross-Channel Allocation via Synchronous HTTP and Async Confirmation

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-28 |
| Produced by | ADD Iteration 3 — Step 6 (driver: QAS-2; concern: CON-15) |

## Context

ADR-005 places the allocation gate inside nopCommerce, operating on the `ProductWarehouseInventory` row. POS, as an external system, must reach the same gate for QAS-2 to hold across channels (CON-15 — web and POS must reach the same allocation gate).

`03-bounded-contexts.md` already declares that POS publishes `pos.sale.completed` over RabbitMQ — described there as "triggers inventory adjustment visible to Commerce and Warehouse". Reusing that event as the allocation step would leave an unbounded window between POS commit and message arrival in nopCommerce, during which the web storefront still shows the unit as available — oversell.

## Decision

POS calls a new synchronous HTTP endpoint exposed by nopCommerce (`POST /api/inventory/reserve`) before finalising an in-store sale. The endpoint runs the same pessimistic gate as the web flow, on the same `ProductWarehouseInventory` row. On `200 OK`, POS proceeds and commits the sale locally; on `409 Conflict` with a structured failure body, the sale is refused at the till.

After the local sale commits, POS calls `POST /api/inventory/confirm` to mark the reservation `Committed` and decrement `StockQuantity`. POS also publishes `pos.sale.completed` over the existing RabbitMQ contract; that event is **repurposed as audit confirmation**, no longer the allocation mechanism. POS-led abandonment is handled by `POST /api/inventory/release`; a TTL-driven schedule task releases reservations whose owners never confirm.

## Rejected Alternatives

**Use `pos.sale.completed` as the allocation step (no synchronous call).**
*Rejected:* QAS-2 explicitly requires synchronous rejection within the same request cycle; the message-delivery window between POS commit and nopCommerce processing breaks this property.

**POS owns local stock with eventual reconciliation.**
*Rejected:* POS has no native authority for cross-channel stock; eventual reconciliation cannot satisfy QAS-2's "exactly one wins" property in real time.

**Two-phase commit between POS and nopCommerce.**
*Rejected:* same operational hazards already noted for 2PC in ADR-004 (blocking failures, recovery complexity), with no benefit beyond what reserve/confirm with a TTL provides.

## Consequences

- POS becomes coupled to nopCommerce uptime for every sale. Mitigation: the endpoint is small, reads/writes one row, and is fast; only sustained nopCommerce outages affect POS.
- The existing `pos.sale.completed` event keeps its role as visibility/audit, consistent with `03-bounded-contexts.md`'s declared semantics.
- A new HTTP surface (`/api/inventory/*`) inside nopCommerce requires its own auth posture — bearer tokens are specified in Step 5; the issuance mechanism is operational config and out of QAS-2's scope.
- Reserve / Confirm / Release semantics introduce a small client protocol POS must implement correctly. Mis-uses (e.g. confirming twice) are absorbed by idempotent gate operations.
- The TTL-bounded reservation model lets POS callers cope with local errors after `reserve` without leaking stock indefinitely.
