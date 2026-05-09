# TODO - Scenario C Implementation

## Part 1 - Architecture Checkpoint (Due May 5-6) - COMPLETE

### All Requirements Met
- [x] Scenario choice (Scenario C)
- [x] Current-state analysis (docs/architecture/current-state-analysis.md)
- [x] Domain and boundary model (docs/architecture/bounded-contexts.md)
- [x] QA scenarios (5 complete - QA-1 through QA-5 in drivers-and-qa-scenarios.md)
- [x] Chosen framework (ADD justification in drivers-and-qa-scenarios.md)
- [x] Target architecture with diagrams (docs/architecture/target-architecture.md)
- [x] Evolution roadmap (docs/architecture/evolution-roadmap.md)
- [x] ADRs (4 complete: RabbitMQ, Outbox, Runtime, Stubs)
- [x] Risk plan document (docs/architecture/risk-plan.md)
- [x] Feasibility spike (proven, merged via PR #1, pushed to remote)
- [x] Presentation started (commit f39dbe96d2)

**Git Status:**
- [x] Spike branch merged to develop
- [x] Pushed to origin (origin/spike/outbox-pattern-feasibility)
- [x] All Part 1 commits pushed to origin/develop

### Documentation Updates (Post-Checkpoint)
- [ ] Update risk-plan.md with spike evidence
- [ ] Update target-architecture.md diagrams to include OSPOS
- [ ] Document OSPOS integration pattern (new ADR or section in ADR-004)
- [ ] Update C4 Context diagram with OSPOS
- [ ] Update sequence diagrams with OSPOS sale flow

---

## Part 2 - Final Delivery (Due June 2-3)

### Week 2 - External Systems & Infrastructure

#### Order Integration Service
- [ ] Project setup (.NET 10 worker service)
- [ ] RabbitMQ consumer configuration
  - [ ] Subscribe to `verdemart.events` exchange
  - [ ] Listen for `order.placed` routing key
  - [ ] Deserialize IntegrationEvent messages
- [ ] ERP Adapter
  - [ ] HTTP client for ERP stub
  - [ ] Polly retry policy (exponential backoff, 3 retries)
  - [ ] Error handling and logging
- [ ] WMS Adapter
  - [ ] HTTP client for WMS stub
  - [ ] Polly circuit breaker (3 failures → open, 30s timeout)
  - [ ] Dead-letter queue routing on circuit open
- [ ] Reconciliation loop
  - [ ] Background service polls DLQ
  - [ ] Retry on circuit half-open
  - [ ] Mark as processed on success
- [ ] Health endpoint
  - [ ] GET /health
  - [ ] Returns: circuit state, DLQ depth, last processed event
- [ ] Idempotency
  - [ ] Track processed eventIds (in-memory cache or database)
  - [ ] Skip duplicate messages
- [ ] Dockerize
  - [ ] Dockerfile
  - [ ] Add to docker-compose.yml

#### ERP Stub
- [ ] Create `services/erp-stub/` project (Node.js/Python/C#)
- [ ] POST /orders endpoint
  - [ ] Accept order data
  - [ ] Store in-memory (simulate ERP acceptance)
  - [ ] Return 200 OK or configurable failure
- [ ] POST /admin/mode endpoint
  - [ ] Modes: `normal`, `down`
  - [ ] In `down` mode: return 503 for all requests
- [ ] GET /orders endpoint (for verification)
- [ ] Dockerize
  - [ ] Dockerfile
  - [ ] Add to docker-compose.yml

#### WMS Stub
- [ ] Create `services/wms-stub/` project (Node.js/Python/C#)
- [ ] POST /reservations endpoint
  - [ ] Accept reservation request
  - [ ] Calculate new stock level
  - [ ] Call WMS Event Adapter webhook (instead of direct RabbitMQ)
  - [ ] Return 200 OK or configurable failure/timeout
- [ ] POST /admin/mode endpoint
  - [ ] Modes: `normal`, `slow` (10s delay), `down` (503 error)
- [ ] GET /stock/{productId} endpoint
- [ ] Dockerize
  - [ ] Dockerfile
  - [ ] Add to docker-compose.yml

#### WMS Event Adapter (NEW)
- [ ] Create `services/wms-adapter/` project
- [ ] POST /webhooks/stock-changed endpoint
  - [ ] Receive webhook from WMS stub
  - [ ] Transform WMS payload → `stock.updated` event format
  - [ ] Publish to RabbitMQ `verdemart.events`
- [ ] Logging with correlation IDs
- [ ] Dockerize
  - [ ] Dockerfile
  - [ ] Add to docker-compose.yml

#### Docker Compose Updates
- [ ] Add health checks for all services
- [ ] Configure `depends_on` with `service_healthy` conditions
- [ ] Add environment variable configuration
- [ ] Document startup sequence

---

### Week 3 - OSPOS & nopCommerce Integration

#### OSPOS (Open Source Point of Sale)
- [ ] Deploy OSPOS system
  - [ ] Pull OSPOS Docker image or setup from source
  - [ ] Configure MySQL database for OSPOS
  - [ ] Run OSPOS container
  - [ ] Access OSPOS web interface
- [ ] Configure OSPOS
  - [ ] Create store location
  - [ ] Create cashier user account
  - [ ] Configure tax rates
  - [ ] Sync product catalog with nopCommerce
    - [ ] Export products from nopCommerce
    - [ ] Import to OSPOS OR use API to sync
- [ ] Document OSPOS setup
  - [ ] Access credentials
  - [ ] Configuration steps
  - [ ] Product sync process
- [ ] Dockerize
  - [ ] Add OSPOS service to docker-compose.yml
  - [ ] Configure volumes for persistence

#### OSPOS Integration Adapter
- [ ] Create `services/ospos-adapter/` project
- [ ] Implement sale polling mechanism
  - [ ] Connect to OSPOS MySQL database OR use OSPOS API
  - [ ] Poll for new sales (query sales table with timestamp filter)
  - [ ] Track last processed sale ID/timestamp
- [ ] Transform sale data
  - [ ] Map OSPOS sale format → `sale.completed` event
  - [ ] Extract: productId, quantity, storeId, timestamp
  - [ ] Generate eventId for idempotency
- [ ] Publish to RabbitMQ
  - [ ] Publish `sale.completed` to `verdemart.events`
  - [ ] Include correlation ID for tracing
- [ ] Idempotency handling
  - [ ] Track processed sale IDs in-memory or database
  - [ ] Skip duplicate sales
- [ ] Error handling and logging
  - [ ] Log all polling cycles
  - [ ] Handle OSPOS database connection failures
  - [ ] Retry logic for RabbitMQ publish failures
- [ ] Dockerize
  - [ ] Dockerfile
  - [ ] Add to docker-compose.yml
  - [ ] Configure polling interval via environment variable

#### nopCommerce - Real Order Events
- [ ] Replace spike's `AppStartedEventConsumer`
- [ ] Hook into `OrderProcessingService.PlaceOrderAsync()`
  - [ ] After order saved to DB
  - [ ] Write IntegrationEvent to outbox (same transaction)
  - [ ] Event type: `order.placed`
  - [ ] Event data: orderId, customerId, items, total, timestamp
- [ ] Update `SpikeOutboxPublisherTask` → `OutboxPublisherTask`
  - [ ] Remove "Spike" prefix
  - [ ] Production-ready error handling
  - [ ] Configurable poll interval (appsettings.json)

#### nopCommerce - Stock Consumer
- [ ] Create `StockUpdateConsumerBackgroundService`
  - [ ] Subscribe to RabbitMQ `stock.updated` routing key
  - [ ] Deserialize event
  - [ ] Call `ProductService.AdjustInventoryAsync()`
- [ ] Cross-channel conflict resolution
  - [ ] If stock becomes negative after POS event
  - [ ] Query recent web orders (last 60s)
  - [ ] Cancel most recent web order(s) until stock ≥ 0
  - [ ] Publish `order.cancelled` event
  - [ ] Send customer notification email
- [ ] Register consumer in `IntegrationStartup.cs`

#### nopCommerce - Health Endpoint
- [ ] Add `/integration/health` endpoint
  - [ ] Return: pending outbox count, last publish time, status
  - [ ] Status codes: 200 (healthy), 503 (degraded)
- [ ] Add controller to `Nop.Web`

---

### Week 4 - Observability & Testing

#### Observability Dashboard
- [ ] Create `services/dashboard/` project (HTML/JS or React)
- [ ] Poll endpoints every 2 seconds:
  - [ ] GET /integration/health (nopCommerce)
  - [ ] GET /health (Integration Service)
- [ ] Display cards:
  - [ ] Circuit breaker state (OPEN/HALF_OPEN/CLOSED)
  - [ ] Dead-letter queue depth
  - [ ] WMS mode (normal/slow/down)
  - [ ] Outbox pending count
  - [ ] Last event published timestamp
- [ ] Demo control buttons:
  - [ ] Toggle WMS mode (normal/slow/down)
  - [ ] Simulate POS sale
  - [ ] Clear DLQ
- [ ] Visual indicators (red/yellow/green for status)
- [ ] Dockerize and add to docker-compose.yml

#### Integration Tests
- [ ] Test: Outbox publishes to RabbitMQ
  - [ ] Place order → verify event in RabbitMQ within 10s
- [ ] Test: Circuit breaker lifecycle
  - [ ] Normal → WMS down → 3 failures → circuit OPEN
  - [ ] Verify DLQ accumulates messages
  - [ ] WMS up → circuit HALF_OPEN → test → CLOSED
  - [ ] Verify DLQ drains
- [ ] Test: Cross-channel conflict resolution
  - [ ] POS sells last unit + web order simultaneously
  - [ ] Verify: POS succeeds, web order cancelled
- [ ] Test: Idempotency
  - [ ] Publish same eventId twice
  - [ ] Verify: WMS called only once
- [ ] Test: ERP retry
  - [ ] ERP returns 500 → verify retry with backoff
  - [ ] ERP returns 200 on 3rd attempt → success
- [ ] Startup smoke test script
  - [ ] `docker-compose up -d`
  - [ ] Verify all health endpoints return 200
  - [ ] Verify startup event published

#### Demo Rehearsal
- [ ] Run full pressure point scenario:
  1. Normal operation: place order → verify in ERP + WMS
  2. Set WMS to `down` mode
  3. Place 5 orders → verify circuit opens, DLQ grows
  4. Observe dashboard: circuit OPEN, DLQ depth = 5
  5. Set WMS to `normal` mode
  6. Verify: circuit closes, DLQ drains within 60s
- [ ] Measure timings:
  - [ ] Time to circuit open (target: < 15s)
  - [ ] Time to drain DLQ (target: < 60s per QA-3)
- [ ] Take screenshots for evidence pack

---

### Week 5 - Documentation & Final Polish

#### Architecture Report
- [ ] Executive summary
- [ ] Business drivers and QA scenarios
- [ ] Current-state analysis
- [ ] Bounded contexts and responsibilities
- [ ] Target architecture (C4 diagrams)
- [ ] Evolution path (step-by-step)
- [ ] Design decisions (ADRs)
- [ ] Cross-cutting concerns (observability, idempotency)
- [ ] Scope decisions and justifications (stubs, POS, adapters)
- [ ] Evidence pack references

#### ADR Updates
- [ ] Review all 4 ADRs for consistency
- [ ] Add consequences section if missing
- [ ] Update status if any decisions changed

#### Evidence Pack
- [ ] Screenshots:
  - [ ] RabbitMQ Management UI (verdemart.events exchange)
  - [ ] Observability dashboard (normal state)
  - [ ] Observability dashboard (WMS down, circuit OPEN)
  - [ ] Observability dashboard (recovery, DLQ draining)
  - [ ] Database query showing outbox events
  - [ ] Integration test results (all green)
- [ ] Log samples:
  - [ ] Order placed → outbox written
  - [ ] Event published to RabbitMQ
  - [ ] Circuit breaker opening
  - [ ] DLQ reconciliation
  - [ ] Cross-channel conflict resolution
- [ ] Video recording (optional):
  - [ ] 2-minute demo of pressure point scenario

#### Presentation Slides
- [ ] Title slide
- [ ] Scenario C overview
- [ ] Business drivers (3-5 bullets)
- [ ] QA scenarios (focus on QA-1 through QA-4)
- [ ] Architecture diagrams (C4 Context, Overview, Sequences)
- [ ] Top 3 architectural risks
- [ ] Key design decisions (ADR highlights)
- [ ] Live demo plan (or video backup)
- [ ] Tradeoffs and limitations (honest defense)
- [ ] Q&A preparation

#### Defense Preparation
- [ ] Anticipated questions:
  - [ ] Why RabbitMQ over Kafka? (ADR-001)
  - [ ] Why stubs instead of real systems? (ADR-004)
  - [ ] What happens if RabbitMQ goes down?
  - [ ] How do you handle message ordering?
  - [ ] Why POS priority over web orders?
  - [ ] What's the latency for cross-channel stock updates?
  - [ ] How would this scale to 100 stores?

---

## Repository & Collaboration

### Git Workflow
- [ ] Merge spike branch to develop (once access granted)
- [ ] Create feature branches:
  - [ ] `feature/integration-service`
  - [ ] `feature/stubs` (ERP, WMS, POS)
  - [ ] `feature/nop-stock-consumer`
  - [ ] `feature/observability-dashboard`
- [ ] Pull request review process
- [ ] Final merge to main for submission

### Team Coordination
- [ ] Assign owners to each service/component
- [ ] Weekly sync meetings
- [ ] Shared evidence pack folder
- [ ] Demo rehearsal schedule

---

## Quick Reference - Success Criteria

### Part 1 Checkpoint (May 5-6)
- [x] Scenario chosen and justified
- [x] 3-5 QA scenarios documented
- [x] Target architecture with diagrams
- [x] At least 3 ADRs
- [x] Risk plan with feasibility spike

### Part 2 Final Delivery (June 2-3)
- [ ] Runnable implementation (all services dockerized)
- [ ] Architecture report (comprehensive documentation)
- [ ] Updated ADRs (4+)
- [ ] Evidence pack (screenshots, logs, tests)
- [ ] Live demo showing degraded + recovering behavior
- [ ] Defense of tradeoffs and limits

### Demo Must Show
- [ ] Normal operation (order flows through all systems)
- [ ] Degraded dependency (WMS down)
- [ ] Observable system behavior (dashboard shows circuit OPEN)
- [ ] Recovery path (WMS up, DLQ drains, circuit CLOSED)

---

## Notes

- **Spike complete:** Outbox pattern proven viable (May 4, 2026)
- **Critical path:** Integration Service + WMS circuit breaker (highest risk for demo)
- **Time estimate:** ~80-100 hours total (4-5 weeks with 20h/week)
- **Repository access needed:** Cannot push spike branch yet
