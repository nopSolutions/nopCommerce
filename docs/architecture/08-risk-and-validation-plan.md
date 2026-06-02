# Risk and Validation Plan

**Scope:** Iterations 1–4 (all complete).
**Purpose:** Identify every material risk still open at the checkpoint, state which quality attribute it threatens, and describe the concrete action that de-risks it — either something already built, a feasibility spike to run, or an accepted limitation.

---

## 1. Risks De-Risked by the Design

These risks were identified at the start of the iteration sequence. The decisions made in Iterations 1–5 close them structurally. No further measurement is required to claim them.

| Risk | Quality attribute | How the design closes it |
| --- | --- | --- |
| Order event lost when RabbitMQ is unreachable at commit time (dual-write problem) | QAS-1 Reliability | ADR-004: transactional outbox writes message intent inside the same DB transaction as the order commit; no broker-down window remains |
| Duplicate fulfillment orders on message redelivery | QAS-1 / QAS-4 Recoverability | Bridge-local dedup table keyed on `OrderGuid` absorbs redeliveries before the OpenBoxes call |
| Schema evolution breaking bridge consumers silently | QAS-1 / QAS-4 | `Version` field added to `OrderPlacedMessage`; tolerant-reader policy; breaking changes use a new routing key and parallel queue binding |
| Concurrent web and POS orders overselling the last unit | QAS-2 Consistency | ADR-005: pessimistic per-row lock on `ProductWarehouseInventory`; exactly one caller acquires the lock, the loser receives a structured rejection within the same request cycle |
| Web checkout and POS reaching different allocation authorities | QAS-2 Consistency | ADR-006: POS calls `POST /api/inventory/reserve` on nopCommerce synchronously — the same gate used by web checkout |
| Checkout blocked on RabbitMQ latency | QAS-3 Availability | ADR-004: outbox dispatcher runs as an `IScheduleTask` on a 1-second poll inside the nopCommerce process; the broker is entirely off the checkout thread |
| No outbound integration or external status polling pattern in the framework (Seam 7) | QAS-5 Visibility | ADR-008: `CarrierStatusPollerTask` polls WireMock every 30 s for shipment status; ADR-009: `OpenBoxesStatusPollerTask` polls OpenBoxes every 30 s for fulfillment state — both establish the polling pattern for future external status integrations |

---

## 2. Architectural Risks Still Open

These are structural gaps that remain in the current design. Each has a named de-risking action.

