# Presentation Script — 8 min + Q&A

---

## p1 · Title (0:00–0:10)

*"We are evolving nopCommerce from a web storefront into a commerce core that stays useful when surrounding systems — WMS and POS — are slow, unavailable, or contradictory."*

One problem. Everything that follows answers it.

---

## p2 · Scenario Choice (0:10–0:45)

**On screen**: The Problem | The Choice

- VerdeMart runs nopCommerce as a web storefront only. Order state needs to reflect stores and warehouses. Operations cannot stop when external systems lag.
- Scenario C maps directly onto nopCommerce's existing domain — orders, warehouses, shipments. We **evolve**, we don't invent.
- A and B rejected: federated commerce and regulated commerce require org/policy modelling outside nopCommerce's domain. We can't credibly demo those.

> **Q — Why not A or B?** A requires inter-org federation, B requires compliance policy modelling. Neither exists in nopCommerce's domain. C does.

---

## p3–p5 · Current State Analysis (0:45–1:30)

**On screen**: title card → layered architecture diagram → same diagram with WMS/POS on the left marked "No connection" and "Sync/Blocking"

- nopCommerce is a modular monolith: Core → Data → Services → Presentation + Plugins.
- Strengths: plugin architecture, `OrderPlacedEvent` (fires post-persistence at `OrderProcessingService.cs:1617`), existing warehouse/stock/shipment models, scheduled tasks.
- Gaps: events are in-process only — not durable. Checkout is fully synchronous. WMS and POS have **no connection** to nopCommerce today. Any call out is sync and blocking.
- *The extension seam exists. What's missing is integration discipline.*

> **Q — Why not call WMS synchronously from checkout?** That couples checkout availability to WMS availability — the exact failure mode we're solving. ADR-0003.

---

## p6–p9 · Target Architecture (1:30–3:10)

**On screen**: title card → target diagram (Omnichannel Plugin + RabbitMQ + Omnichannel Worker + OmniOutboxMessage) → happy-path sequence → resilience sequence

Iteration order is not arbitrary — each pays the debt introduced by the previous.

**Iteration 1 — Resilience** (QA-1):
- Tactics: outbox pattern, async RabbitMQ, retry + exponential backoff, circuit breaker, DLQ.
- `OrderPlacedEvent` → plugin writes `OmniOutboxMessage` in same DB transaction → scheduled publisher sends to RabbitMQ → Worker calls WMS HTTP → WMS returns `externalRequestId` → callback updates projection.
- Trade-off accepted: **at-least-once delivery**. Paid by Iteration 2.
- ADR-0003, ADR-0005.

**Iteration 2 — Consistency** (QA-2):
- Tactics: `messageId` inbox (dedup), `sourceVersion` guard (stale detection), local stock projection.
- Trade-off: two views of stock — visible drift over silent corruption.
- ADR-0006, ADR-0007.

**Iteration 3 — Traceability & Operability** (QA-3 + QA-4):
- Tactics: 3-ID propagation (`OrderGuid` + `messageId` + `externalRequestId`), structured state, queue/DLQ visibility.
- Trade-off: every component must propagate IDs on error paths too.
- ADR-0008.

Recovery (resilience sequence on p9): WMS 503 → retry + backoff → circuit breaker opens → fulfillment marked degraded → WMS recovers → backlog drains **≤ 60 s** → 0 orders stuck > 5 min.

> **Q — Why outbox instead of direct publish?** Direct publish from checkout is a distributed transaction. Outbox writes in the same DB tx as the order — publish timing decoupled from checkout. ADR-0003.
>
> **Q — Why projection-first for stock?** Write-through to core `ProductWarehouseInventory` silently merges external state. Projection keeps the boundary explicit — visible drift over silent corruption. ADR-0007.
>
> **Q — Why not exactly-once delivery?** Exactly-once requires broker-level coordination we don't control. At-least-once + idempotent receiver gives the same application-level guarantee with less coupling.

---

## p10–p11 · Chosen Framework (3:10–3:55)

