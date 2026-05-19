# ADR 12.1 — Evaluation Forces Matrix

**Class #12 — 04.02 Data Movement and Resilience Patterns**
**Exercise (slides 16-17): Build the evaluation forces matrix for ADR 12.1**

---

## 1. Problem recap

ADR 12.1 deals with **data ownership for ticket processing** in the Sysops Squad system. The squad is being decomposed into services and bounded contexts, and the database/development teams need to assign table ownership. The source material (Software Architecture: The Hard Parts, Ch. 9 + Appendix B) splits the problem into three cases; this ADR addresses two of them:

| Sub-case | What is happening | Concrete example |
|---|---|---|
| **Single ownership with read pressure** | one service writes the table; other services need to read it | `User Maintenance` owns the `Expert Profile` table; `Ticket Assignment` and `Ticket Completion` need profile data during ticket flows |
| **Joint ownership** | two services in the same domain write the same table | `Ticket Completion` writes completion data and `Survey Service` writes survey-sending and survey-result data into the same `Survey` table — table-splitting is not possible due to the table's structure |

The five candidate options across the two sub-cases:

| # | Option | Sub-case | What it tries to preserve |
|---|---|---|---|
| 1 | Direct table access | Single ownership | shortest read path |
| 2 | Owner API | Single ownership | strict ownership through the owning service |
| 3 | Replicated read model / cache | Single ownership | local read performance and availability |
| 4 | Common shared data domain | Joint ownership | both services can still write the shared table |
| 5 | Delegation to Survey Service | Joint ownership | one service writes; the other sends the data it needs |

## 2. Evaluation forces (slide 15)

The forces the matrix scores against, plus the damage the ADR is willing to absorb:

**Dominant forces**
- **Data ownership** — is there a single, identifiable writer for each table?
- **Bounded-context integrity** — do services stay independent across their context boundaries?
- **Data consistency** — how fresh and authoritative are the reads?
- **Read performance / availability** — can consumers read fast, even under load and partial failures?
- **Fault isolation** — does the failure of one service stay contained?
- **Operational supportability** — can the team change schemas, deploy, and operate the system without coordination cliffs?

**Accepted damage (force we knowingly weaken)**
- Latency through owner APIs
- Stale read models
- Shared schema governance
- Reliable publication and duplicate handling

> The slide is explicit: *"Do not average the row. Decide which forces dominate this scenario."* A single critical "low" can kill an option even if every other column is "high".

## 3. The matrix

Rows = options. Columns = the forces drawn from slide 17's skeleton (a re-projection of the slide-15 forces onto observable system properties).

Score scale per cell: **High** = the option preserves the force well; **Medium** = neutral / mixed; **Low** = the option actively damages the force. Ownership is reported as **clear** / **unclear**.

| Option | Deployability | Consistency | Operations | Fault isolation | Ownership | Supportability | Reversibility |
|---|---|---|---|---|---|---|---|
| **1. Direct table access** | High short-term — nothing extra to deploy | Fresh reads, but unsafe boundary (writes can occur from a reader) | Low until schemas change; then every schema change requires consumer discovery | Low — readers and writer share the same DB | **Unclear** — readers can mutate; ownership is implicit | Weak under change — hidden coupling explodes during refactors | Easy now, costly later — every consumer must be migrated off the schema |
| **2. Owner API** | Medium — consumers are coupled to the API contract and its versioning | Strong, authoritative reads — always live | Medium — API contracts, versioning, timeouts, fallbacks, rate limits | Low — owner outage blocks readers (latency / availability coupling) | **Clear** — the owner is the single source for both writes and reads | Strong — schema changes stay behind the API; no consumer discovery needed | Medium — contract changes are feasible; migration to events later is doable |
| **3. Replicated read model / cache** | High — readers have full deployment independence | Eventually consistent; can be stale, needs repair/replay | High — freshness checks, replay, repair, retention, cache invalidation | High — readers survive owner outage from their local copy | **Clear** if the owner is the publisher; muddled if the cache silently becomes authoritative | Medium — needs an event contract and observability around it | Low — pipeline + consumers + storage are hard to retire once in production |
| **4. Common shared data domain** | Low — pulls the decomposed system back toward a shared database | Strong — single store, no replication lag | High — coordinated schema change, shared permissions, shared regression risk | Low — shared blast radius (one DB outage takes both services down) | **Unclear** — joint writers, no single owner (the anti-pattern) | Weak — coordination cost between teams grows over time | Very low — hardest to undo; the shared boundary calcifies |
| **5. Delegation to Survey Service** | High — clean service boundaries preserved | Eventually consistent via event; payload is authoritative | Medium — needs outbox + inbox + idempotency + monitoring (well-known patterns) | High — services are independent; queue absorbs short outages | **Clear** — Survey Service is the sole writer of the `Survey` table | Strong if the event contract is honest and versioned | Medium — could be swapped for a synchronous API call without giving up ownership |

