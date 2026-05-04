# Part 1 Presentation Script — 8 minutes

**Format**: 10 slides, 8 minutes total, ~5 minutes Q&A defense after.

**Six principles to land** (from the brief):

1. one clear architectural problem
2. focused evolution
3. explicit tradeoffs
4. believable runtime challenge and recovery
5. tight traceability
6. honest defense of limitations

**Eight rubric topics** (must all appear): scenario choice · current-state · domain & boundary · QA scenarios · framework · target architecture · ≥3 ADRs · risk plan + spike.

---

## Slide 1 — Title + Problem (0:00–0:20)

**Title**: *Architectural Evolution of nopCommerce — Scenario C: Omnichannel Commerce Core*

**Talking point** (one sentence): *"We are evolving nopCommerce from a web storefront into a commerce core that must stay useful when surrounding operational systems — WMS, POS — are slow, stale, unavailable, or contradictory, and recover with full traceability."*

**Land**: this is **one** architectural problem, not a feature pile. Everything that follows answers it.

---

## Slide 2 — Scenario Choice (0:20–1:00)

**Slide content**: scenario C selected; one-line business reason; one-line rejection of A and B.

**Talking points**:

- VerdeMart Retail uses nopCommerce as its web storefront. The business needs nopCommerce to coordinate orders, stock, and fulfillment **across web, store, and warehouse channels**.
- Cross-channel state must reflect what happens outside the web shop. Operations must continue when external systems lag.
- We rejected Scenario A (federated commerce) and B (regulated commerce): both require deeper organizational/policy modelling we cannot credibly demo. Scenario C **already maps to nopCommerce's domain** — orders, warehouses, shipments — so we **evolve**, we don't invent.
- *The architectural problem: **coordination under delay and degradation**. The rest of the design follows from this.*

---

## Slide 3 — Current-State Analysis (1:00–1:45)

**Slide content**: layered diagram of nopCommerce; key extension points; key gaps.

**Talking points**:

- nopCommerce is a modular monolith with five layers — Core, Data, Services, Web, Plugins.
- **Strengths for Scenario C**: plugin architecture (extension seam), in-process domain events including `OrderPlacedEvent`, warehouse + stock + shipment models, scheduled tasks.
- **Gaps for Scenario C**: events are in-process only — not durable; checkout is fully synchronous; no outbox/inbox; no model for delayed external state.
- Specific source citations in `architecture.md`: `EventPublisher.cs:20`, `OrderProcessingService.cs:1617`, `ProductService.cs:1438` and `:1699`.
- *Conclusion: the extension seam is solid; what's missing is integration discipline. That's the exact gap we close.*

---

## Slide 4 — Domain and Boundary Model (1:45–2:25)

**Slide content**: context-map diagram (4 bounded contexts + ACL annotation).

**Talking points**:

- DDD vocabulary: **subdomains** are the problem space, **bounded contexts** are the solution space. The two don't have to map 1-to-1.
- Six subdomains: Commerce Core, Inventory Visibility, Fulfillment Coordination, Warehouse Ops, Store/POS, Integration Reliability.
- Four bounded contexts: nopCommerce Core, Omnichannel Integration, WMS, POS. The Omnichannel Integration context covers three subdomains because they share one model — correlated by `OrderGuid` + `messageId`.
- The **Worker is an Anti-Corruption Layer** on the WMS and POS edges: external schemas, statuses, and quirks are translated into our envelope before they reach the plugin.
- nopCommerce Core ↔ Omnichannel is a **customer/supplier** relationship via versioned event contracts, **not** a shared model. ADR-0005: no shared DB across boundaries.

---

## Slide 5 — Quality Attribute Scenarios (2:25–3:25)

**Slide content**: 5-row table; each row has measure with a number.

**Talking points**:

- Five SEI 6-part scenarios with **numeric measures**. Full doc: `quality-attribute-scenarios.md`.
- **QA-1 Resilience + Recovery**: WMS returns 503 for 30 s → checkout P95 ≤ 1.5× baseline; **recovery**: backlog drain ≤ 60 s after WMS returns 200; 0 orders pending > 5 min.
- **QA-2 Consistency**: duplicate or stale POS event → rejected ≤ 50 ms; 0 duplicate fulfillment.
- **QA-3 Traceability**: support investigates pending order → 100% link outbox → MQ → worker → projection; resolution ≤ 3 admin clicks.
- **QA-4 Operability**: WMS degraded → queue/retry/DLQ visible in single dashboard view.
- **QA-5 Performance**: outbox row written ≤ 100 ms after `OrderPlacedEvent`; 0 sync HTTP in checkout.
- **Prioritization** (lecturer's grid): QA-1 and QA-2 are upper-right "attack first"; QA-3/4/5 are cheap wins. Iteration order follows this directly.

---

## Slide 6 — Chosen Framework (3:25–4:25)

**Slide content**: 3-row comparison table (ADD / ACDM / ADM); borrowed-pieces table.

**Talking points**:

- We compared **ADD**, **ACDM**, and **ADM** — all three covered in class.
- Chosen: **ADD as primary**, with **ACDM go/no-go vocabulary borrowed** and **ADM Phase-F migration table borrowed**.
- **Why ADD wins for this scenario**: multi-driver (five QAs) — ADD is iterative (one driver per iteration); ACDM is explicitly single-pass single-driver. ADD's per-iteration output (goal, drivers, refined element, tactics, responsibilities, interfaces, decisions, analysis) maps **line-by-line** to this rubric. The tactics vocabulary (outbox, idempotency, circuit breaker, retry, projection, ACL) is our exact solution vocabulary.
- **Why not ACDM primary**: compressing five interacting drivers into one ACDM pass would lose fidelity. We use ACDM where it adds value: the go/partial-go/no-go decision at the end of each ADD iteration, and the experiment-charter framing for the spike.
- **Why not ADM primary**: ADM expects enterprise breadth (Phase B Business Architecture, Phase G cross-team governance). We evolve **one** product. The rubric explicitly penalises inflated scope. We use ADM where it adds value: the Phase-F migration table for the roadmap.
- *This is honest method use: ADD itself says it does not cover evaluation or migration — we filled those holes with the methods that do.*

---

## Slide 7 — Target Architecture: 3 ADD Iterations (4:25–5:55)

**Slide content**: three columns, one per iteration; each shows driver / element / tactics / decisions / trade-off.

**Talking points**:

- Iteration order is not arbitrary. Resilience first because it gates everything; consistency second because Iteration 1 introduces at-least-once delivery; traceability third because it instruments what now exists.

**Iteration 1 — Resilience and Recovery** (driver QA-1, element: Order → WMS edge):

- Tactics: outbox, async messaging, retry + exponential backoff, circuit breaker, DLQ.
- Concepts considered: **chosen** outbox + RabbitMQ; **rejected** sync HTTP from checkout (degrades checkout); **rejected** worker DB polling (hides shared-DB coupling).
- Decisions: ADR-0003 (outbox + RabbitMQ), ADR-0005 (no shared DB).
- **Trade-off consciously accepted**: at-least-once delivery; paid by Iteration 2.

**Iteration 2 — Consistency** (driver QA-2, element: Inbox/projection):

- Tactics: idempotent receiver (`messageId`), version check (`sourceVersion`), local projection.
- Concepts considered: **chosen** `messageId` inbox + `sourceVersion`; **rejected** queue-level dedup (not portable); **rejected** last-write-wins (silent corruption).
- Decisions: ADR-0006 (idempotency), ADR-0007 (projection vs write-through — projection-first).
- **Trade-off**: two views of stock (projection ≠ core); visible drift over silent corruption.

**Iteration 3 — Traceability and Operability** (drivers QA-3 + QA-4, element: cross-cutting):

- Tactics: 3-ID propagation (`OrderGuid` + `messageId` + `externalRequestId`), structured state, queue/DLQ exposure.
- Concepts considered: **chosen** explicit IDs; **rejected** OpenTelemetry (Part 2 scope); **rejected** timestamp-only (clock skew).
- Decisions: ADR-0008 (correlation).
- **Trade-off**: discipline cost of propagating IDs even on error paths, over a heavier observability stack.

Each iteration ends with **go / partial-go / no-go**: Go on Iter. 1, Go on Iter. 2 idempotency + Partial-go on projection-only stock, Go on Iter. 3.

---

## Slide 8 — Architectural Decisions (5:55–6:45)

**Slide content**: 3-ADR table — decision / one rejected alternative / tradeoff. Note the full set: 8 Accepted + 2 Rejected.

**Talking points**:

- **10 ADRs total**: 8 Accepted, 2 Rejected. Each Accepted ADR has Status, Context, Decision, Consequences, **Tradeoffs**, Rejected Alternatives. Rejected ADRs replace the last section with **Triggers to revisit**.
- Three highest-impact Accepted, presented now:
  - **ADR-0003** — Outbox + RabbitMQ. Rejected alternative: synchronous HTTP from checkout. Tradeoff: at-least-once delivery + operational surface.
  - **ADR-0005** — No shared DB across boundaries. Rejected alternative: worker SQL access. Tradeoff: data duplication in projections.
  - **ADR-0008** — 3-ID correlation. Rejected alternative: timestamp-only. Tradeoff: every component must propagate IDs correctly.
- **Two explicit Rejected ADRs** — recorded so non-decisions don't quietly return as proposals:
  - **ADR-0009** — Reject microservices extraction. The assignment brief itself warns against it; coordination is the architectural problem, not decomposition.
  - **ADR-0010** — Reject distributed-tracing infrastructure. 3-ID correlation in ADR-0008 satisfies QA-3 with less infra.
- Frame decisions (ADR-0001 scenario, ADR-0002 monolith, ADR-0004 simulators) keep scope honest. Iteration decisions (ADR-0003/0005/0006/0007/0008) are linked to the iteration that produced them.
- *Every rejection is concrete — "X fails QA-N because Y" — not "X is bad."*

---

## Slide 9 — Risks, Feasibility Spike, Recovery (6:45–7:35)

**Slide content**: risk table (each risk has a success signal); spike outcome; recovery sequence.

**Talking points**:

- **5 risks**, each with an observable success signal:
  - Checkout depends on WMS → e2e test with WMS 503 + checkout P95 measure.
  - Duplicates create duplicate fulfillment → inbox replay test.
  - Projection drift misleads → admin drift view.
  - Scope inflation → migration table caps stages.
  - Plugin integration harder than expected → spike outcome.
- **Feasibility spike** (= ACDM experiment charter):
  - **Question**: can a placed order start an async workflow without rewriting checkout?
  - **Success signal**: `OrderPlacedEvent` consumer writes outbox row in < 100 ms; scheduled task publishes later; no sync external HTTP in checkout.
  - **Outcome**: **feasible**. `OrderPlacedEvent` fires after order persistence (line 1617). Plugins own their tables. Scheduled task decouples publish.
- **Recovery story** (the mandatory pressure point): WMS goes down → checkout still proceeds → backlog accumulates in queue → WMS recovers → backlog drain ≤ 60 s → 0 orders left in `pending` > 5 min.

---

## Slide 10 — Limitations and Close (7:35–8:00)

**Slide content**: bullet list of known limits; one-line landing on the six principles.

**Talking points** (honest defense):

- **Limitations we accept**:
  - At-least-once delivery — not exactly-once.
  - Demo-token auth on internal endpoints — not real SSO.
  - Simulators replace real WMS/POS for the demo — they create the architectural pressure, but real-system quirks (auth, latency profiles) are not exercised.
  - Inbox table grows linearly; pruning deferred to Part 2.
  - Stock projection visibly drifts from core stock — visible drift over silent merge.
- **Landing**: one architectural problem (coordination under degradation), focused evolution (plugin + worker, monolith preserved), explicit tradeoffs (every ADR + every iteration has one), believable runtime challenge with recovery (QA-1 with numeric measure), tight traceability (3 IDs end-to-end), honest defense of limitations (this slide).
- *"Questions?"*

---

## Speaker notes — pacing tips

- Keep Slides 1–4 tight. They set up the problem. Don't over-explain nopCommerce internals — graders know it.
- Slide 6 (framework) is where the team earns the "methodology choice" rubric points. **Slow down.** Read the comparison from the slide, then say *why ADD wins for this scenario* in your own words.
- Slide 7 (target architecture) is the longest slot at 90 s. Spend 30 s per iteration. Don't read the trade-off line — say it.
- Slide 9 (spike + recovery) is where the team earns the "believable runtime challenge" rubric points. Hit the recovery numbers (≤ 60 s drain, 0 stuck > 5 min) in your own voice — not from the slide.
- Slide 10 (limitations) is short on time but high on grading impact. Don't apologise for limitations — name them as deliberate trade-offs.

## Q&A defense — likely questions, prepared answers

- *"Why not OpenTelemetry?"* → Part 2 scope; explicit IDs in admin view satisfy QA-3 with less infra. ADR-0008.
- *"Why not write-through stock?"* → Visible drift over silent corruption. Revisit after Part 2 measures impact. ADR-0007.
- *"Why not ACDM as primary?"* → Single-pass single-driver; we have five interacting drivers. We use ACDM's go/no-go inside ADD's analysis step, which is the right granularity.
- *"Why simulators not real systems?"* → Real systems would require auth, schema, vendor latency profiles we cannot reproduce. Simulators preserve the **architectural pressure** the assignment requires (degradation, recovery), and contracts are versioned for production swap-out. ADR-0004.
- *"What if RabbitMQ is the bottleneck?"* → Outbox has publisher confirms; backlog drain rate measured in Part 2. If proven inadequate, the outbox publisher can target a different broker without changing producers/consumers — ADR-0003 keeps the contract abstract.
- *"Are 5 minutes really enough to defend 10 ADRs?"* → Defense focuses on the three with highest leverage (ADR-0003, ADR-0005, ADR-0008). Frame ADRs (0001/0002/0004) are short and scope-bounded; iteration ADRs (0006/0007) follow the same pattern as 0003. The two **Rejected** ADRs (0009 microservices, 0010 tracing) are themselves a defense — they show non-decisions made explicit instead of left implicit.
- *"Why are there Rejected ADRs at all?"* → Documenting non-decisions explicitly prevents them returning as informal proposals during Part 2. Microservices extraction and distributed tracing are the two we considered seriously and turned down for scope/scenario reasons. Recording them is honest about what we chose **not** to do, in addition to what we chose to do.
