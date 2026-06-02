# ADR-002 — Plugin Architecture as Integration Boundary

| | |
|---|---|
| Status | Accepted |
| Date | 2026-04-27 |
| Produced by | ADD Iteration 1 — Step 6 (driver: QAS-1 Reliability) |

## Context

nopCommerce core must not be modified. The integration logic must be addable and removable without touching the commerce engine.

## Decision

All integration code lives inside nopCommerce plugins. Plugins use `IConsumer<T>` to react to domain events and `INopStartup` to register their services. The core has no reference to any plugin.

## Rejected Alternatives

**Direct modification of the nopCommerce core.** Embedding integration code directly into the core source would allow any integration pattern but couples the commerce engine's upgrade path to every integration change. *Rejected:* violates the hard constraint that the commerce core must remain unmodified; every nopCommerce version bump would require re-applying the integration patch on top of upstream changes.

**Direct method call from the order processing service to the integration code.** Instead of reacting to events via `IConsumer<T>`, the integration could be invoked directly by the checkout service. *Rejected:* the core would then hold a compile-time reference to the integration code, breaking the decoupling boundary; the plugin could no longer be removed without modifying the core. The `IConsumer<T>` event subscription model keeps the core entirely unaware of the plugin.

## Consequences

- Integration can be enabled or disabled from the admin panel
- Each bounded context gets its own plugin, keeping concerns separated
- Plugin boundaries enforce the rule that the commerce core does not know about external systems
