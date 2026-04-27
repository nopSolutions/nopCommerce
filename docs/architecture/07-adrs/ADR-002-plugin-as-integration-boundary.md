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

## Consequences

- Integration can be enabled or disabled from the admin panel
- Each bounded context gets its own plugin, keeping concerns separated
- Plugin boundaries enforce the rule that the commerce core does not know about external systems