| Risk | Quality attribute | De-risking action |
| --- | --- | --- |
| Dispatcher single point of failure. `OutboxDispatcherTask` runs inside the nopCommerce process; if all web nodes are down, no outbox rows are dispatched. | QAS-1 Reliability | Accepted at VerdeMart's scale — multi-instance leader election was explicitly deferred. Document that the dispatcher restarts automatically on process recovery. For future scale-out: the dispatcher's polling query already uses `FOR UPDATE SKIP LOCKED` — extraction to an independent host requires no contract change. |
| Bridge dedup table lost on container restart. The dedup `processed_orders` table is co-deployed inside the bridge container. If the container is removed and recreated without a persisted volume, the table is empty on restart — redelivered messages that were already processed before the restart will be submitted to OpenBoxes again. | QAS-1 / QAS-4 | Confirm the store file is mounted on a named Docker volume (not the container's writable layer) so the table survives `docker rm` and recreate. During the feasibility spike, verify that OpenBoxes returns a recognisable response (e.g. `409 Conflict`) on a duplicate `OrderGuid` submission, so the bridge can absorb the duplicate case even if the table is cold. |
| Process-local mutex on order placement (Seam 6). `OrderProcessingService` uses a `Mutex` — per-process. Two horizontally-scaled nopCommerce nodes can each accept an order for the same customer simultaneously. | QAS-2 Consistency | The allocation gate (ADR-005) prevents inventory oversell at the DB level regardless of node count. The duplicate-order check remains process-local. For the demo: single-node deployment means the mutex is sufficient. Record as a known horizontal-scaling gap; long-term path is a DB-level idempotency key check. |
| Recurring-payment subscription path not protected by the gate. `OrderProcessingService.cs:1982` calls `AdjustInventoryAsync` for subscription renewals; the decorator added in ADR-005 intercepts only the standard checkout path. | QAS-2 Consistency | Explicitly out of Iteration 3 scope (Step 2). If recurring payments appear in the demo scenario, wire the decorator to this path before the presentation. Otherwise accepted as a bounded out-of-scope exception. |

---

## 3. Operational Risks Still Open

These are not structural design flaws but conditions that could surface in production or during the demo run.

| Risk | What could go wrong | De-risking action |
| --- | --- | --- |
| OpenBoxes API contract still uncertain | The bridge's correctness (dedup behaviour, duplicate-detection response shape, fulfillment-creation endpoint) was flagged as unknown at Iteration 3 Step 1 and has not been confirmed by a spike. A wrong assumption here could require a design change, not just a configuration fix. | Run the OpenBoxes feasibility spike before the demo. Stand up OpenBoxes locally, call the fulfillment-creation endpoint with a duplicate `OrderGuid`, verify the response shape, confirm dedup table behaviour. This is the highest-priority open action in the project. |
| Outbox table unbounded growth | `Sent` rows accumulate with no retention policy; not a demo blocker on a fresh environment, but a risk if the demo environment is long-lived or pre-seeded. | Add a cleanup scheduled task (`DELETE WHERE Status = 'Sent' AND SentAtUtc < NOW() - interval`) before running any sustained demo load. Deferred from Iteration 2; implement before the final presentation environment is prepared. |
| Rejected web-order DB pollution | A `409`-rejected checkout leaves an `Order` row committed at `OrderProcessingService.cs:1589` with `Success=false`. Not a functional defect; purely table noise. | Accepted for the demo. A periodic cleanup task or admin-facing filter addresses it post-demo. |
| `ReleaseExpiredReservationsTask` polling interval unmeasured | The 30-second default for releasing expired `ProductReservation` rows is untested under realistic reservation volume; reservations may be held longer than necessary under load. | Include in the QAS-2 concurrent load spike. Measure how many expiry rows accumulate during the load test and whether the 30-second cadence is sufficient. |
| DLQ replay tooling not fully automated | ADR-011's circuit breaker prevents transient outage messages from reaching the DLQ — the bridge self-heals automatically on recovery and messages remain durably queued. DLQs now receive only genuine poison messages. | Transient-failure risk closed by ADR-011. Prepare a minimal Management Console workflow (or one-shot `rabbitmqadmin` script) for genuine poison messages before the demo, covering all three DLQs. |
| RabbitMQ connection failure on first call | `RabbitMqConnectionFactory` opens the connection lazily. If the broker is unreachable at the moment the plugin first initialises, the failure behaviour is undefined. | Confirm in the feasibility spike that the outbox dispatcher handles "broker unreachable at startup" gracefully — outbox rows remain `Pending` and the next dispatcher tick retries. |

---

## 4. Empirical Gaps — Feasibility Spikes Required

The mechanisms for all four QAS clauses below are design-complete across Iterations 1–4. No measurement has been taken. The spikes are required to support the response measures with evidence, not argument alone.

| QAS | Response measure | Mechanism in place | Spike to run |
| --- | --- | --- | --- |
| QAS-1 — Recovery after broker outage | Order appears in OpenBoxes within 60 seconds of broker recovery | Durable outbox + dispatcher auto-poll + consumer auto-reconnect + durable queue | Bring RabbitMQ down for 5 minutes while orders are placed; restart the broker; measure the time from broker restart to OpenBoxes fulfillment row creation for each queued order |
| QAS-2 — Zero oversell under concurrent load | Zero confirmed oversell events under concurrent load; losing order rejected in the same request cycle | Pessimistic row lock on `ProductWarehouseInventory` (ADR-005); reservation-aware availability check | Run concurrent HTTP requests against the checkout endpoint with exactly 1 unit in stock; vary concurrency from 2 to 20 simultaneous requests; confirm zero oversell rows and that the loser receives a `PlaceOrderResult` failure, not a committed order |
| QAS-4 — Backlog drain after consumer outage | All 12 orders placed during a 30-minute outage processed within 5 minutes of consumer recovery; no operator action | Durable queue + idempotent bridge consumer + ordered redelivery on a single-consumer queue | Place 12 orders while the bridge service is stopped; restart the bridge; measure the time from bridge start to all 12 OpenBoxes fulfillment rows being present and confirmed |
| QAS-5 — Carrier tracking latency | Tracking status visible to the customer within 30 seconds of the carrier status changing | `CarrierStatusPollerTask` (ADR-008): polls `GET /api/shipments/{ExternalShipmentId}/status` on WireMock every 30 s; status update and email enqueue commit in one DB transaction within the same tick | Update a WireMock shipment to a new status; confirm nopCommerce reflects it within one polling cycle (≤30 s) |
| QAS-5 — OpenBoxes fulfillment state visibility | OpenBoxes `ISSUED` state visible in nopCommerce within the polling interval (default 30 s) | `OpenBoxesStatusPollerTask`: polls `GET /api/generic/shipment` on schedule; updates order status in nopCommerce on `ISSUED` detection | Mark a fulfillment order as `ISSUED` in OpenBoxes; confirm nopCommerce order status updates within one polling cycle |

---

## 5. Accepted Limitations

These items are known, documented, and deliberately out of scope for the current assignment iterations. They do not threaten the demo.

| Limitation | Rationale for acceptance |
| --- | --- |
| POS bearer-token authentication not specified | Operational configuration; not an architectural decision. Stated as out of scope in ADR-006. The gate's correctness does not depend on the specific token issuance mechanism. |
| `IAmbientOrderContext` implementation choice open (`IHttpContextAccessor` vs `AsyncLocal`) | Implementation detail within the gate decorator. If a coupling issue surfaces during implementation, a follow-up ADR is the path. |
| No shared outbound HTTP policy library (Seam 7) | Each integration plugin implements its own HTTP client. Acceptable at this scope; a shared Polly-based HTTP policy library is a natural later refactoring once more integrations exist. |
| HMAC payload signing not implemented for inbound webhook | Bearer token is sufficient for the demo; ADR-008 records HMAC as the production-hardening alternative. Not an architectural gap — an operational hardening step. |
| Carrier webhook delivery relies on carrier retry window. The inbound webhook path (ADR-008) does not own its own delivery durability — if nopCommerce is unreachable long enough for the carrier to exhaust its retry window, a status update is permanently lost. This is accepted because: (a) the scenario requires extended nopCommerce downtime, at which point order placement and the admin panel are also down — a lost tracking update is not the primary concern; (b) this is standard industry behaviour for webhook integrations; (c) the business impact is visibility-only — the order and shipment still exist. A `CarrierStatusPollerTask` (polling `GET /shipments/{ExternalShipmentId}` on WireMock, same polling pattern as the OpenBoxes poller) is the natural future hardening if operational reality demands it. | Industry-standard webhook delivery model; business impact is informational, not transactional. Not a demo blocker. |
| Stock back-propagation OpenBoxes → nopCommerce is not implemented. The `inventory.adjusted` flow declared in `03-bounded-contexts.md` (Commerce-visible stock derived from OpenBoxes) covers only the nopCommerce → OpenBoxes direction in the current design (`order.placed` decrements in OpenBoxes via the bridge). Stock corrections that originate at the warehouse — direct returns at the counter, shrinkage, supplier goods receipts — do not update `Product.StockQuantity` in nopCommerce, so the two values can drift. Iteration 5's `OpenBoxesStatusPollerTask` only reflects fulfillment-order *status* (`ISSUED`) back to nopCommerce; it does not pull stock quantities. ADR-005's reference to "the existing `inventory.adjusted` event flow" should be read as declared-but-not-implemented for the OpenBoxes-originated direction. | Use case 2 of the brief asks for "stock **or** order-state visibility" — the order-state half is structurally satisfied by the poller (QAS-5). Stock back-sync is an additional path, not a mandatory one for the chosen scenario. Under the controlled demo (we set initial stock and we drive every movement), the divergence does not surface. Hardening path is small: extend the same `OpenBoxesStatusPollerTask` to pull OpenBoxes stock on the same 30 s cadence and call `IProductService.UpdateStockQuantityAsync`. Explicitly deferred from Iteration 5 scope. |

