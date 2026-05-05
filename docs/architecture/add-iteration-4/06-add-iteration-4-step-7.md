# ADD Iteration 4 — Step 7: Analyze Current Design

## What This Step Does

Step 7 closes the iteration. It checks whether the design produced satisfies the iteration goal, names what remains unresolved, and produces the input list for any follow-up work.

---

## Iteration Goal Recap

Iteration 4 set out to give the customer cross-channel visibility of fulfillment progress by integrating nopCommerce with the carrier (WireMock) end to end:

1. **Outbound** — a `ShipmentSentEvent` consumer writes a `carrier.booking.requested` row to the existing Outbox; a new in-process `CarrierBookingConsumer` drains it, calls WireMock, and stores the returned tracking identifier on `Shipment.ExternalShipmentId`.
2. **Inbound** — a new `CarrierStatusPollerTask` (`IScheduleTask`) polls WireMock every 30 seconds for each open shipment; on detecting a status change it updates `Shipment.ExternalShippingStatus` and enqueues a customer notification email inside one DB transaction.

One architectural decision was recorded (ADR-008). QAS-5's carrier half is now structurally satisfied; the warehouse visibility half (OpenBoxes state → nopCommerce) is carried to Iteration 5.

---

## Analysis Against QAS-5's Response Measure

QAS-5 requires that the carrier tracking status becomes visible to the customer within 30 seconds of the state change occurring in the carrier system.

| Clause | Result |
| --- | --- |
| Tracking status visible to the customer within 30 s | The poller runs every 30 s; worst-case detection latency equals the poll interval. On detection the status update and email enqueue commit in a single local DB transaction — sub-second. The 30 s budget is met by construction |
| Email queued within the same window | The status update and `IWorkflowMessageService` enqueue happen inside the same DB transaction; either both succeed or the tick logs an error and retries on the next interval |

Empirical confirmation under load remains pending — listed in "Partially Satisfied" below alongside the carry-over spikes from Iter 3.

---

## End-to-End Coverage of the Full QAS Set

| QAS | Quality | Status before Iter 4 | Status after Iter 4 |
| --- | --- | --- | --- |
| QAS-1 | Reliability | Mechanism-complete (Iter 1+2+3); empirical timing pending spike | Unchanged — empirical timing still pending |
| QAS-2 | Consistency | Structurally satisfied (Iter 3); empirical load test pending spike | Unchanged — empirical load test still pending |
| QAS-3 | Availability | Solved (Iter 1) | Unchanged |
| QAS-4 | Recoverability | Mechanism-complete (Iter 1+2+3); empirical drain pending spike | Unchanged — empirical drain still pending |
| QAS-5 | Visibility | Not addressed | Carrier half structurally satisfied; warehouse visibility half (OpenBoxes polling) carried to Iteration 5 |

QAS-5 is partially satisfied. The carrier polling path closes the tracking-status clause. The warehouse fulfillment-state clause (OpenBoxes `ISSUED` → nopCommerce) requires a polling `IScheduleTask` — deferred to Iteration 5.

---

## What Is Fully Satisfied

- **QAS-5 carrier half structurally satisfied** — both halves of the carrier integration land; the customer-visible tracking state changes within the 30 s budget by construction. The warehouse visibility half is carried to Iteration 5.
- **Pressure point #7** from `02-current-state.md` ("no outbound integration pattern in the framework") closed with a working pattern: Outbox reuse for outbound booking, and a scheduled poller for inbound status detection.
- **CON-17, CON-19, CON-21, CON-22** all addressed: race between poll tick and dispatch (CON-17, poller skips null `ExternalShipmentId`); concurrent poll tick safety (CON-19, row-level last-write-wins); status-vocabulary preservation (CON-21, parallel `ExternalShippingStatus`); admin UI not blocked (CON-22, Outbox reuse).
- **ADR-002's plugin boundary** preserved — all new code in the plugin; the four `Shipment` columns are added by the plugin's FluentMigrator migration; `Nop.Core` source is not modified.
- **ADR-003's idempotent-consumption mandate** extended to `CarrierBookingConsumer` using the same dedup shape (skip if `ExternalShipmentId` already set).
- **ADR-004's Outbox** reused for the outbound flow — no new dispatcher, no new schedule task, no parallel outbox table.
- **The wire-contract versioning policy** (Version field + tolerant readers) applied uniformly across all new message types.

