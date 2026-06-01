# Part 2 Execution Plan

Per-dev, per-date plan for delivering Part 2 of *Architectural Evolution of nopCommerce* (Scenario C — Omnichannel Commerce Core). Read this together with [roadmap.md](roadmap.md): the roadmap defines the phases and their verification gates; this plan assigns owners, dates, and parallel tracks for a **4-developer, half-time** team.

If `plan.md` and `roadmap.md` ever conflict, `roadmap.md` is authoritative for *what* must be delivered; this file is authoritative for *who* delivers it *when*.

## Calendar

- **Today**: 2026-05-14 (Thursday).
- **Soft freeze**: 2026-05-31 (Sunday). All scope below must be complete on this date.
- **Buffer + rehearsal**: 2026-06-01 (Mon), 2026-06-02 (Tue). Small fixes and evidence tweaks allowed; no new features.
- **Final presentation**: 2026-06-03 (Wednesday).

Working window 14–31 May = **18 calendar days** (12 weekdays + 6 weekend days). At half-time (~4 productive hours/day/dev) this is roughly **180–220 person-hours total**.

## Team & pairs

Two pairs, each owning one *edge* of the integration plus the simulator that exercises that edge. Pair members review each other's PRs (cheap, fast, knowledge stays redundant). One cross-pair pairing session per phase keeps boundary contracts aligned.

Fill in the placeholder names before kickoff.

### Pair A — Plugin & Consistency edge (+ POS sim)

Owns the **receive** side: how nopCommerce learns of events and how external state lands in the plugin.

- **João Roldão — Plugin scaffold + outbox track**
  - `Nop.Plugin.Misc.OmnichannelCore` scaffold, plugin lifecycle, migrations for the four tables.
  - `OrderPlacedEvent` consumer; scheduled outbox publisher with publisher confirms.
  - **Reconciler scheduled task** (per [ADR-0011](docs/adr/0011-order-outbox-insertion-strategy.md) — to be written in Phase 1).
  - Plugin admin view (Phase 5).
  - Owns `journal.md` integrity (no missing entries across team).
  - Slides-lead for the **design half** of the Part 2 deck (target arch, ADRs, iterations).

- **João Varela — Inbox + POS track**
  - Inbox `messageId` dedup at every plugin callback endpoint.
  - `sourceVersion` comparison for stock updates.
  - POS callback endpoint (internal HTTP, demo token).
  - **POS simulator** (normal / duplicate / stale modes).
  - Evidence for **QA-2** (consistency) and **QA-3** (traceability).

### Pair B — Worker & Resilience edge (+ WMS sim + Infra)

Owns the **send-out** side: how messages flow to WMS, how retries/breakers/DLQ behave, how the stack runs.

- **António — Worker + messaging track**
  - `.NET Worker Service` (independent deployable).
  - RabbitMQ topology: queues, bindings, dead-letter exchange.
  - Shared **message envelope library** consumed by plugin + worker.
  - Polly retry + circuit breaker on worker → WMS HTTP call.
  - DLQ wiring and backlog drain logic.
  - Structured logs in worker (3-IDs from ADR-0008).

- **Diogu — Sims + Infra + Measurement track**
  - **WMS simulator** (normal / slow / unavailable / contradictory modes + admin toggle endpoint).
  - **Docker Compose** for the full stack (nopCommerce + SQL Server + RabbitMQ + worker + WMS sim + POS sim).
  - `docs/setup.md` (skeleton in Phase 1; filled incrementally through Phases 2, 3, 6).
  - **Baseline measurement** in Phase 1, before plugin install (replaces the Phase-6 baseline line in `roadmap.md`).
  - Evidence for **QA-1** (resilience) and **QA-4** (operability).
  - Slides-lead for the **demo/measurement half** of the Part 2 deck.

### Shared in Phase 6

- Demo dry-runs: all 4 devs run all 5 scenarios end-to-end.
- Evidence split: Pair A → QA-2 + QA-3; Pair B → QA-1 + QA-4 + baseline.
- ADR updates reflecting Part 2 reality (João Roldão leads, all review).

## Folded-in patches (from prior audit)

