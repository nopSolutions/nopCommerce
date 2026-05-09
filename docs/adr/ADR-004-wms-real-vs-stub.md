# ADR-004: External Systems — Real vs Stub

**Status:** Accepted  
**Date:** 2026-04-26 (Updated: 2026-05-09)  
**Owner:** Sebastião  
**Deciders:** Full team

---

## Context

Scenario C names multiple surrounding systems: ERP (ERPNext/Odoo), WMS (OpenBoxes), and POS (Open Source Point of Sale). We must decide for each system whether to integrate with the real implementation or build a purpose-built stub service.

---

## Decision

- **ERP**: Use stub service
- **WMS**: Use stub service
- **POS**: Use real OSPOS (Open Source Point of Sale)

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

### POS — Real OSPOS

**Decision:** Use real Open Source Point of Sale (OSPOS) system, not a stub.

**Rationale:**

1. **Demonstrates third-party integration patterns**: OSPOS lacks native RabbitMQ support, requiring an integration adapter that polls the OSPOS database and publishes events. This validates our ability to integrate with systems we don't control.

2. **Avoids artificial coupling**: Using WMS to simulate POS sales creates false architectural coupling between warehouse and retail operations. In real omnichannel retail, POS and WMS are independent systems.

3. **Cross-channel conflict resolution**: Real OSPOS provides authentic retail sale workflow, proving the cross-channel conflict resolution pattern (physical store sales prioritize over web orders) against a genuine third-party system.

4. **Minimal operational overhead**: OSPOS Docker deployment is lightweight (MySQL + PHP application), and setup time is acceptable for the architectural learning value gained.

5. **Different integration pattern than stubs**: While ERP/WMS stubs accept HTTP calls from our Integration Service, OSPOS requires polling (adapter pulls data), demonstrating bidirectional integration patterns.

**Trade-off accepted**: OSPOS Adapter must poll the database (or API) on an interval, introducing 30-60 second latency for cross-channel stock updates. This is acceptable for UC2 requirements and QA-2 (30 second stock sync target).

---

## Rejected Alternatives

### Real OpenBoxes

Rejected because:
- High operational overhead for negligible architectural benefit
- No native failure injection capability for the mandatory pressure point demo
- Integrating with OpenBoxes's real API would couple our architecture to their specific data model, which is not the assignment goal

---

### POS Stub

Rejected because:
- Would artificially couple POS sales to WMS (same stub simulating both)
- Would not prove integration with third-party systems lacking native event publishing
- Misses opportunity to demonstrate polling-based integration adapter pattern
- Creates unrealistic architecture where all external systems are under our control

---

## Consequences

### ERP and WMS Stubs
- `services/erp-stub/` and `services/wms-stub/` are purpose-built lightweight HTTP services
- WMS stub publishes `stock.updated` to RabbitMQ directly (simulating real WMS webhook/event behavior)
- Stub behavior is explicitly documented in architecture report as justified scope cut
- Architectural pressure (circuit breaker trigger, dead-letter accumulation, reconciliation) is fully preserved

### Real OSPOS
- `services/ospos/` runs real OSPOS Docker container with MySQL database
- `services/ospos-adapter/` polls OSPOS sales table and publishes `sale.completed` events to RabbitMQ
- Product catalog must be synced between nopCommerce and OSPOS (setup overhead)
- Integration adapter introduces polling latency (30-60s configurable interval)
- Cross-channel conflict resolution implemented in nopCommerce StockUpdateConsumer (OSPOS sales prioritized)
- Demonstrates realistic integration with third-party retail system
- Adapter pattern is reusable for other systems lacking native event publishing (e.g., legacy ERP APIs)
