# ADD Iteration 3 — Step 7: Analyze Current Design

## What This Step Does

Step 7 closes the iteration. It checks whether the design produced satisfies the iteration goal, names what remains unresolved, and produces the input list for Iteration 4.

---

## Iteration Goal Recap

Iteration 3 set out to establish OpenBoxes as the **operational authority for cross-channel stock allocation** by introducing two cooperating artefacts in one structural change:

1. The synchronous **allocation gate** inside nopCommerce, reached by web checkout via decorator and by POS via a new HTTP endpoint, satisfying QAS-2.
2. The asynchronous **OpenBoxes bridge consumer**, completing the publish chain proven in Iterations 1 and 2 and realising ADR-003's idempotent-consumption mandate.

Five architectural decisions were recorded (ADR-006 through ADR-010); the brief's "at least one independently deployable subsystem" requirement was met for the first time.

---

## Analysis Against QAS-2's Response Measure

QAS-2's response measure has two clauses. Each is checked against the design.

| Failure mode | Iteration 2 result | Iteration 3 result |
|---|---|---|
| Two simultaneous web orders for the last unit | Both could succeed — `02-current-state.md:38` ("no atomic hold mechanism") | Exactly one succeeds — pessimistic row lock (ADR-006) plus reservation-aware availability |
| One web order + one POS sale, simultaneous, last unit | Both could succeed (no shared gate) | Exactly one succeeds — POS reaches the same gate via `/api/inventory/reserve` (ADR-007) |
| Loser rejected within the same request cycle | Not applicable — no gate | Yes — `409 Conflict` with structured failure body (POS); `PlaceOrderResult` error caught at `OrderProcessingService.cs:1634` (web) |
| Inventory never below zero | Could go negative under contention | Cannot — gate refuses to decrement below the effective availability |

Empirical confirmation under load remains pending — listed in "Partially Satisfied" below.

---

## End-to-End Coverage of QAS-1 and QAS-4

Iteration 1 produced the durable publish path under the assumption RabbitMQ is reachable. Iteration 2 closed the broker-down hole. Iteration 3's bridge consumer makes the entire chain observable for the first time.

| QAS clause | Before Iter 3 | After Iter 3 |
|---|---|---|
| QAS-1: order in OpenBoxes within 60 s of consumer recovery | Mechanism end-to-end was untestable — no real consumer | Mechanism + consumer; idempotent on `OrderGuid` (ADR-009); empirical timing remains spike-pending |
| QAS-4: 12 orders processed within 5 minutes after consumer recovery, no operator action | Mechanism only — consumer did not exist | Mechanism complete; redelivered messages produce no duplicates (ADR-009); empirical drain time remains spike-pending |

---

## What Is Fully Satisfied

- **QAS-2 structurally satisfied** across web + POS — zero oversell by design under concurrent contention.
- **ADR-003's idempotent-consumption mandate**, a policy without an implementation since Iteration 1, is finally realised by ADR-009.
- **CON-9 through CON-16** all addressed: bridge hosting (CON-9), idempotency (CON-10), broker/OpenBoxes degradation (CON-11), poison handling (CON-12), wire-contract versioning (CON-13), gate latency (CON-14, addressed by hosting the gate locally per ADR-006), shared gate (CON-15, ADR-007), reservation TTL (CON-16, schedule task).
- **Brief's "at least one independently deployable subsystem"** — satisfied by ADR-008.
- **Cross-cutting concern from Iter 2 Step 7 — message versioning policy** — closed by ADR-010.
- The publish path established in Iterations 1 and 2 is **untouched** — no regression on QAS-1's publisher-side guarantees.

---

## What Is Partially Satisfied

| Item | What is in place | What is missing |
|---|---|---|
| QAS-2 zero-oversell under concurrent load | Design (row lock + reservation-aware availability) prevents oversell by construction | Empirical load test under concurrent contention — feasibility spike (topic 10) still required |
| QAS-1 60-second recovery | Mechanism end-to-end (durable queue + outbox + idempotent consumer); broker reconnects automatically | Empirical timing under broker recovery — spike still required |
| QAS-4 5-minute backlog drain | Same — mechanism complete; ordered redelivery via single-consumer queue; idempotent insert | Empirical drain time under realistic backlog — spike still required |
| OpenBoxes API integration | `IOpenBoxesClient` interface defined; consumer logic agnostic to specifics (ADR-008) | Concrete API contract pending the OpenBoxes feasibility spike noted in Step 1 |

