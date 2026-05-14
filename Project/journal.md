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

## 2026-05-14 — Phase 1 schema tightening (indexes + correlation columns)

**Phase**: 1 (follow-up to Varela's plugin scaffold; closes the four "Minor" items from the same-day review entry below).
**Driver**: ADR-0003 (outbox publisher scan path), ADR-0006 (sourceVersion lookup path), ADR-0008 (3-ID correlation on every artefact in the integration path).
**Files**:

- `nopCommerce/src/Plugins/Nop.Plugin.Misc.OmnichannelCore/Data/Migrations/SchemaMigration.cs` (composite index Up + matching Down).
- `nopCommerce/src/Plugins/Nop.Plugin.Misc.OmnichannelCore/Data/Mapping/Builders/OmniOutboxMessageBuilder.cs` (index `StatusId`).
- `nopCommerce/src/Plugins/Nop.Plugin.Misc.OmnichannelCore/Data/Mapping/Builders/OmniStockSyncStateBuilder.cs` (drop redundant single-column `ProductId` index; composite covers it).
- `nopCommerce/src/Plugins/Nop.Plugin.Misc.OmnichannelCore/Domains/OmniInboxMessage.cs` (add nullable `OrderGuid`).
- `nopCommerce/src/Plugins/Nop.Plugin.Misc.OmnichannelCore/Data/Mapping/Builders/OmniInboxMessageBuilder.cs` (map + index `OrderGuid`).
- `nopCommerce/src/Plugins/Nop.Plugin.Misc.OmnichannelCore/Domains/OmniOrderFulfillment.cs` (add nullable `CorrelationId`).
- `nopCommerce/src/Plugins/Nop.Plugin.Misc.OmnichannelCore/Data/Mapping/Builders/OmniOrderFulfillmentBuilder.cs` (map + index `CorrelationId`).

**Change**: indexes Phase 2's outbox-publisher scan (`WHERE StatusId = Pending`) and Phase 4's sourceVersion lookup (`WHERE ProductId = @p AND WarehouseId = @w`). Adds the missing 3-ID propagation columns (`OrderGuid` on inbox, `CorrelationId` on fulfillment) so ADR-0008 holds on every artefact in the integration path.

**Tradeoff/risk introduced**: `SchemaMigration.cs` is amended in place rather than via a new migration. Anyone who installed the prior schema must uninstall + reinstall to pick up the new columns and index. Acceptable because Phase 1 uninstall validation is still open and only Varela has a local install (the other three devs have never installed). Documented here so the audit trail is honest about why the timestamp `2026/05/12 10:00:00:0000000` carries different content than the day-of commit.

**Verification**: build through Docker as in Varela's 2026-05-12 entry; reinstall the plugin from a clean DB (`docker compose down -v` first); confirm via DBeaver that `OmniInboxMessage` has `OrderGuid` column, `OmniOrderFulfillment` has `CorrelationId` column, `OmniOutboxMessage.StatusId` is indexed, and `IX_OmniStockSyncState_ProductId_WarehouseId` exists. The same run **also closes the open Phase 1 uninstall DB validation gate**: uninstall the plugin, confirm all four `Omni%` tables are dropped and the index is gone.

## 2026-05-12 — Phase 1 plugin scaffold review (audit, no code change)

**Phase**: 1 (review of Varela's commits `131cb44` + `f3f9e80`, merged in `ac28b69`).
**Driver**: ADR-0002 (monolith retention), ADR-0005 (no shared DB), ADR-0006 (idempotency), ADR-0007 (projection-first stock), ADR-0008 (3-ID correlation); cross-cutting principle "surgical changes".
**Files**: read-only review of `nopCommerce/src/Plugins/Nop.Plugin.Misc.OmnichannelCore/**` (22 files, ~1,200 lines).
**Change**: none — this is a documented audit. Findings below.
**Tradeoff/risk introduced**: none.
**Verification**: file-by-file read against principles checklist.

### Surgical assessment — PASS

- 0 lines modify nopCommerce core libraries (`Nop.Services`, `Nop.Data`, `Nop.Core`, `Nop.Web`). Only addition outside the plugin directory is the 15-line `NopCommerce.sln` project-reference entry.
- ADR-0002 preserved: commerce state ownership stays in nopCommerce; plugin adds parallel storage.
- ADR-0005 preserved: plugin uses its own four tables; no JOINs across the boundary.
- No external HTTP calls. No domain-event consumers wired to `OrderPlacedEvent` (correctly Phase 2 work). No business logic. No scheduled tasks. The Phase 1 surface area is the minimum needed to land the admin shell.

### Scope adherence — PASS

Maps cleanly to Phase 1 deliverables in `roadmap.md`:

- Plugin project registered with standard lifecycle (`OmnichannelCorePlugin.cs:10` extends `BasePlugin, IMiscPlugin`).
- Migration creates four tables and reverses cleanly (`Data/Migrations/SchemaMigration.cs:18-22` Up, `:29-32` Down).
- Admin shell present (`Controllers/OmnichannelCoreController.cs:34` `Configure` action; `Views/Configure.cshtml:1`).
- Install path manually verified per Varela's 2026-05-12 entry; uninstall DB round-trip remains open.

### Code-quality notes — mostly PASS

**Strong**:

- `OmnichannelCorePlugin.InstallAsync`/`UninstallAsync` are pure base passthroughs (`OmnichannelCorePlugin.cs:41-53`). nopCommerce's `BasePlugin` automatically triggers FluentMigrator Up/Down via the `[NopMigration(..., MigrationProcessType.Installation)]` attribute — schema lifecycle is correctly delegated.
- `Services/EventConsumer.cs:9` extends `BaseAdminMenuCreatedEventConsumer` (admin-menu-only) and NOT `IConsumer<OrderPlacedEvent>` — the naming is slightly misleading but the choice is right: Phase 2 owns the domain-event hook.
- `Services/OmnichannelCoreService.cs` is read-only (four `CountAsync()` methods). No writes, no external calls.
- `Controllers/OmnichannelCoreController.cs` is admin-only (`[AuthorizeAdmin]`, `[Area(AreaNames.ADMIN)]`, `[CheckPermission(MANAGE_PLUGINS)]`), single `Configure` GET action, no write endpoints.
- Domain entities are forward-compatible with future phases: `OmniOutboxMessage` already has `MessageId` (Guid), `CorrelationId`, `OrderGuid`, `RetryCount`, `LastError`, `PublishedOnUtc`, `NextAttemptOnUtc`, `Status` — Phases 2 and 3 will not need schema additions.
- Status enums use sparse integer values (10, 20, 30…) per nopCommerce convention — leaves room for intermediate states without migration.
- `OmnichannelCoreNameCompatibility.cs:14-20` locks plugin table names so future entity renames don't break installed instances.

**Minor — defer to Part 2 measurement, not blocking**:

- `OmniStockSyncStateBuilder` does not declare a `(ProductId, WarehouseId)` composite index. Phase 4 stock-update path will scan by that key. Acceptable for demo volumes; flag if Phase 4 measurement shows lookup cost.
- `OmniOutboxMessageBuilder` does not index `StatusId`. The Phase 2 outbox publisher will scan `WHERE StatusId = Pending`. Defer until Phase 2 measures.
- `OmniInboxMessage` has no `OrderGuid` column. For order-related callbacks (e.g., `fulfillment.status.changed.v1`) Phase 5 traceability will join inbox → fulfillment by `MessageId`/`CorrelationId` to recover order context. Workable; flagged for Phase 5 review.
- `OmniOrderFulfillment` has `MessageId` but not `CorrelationId`. QA-3's primary lookup path (start from `OrderGuid`) still works directly. Reverse lookup (start from `messageId` → find order) requires one extra join via inbox. Acceptable.
- View text in `Views/Configure.cshtml` is hard-coded English (no `@T(...)` localization keys). Acceptable for an academic demo; would not ship to a localized production.

**Follow-up landed same day**: items 1–4 above were judged too cheap to defer and addressed in the **2026-05-14 — Phase 1 schema tightening** entry directly above. Item 5 (localization) is genuinely demo-only and remains skipped.

### Verification remaining

- Phase 1 verification gate ("plugin builds; install/uninstall round-trip leaves DB clean") is not yet met — install + table visibility + admin page were confirmed, **uninstall DB validation is pending**. Phase remains `In review` in `roadmap.md` until that gate passes. Owner of the uninstall test: Roldão or Varela, on the next Docker run.

### Verdict

The merged Phase 1 work is well-scoped, ADR-aligned, and free of scope creep. The four minor items above are deferrable to their natural phases. No rework recommended. **Phase 1 progresses to `In review` cleanly; pending uninstall DB validation before `Done`.**

## 2026-05-12 — Phase 1 plugin scaffold

**Phase**: 1.
**Driver**: ADR-0003, ADR-0004, ADR-0006, ADR-0007, ADR-0008; QA-1 resilience, QA-2 consistency, QA-3 traceability.
**Files**: `nopCommerce/src/Plugins/Nop.Plugin.Misc.OmnichannelCore/**`, `nopCommerce/src/NopCommerce.sln`, `roadmap.md`, `docs/evidence/phase-1-plugin-scaffold.md`, `docs/README.md`, `journal.md`.
**Change**: Added the `Nop.Plugin.Misc.OmnichannelCore` plugin scaffold, schema migration, domain entities, mapping builders, admin shell and solution registration for the Phase 1 foundation.
**Tradeoff/risk introduced**: Runtime plugin install/uninstall is not yet proven against a fresh nopCommerce database.
**Verification**: `jq` validates `plugin.json`; `rg` confirms the plugin, solution and evidence references; host `dotnet --info` fails with `command not found`; `docker build --target build -t nopcommerce-omni-phase1-check .` succeeds from `nopCommerce/` with 3 existing warnings and 0 errors. Local install, DBeaver table visibility and admin page visibility were confirmed; uninstall DB validation remains open.