**On screen**: ADD / ACDM / ADM labels → comparison table + "What we use from each"

*"Here's what we built — now the method that structured how we got there."*

- **ADD primary**: iterative, one QA driver per iteration. Tactics vocabulary (outbox, idempotency, circuit breaker, ACL) is our exact solution vocabulary.
- **ACDM borrowed**: go / partial-go / no-go at the end of each iteration; experiment-charter framing for the spike.
- **ADM borrowed**: Phase-F migration table for the roadmap.
- Not ACDM primary: single-pass single-driver — we have five interacting QAs. Not ADM primary: enterprise breadth we don't need; rubric penalises inflated scope.

> **Q — Why not ACDM primary?** ACDM is single-driver single-pass. Five interacting QAs need ADD's iterative structure. We use ACDM's go/no-go vocabulary inside each ADD iteration.
>
> **Q — Isn't mixing methods unprincipled?** ADD itself says it doesn't cover evaluation or migration. We filled those holes with the methods that do — that's honest method use.

---

## p12 · Architectural Decisions (3:55–4:35)

**On screen**: ADR table — 3 green (accepted) + 2 red (rejected)

- 10 ADRs total: 8 accepted (decision + tradeoff + rejected alternative), 2 explicitly rejected (with triggers to revisit).
- **ADR-0003** Outbox + RabbitMQ — checkout stays independent from WMS failures.
- **ADR-0005** No shared DB — boundaries stay explicit and deployable.
- **ADR-0008** 3-ID correlation — delayed orders stay explainable end-to-end.
- **ADR-0009** Reject microservices — coordination is the problem, decomposition adds overhead.
- **ADR-0010** Reject OpenTelemetry stack — ADR-0008's 3-ID correlation satisfies QA-3 with less infra.

> **Q — Why rejected ADRs?** Explicit non-decisions prevent them returning as proposals in Part 2. Microservices and distributed tracing were seriously considered and turned down.
>
> **Q — What about the other 5 ADRs?** Frame ADRs (0001 scenario, 0002 monolith, 0004 simulators) are short scope decisions. Iteration ADRs (0006, 0007) follow the same structure as 0003.

---

## p13–p14 · Domain and Boundary Model (4:35–5:15)

**On screen**: 6 subdomains list → bounded context map

- **6 subdomains**: Commerce Core · Inventory Visibility · Fulfillment Coordination · Integration Reliability · Warehouse Operations · Store/POS.
- **4 bounded contexts**: nopCommerce Core [U], Omnichannel Integration [D], WMS [U], POS [U].
- Omnichannel Integration covers three subdomains — they share one correlation model (`OrderGuid` + `messageId`) and the same reliability infrastructure.
- Worker is an **ACL** on WMS and POS edges: external schemas and quirks translated before reaching the plugin.
- Core ↔ Omnichannel: customer/supplier via versioned event contracts — not a shared model. ADR-0005.

> **Q — Why one integration context for three subdomains?** Same correlation model, same infrastructure. Splitting would create three outboxes and three inboxes with no boundary benefit.
>
> **Q — What does [U] and [D] mean?** Upstream/downstream in the customer-supplier relationship. Omnichannel Integration [D] depends on nopCommerce Core [U] for order events.

---

## p15 · Quality Attribute Scenarios (5:15–5:55)

**On screen**: 5 bullet points with numeric measures

- **QA-1 Resilience**: WMS 503 for 30 s → backlog drains ≤ 60 s after recovery; 0 orders pending > 5 min.
- **QA-2 Consistency**: duplicate POS event → rejected ≤ 50 ms; 0 duplicate fulfillments.
- **QA-3 Traceability**: pending order → outbox → MQ → worker → projection; resolution ≤ 3 admin clicks.
- **QA-4 Performance**: `OrderPlacedEvent` → outbox row written ≤ 100 ms; 0 sync HTTP in checkout.
- **QA-5 Cross-Channel Visibility**: WMS marks shipped → nopCommerce status updated ≤ 10 s; 0 state divergences.

