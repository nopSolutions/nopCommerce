# Code Change Journal

Append-only log of every code alteration during Part 2. The point is to keep changes **surgical**: every change is traceable to an ADR or QA scenario, and bounded in scope.

The rubric explicitly penalises "large amounts of generated code with little architectural justification." This journal is how we defend that we wrote only what was needed.

## How to use

- **One entry per change.** A change = one logical unit (a feature, a bugfix, a refactor) — not one commit.
- **Append at the top** (most recent first).
- **Link to ADR or QA**, or explain in one line why no link is needed (rare; usually means refactor or fixup).
- **Verification** is mandatory. If you cannot say how you verified the change, the change is not done.

## Entry template

```markdown
## YYYY-MM-DD — Short title

**Phase**: roadmap phase number (0–6) or "fixup".
**Driver**: ADR-NNNN and/or QA-N. If multiple, list them.
**Files**: `path/to/file.cs:line-range` (one or many).
**Change**: 1–3 sentences. What changed, not why — the driver field is why.
**Tradeoff/risk introduced**: one line if any; "none" if pure addition with no surface increase.
**Verification**: how proven (test name, manual demo step, measurement). Required.
```

## Rules

1. **No bulk changes without an entry.** A 200-line plugin scaffold is one entry; a 50-file lint fix is one entry with a glob in `Files`.
2. **Generated code counts.** AI-generated code gets a journal entry like any other, with verification evidence.
3. **If a change spans multiple ADRs, list all of them.** This surfaces unexpected coupling.
4. **If a change was reverted, leave the original entry and add a follow-up entry.** Do not delete history.
5. **If a change required updating an ADR, link the ADR change in the entry.** Plan vs reality lives in ADRs; the journal links them.

---

## Entries

<!-- Most recent first. -->

_No entries yet — Part 2 has not started. First entry will land in Phase 1 (plugin scaffolding)._
