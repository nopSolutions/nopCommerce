# Risk Plan and Validation

**Owner: Sebastião**  
**Scenario C — Omnichannel Commerce Core (VerdeMart Retail)**

---

## Top Architectural Risks

### Risk 1 — Outbox Integration with nopCommerce (HIGH)
**Description:** Adding the `IntegrationEvent` table and `OutboxPublisherBackgroundService` requires modifying the nopCommerce monolith — specifically hooking into `OrderProcessingService.PlaceOrderAsync()` and registering a background hosted service via `INopStartup`. nopCommerce uses Autofac DI, FluentMigrator for migrations, and a specific `IRepository<T>` pattern. Getting this wrong means events are never published.

**Mitigation:** Feasibility spike (Sebastião, Week 1) — prove end-to-end: order placed → outbox row written → background service reads → message delivered to RabbitMQ. This must be green before Week 2 implementation begins.

**Residual risk:** Low after spike.

---

### Risk 2 — Circuit Breaker Behavior in Live Demo (MEDIUM)
**Description:** The mandatory pressure point requires the circuit breaker to visibly open when WMS goes down, then reset and drain the dead-letter queue on recovery. If the Polly configuration is wrong (e.g., circuit opens too slowly or too quickly), the demo behavior is unconvincing. If the dead-letter reconciliation loop has a bug, the recovery path fails.

**Mitigation:** Sebastião's integration test suite (Week 3–4) specifically tests the circuit breaker lifecycle: normal → WMS down → circuit open → DLQ grows → WMS up → circuit resets → DLQ drained. Tests run against the real Docker Compose stack, not mocks.

**Residual risk:** Medium — live demo has inherent variability. Demo rehearsal (Week 5) is mandatory.

---

### Risk 3 — Docker Compose Startup Order (LOW-MEDIUM)
**Description:** nopCommerce must start after PostgreSQL is ready, and Integration Service must start after RabbitMQ is ready. If health-check dependencies in `docker-compose.yml` are wrong, the demo environment fails to start cleanly.

**Mitigation:** Use Docker Compose `depends_on` with `condition: service_healthy` and explicit healthcheck commands. Tested in Duarte's Docker Compose work (Week 2).

**Residual risk:** Low after testing.

---

## Validation Plan

| Risk | Validation method | When |
|------|------------------|------|
| Outbox integration | Feasibility spike — manual end-to-end test | May 7–9 |
| Circuit breaker lifecycle | Automated integration tests (Sebastião) | May 19–25 |
| Recovery/reconciliation correctness | Integration test + manual demo rehearsal | May 26–Jun 1 |
| Docker Compose startup | `docker compose up` + smoke test script | May 12–14 |
| Stock update in nopCommerce | Integration test: place order → WMS event → verify product stock | May 19–25 |

---

## Feasibility Spike

**Goal:** Prove the outbox mechanism works before committing to full implementation.

**Spike scope** (`spikes/outbox-spike/`):
1. Add a minimal `IntegrationEvent` table via FluentMigrator migration (just `Id`, `Payload`, `PublishedAt`)
2. Add a `SpikeOutboxPublisher` that writes one row on application startup (not tied to real order placement yet)
3. Add a `SpikeRabbitPublisher` background service that reads the row and publishes to RabbitMQ
4. Confirm message received via RabbitMQ Management UI

**Success criteria:**
- Message appears in RabbitMQ `verdemart.events` exchange within 10 s of nopCommerce startup
- Row marked as published in DB
- No crashes or DI registration errors

**If spike fails:** The main risk is nopCommerce's DI setup for background services. Fallback: implement the outbox publisher as a nopCommerce `IScheduleTask` (recurring task via nopCommerce's own scheduler) instead of a .NET `IHostedService`. This is a known extension point and requires less DI wiring.
