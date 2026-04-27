# Project Plan — Scenario C: Omnichannel Commerce Core (VerdeMart Retail)

## Context

Assignment 2 (50% of grade) for Software Architectures, MEI. Team: Henrique, Martim, Duarte, Sebastião.

**Scenario C**: nopCommerce becomes the commerce core of a wider enterprise ecosystem (ERP, warehouse, POS, shipping). The key pressure: surrounding systems become delayed, stale, or unavailable — the commerce core must remain useful, make the degradation visible, and recover.

**Two mandatory use cases:**
1. Buy-online / fulfill-through-another-channel (order → ERP + warehouse)
2. Cross-channel stock/order-state visibility (external change reflected back in nopCommerce)

**Mandatory pressure point:** WMS goes unavailable → nopCommerce stays functional → orders queue → WMS recovers → reconciliation.

**Deadlines:**
- Part 1 (Architecture Checkpoint) — May 5–6, 2026 — 7 min presentation — 20% weight
- Part 2 (Final Delivery + Demo) — June 2–3, 2026 — 15 min live demo — 80% weight

---

## Target Architecture

```
nopCommerce (monolith, minimal changes)
  └─ Outbox table → Background publisher → RabbitMQ
                                              │
                               ┌──────────────┴─────────────┐
                               ▼                             ▼
                   Order Integration Service          (reverse flow)
                   [independently deployable]         Stock Update Consumer
                        │            │                (inside nopCommerce)
                        ▼            ▼
                    ERP Stub      WMS Stub
                   (Dockerized)  (Dockerized, controllable failure switch)
```

**Key reliability decisions:**
- **Outbox pattern** in nopCommerce — guarantees at-least-once delivery to RabbitMQ even if broker is temporarily down
- **Circuit breaker** (Polly) in Order Integration Service — detects WMS failure, opens circuit
- **Dead-letter queue** in RabbitMQ — holds undeliverable messages for reconciliation
- **Retry with exponential backoff** in Order Integration Service

**No shared database** across extracted boundaries.  
**Framework:** ADD (Attribute-Driven Design) — QA scenarios directly drive decomposition.

---

## Part 1 — Architecture Checkpoint (due May 5–6)

All documentation. Four people work in parallel on independent deliverables.

### Henrique — Current-state analysis + Target architecture
- Map nopCommerce order flow and inventory management
- Identify architectural seams (where events can be injected)
- Produce C4-style target architecture diagram
- Deliverables: `docs/architecture/current-state-analysis.md` ✅ + `docs/architecture/target-architecture.md` ✅

### Martim — Drivers + QA Scenarios + Framework application
- Define business and architectural drivers
- Write 4–5 QA scenarios (stimulus / environment / response / measure)
- Justify ADD framework choice
- Deliverables: `docs/architecture/drivers-and-qa-scenarios.md` + `docs/architecture/framework-application.md`

### Duarte — Bounded contexts + Evolution path
- Identify bounded contexts and data ownership
- Draw context map (upstream/downstream relationships)
- Document phased evolution: monolith → outbox → integration service → pressure demo
- Deliverables: `docs/architecture/bounded-contexts.md` + `docs/architecture/evolution-roadmap.md`

### Sebastião — ADRs + Risk plan + Feasibility spike
- ADR-001: RabbitMQ vs Kafka ✅
- ADR-002: Outbox vs direct publish ✅
- ADR-003: .NET Worker Service vs Python ✅
- ADR-004: WMS stub vs real OpenBoxes ✅
- Risk plan ✅
- Feasibility spike: prove outbox → RabbitMQ works in nopCommerce (`spikes/outbox-spike/`)
- Deliverables: `docs/adr/`, `docs/architecture/risk-plan.md` ✅

---

## Part 2 — Implementation (May 7 → June 1)

**Load estimate:** Henrique ~40% · Martim ~30% · Duarte ~20% · Sebastião ~10%

### Henrique — nopCommerce integration layer
Changes to the monolith only. No new service.

1. `IntegrationEvent` entity + FluentMigrator migration (`src/Libraries/Nop.Data/Migrations/`)
2. `OutboxPublisherBackgroundService` — polls pending rows, publishes to RabbitMQ, marks sent
3. Hook in `OrderProcessingService.PlaceOrderAsync()` — write `order.placed` event to outbox
4. `StockUpdateConsumerBackgroundService` — receives `stock.updated`, calls `ProductService.AdjustInventoryAsync()`
5. `/integration/health` endpoint — reports pending outbox count + last publish time
6. `docs/setup-and-run.md`

Key files: `src/Libraries/Nop.Services/Orders/OrderProcessingService.cs`, new `src/Libraries/Nop.Services/Integration/`, `src/Presentation/Nop.Web/Controllers/CommonController.cs`

### Martim — Order Integration Service
New independently deployable .NET Worker Service at `services/order-integration-service/`.