| Patch | Where | Owner | Why now |
|-------|-------|-------|---------|
| ADR-0011 — Outbox crash safety net (consumer + reconciler) | Phase 1 | João Roldão | `EventPublisher.cs:20` catches consumer exceptions silently; without a reconciler an order can exist with no integration trail. |
| ADR-0005 — Demo-token note (Consequences + Tradeoffs bullet) | Phase 1 | João Roldão (5 min) | Internal callback auth is a real design decision; presentation already mentions it, ADR must too. |
| Baseline measurement | Phase 1 (was Phase 6) | Diogu | Phase 3's verification gate references "1.5× baseline" — the number must exist before Phase 3, not after. |
| `docs/setup.md` skeleton | Phase 1, filled in 2/3/6 | Diogu | Assignment rubric requires "build and run instructions"; cheap to skeleton now, expensive if left to the end. |

## Phase plan

### Phase 1 — Scaffold + tables + baseline + patches

- **Days**: 1–4 · **Dates**: Thu 14 – Sun 17 May · **Calendar days**: 4
- **Goal**: foundation ready. Plugin builds and installs cleanly; baseline number captured; all four scaffolds (plugin, worker, WMS sim, POS sim) exist; Compose can boot every container.

**Tasks**

| Owner | Task | Files / paths |
|-------|------|---------------|
| João Roldão | `[x]` Plugin scaffold (copy `nopCommerce/src/Plugins/Nop.Plugin.Misc.Omnisend/` structure), `plugin.json`, lifecycle, `Install/Uninstall` | `nopCommerce/src/Plugins/Nop.Plugin.Misc.OmnichannelCore/` |
| João Roldão | `[x]` Migrations for `OmniOutboxMessage`, `OmniInboxMessage`, `OmniOrderFulfillment`, `OmniStockSyncState` | `.../OmnichannelCore/Migrations/` |
| João Roldão | `[ ]` Write [ADR-0011](docs/adr/0011-order-outbox-insertion-strategy.md) (consumer + reconciler strategy) | `docs/adr/0011-order-outbox-insertion-strategy.md` |
| João Roldão | `[ ]` Add Consequences + Tradeoffs bullet for demo-token auth | `docs/adr/0005-no-shared-database-boundaries.md` |
| João Varela | `[x]` Admin view shells (empty MVC controller + view skeleton) | `.../OmnichannelCore/Controllers/`, `.../Views/` |
| João Varela | `[x]` POS simulator scaffold (HTTP server, mode placeholder) | `services/pos-sim/` (new) |
| António | `[ ]` Worker service project scaffold; envelope library project | `services/worker/`, `services/contracts/` (new) |
| António | `[ ]` RabbitMQ topology design (queues, bindings, DLX) documented | `services/worker/README.md` |
| Diogu | `[x]` WMS simulator scaffold (HTTP server, mode placeholder) | `services/wms-sim/` (new) |
| Diogu | `[x]` Docker Compose v1: services start, healthchecks pass, no logic yet | `docker-compose.yml` (project root, new) |
| Diogu | `[ ]` `docs/setup.md` skeleton with section headers + Phase markers | `docs/setup.md` |
| Diogu | `[x]` **Baseline measurement**: place 50 orders against vanilla nopCommerce, capture P50/P95 checkout latency | `docs/evidence/baseline.md` |

**Verification gate** (Sun 17 May)

- Plugin install/uninstall round-trip leaves DB clean; the four tables appear with correct columns. **Status (2026-05-14)**: install + tables + admin page confirmed locally (see [evidence](docs/evidence/phase-1-plugin-scaffold.md)); **uninstall DB validation still pending** — phase remains `In review` in `roadmap.md` until that gate passes.
- `docker compose up` starts every service with healthcheck green (even if some endpoints return placeholders).
- `docs/evidence/baseline.md` exists with P50/P95 numbers and the storefront URL used.
- ADR-0011 exists; ADR-0005 has the demo-token bullet.

**Phase 1 progress (as of 2026-05-14)** — Varela completed the plugin scaffold, four-table migration, and admin shell on `feat/phase-1-omnichannel-plugin-scaffold` (merged). Diogu completed Docker Compose v1 (`docker compose up` starts every service with healthchecks green). **Remaining**: ADR-0011, ADR-0005 demo-token note, POS sim scaffold, worker scaffold + RabbitMQ topology design, WMS sim scaffold, `docs/setup.md` skeleton, baseline measurement, uninstall DB validation.

