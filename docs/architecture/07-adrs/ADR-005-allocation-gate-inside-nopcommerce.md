# ADR-005 — Allocation Gate Hosted Inside nopCommerce

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-28 |
| Produced by | ADD Iteration 3 — Step 6 (driver: QAS-2 Consistency) |

## Context

QAS-2 requires that exactly one of two simultaneous orders for the last unit succeeds, and that the loser is rejected within the same request cycle. The two channels are nopCommerce web checkout and the in-store POS. `03-bounded-contexts.md` names OpenBoxes as authoritative for **physical** stock truth, but does not settle where the **operational allocation** decision should live for the omnichannel flow.

`02-current-state.md` confirms that today there is no atomic hold mechanism and no enforcement at checkout: two concurrent orders for the last unit can both succeed, driving stock negative. Iteration 2 Step 7 carried forward two candidate concepts to evaluate here: pessimistic DB row lock with serializable transaction, and optimistic concurrency with row version + retry. QAS-3's "under 3 s" checkout budget and QAS-2's "same request cycle" constraint narrow the viable mechanisms.

## Decision

Host the allocation gate inside nopCommerce. The gate operates on `ProductWarehouseInventory` rows under a pessimistic database row-level lock, evaluating effective availability as `StockQuantity − SUM(active reservations)` and either decrementing or returning a structured failure synchronously.

The bounded-contexts model is preserved by reframing the two roles: **OpenBoxes** is authoritative for physical stock truth (what is on the shelf); **nopCommerce** is authoritative for operational allocation arbitration across channels. The two reconcile via the existing `inventory.adjusted` event flow declared in `03-bounded-contexts.md`.

The gate is reached by web checkout via a decorator over `IProductService.AdjustInventoryAsync`, and by POS via a new HTTP endpoint (recorded as ADR-006). A new `ProductReservation` table holds time-bounded reservation rows; the legacy `ProductWarehouseInventory.ReservedQuantity` field is left untouched so the new behaviour is reversible.

## Rejected Alternatives

**Optimistic concurrency with row version + retry.** Re-issue the read–decrement–write on row-version conflict.
*Rejected:* under contention on a popular last unit, multiple retries inside the same request consume the QAS-3 latency budget; pessimistic locking gives a directly bounded rejection path.

**OpenBoxes as the synchronous allocation authority called by both web and POS.**
*Rejected:* (a) the OpenBoxes API capability for synchronous allocation reservation is uncertain — open at Iter 3 Step 1 until a feasibility spike confirms; (b) every checkout becomes a network hop into an external system, hostile to QAS-3; (c) it places OpenBoxes uptime on the critical path of every sale, the coupling Iterations 1 and 2 worked to remove elsewhere.

**Redis distributed lock service.**
*Rejected:* introduces new infrastructure for a property already achievable inside the nopCommerce database. The brief explicitly warns against "too many technologies with shallow purpose".

## Consequences

- QAS-2 becomes structurally satisfiable: row lock + reservation-aware availability prevents oversell across channels.
- nopCommerce gains an explicit operational role beyond commerce — the allocation arbiter — that must be defended in the bounded-contexts narrative.
- A new table (`ProductReservation`) is introduced with TTL semantics; row growth is bounded by reservation lifetimes and a release task.
- Lock granularity is per-product row; throughput on the most popular product is bounded by lock-and-release time. Acceptable at VerdeMart's scale.
- The web checkout's existing try/catch in `OrderProcessingService.PlaceOrderAsync` (verified at line 1634) means a rejected order leaves an `Order` row behind with `Success=false`. Operational cleanup of these rows is recorded as a residual concern for Step 7.
- The `IProductService` decorator must be wired via descriptor swap on `IServiceCollection` because `IDependencyRegistrar` does not exist in this nopCommerce version and Autofac is conditional (`UseAutofac` flag, `Nop.Web/Program.cs:26-29`); the descriptor swap works in both DI configurations.