1. RabbitMQ consumer for `order.placed` events
2. `ErpAdapter` — HTTP POST to ERP stub + retry with exponential backoff (Polly)
3. `WmsAdapter` — HTTP POST to WMS stub + circuit breaker (Polly) — when open, push to DLQ
4. Reconciliation loop — drains dead-letter queue on circuit breaker reset
5. Publishes `stock.updated` to RabbitMQ after WMS success
6. Structured logging (Serilog) with `correlationId`
7. `/health` endpoint — circuit breaker state (consumed by Duarte's dashboard)

### Duarte — ERP Stub + WMS Stub + Observability Dashboard + Docker Compose

**ERP stub** (`services/erp-stub/`): `POST /orders`, `POST /admin/mode {normal|down}`

**WMS stub** (`services/wms-stub/`): `POST /reservations`, `GET /stock/{productId}`, `POST /admin/mode {normal|slow|down}` — on success publishes `stock.updated` to RabbitMQ

**Observability dashboard** (`services/dashboard/`): single-page HTML/JS polling `/health` every 2s — shows live circuit breaker state, WMS mode, pending outbox count, DLQ depth. Visual centerpiece of the demo pressure point.

**`docker-compose.yml`**: single `docker compose up` starts everything — nopCommerce + PostgreSQL + RabbitMQ + Integration Service + ERP Stub + WMS Stub + Dashboard.

### Sebastião — Integration tests + Architecture report + Evidence + Demo script

1. **Integration test suite** (`tests/integration/`): end-to-end tests covering happy path, WMS down → DLQ grows, WMS recovery → reconciliation. These double as reproducible evidence.
2. **Update ADRs** as implementation decisions crystallize
3. **Architecture report** (`docs/architecture-report.md`)
4. **Evidence pack** (`docs/evidence/`) — logs, screenshots, latency measurements, known limitations
5. **Demo script** (`docs/demo-script.md`) — step-by-step for the 15-minute live presentation

---

## Repository Structure

```
AS-2-NopCommerce/
├── src/                                    # nopCommerce monolith (Henrique)
│   └── Libraries/Nop.Services/Integration/
├── services/
│   ├── order-integration-service/          # Martim
│   ├── erp-stub/                           # Duarte
│   ├── wms-stub/                           # Duarte
│   └── dashboard/                          # Duarte
├── docs/
│   ├── project-plan.md                     # this file
│   ├── architecture/                       # Part 1 architecture docs
│   ├── adr/                                # Architecture Decision Records
│   └── evidence/                           # Part 2 evidence pack
├── spikes/
│   └── outbox-spike/                       # Sebastião (Week 2)
├── tests/
│   └── integration/                        # Sebastião (Part 2)
├── docker-compose.yml                      # Duarte
└── docs/setup-and-run.md                   # Henrique
```

---

## Integration Contracts (fixed from day 1 — unblocks parallel work)

**RabbitMQ exchange:** `verdemart.events` (topic exchange)  
**Dead-letter exchange:** `verdemart.dlx` → queue `verdemart.dead-letter`

**`order.placed`** routing key — nopCommerce → Integration Service:
```json
{
  "eventId": "uuid",
  "orderId": 123,
  "customerId": 456,
  "items": [{ "productId": 789, "sku": "ABC", "quantity": 2 }],
  "totalAmount": 49.99,
  "occurredAt": "2026-05-10T14:00:00Z"
}
```

**`stock.updated`** routing key — WMS Stub → nopCommerce:
```json
{
  "eventId": "uuid",
  "productId": 789,
  "warehouseId": 1,
  "newStockQuantity": 45,
  "occurredAt": "2026-05-10T14:00:05Z"
}
```

**WMS stub reservation** `POST /reservations`:
```json
{ "orderId": 123, "items": [{ "productId": 789, "quantity": 2 }] }
```

---

## Timeline

| Week | Dates | Focus |
|------|-------|-------|
| Week 1 | Apr 26 – May 2 | Part 1 docs in parallel (all 4 independently) |
| Buffer | May 3–4 | Cross-review docs, confirm integration contracts |
| **Part 1** | **May 5–6** | **Architecture Checkpoint presentation** |
| Week 2 | May 7–11 | Feasibility spike + implementation kickoff |
| Week 3 | May 12–18 | Happy path end-to-end working |
| Week 4 | May 19–25 | Pressure point + recovery + evidence collection |
| Week 5 | May 26–Jun 1 | Demo rehearsal, report finalization, buffer |
| **Part 2** | **Jun 2–3** | **Final presentation + live demo** |

---

## Key Principles

- **Smaller and defensible beats bigger and vague.** The WMS degradation → recovery story is the core demo; everything else serves it.
- **Feasibility spike first.** The outbox integration with nopCommerce is the riskiest point. Sebastião proves it works in Week 2 before anyone builds on top of it.
- **Integration contracts are frozen.** Everyone codes against the schemas above from day 1 to avoid blocking each other.
- **Stubs over real systems.** Justified in ADR-004 — same architectural pressure, full failure injection control, minimal ops overhead.