**Risks**

- nopCommerce migration tooling misbehaves on the active branch — mitigation: João Roldão runs install/uninstall round-trip on day 1, before any other plugin code.
- `.NET 10.0.100` SDK not on devs' PATHs — mitigation: README already warns; everyone confirms `dotnet --version` locally on day 1, or commits to Docker-only iteration.

### Phase 2 — Happy path (Iteration 1 normal flow)

- **Days**: 5–11 · **Dates**: Mon 18 – Sun 24 May · **Calendar days**: 7
- **Goal**: place an order through the storefront → fulfillment row in plugin with state `accepted`. No pressure, no error injection. WMS sim in `normal` mode only.

**Tasks**

| Owner | Task | Files / paths |
|-------|------|---------------|
| João Roldão | `[ ]` `OrderPlacedEvent` consumer writes outbox row | `.../OmnichannelCore/Infrastructure/EventConsumer.cs` |
| João Roldão | `[ ]` Scheduled outbox publisher (publisher confirms on) | `.../OmnichannelCore/ScheduleTasks/OutboxPublisherTask.cs` |
| João Roldão | `[ ]` Reconciler scheduled task (per ADR-0011): scans recent orders without outbox rows | `.../OmnichannelCore/ScheduleTasks/OutboxReconcilerTask.cs` |
| João Roldão | `[ ]` Internal callback endpoint for `fulfillment.status.changed.v1` | `.../OmnichannelCore/Controllers/OmnichannelCallbackController.cs` |
| João Varela | `[ ]` Inbox table read/write skeleton (used in Phase 4) | `.../OmnichannelCore/Services/OmniInboxService.cs` |
| António | `[ ]` Worker consumes `commerce.order.placed.v1`, calls WMS, publishes `fulfillment.status.changed.v1` | `services/worker/` |
| António | `[ ]` Message envelope library finalized (`messageId`, `correlationId`, `eventType`, `occurredOnUtc`) | `services/contracts/Envelope.cs` |
| Diogu | `[x]` WMS sim `normal` mode: accept fulfillment request, return `externalRequestId` + `accepted` | `services/wms-sim/` |
| Diogu | `[ ]` Docker Compose stitches everything end-to-end | `docker-compose.yml` |
| Diogu | `[ ]` `docs/setup.md` filled in for Phase 2 stack | `docs/setup.md` |
| João Roldão + António | `[ ]` Cross-pair pairing session on envelope contract (~half a day) | `services/contracts/` |

**Verification gate** (Sun 24 May)

- E2E smoke: storefront order → fulfillment row exists in `OmniOrderFulfillment` with state `accepted`.
- Checkout P95 within baseline + small overhead (≤ baseline + 20 ms is the design target; record the actual number).
- No synchronous external HTTP visible on the checkout thread (verify by tracing or by inspecting the consumer code).
- Reconciler task runs on schedule and reports 0 missing rows on a healthy run.

**Risks**

- Scheduled-task publish lag larger than expected → mitigation: João Roldão measures publish latency from outbox-row-created to MQ-published; target ≤ 60 s on day 1 of Phase 2.
- RabbitMQ publisher confirm semantics misconfigured → mitigation: António writes a one-page note on how confirms are wired, reviewed by João Roldão during the envelope pairing session.

### Phase 3 — Resilience (Iteration 1 pressure work)

- **Days**: 12–14 · **Dates**: Mon 25 – Wed 27 May · **Calendar days**: 3
- **Runs in parallel with Phase 4** (different pair, no shared files).
- **Owner**: Pair B.
- **Goal**: deliver the mandatory pressure point — WMS unavailable for 30 s, then recover. Backlog drains on its own; nobody touches the storefront.

**Tasks**