---

## What Is Partially Satisfied

| Item | What is in place | What is missing |
| --- | --- | --- |
| QAS-5 within-30-second response measure | Design (30 s poll interval, single DB transaction) is bounded by construction | Empirical timing — bundled with the same spike that times QAS-1/2/4 |
| WireMock contract | `IWireMockClient` interfaces defined; booking and status shapes fixed at Step 5 | Concrete WireMock stub configuration to be authored alongside the demo deployment |
| Status update visibility on the order page | Render path uses `ExternalShippingStatus` when present; falls back to internal `ShippingStatus` | Admin/customer template review — cosmetic, not architectural |

---

## What Is Not Satisfied — Residual Risks

| Residual risk | Why it remains | Where it goes |
| --- | --- | --- |
| Tracking URL, location, and full event history on the order page | QAS-5 only requires status text visible | UX polish; the data is captured (`ExternalShippingStatus`, `LastStatusOccurredAtUtc`) and ready to surface when the design dictates it |
| Multi-carrier routing | `ExternalCarrierCode` is captured but only one carrier (WireMock) is wired; `IExternalStatusMapper` is single-carrier | Future iteration if a second carrier is added; the column reservation makes the future work additive |
| Returns flow on `RETURNED` status | Status is recorded but no downstream refund/restock is triggered | Future iteration — likely paired with the recurring-payment work also flagged at Iter 3 Step 2 |
| `IShipmentService` does not expose a `GetByExternalIdAsync` | The plugin's poller reads via `IRepository<Shipment>` directly | Acceptable scope-bend; if another consumer needs the same lookup, promote to `IShipmentService` |
| Bundled spike (QAS-1/2/4/5 empirical timing + OpenBoxes API capability) | Carried from Iter 3; this iteration adds QAS-5 timing to the same spike list | Must run before the live demo |
| Outbox row retention policy | Carried from Iter 2 | Operational concern — periodic archive |
| DLQ replay tooling (carrier-booking DLQ joins the openboxes one from Iter 3) | Replay still manual | Operational tooling — same residual as Iter 3 |
| Rejected-order DB pollution cleanup | Carried operational residual from Iter 3 | Operational concern |

---

## Inputs Carried Forward

| Carry-over | Origin |
| --- | --- |
| **Primary driver: QAS-5 warehouse half** — OpenBoxes `ISSUED` state must become visible in nopCommerce; polling `IScheduleTask` required | This iteration — OpenBoxes has no outbound webhook capability (confirmed spike) |
| Bundled feasibility spike — empirical timing of QAS-1/2/4/5; OpenBoxes API capability | Iter 3 Step 7 + this iteration |
| Rejected-order DB pollution cleanup | Iter 3 Step 7 |
| Outbox row retention policy | Iter 2 Step 7 |
| DLQ replay tooling (two DLQs: openboxes, carrier-booking) | Iter 3 Step 7 + this iteration |

---

## Iteration Verdict

Iteration 4 closed the carrier half of QAS-5. The carrier integration is complete end to end: outbound booking via the existing Iter 2 Outbox, and inbound status detection via a scheduled polling task that reads carrier state autonomously every 30 seconds. One architectural decision was recorded:

- [ADR-008 — Carrier Status via Scheduled Polling](../07-adrs/ADR-008-carrier-status-polling.md)

The plugin produced here establishes the bidirectional carrier channel within a single nopCommerce plugin, with no new independently deployable service and no inbound HTTP surface. The outbound Outbox reuse and the polling pattern are both candidates for future carrier integrations, but no generalisation work is performed in this iteration.

QAS-1 through QAS-4 are unaffected. QAS-5's warehouse visibility half is carried to Iteration 5, which will add the `OpenBoxesStatusPollerTask`.

The iteration is closed. Iteration 5 begins with the warehouse half of QAS-5 and the OpenBoxes polling task.