QA-1 and QA-2 are "attack first" — iteration order follows directly from this prioritisation.

> **Q — Are these measures validated?** QA-4/QA-5 thresholds are derived from the feasibility spike (source analysis at line 1617). QA-1 drain rate and QA-2 detection latency are design targets — measured in Part 2. The spike confirms the seam exists.

---

## p16 · Risks and Feasibility Spike (5:55–6:35)

**On screen**: Key risks (left) + Spike question (right)

**5 risks with observable success signals:**
1. Checkout accidentally depends on WMS → e2e test: WMS 503 + checkout P95 measure.
2. Duplicate messages create duplicate fulfillment → inbox replay test.
3. Stock projection becomes misleading → admin drift view.
4. Scope grows beyond selective evolution → migration table caps stages.
5. Plugin integration harder than expected → spike outcome (next slide).

**Spike question**: *"Can a placed order start an async omnichannel workflow without making checkout depend on WMS/POS availability?"*

> **Q — What if RabbitMQ is the bottleneck?** Outbox publisher is broker-agnostic. If throughput is insufficient, swap the broker without changing producers or consumers — ADR-0003 keeps the contract abstract.

---

## p17 · Evolution Roadmap (6:35–6:50)

**Note: slide is blank — image not rendered. Describe verbally.**

Four phases aligned to ADM Phase-F migration planning:
- **Phase 1** (now): architecture, ADRs, QA scenarios, spike — Part 1 deliverable.
- **Phase 2**: plugin + worker + outbox publisher + simulators; QA-1 and QA-2 go/no-go gates.
- **Phase 3**: 3-ID instrumentation, admin view, QA-3/4/5 gates.
- **Phase 4**: Part 2 demo — resilience, idempotency, and traceability scenarios.

Monolith is never rewritten — only extended.

---

## p18 · Feasibility Spike (6:50–7:25)

**On screen**: spike question + "Plugin can write durable outbox row and return" + "Checkout stays independent from WMS/POS availability"

- Question: can a placed order start an async workflow without rewriting checkout?
- `OrderPlacedEvent` fires **after** order persistence at `OrderProcessingService.cs:1617` — plugin consumer is the right hook.
- Plugin writes `OmniOutboxMessage` in the same DB transaction, then returns. Scheduled task publishes later.
- Outcome: **feasible**. Checkout is never blocked on WMS/POS.

> **Q — Did you actually run it?** We traced the extension points in source code and confirmed the seam exists at line 1617. The < 100 ms timing is a design target measured in Part 2 — the spike proves the mechanism exists.
>
> **Q — What if the plugin integration is harder than expected?** That was risk #5. The spike directly answers it — `IConsumer<OrderPlacedEvent>` is the proven integration point, and existing plugins demonstrate the full scaffolding (controllers, services, migrations, event consumers).

---

## p19 · Close (7:25–8:00)

**Limitations — no dedicated slide, deliver verbally:**
- At-least-once delivery, not exactly-once.
- Demo-token auth on internal endpoints — not real SSO.
- Simulators replace real WMS/POS — architectural pressure is real, vendor quirks are not exercised.
- Inbox table grows linearly; pruning deferred to Part 2.
- Stock projection can drift from core stock — visible drift over silent merge.

*Don't apologise for these — name them as deliberate tradeoffs.*

**Landing**: one problem · focused evolution · explicit tradeoffs · believable runtime challenge with recovery numbers · tight traceability · honest limitations.

*"Questions?"*

---

## Pacing

- p3–p5: advance slides deliberately — don't rush past the gaps diagram, it sets up everything.
- p6–p9: 30 s per iteration on p6–p7, then walk through the sequences on p8–p9. Say the trade-off out loud, don't read it.
- p10–p11: use the transition sentence — without it the framework-after-architecture order feels backwards.
- p17: don't acknowledge the blank slide — go straight into describing the four phases.
- p19: limitations land last before Q&A — say them with confidence, not apology.