---

## What Is Not Satisfied — Residual Risks

| Residual risk | Why it remains | Where it goes |
|---|---|---|
| nopCommerce DB pollution on rejected web orders | The decorator throws inside `AdjustInventoryAsync`; `SaveOrderDetailsAsync` already committed the `Order` row at line 1589; the row is left with `Success=false` | Operational concern — periodic cleanup task or accepted as audit. Recorded for Iter 4+ |
| OpenBoxes API contract still uncertain | Open since Iter 3 Step 1; not closeable without the spike | Feasibility spike (topic 10) — must run before the live demo |
| Bridge dedup store is a single point of failure | If the bridge's local DB is unreachable, the bridge cannot safely process | Small store; backup/restore is operational. Documented in ADR-009; revisit if scale rises |
| POS authentication mechanism deferred | Step 4 specified bearer-token style, no mechanism chosen | Operational config — not architectural |
| `IAmbientOrderContext` implementation choice not decided | Step 5 left `IHttpContextAccessor` vs `AsyncLocal` open | Implementation detail; if a coupling issue surfaces, follow-up ADR |
| DLQ replay tooling not implemented | The DLQ exists; no UI/CLI for replay yet | Operational concern — later iteration |
| Outbox row retention | Carried from Iter 2; `Sent` rows accumulate | Operational; retention task in later iteration |
| Recurring-payment subscription path | Explicitly out of scope per Iter 3 Step 2 — same `AdjustInventoryAsync` seam at `OrderProcessingService.cs:1982` is not protected by the gate | Future iteration if recurring becomes business-relevant |
| Polling interval and load characteristics for `ReleaseExpiredReservationsTask` | Default 30 s is a guess at this scale | Same spike that times the rest of the iteration |

---

## Inputs Carried Into Iteration 4

| Inherited input | Origin |
|---|---|
| **Primary driver:** QAS-5 (Visibility — carrier tracking webhook reaches customer in ≤ 10 s) | `04-qas.md:71-83` — Scenario QAS set; not yet addressed |
| **Surrounding system to integrate:** WireMock as carrier simulator | `01-scenario.md:26`; `03-bounded-contexts.md` Shipping Context |
| **New mechanism needed:** webhook ingestion endpoint inside nopCommerce | `02-current-state.md:84-90` — pressure point #7 ("no webhook ingestion or outbound integration pattern in the framework") |
| **Schema addition needed:** `ExternalShipmentId` on `Shipment` entity, plus an external-status mapping | `04-qas.md:83` — *"The Shipment entity needs an ExternalShipmentId field to correlate the incoming webhook to the correct order"* |
| **Open spike from Iter 3:** OpenBoxes API capability | Iter 3 Step 1 — must close before final demo |
| **Open spike from Iter 3:** empirical timing of QAS-1, QAS-2, QAS-4 measures | Iter 3 partial-satisfaction list — bundled spike |
| **Carry-over residual:** rejected-order DB pollution cleanup | Iter 3 residual list |

---

## Iteration Verdict

Iteration 3 made OpenBoxes the operational authority for cross-channel stock allocation. QAS-2 is structurally satisfied; QAS-1 and QAS-4's end-to-end behaviour is now mechanism-complete and awaits empirical confirmation by feasibility spike. Five architectural decisions were recorded:

- [ADR-006 — Allocation Gate Hosted Inside nopCommerce](../07-adrs/ADR-006-allocation-gate-inside-nopcommerce.md)
- [ADR-007 — Cross-Channel Allocation via Synchronous HTTP and Async Confirmation](../07-adrs/ADR-007-cross-channel-allocation-sync-http.md)
- [ADR-008 — OpenBoxes Bridge as a Separate Deployable Service](../07-adrs/ADR-008-bridge-as-separate-deployable.md)
- [ADR-009 — Idempotent Consumer with Bridge-Local Dedup and DLQ](../07-adrs/ADR-009-idempotent-consumer-and-dlq.md)
- [ADR-010 — Versioned Wire Contract with Tolerant Readers](../07-adrs/ADR-010-wire-contract-versioning.md)

The brief's "at least one independently deployable subsystem" requirement is met for the first time, by ADR-008.

The iteration is closed. Iteration 4 begins with QAS-5 and the carrier integration via WireMock.
