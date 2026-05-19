# Delivery Roadmap

Authoritative phasing for Assignment 2. Phase 0 covers Part 1 (architecture checkpoint). Phases 1–5 mirror the migration table in [architecture-checkpoint.md §9](docs/part1/architecture-checkpoint.md#9-evolution-roadmap), each one delivering one ADD iteration in code. Phase 6 is the Part 2 evidence pack, demo, and final presentation.

| #   | Title                                  | Maps to                              | Status     |
|-----|----------------------------------------|--------------------------------------|------------|
| 0   | Architecture Checkpoint                | Part 1 (Wave 1 + Wave 2)             | In review  |
| 1   | Plugin scaffolding + tables            | Migration Stage 1                    | In review  |
| 2   | RabbitMQ + worker + normal flow        | Migration Stage 2 (Iter. 1 happy)    | Not started|
| 3   | Resilience under pressure              | Migration Stage 3 (Iter. 1 pressure) | Not started|
| 4   | Consistency: idempotent inbox + POS    | Migration Stage 4 (Iter. 2)          | Not started|
| 5   | Traceability: correlation + admin      | Migration Stage 5 (Iter. 3)          | Not started|
| 6   | Evidence pack + demo + presentation    | Part 2 final delivery                | Not started|

---

## Phase 0 — Architecture Checkpoint

- **Goal**: produce the Part 1 deliverable: justified framework choice, three ADD iterations, QA scenarios with measurable thresholds, ADRs with tradeoffs, migration table, feasibility spike.
- **Deliverables**:
  - [x] `docs/part1/architecture-checkpoint.md` (3 iterations, framework justification, Phase-F migration table, constraints-coverage table)
  - [x] `docs/part1/quality-attribute-scenarios.md` (5 SEI 6-part scenarios with numeric measures, prioritization grid)
  - [x] `docs/part1/diagrams.md` (DDD context map, C4 L1/L2/L3, runtime sequences)
  - [x] `docs/adr/0001…0008.md` (8 ADRs each with Tradeoffs and Rejected Alternatives)
  - [x] `docs/adr/template.md` (reusable shell)
  - [x] `docs/architecture.md` (current-state analysis with source-line citations)
  - [x] `docs/evidence/feasibility-spike.md` (experiment charter + findings)
- **Verification gate**: every "yes" in the verification list of the latest plan (`/home/roldao/.claude/plans/analyse-this-slides-and-peppy-octopus.md`).
- **Risks**: framework choice not defensible under questioning; QA measures not actually instrumentable in Part 2.

## Phase 1 — Plugin scaffolding + tables

- **Goal**: create `Nop.Plugin.Misc.OmnichannelCore` with the minimum schema to support Iterations 1–3, no integration logic yet.
- **Deliverables**:
  - [x] Plugin project under `nopCommerce/src/Plugins/` registered with the standard plugin lifecycle.
  - [x] Migrations creating `OmniOutboxMessage`, `OmniInboxMessage`, `OmniOrderFulfillment`, `OmniStockSyncState` tables (schemas defined per ADRs 0006/0007/0008).
  - [x] Empty admin views for outbox/projection (shells only).
  - [x] Plugin installs cleanly into a fresh nopCommerce DB.
- **Verification gate**: plugin builds; install/uninstall round-trip leaves DB clean; tables visible with correct columns.
- **Current evidence**: see [Phase 1 plugin scaffold evidence](docs/evidence/phase-1-plugin-scaffold.md). Code builds through Docker; install, table visibility and admin page visibility were confirmed locally. Uninstall DB validation is still required before marking the phase done.
- **Risks**: nopCommerce migration tooling not behaving on the active branch; .NET 10 SDK availability in the dev environment.

## Phase 2 — RabbitMQ + worker + normal flow (Iteration 1 happy path)

- **Goal**: prove the async path end-to-end with WMS in `normal` mode only — no pressure, no error injection.
- **Deliverables**:
  - [ ] `OrderPlacedEvent` consumer in plugin writes outbox row.
  - [ ] Scheduled task publishes pending outbox rows to RabbitMQ with publisher confirms.
  - [ ] Worker service consumes `commerce.order.placed.v1`, calls WMS sim, publishes `fulfillment.status.changed.v1` to plugin.
  - [ ] WMS simulator with `normal` mode only; POS simulator stub.
  - [ ] Docker Compose runs nopCommerce + SQL Server + RabbitMQ + worker + WMS sim + POS sim in one command.
- **Verification gate**: place an order through the storefront → fulfillment row exists in plugin with state `accepted`; checkout latency unchanged from baseline; no synchronous external HTTP on the checkout thread.
- **Risks**: scheduled-task lag larger than expected; RabbitMQ publisher confirm semantics misconfigured.

## Phase 3 — Resilience under pressure (Iteration 1 pressure work)

- **Goal**: deliver the assignment's mandatory pressure point and recovery behavior.
- **Deliverables**:
  - [ ] WMS sim modes: `slow`, `unavailable`, `contradictory` (mode toggle via env or admin endpoint).
  - [ ] Worker retry with exponential backoff + circuit breaker (Polly).
  - [ ] Dead-letter queue for poison messages.
  - [ ] Recovery behavior: backlog drain on circuit-breaker close.
  - [ ] Demo script: `normal → unavailable → recovery` with logs and queue state captured.
- **Verification gate**: QA-1 measures hit (checkout P95 ≤ 1.5× baseline during 30 s 503; backlog drain ≤ 60 s after recovery; 0 orders pending > 5 min after recovery). Go/no-go: **Go** on Iteration 1.
- **Risks**: circuit-breaker thresholds tuned for tests but not for demo; recovery time depends on backlog size.

## Phase 4 — Consistency: idempotent inbox + POS (Iteration 2)

- **Goal**: handle at-least-once delivery and stale POS updates without losing or duplicating state.
- **Deliverables**:
  - [ ] Inbox enforcement on every plugin callback endpoint.
  - [ ] `sourceVersion` comparison on stock updates; older versions ignored.
  - [ ] POS simulator with `duplicate` and `stale` modes.
  - [ ] Unit tests for `messageId` deduplication and `sourceVersion` staleness.
  - [ ] Demo script: duplicate POS event → ignored; stale POS event → ignored; legitimate update → applied.
- **Verification gate**: QA-2 measures hit (duplicate detected ≤ 50 ms; 0 duplicate fulfillment rows; older `sourceVersion` ignored). Go/no-go: **Go** on idempotent inbox; **Partial-go** on projection-only stock (write-through deferred).
- **Risks**: `sourceVersion` clock semantics broken by POS sim's clock model; inbox table growth not capped (acceptable for demo).

## Phase 5 — Traceability: correlation + admin (Iteration 3)

- **Goal**: make a delayed/recovering order self-explanatory from the admin view.
- **Deliverables**:
  - [ ] Standard envelope (`messageId`, `correlationId`, `eventType`, `occurredOnUtc`) on every message; producer/consumer enforce.
  - [ ] `OrderGuid` + `messageId` + `externalRequestId` on every log line and DB row in the integration path.
  - [ ] Plugin admin view: enter `OrderGuid` → see outbox row, MQ message ID, worker attempts, fulfillment state.
  - [ ] Worker logs structured with same IDs.
- **Verification gate**: QA-3 + QA-4 measures hit (100% of orders link end-to-end; resolution path ≤ 3 admin clicks; queue/retry/DLQ visible in single dashboard view). Go/no-go: **Go** on correlation propagation.
- **Risks**: admin view scope creep; structured-logging discipline drifts late in the project.

## Phase 6 — Evidence pack + demo + presentation

- **Goal**: produce the Part 2 final deliverable — a runnable demo, an evidence pack, an updated architecture report, and the live presentation.
- **Deliverables**:
  - [ ] `docs/evidence/` populated with: baseline measurement, Iteration 1/2/3 measurements vs QA scenario thresholds, demo screenshots/logs, known limitations, reproduction steps.
  - [ ] Updated `docs/architecture-report.md` (short, focused — scenario, drivers, ADD application, target arch, evolution path, limits).
  - [ ] Updated ADRs reflecting any Part 2 reality vs Part 1 plan (e.g., write-through decision after measurement).
  - [ ] Live demo script: 5 scenarios from `presentation-script.md` slide 10 (normal flow, WMS down, recovery, POS update, duplicate/stale).
  - [ ] Final presentation slides + 5-min technical defence prep.
- **Verification gate**: every demo scenario runs from `docker compose up` with no manual fix-ups; every QA scenario measure has a number in the evidence pack; team can answer rejected-alternative questions on every ADR within 30 s.
- **Risks**: Docker Compose drift between dev machines; running out of time on evidence collection vs implementation polish.

---

## Cross-cutting principles

- **No phase claims completion until its verification gate passes.** A go/no-go decision is recorded in the relevant iteration's checkpoint section.
- **Plan vs reality.** ADRs are updated when implementation reveals a deviation. The checkpoint stays authoritative for the design intent; ADRs carry the actual decisions.
- **Surgical changes.** Every code alteration in Phases 1–6 is recorded in [journal.md](journal.md) with its driver (ADR/QA) and verification. This is how we defend "selective evolution" against the rubric.
- **Selectivity.** Adding work outside the migration table requires explicit re-baselining. The rubric penalises scope inflation.
