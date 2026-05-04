# Business & Architectural Drivers — Quality Attribute Scenarios

**Owner: Martim**  
**Scenario C — Omnichannel Commerce Core (VerdeMart Retail)**

---

## Business Drivers

1. **Cross-channel unified commerce** — VerdeMart operates web, physical stores, and warehouse. Customers expect consistent order state and stock visibility across all channels.
2. **Operational resilience** — The web store must remain functional even when warehouse or ERP systems are temporarily unavailable. An outage in one system must not block customer orders.
3. **Real-time stock accuracy** — Stock sold in physical stores must be reflected on the website within seconds, preventing overselling.
4. **Fulfillment traceability** — Operations team must be able to trace an order from web placement through warehouse pick/pack to shipping carrier, even across system boundaries.

---

## Architectural Drivers

- At-least-once delivery of order events to ERP and WMS
- Circuit isolation: WMS failure must not block order acceptance
- Stock consistency across channels (eventual, bounded lag)
- Full observability of integration state during degradation

---

## Quality Attribute Scenarios

### QA-1: Availability — WMS Unavailable During Order Peak

| Field | Value |
|-------|-------|
| Source | Customer placing an order on the web store |
| Stimulus | WMS service becomes unavailable (network partition or crash) |
| Environment | Normal operating hours; 10 concurrent orders/minute |
| Artifact | Order Integration Service + nopCommerce outbox |
| Response | Order is accepted and confirmed to customer; order event is queued; WMS is not contacted synchronously |
| Measure | 0% order failures attributable to WMS unavailability; all queued events delivered within 60 s of WMS recovery |

### QA-2: Consistency — Cross-Channel Stock Visibility

| Field | Value |
|-------|-------|
| Source | Physical store POS (simulated via WMS stub) |
| Stimulus | A product is sold in the physical store, reducing warehouse stock |
| Environment | Normal operating condition; RabbitMQ healthy |
| Artifact | WMS stub → RabbitMQ → nopCommerce stock consumer |
| Response | nopCommerce product stock quantity is updated to reflect the new warehouse level |
| Measure | Stock update reflected in nopCommerce web store within 30 s of WMS event |

### QA-3: Recoverability — WMS Returns After Outage

| Field | Value |
|-------|-------|
| Source | Operator (WMS service restarts) |
| Stimulus | WMS stub switches from `down` mode to `normal` mode |
| Environment | 5 orders accumulated in dead-letter queue during outage |
| Artifact | Order Integration Service reconciliation loop + WMS adapter circuit breaker |
| Response | Circuit breaker resets to half-open; dead-letter queue is drained; all 5 orders are sent to WMS; stock updates are published back |
| Measure | All queued orders processed within 60 s of WMS recovery; no duplicate stock adjustments (idempotent) |

### QA-4: Observability — Degradation Visible to Operator

| Field | Value |
|-------|-------|
| Source | Operator monitoring the dashboard |
| Stimulus | WMS becomes slow (> 5 s response time) |
| Environment | Normal operating hours |
| Artifact | Integration Service health endpoint + observability dashboard |
| Response | Dashboard shows circuit breaker state as `OPEN` or `HALF_OPEN`, dead-letter queue depth increasing, WMS mode |
| Measure | State change visible on dashboard within 5 s of first WMS timeout |

### QA-5: Reliability — ERP Transient Failure Recovery

| Field | Value |
|-------|-------|
| Source | Order Integration Service processing an `order.placed` event |
| Stimulus | ERP stub returns 503 (Service Unavailable) for two consecutive requests before recovering |
| Environment | Normal operating hours; transient ERP failure lasting under 30 s |
| Artifact | `ErpAdapter` + Polly retry policy (3 attempts, exponential backoff) |
| Response | Order Integration Service retries automatically with backoff; on the third attempt ERP accepts the order; the event is neither lost nor delivered twice |
| Measure | Order confirmed in ERP within 30 s of first failure; zero events lost; no duplicate orders visible in ERP; all retry attempts logged with `correlationId` |

---

## Chosen Framework: ADD (Attribute-Driven Design)

ADD was chosen because it starts from quality attribute scenarios and uses them directly to drive decomposition decisions. Every major architectural choice in this design — the outbox pattern (QA-1), the circuit breaker (QA-3, QA-4), the dead-letter queue (QA-3), and the retry policy (QA-5) — is traceable to a specific QA scenario. This traceability is a core ADD principle and makes the design defensible: each structural decision exists because a measurable quality requirement demands it. ADD's iterative refinement also aligns with our approach of selectively evolving nopCommerce rather than redesigning it from scratch, allowing each phase to be validated against the scenarios before the next begins.