| Owner | Task | Files / paths |
|-------|------|---------------|
| Diogu | `[x]` WMS sim `slow`, `unavailable`, `contradictory` modes + admin toggle endpoint | `services/wms-sim/` |
| António | `[ ]` Polly retry with exponential backoff on worker → WMS HTTP | `services/worker/Resilience/` |
| António | `[ ]` Polly circuit breaker; trip → mark fulfillment `pending/degraded` | `services/worker/Resilience/` |
| António | `[ ]` Dead-letter queue + handler for poison messages | `services/worker/` |
| António | `[ ]` Backlog drain on circuit-breaker close | `services/worker/` |
| Diogu | `[ ]` Pressure-test harness: toggle WMS to `unavailable` for 30 s, capture P95 + recovery time + orders-pending count | `docs/evidence/qa-1-pressure.md` |

**Verification gate** (Wed 27 May)

- **QA-1**: checkout P95 ≤ 1.5× baseline during 30 s WMS 503; backlog drain ≤ 60 s after recovery; 0 orders pending > 5 min after recovery.
- Go decision on Iteration 1 logged in `docs/part1/architecture-checkpoint.md` §6.

**Risks**

- Circuit-breaker thresholds tuned for tests but not for demo → mitigation: António records the thresholds in `services/worker/README.md` and Diogu reproduces the QA-1 scenario at least once in demo conditions on day 14.
- Recovery time depends on backlog size; if a long pressure window leaves a big backlog, drain time can exceed 60 s → mitigation: cap the pressure window in the demo to 30 s, document in `qa-1-pressure.md`.

### Phase 4 — Consistency (Iteration 2)

- **Days**: 12–14 · **Dates**: Mon 25 – Wed 27 May · **Calendar days**: 3
- **Runs in parallel with Phase 3** (different pair, no shared files).
- **Owner**: Pair A.
- **Goal**: duplicate and stale POS events are ignored without silently corrupting state. Inbox table holds every `messageId`; `OmniStockSyncState` ignores `sourceVersion` ≤ last applied.

**Tasks**

| Owner | Task | Files / paths |
|-------|------|---------------|
| João Varela | `[x]` Inbox `messageId` dedup at every callback endpoint | `.../OmnichannelCore/Services/OmniInboxService.cs` |
| João Varela | `[x]` `sourceVersion` comparison for stock updates (`OmniStockSyncState`) | `.../OmnichannelCore/Services/OmniStockSyncService.cs` |
| João Varela | `[x]` POS callback endpoint | `.../OmnichannelCore/Controllers/OmnichannelCallbackController.cs` |
| João Varela | `[x]` POS sim `duplicate` and `stale` modes | `services/pos-sim/` |
| João Roldão | `[x]` Plugin integration: connect POS callback into the inbox + stock projection paths | `.../OmnichannelCore/` |
| João Varela | `[x]` Unit tests for `messageId` dedup and `sourceVersion` staleness; e2e: duplicate ignored, stale ignored, legitimate update applied | `nopCommerce/src/Tests/Nop.Tests/Nop.Plugin.Misc.OmnichannelCore.Tests/`; evidence in `docs/evidence/qa-2-consistency.md` |

**Verification gate** (Wed 27 May)

- **QA-2**: duplicate `messageId` rejected ≤ 50 ms; 0 duplicate fulfillment rows; older `sourceVersion` ignored.
- Go decision on Iteration 2 logged in `docs/part1/architecture-checkpoint.md` §6 (and partial-go on projection-only stock per ADR-0007).

**Risks**

- `sourceVersion` clock semantics broken by POS sim's clock model → mitigation: João Varela documents the version-generation rule in `services/pos-sim/README.md`.
- Inbox table growth not capped → acceptable for demo; documented in ADR-0006.

### Phase 5 — Traceability (Iteration 3)

- **Days**: 15–16 · **Dates**: Thu 28 – Fri 29 May · **Calendar days**: 2
- **Goal**: a delayed/recovering order is explainable from the plugin admin view alone, in ≤ 3 admin clicks.

**Tasks**

| Owner | Task | Files / paths |
|-------|------|---------------|
| João Roldão | `[ ]` Plugin admin view by `OrderGuid`: returns outbox row, MQ message ID, worker attempts, fulfillment state | `.../OmnichannelCore/Views/Admin/`, `.../OmnichannelCore/Controllers/OmnichannelAdminController.cs` |
| João Varela | `[ ]` Plugin-side structured logs carrying `OrderGuid` + `messageId` + `externalRequestId` | `.../OmnichannelCore/` (cross-cutting) |
| António | `[ ]` Worker-side structured logs with the same 3 IDs; envelope enforced on inbound + outbound messages | `services/worker/` |
| Diogu | `[ ]` RabbitMQ Management UI exposed in Compose; one-page ops walkthrough | `docker-compose.yml`, `docs/setup.md` |
| João Roldão + António | `[ ]` Cross-pair: align log field names (`order_guid`, `message_id`, `external_request_id`) so QA-3 query works end-to-end | (review only) |

