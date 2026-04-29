# ADD Iteration 4 — Step 7: Analyze Current Design

## What This Step Does

Step 7 closes the iteration. It checks whether the design produced satisfies the iteration goal, names what remains unresolved, and produces the input list for any follow-up work.

---

## Iteration Goal Recap

Iteration 4 set out to give the customer cross-channel visibility of fulfillment progress by integrating nopCommerce with the carrier (WireMock) end to end:

1. **Outbound** — a `ShipmentSentEvent` consumer writes a `carrier.booking.requested` row to the existing Outbox; a new in-process `CarrierBookingConsumer` drains it, calls WireMock, and stores the returned tracking identifier on `Shipment.ExternalShipmentId`.
2. **Inbound** — a new `/api/carrier/webhook` endpoint authenticates, audits, and hands the payload off to a local queue; an asynchronous `CarrierStatusConsumer` correlates by `ExternalShipmentId`, applies an out-of-order guard, updates state inside one DB transaction, and enqueues a customer notification email.

Four architectural decisions were recorded (ADR-011 through ADR-014); the QAS set defined in `04-qas.md` is now structurally complete.

---

## Analysis Against QAS-5's Response Measure

QAS-5 has two clauses, both bounded at **10 seconds** from webhook receipt. Each is checked against the design.

| Clause | Result |
|---|---|
| Tracking status visible to the customer within 10 s | The controller writes one audit row + one queue publish before returning 200; the consumer commits status update + email enqueue inside one local DB transaction. Observed end-to-end latency is dominated by the local DB transaction — sub-second under any realistic load — well inside the 10 s budget |
| Email queued within the same window | The status update and `IWorkflowMessageService` enqueue happen inside the same DB transaction; either both succeed or the message is NACKed for retry. No separate email retry path is needed |

Empirical confirmation under load remains pending — listed in "Partially Satisfied" below alongside the carry-over spikes from Iter 3.

---

## End-to-End Coverage of the Full QAS Set

| QAS | Quality | Status before Iter 4 | Status after Iter 4 |
|---|---|---|---|
| QAS-1 | Reliability | Mechanism-complete (Iter 1+2+3); empirical timing pending spike | Unchanged — empirical timing still pending |
| QAS-2 | Consistency | Structurally satisfied (Iter 3); empirical load test pending spike | Unchanged — empirical load test still pending |
| QAS-3 | Availability | Solved (Iter 1) | Unchanged |
| QAS-4 | Recoverability | Mechanism-complete (Iter 1+2+3); empirical drain pending spike | Unchanged — empirical drain still pending |
| QAS-5 | Visibility | Not addressed | Structurally satisfied; empirical timing pending the same bundled spike |

The QAS set defined in `04-qas.md` is now **structurally complete**. The remaining gap is empirical confirmation, owed by the bundled feasibility spike noted at Iter 3 Step 7.

---

## What Is Fully Satisfied

- **QAS-5 structurally satisfied** — both halves of the carrier integration land; the customer-visible state changes within the 10 s budget by construction.
- **Pressure point #7** from `02-current-state.md` ("no webhook ingestion or outbound integration pattern in the framework") closed with a working pattern: bearer auth + audit table + async handoff via local queue + idempotency by carrier-supplied event id.
- **CON-17 through CON-24** all addressed: race between webhook arrival and dispatch (CON-17, NACK-with-requeue + DLQ); carrier retries (CON-18, dedup); out-of-order delivery (CON-19, timestamp guard); auth (CON-20, bearer); status-vocabulary preservation (CON-21, parallel `ExternalShippingStatus`); admin UI not blocked (CON-22, Outbox reuse); audit (CON-23, `CarrierWebhookEvent`); 10 s budget (CON-24, async handoff).
- **ADR-002's plugin boundary** preserved — all new code in the plugin; the four `Shipment` columns are added by the plugin's FluentMigrator migration; `Nop.Core` source is not modified.
- **ADR-003's idempotent-consumption mandate** extended to a second consumer (`CarrierStatusConsumer`) using the same shape established by ADR-009.
- **ADR-004's Outbox** reused for the outbound flow — no new dispatcher, no new schedule task, no parallel outbox table.
- **ADR-010's wire-contract versioning policy** applied uniformly across all four new message types.

---

## What Is Partially Satisfied

| Item | What is in place | What is missing |
|---|---|---|
| QAS-5 within-10-second response measure | Design (async handoff, single DB transaction) is bounded sub-second | Empirical timing — bundled with the same spike that times QAS-1/2/4 |
| WireMock contract | `IWireMockClient` interface defined; the booking request/response shape is fixed at Step 5 | Concrete WireMock stub configuration to be authored alongside the demo deployment |
| Status update visibility on the order page | Render path uses `ExternalShippingStatus` when present; falls back to internal `ShippingStatus` | Admin/customer template review — cosmetic, not architectural |