## 4. In-depth analysis of each option

### Option 1 — Direct table access

**Mechanism.** The read-only service opens a connection to the owner's database (or schema) and queries the table directly.

**Why it works.** It is the shortest possible read path: no extra hop, no replication lag, no contract. Deployment is also simple — there is nothing new to ship.

**Why it fails.**
- *Schema and ownership coupling stay hidden.* Because the reads are inside the database, the owner's team cannot tell who depends on which column. The owner cannot change the table independently.
- *Implicit write authority.* A "read-only" connection is a social contract, not a technical one. The boundary is unsafe; a future commit can add an `UPDATE` and nobody will notice until production.
- *Fault isolation is low.* Readers and writers share the database; outages and lock contention propagate.

**Operational burden.** Every schema change becomes a consumer-discovery exercise across teams. Migrations stall.

**Verdict for this ADR.** Ruled out by the verdict slide: *"read-only services in another bounded context do not access the owner database or schema directly."*

### Option 2 — Owner API

**Mechanism.** The owning service exposes a synchronous API (REST/gRPC) and consumers call it instead of touching the table. The owner remains the only process that knows the schema.

**Why it works.** Ownership becomes explicit and enforceable: there is one writer, one reader-of-truth, and one place where the schema is known. Refactors of the table do not leak out of the owning service.

**Why it fails.**
- *Latency coupling.* A read in the consumer becomes a network round-trip; tail latencies in the owner show up as tail latencies in the consumer.
- *Availability coupling.* If the owner is down, the consumer cannot read. This is the worst weakness for high-volume read paths.
- *Contract pressure.* As more consumers depend on the API, breaking changes become expensive; versioning becomes a permanent operational concern.

**Operational burden.** API contracts, versioning, timeouts, retries, circuit breakers, and fallback behavior all become part of the data-access design rather than the data-access design itself.

**Verdict for this ADR.** Strong default for the `Expert Profile` reads when read volume is moderate. It preserves ownership and bounded-context integrity, which are the dominant forces for ADR 12.1. The accepted damage ("latency through owner APIs") is exactly what this option pays.

### Option 3 — Replicated read model / cache

**Mechanism.** The owner emits change events (CDC, domain events, or both); each consumer maintains its own local read model (denormalized table, cache, materialized view) shaped for its query patterns.

**Why it works.** Reads are local — no cross-service latency, no availability coupling on the read path. Consumers keep running even if the owner is down. Different consumers can shape the read model differently without negotiating with the owner.

**Why it fails.**
- *Stale data.* The local copy is always slightly behind. If a consumer treats it as authoritative for a decision that requires "now" semantics, it produces wrong outcomes.
- *Repair complexity.* When events are missed, dropped, or reordered, the read model drifts. Replay, reconciliation, and retention policies must be designed and tested.
- *Ownership creep.* If the local read model gets used for writes ("just update it here, we'll fix it later"), ownership silently moves.

**Operational burden.** Event contracts, broker operations, replay tooling, repair playbooks, schema-evolution rules, freshness SLOs and monitoring all become part of the system.

**Verdict for this ADR.** Justified when the `Expert Profile` read pressure or availability requirement makes Option 2 untenable. Pays the accepted damage of "stale read models". Heavier than Owner API — pick it only when its specific strengths are needed.

### Option 4 — Common shared data domain

**Mechanism.** Place the `Survey` table in a data boundary that both `Ticket Completion` and `Survey Service` can write to. Both services keep direct write access.

**Why it works.** No new event, no new contract, no message bus. The current write shape is preserved. Both writers see fresh data immediately.