**Verification gate** (Fri 29 May)

- **QA-3**: pick 10 placed orders; for each, the outbox row, MQ message id, worker attempt log, and fulfillment projection are linkable by `OrderGuid` and `messageId`. Resolution path ≤ 3 admin clicks. Record in `docs/evidence/qa-3-traceability.md`.
- **QA-4**: RabbitMQ Management UI + plugin admin view together expose queue depth, retry count, DLQ size, fulfillment-pending count. Refresh latency ≤ 5 s. Record in `docs/evidence/qa-4-operability.md`.

**Risks**

- Admin view scope creep → mitigation: stop at "lookup by `OrderGuid` returns the chain"; no search, no filters, no editing.
- Structured-logging discipline drifts late in the project (someone logs without IDs on an error path) → mitigation: João Varela + António do a 30-min cross-pair log review on day 16.

### Phase 6 — Evidence + demo polish

- **Days**: 17–18 · **Dates**: Sat 30 – Sun 31 May · **Calendar days**: 2
- **Goal**: every demo scenario runs from `docker compose up` with no manual fix-ups; every QA scenario has a number in the evidence pack; slides done.

**Tasks**

| Owner | Task | Files / paths |
|-------|------|---------------|
| João Varela | `[ ]` Evidence pack: QA-2 + QA-3 (consolidate numbers, screenshots, run logs) | `docs/evidence/qa-2-consistency.md`, `docs/evidence/qa-3-traceability.md` |
| Diogu | `[ ]` Evidence pack: QA-1 + QA-4 + baseline (consolidate numbers, screenshots, run logs) | `docs/evidence/qa-1-pressure.md`, `docs/evidence/qa-4-operability.md`, `docs/evidence/baseline.md` |
| João Roldão | `[ ]` ADR updates reflecting Part 2 reality (e.g., write-through decision for ADR-0007 after measurement) | `docs/adr/` |
| João Roldão | `[ ]` Design slides (target arch, ADRs, iterations) for Part 2 deck | (slides repo / shared deck) |
| António | `[ ]` Worker hardening: clean shutdown, log polish, README finalised | `services/worker/` |
| Diogu | `[ ]` Demo + measurement slides; final Compose smoke from a fresh clone | (slides repo), `docker-compose.yml` |
| All 4 | `[ ]` Full end-to-end dry run of all 5 demo scenarios (normal, WMS unavailable + recovery, POS legitimate update, POS duplicate, POS stale) | (demo scripts) |

**Verification gate** (Sun 31 May)

- Fresh `git clone` + `docker compose up` runs the full stack to a healthy state.
- All 5 demo scenarios executed end-to-end at least twice; logs captured.
- `docs/evidence/` has a numbered measurement for every QA scenario (QA-1, QA-2, QA-3, QA-4, QA-5).
- `journal.md` has an entry for every change since the start of Phase 1.
- Team can answer "why was *X* alternative rejected?" within 30 s for every ADR.

**Risks**

- Docker Compose drift between dev machines → mitigation: Diogu does the fresh-clone smoke on a different machine if possible on day 18.
- Running out of time on evidence collection vs implementation polish → mitigation: João Varela + Diogu start evidence consolidation on the morning of day 17, not day 18.

## Parallelism map

```
Day:        1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18
Date(May): 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31

Pair A:    [---- Phase 1 ----][--------- Phase 2 ---------][- Phase 4 -][--- Phase 5 ---][- Phase 6 -]
Pair B:    [---- Phase 1 ----][--------- Phase 2 ---------][- Phase 3 -][--- Phase 5 ---][- Phase 6 -]
                                                            ^^^^^^^^^^^^
                                                            fully parallel
```

