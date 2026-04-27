# ADR-004: WMS Integration — Real OpenBoxes vs Stub Service

**Status:** Accepted  
**Date:** 2026-04-26  
**Owner:** Sebastião  
**Deciders:** Full team

---

## Context

Scenario C names OpenBoxes (or similar WMS/inventory platform) as a surrounding system. We must decide whether to integrate with the real OpenBoxes system or build a purpose-built stub service.

The same question applies to the ERP (ERPNext vs ERP stub).

---

## Decision

**Use stub services** for both WMS and ERP.

---

## Rationale

The assignment brief explicitly states:

> "Any surrounding system may be delivered through a surrogate simulator, stub, or mock service if that choice preserves the pressure, behavior, and architectural consequences of the scenario."
> "You do not need to reproduce the full depth of every surrounding system. You do need to represent enough of the ecosystem to make the architectural problem real."

The **architectural problem** in Scenario C is how the commerce core behaves when a surrounding system is degraded. The pressure is created by:
- A WMS that can be set to `normal | slow | down`
- Controllable failure injection to trigger circuit breaker behavior
- A realistic `POST /reservations` / `stock.updated` event flow

A real OpenBoxes deployment would:
- Require ~2 GB Docker image + PostgreSQL + Java runtime
- Provide no benefit for the failure injection scenario (OpenBoxes doesn't have a `POST /admin/fail` endpoint)
- Consume significant team time on ops rather than architecture

A WMS stub provides:
- Identical architectural pressure (HTTP POST to reserve, event back to RabbitMQ)
- Native failure injection (`POST /admin/mode`) for the demo pressure point
- Lightweight Docker image (< 50 MB)
- Full control over response timing (slow mode) and failure behavior (down mode)

The same reasoning applies to ERP (ERPNext would require a full Odoo/ERPNext stack with no demo value over a stub).

---

## Rejected Alternative: Real OpenBoxes

Rejected because:
- High operational overhead for negligible architectural benefit
- No native failure injection capability for the mandatory pressure point demo
- Integrating with OpenBoxes's real API would couple our architecture to their specific data model, which is not the assignment goal

---

## Consequences

- `services/wms-stub/` and `services/erp-stub/` are purpose-built lightweight HTTP services
- WMS stub publishes `stock.updated` to RabbitMQ directly (simulating the real WMS webhook/event behavior)
- Stub behavior is explicitly documented in the architecture report as a justified scope cut
- The architectural pressure (circuit breaker trigger, dead-letter accumulation, reconciliation) is fully preserved