**Why it fails.**
- *Shared database coupling returns.* Decomposition is partially reversed; a schema change requires both teams to coordinate.
- *Joint ownership is the anti-pattern.* There is no single writer to point at when something goes wrong, and the table's invariants are owned by no one team.
- *Fault and deployment blast radius are shared.* An outage or a bad migration in the shared boundary affects both services.

**Operational burden.** Coordinated schema change, shared permissions, shared regression risk, and the cultural cost of two teams negotiating every change.

**Verdict for this ADR.** Rejected. It re-introduces the very coupling the decomposition is trying to remove; the dominant forces (ownership, bounded-context integrity) score Low.

### Option 5 — Delegation to Survey Service

**Mechanism.** `Ticket Completion` no longer writes the `Survey` table. When a ticket is completed, it publishes a `TicketCompleted` event that carries the data needed to start a survey. `Survey Service` consumes the event and inserts the survey record. The `Survey` table now has exactly one writer.

**Why it works.**
- *Single writer.* Ownership becomes explicit and unambiguous.
- *Bounded contexts stay clean.* Neither service reaches into the other's database.
- *Fault isolation.* A short outage of `Survey Service` is absorbed by the queue; `Ticket Completion` finishes its work and moves on.

**Why it fails.** All the failure modes are about the event itself:
- *Incomplete payload* — if the event lacks data the survey needs, you re-introduce a synchronous lookup.
- *Lost events* — a publication failure means a missing survey, silently.
- *Duplicates* — at-least-once delivery means the consumer can be called twice for the same completion.
- *Invisible side effect* — the survey starting from an event makes the workflow harder to trace if observability is not built in.

**Operational burden.** A reliable publication mechanism on the producer side, an idempotent consumption mechanism on the consumer side, and end-to-end monitoring of the handoff. This is exactly the work that ADR 12.2 (transactional outbox) and ADR 12.3 (inbox / idempotent consumer) address next.

**Verdict for this ADR.** Selected for the `Survey` table. The accepted damage ("reliable publication and duplicate handling") is the damage we are explicitly willing to pay — and the next two ADRs are how we pay it without losing data.

## 5. Which forces dominate, per sub-case

Per slide 15, the **dominant forces** for ADR 12.1 are *data ownership*, *bounded-context integrity*, and *operational supportability*. Different sub-cases stress them differently:

| Sub-case | Dominant forces | Accepted damage we are willing to pay | Eliminated by |
|---|---|---|---|
| Single ownership — `Expert Profile` reads | ownership, bounded-context integrity, supportability | latency through owner APIs **or** stale reads | Direct table access (Low on ownership and supportability) |
| Joint ownership — `Survey` table | ownership, bounded-context integrity, fault isolation | reliable publication + duplicate handling | Common shared data domain (Low on ownership, fault isolation, supportability) |

## 6. Verdict (matches slides 18-20)

1. **Single Table Ownership for Bounded Contexts.** Every table is assigned to the only service that writes and maintains it. Services outside the owner's bounded context cannot access that database or schema directly. → eliminates Option 1.
2. **Survey Service Owns the `Survey` Table.** The Survey Service becomes the only writer. `Ticket Completion` sends the survey data it has when it triggers the customer-survey process. → selects Option 5.

For the `Expert Profile` reads (Option 2 vs Option 3), the choice is contingent:
- Default to **Owner API** — it scores highest on the dominant forces (clear ownership, strong consistency, strong supportability) and pays the smallest accepted damage (a bit of latency).
- Escalate to **Replicated read model** only when read volume, availability requirements, or owner-outage tolerance make Option 2's latency/availability coupling unacceptable.

## 7. Consequence — what this ADR hands to the next two

Selecting Option 5 for the `Survey` table commits the system to the accepted damage of "reliable publication and duplicate-safe consumption". That damage is precisely the scope of:

- **ADR 12.2 — Transactional outbox.** Guarantees that the `TicketCompleted` event is published exactly when the local transaction commits, even under crashes.
- **ADR 12.3 — Inbox / idempotent consumer.** Guarantees that `Survey Service` processes each `TicketCompleted` event at most once in effect, even under at-least-once delivery.

This is why the agenda places these three ADRs in sequence: 12.1 chooses delegation, and 12.2 + 12.3 make delegation safe.