Phase 3 and Phase 4 share **zero files**: Phase 3 touches `services/worker/` + `services/wms-sim/` + `docs/evidence/qa-1*`; Phase 4 touches `.../OmnichannelCore/` + `services/pos-sim/` + `docs/evidence/qa-2*`. The boundary is the envelope contract, which is frozen at the end of Phase 2.

## Cross-cutting discipline

- **`journal.md`**: one entry per logical change, linking to its driver (ADR-NNNN and/or QA-N). João Roldão owns file integrity — checks weekly that no commit is unreferenced.
- **In-pair PR review**: every PR reviewed by the partner. Cheap and fast; serves as knowledge redundancy.
- **One cross-pair review per phase**: only on changes that touch the boundary contract (envelope, callback HTTP shape, log field names). Avoids review overhead while preserving boundary integrity.
- **Daily 15-min stand-up** (recommended): yesterday / today / blockers. Voice or async, doesn't matter.
- **Branching**: short-lived branches per task; merge to `main` daily where possible. Avoid week-long branches — half-time devs cannot afford big merges.

## Risk register

| Risk | Phase | Owner | Mitigation |
|------|-------|-------|------------|
| Scheduled-task publish lag exceeds 60 s under load | 2 | Pair A (João Roldão) | Measure on day 1 of Phase 2; if > 60 s, reduce task interval or batch size before Phase 3. |
| Circuit-breaker thresholds tuned for unit tests, not the live demo | 3 | Pair B (António) | Diogu runs the QA-1 scenario in demo-like conditions on day 14, not just day 12. |
| Structured-logging discipline drifts on error paths | 5 | Pair A + Pair B | 30-min cross-pair log review on day 16 catches missing IDs before evidence collection. |
| Docker Compose works on one machine, fails on another | 6 | Diogu | Fresh-clone smoke from a second machine on day 18. |
| One dev goes silent for several days (classes / illness) | any | Pair partner | Pair PR review means the partner has full context; daily stand-up surfaces blockers within 24 h. |

## Pre-freeze verification checklist (2026-05-31)

Tick every line before declaring the plan complete.

- [ ] `docker compose up` from a fresh clone reaches healthy state without manual steps.
- [ ] Placing an order through the storefront produces an `OmniOrderFulfillment` row in state `accepted` under normal conditions.
- [ ] **QA-1**: WMS-unavailable scenario shows checkout P95 ≤ 1.5× baseline; backlog drains ≤ 60 s after recovery; 0 orders pending > 5 min after recovery.
- [ ] **QA-2**: duplicate POS event rejected ≤ 50 ms; 0 duplicate fulfillment rows; stale `sourceVersion` ignored.
- [ ] **QA-3**: 10 sample orders link end-to-end via `OrderGuid` in the plugin admin view, resolution ≤ 3 clicks.
- [ ] **QA-4**: queue depth + retry count + DLQ size visible in RabbitMQ Management UI; pending-fulfillment count in plugin admin view; combined refresh ≤ 5 s.
- [ ] **QA-5**: outbox row written ≤ 100 ms after `OrderPlacedEvent`; 0 synchronous external HTTP calls in checkout trace.
- [ ] All five demo scenarios runnable from `docker compose up` (normal, WMS unavailable + recovery, POS legitimate, POS duplicate, POS stale).
- [ ] `docs/evidence/` populated with numbered measurements for every QA scenario.
- [ ] `journal.md` has an entry per logical change since Phase 1, each linking to an ADR and/or QA.
- [ ] ADRs updated to reflect Part 2 reality (notable: ADR-0007 write-through decision after measurement).
- [ ] `docs/setup.md` is sufficient for a stranger to run the demo.
- [ ] Part 2 slides drafted (design half + demo half).

## June 1–3 plan

- **Mon 2026-06-01** — Full rehearsal of the 15-min Part 2 presentation. All 4 devs participate. Time each section.
- **Mon 2026-06-01 (evening)** — ADR rejected-alternative defense rehearsal: each dev picks 3 ADRs, must answer "why was the rejected alternative rejected?" in 30 s. Rotate.
- **Tue 2026-06-02** — Second rehearsal. Slide polish. Small evidence tweaks only — no new features. Dry-run any demo scenario that felt unstable in rehearsal.
- **Wed 2026-06-03** — Presentation day. Pre-flight: `docker compose up` 30 min before, leave it warm.
