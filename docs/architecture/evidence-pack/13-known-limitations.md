# Evidence Pack — Known Limitations

**Purpose:** Document the boundaries of the current design — what was deliberately left out, what degrades under specific conditions, and what was discovered during testing. These are not defects; they are honest statements about where the architecture stops and what the next iteration would address.

Limitations are organised by origin: design-time decisions, testing discoveries, and operational gaps.

---

## 1. Design-Time Accepted Limitations

These were identified during the ADD iterations and accepted deliberately. The rationale for each is in `08-risk-and-validation-plan.md` §5.

| Limitation | QAS affected | Impact on demo |
| --- | --- | --- |
| **Stock back-propagation OpenBoxes → nopCommerce not implemented.** Warehouse-originated corrections (returns, shrinkage, goods receipts) do not update `Product.StockQuantity` in nopCommerce. The `OpenBoxesStatusPollerTask` reflects fulfillment-order *status* (`ISSUED`) but not stock quantities. | QAS-2 (partial) | None — demo stock is controlled; divergence never surfaces |
| **Recurring-payment subscription path not protected by the AllocationGate.** `OrderProcessingService.cs:1982` calls `AdjustInventoryAsync` for subscription renewals without going through the decorator. | QAS-2 | Not triggered in the demo scenario |
| **HMAC payload signing not implemented for inbound webhook.** The `CarrierStatusPollerTask` uses polling rather than inbound webhooks; bearer-token auth is the operative mechanism where HTTP is used (POS API). | Security | Not a demo blocker; polling eliminates the inbound surface |
| **No shared outbound HTTP policy library.** The `CarrierTracking` plugin implements its own `HttpClient` with no shared Polly-based policy. The OpenBoxes Bridge has a process-level circuit breaker (ADR-011); other integration plugins remain timeout-only. A shared HTTP policy library is a natural future refactoring once more integrations exist. | Resilience | Bridge is circuit-breaker protected; carrier plugin is timeout-only |
| **Redis (ADR-010) designed but not implemented.** ADR-010 specifies Redis as a distributed lock and read-through cache for `CarrierStatusPollerTask` and `OpenBoxesStatusPollerTask`. The implementation is absent: both pollers read from the DB on every tick and there is no distributed lock to prevent concurrent nodes from calling external APIs multiple times per interval. | QAS-5 (at scale) | No impact on single-node demo deployment |
| **POS API authentication is static API-key only.** `AllocationSettings.PosApiKey` is a shared secret configured at plugin install time. No token rotation, no per-POS device identity, no JWT issuer. | Security | Sufficient for a demo environment; operational config in production |

---

## 2. Architectural Risks Accepted at Scale

These are not limitations for the demo but would require attention before production deployment at higher scale.

| Limitation | Condition that triggers it | Documented in |
| --- | --- | --- |
| **`OutboxDispatcherTask` is a single point of failure.** If all nopCommerce web nodes are down simultaneously, no outbox rows are dispatched. | Multi-node outage during message backlog | `08-risk-and-validation-plan.md` §2 |
| **Process-local mutex on order placement.** `OrderProcessingService` uses a `Mutex` — per-process. Two horizontally-scaled nopCommerce nodes could each accept an order for the same customer simultaneously. The AllocationGate prevents *inventory* oversell, but not duplicate orders. | Horizontal scale-out (> 1 nopCommerce node) | `08-risk-and-validation-plan.md` §2 |
| **Bridge dedup table lost if Docker volume is destroyed.** The `processed_orders.db` file lives on the `openboxes_bridge_data` named volume. Running `docker compose down -v` destroys the volume; the next restart processes all messages as new. OpenBoxes may or may not detect the duplicate depending on its own state. | `docker compose down -v` or volume loss | `08-risk-and-validation-plan.md` §2 |

---

## 3. Operational Gaps

These are absent features that would be needed to operate the system in production but do not affect the architectural correctness of the demo.

| Gap | Impact |
| --- | --- |
| **DLQ replay tooling** is not automated. ADR-011's circuit breaker prevents transient bridge failures from reaching the DLQ — the DLQ now contains only genuine poison messages. For those, manual republishing via the RabbitMQ Management Console is the only path. | Reduced: transient outage messages no longer reach the DLQ |
| **Outbox table unbounded growth.** `OutboxMessage` rows with `Status=Sent` accumulate with no cleanup task. Not a functional issue on a short-lived demo environment; on a long-running instance the table grows indefinitely. | DB storage over time |
| **Rejected web-order DB pollution.** A checkout that fails the AllocationGate leaves an `Order` row in the DB with `Success=false`. The row is not visible to the customer but accumulates in the `Order` table. | Table noise; no functional impact |

---

## 4. Limitations Discovered During Testing

These were not anticipated in the pre-test risk plan. They surfaced during the measurement runs; each subsection below names the measurement where the finding first appeared. Full measurement records are in `11-measurements.md`.

### Two-plugin dependency for POS API

**Discovered during:** M3 setup (POS reserve calls required both plugins active).

**Finding:** The POS HTTP adapter was extracted from `Nop.Plugin.Inventory.AllocationGate` into a new plugin `Nop.Plugin.Integration.Pos` as a separation-of-concerns refactoring. Both plugins must be installed and active for the POS API to function. If `Integration.Pos` is installed but `AllocationGate` is not, the DI registration for `IAllocationGate` is absent and the controller fails to resolve its dependency.

**Operational impact:** A fresh nopCommerce installation requires installing both plugins explicitly. Forgetting one silently breaks the POS channel.

**Mitigation:** Both plugins are registered in the solution and compiled into the Docker image. Installation via the admin panel is a one-time step per environment that persists across restarts (DB volume is retained).

---

### Bridge auto-provisioning of destination location, products, and categories has no ADR

**Discovered during:** M1 first run (FK constraint failure in OpenBoxes; bridge extended to self-provision).

**Finding:** The bridge (`VerdeMart.OpenBoxesBridge`) was extended to auto-create the OpenBoxes destination location ("VerdeMart Store"), product categories, and product records on demand if they do not exist — based on the `OrderPlacedMessage` SKU. This extension is not documented in any ADR and was not in the original ADR-007 scope.

**Architectural implication:** The bridge now owns master data provisioning responsibility in OpenBoxes in addition to its original fulfillment-order creation role. This broadens the bridge's boundary beyond what ADR-007 scoped it to. The lazy-creation approach eliminates the need for manual OpenBoxes seeding, which is operationally convenient, but introduces implicit coupling between the bridge's product-creation logic and the OpenBoxes data model.

**Status:** Accepted and working. The decision is not governed by any formal ADR; it represents an undocumented boundary expansion relative to ADR-007.

---

## 5. What the Architecture Does Not Cover

These are explicitly out-of-scope items from the chosen scenario. They are not limitations of the design — they are deliberate scope cuts documented in `01-scenario.md` and the bounded-context model.

| Out of scope | Reason |
| --- | --- |
| ERPNext financial integration | Declared as a surrounding system in `01-scenario.md`; not implemented. The `verdemart.orders` exchange supports additional consumers via queue binding — ERPNext would require only a new queue and a bridge service, no publisher change. |
| Keycloak SSO across channels | Identity federation is a generic subdomain; the architecture supports it via the plugin boundary but does not implement it. |
| POS commerce engine | POS is an external system. The architecture exposes the allocation API to POS; the POS-side commerce engine is out of scope. |
| Customer profile cross-channel synchronisation | Documented in `03-bounded-contexts.md` as a known tension. Out of scope for the assignment. |
| Meilisearch catalogue freshness | Generic subdomain, low priority per `01-scenario.md`. Not implemented. |