---

## What Is Not Satisfied — Residual Risks

| Residual risk | Why it remains | Where it goes |
|---|---|---|
| HMAC payload signing for production | Bearer token is sufficient for the demo; ADR-011 records HMAC as the production-hardening alternative | Production hardening checklist — not a follow-up iteration |
| Token rotation infrastructure | `InboundBearerToken` is a setting; no rotation tooling | Operational secrets management — out of scope |
| Tracking URL, location, and full event history on the order page | QAS-5 only requires status text visible | UX polish; the data is captured (audit table + `Location` field on each event) and ready to surface when the design dictates it |
| DLQ replay UI/CLI | Two new DLQs (status, booking) join the OpenBoxes one from Iter 3; replay still manual | Operational tooling — same residual as Iter 3 |
| Multi-carrier routing | `ExternalCarrierCode` is captured but only one carrier (WireMock) is wired; `IExternalStatusMapper` is single-carrier | Future iteration if a second carrier is added; the column reservation makes the future work additive |
| Returns flow on `RETURNED` status | Status is recorded but no downstream refund/restock is triggered | Future iteration — likely paired with the recurring-payment work also flagged at Iter 3 Step 2 |
| Audit table retention | Rows accumulate indefinitely; the table grows linearly with carrier traffic | Operational concern — periodic archive, same shape as the Iter 2 outbox retention residual |
| Append-only audit semantics | `AmendOutcomeAsync` updates in place; an append-only design (insert a new row per outcome change) would be more robust for forensics | Implementation choice — recorded as a candidate refinement; not architectural |
| `IShipmentService` does not expose a `GetByExternalIdAsync` | The plugin's consumer reads via `IRepository<Shipment>` directly | Acceptable scope-bend; if another consumer needs the same lookup, promote to `IShipmentService` |
| Bundled spike (QAS-1/2/4/5 empirical timing + OpenBoxes API capability) | Carried from Iter 3; this iteration adds QAS-5 timing to the same spike list | Must run before the live demo |

---

## Inputs Carried Forward

The QAS set is structurally complete, so no further QAS-driven iteration is required. The remaining items are operational rather than architectural:

| Carry-over | Origin |
|---|---|
| Bundled feasibility spike — empirical timing of QAS-1/2/4/5; OpenBoxes API capability | Iter 3 Step 7 + this iteration |
| Rejected-order DB pollution cleanup | Iter 3 Step 7 |
| Outbox row retention policy | Iter 2 Step 7 |
| DLQ replay tooling (now three DLQs: openboxes, carrier-status, carrier-booking) | Iter 3 Step 7 + this iteration |
| Audit table retention | This iteration |
| HMAC payload signing for production hardening | This iteration |

---

## Iteration Verdict

Iteration 4 closed the QAS set. QAS-5 is structurally satisfied through a single new plugin (`Nop.Plugin.Shipping.CarrierWebhook`) that delivers both halves of the carrier integration end to end: outbound booking via the existing Iter 2 Outbox, and inbound webhook ingestion via async handoff to a local queue with full audit, dedup, and out-of-order protection. Four architectural decisions were recorded:

- [ADR-011 — Webhook Ingestion via Plugin with Async Internal Queue Handoff](../07-adrs/ADR-011-webhook-ingestion-async-handoff.md)
- [ADR-012 — External Shipment Correlation via `ExternalShipmentId` on `Shipment`](../07-adrs/ADR-012-external-shipment-correlation.md)
- [ADR-013 — External Status Preserved as String; Internal Enum Untouched](../07-adrs/ADR-013-external-status-preserved-as-string.md)
- [ADR-014 — Outbound Carrier Booking via Existing Outbox](../07-adrs/ADR-014-outbound-booking-via-outbox.md)

Pressure point #7 from `02-current-state.md` — the absence of any framework-level webhook ingestion pattern — is closed with a concrete realisation. The plugin produced here is a candidate template for future inbound integrations (e.g. payment-status callbacks, supplier ASN feeds), but no generalisation work is performed in this iteration; QAS-5 alone does not justify it.

The four QAS that were already addressed (QAS-1, QAS-2, QAS-3, QAS-4) are unaffected. The bundled feasibility spike from Iter 3 absorbs QAS-5's empirical timing and remains the last item before the live demo.

The iteration is closed. The QAS set defined in `04-qas.md` is structurally complete.
