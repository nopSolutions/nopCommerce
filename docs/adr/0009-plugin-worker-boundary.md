# ADR-0009 - Plugin + Worker Boundary as Integration Pattern

## Status

Accepted.

## Context

Scenario C asks nopCommerce to act as a commerce core while integrating with WMS and POS systems. Several structural patterns were considered, ranging from extracting `Order`/`Catalog` as microservices to handling all integration inside the monolith. The chosen pattern must add integration capability without rewriting nopCommerce, must allow independent deployment of the integration logic, and must keep the boundary defensible against scope inflation.

The assignment brief itself states: *"This is not a 'rewrite nopCommerce into microservices' assignment."* The architectural problem of Scenario C is **coordination under degradation**, not service decomposition.

## Decision

All omnichannel integration code lives in two units **outside** the nopCommerce core libraries:

- A nopCommerce plugin (`Nop.Plugin.Misc.OmnichannelCore`) that owns event consumption (`OrderPlacedEvent`), outbox/inbox/projection tables, and internal callback HTTP endpoints.
- An **independently deployable Worker service** that consumes RabbitMQ messages, calls WMS, and posts back to the plugin.

`Nop.Services`, `Nop.Data`, and `Nop.Core` remain unchanged. The plugin and worker communicate with nopCommerce via in-process domain events (inbound) and HTTP callbacks (outbound); they communicate with each other via RabbitMQ and HTTP (no shared DB — see ADR-0005). nopCommerce is treated as a *customer/supplier* peer over versioned event contracts (`commerce.order.placed.v1`, `pos.stock.changed.v1`, `fulfillment.status.changed.v1`), not as a substrate to be carved up.

## Consequences

- Commerce state (orders, catalog, stock) stays in the nopCommerce DB owned by nopCommerce services. ADR-0002 is preserved.
- The plugin is the only code that talks to both nopCommerce internals and the integration boundary, which makes the boundary auditable in one place.
- The worker can be deployed, restarted, and scaled independently from nopCommerce.
- Architectural discussion focuses on integration quality (resilience, consistency, traceability) rather than decomposition mechanics.
- Existing nopCommerce extension seams (plugin events, scheduled tasks) are sufficient — proven by the feasibility spike.

## Tradeoffs

- We cannot scale `Order` or `Catalog` independently of the rest of nopCommerce; they inherit nopCommerce's release cadence and runtime characteristics.
- Future moves like CQRS read-model split or independent-deploy of order services are blocked until the integration boundary is re-evaluated.
- Some performance characteristics of nopCommerce (in-process plugin calls, shared SQL connection pool) become implicit constraints rather than challenged.
- Moving more behavior into the plugin over time risks the plugin growing into a second monolith; this must be watched in review.

## Rejected Alternatives

- **Extract `Order`/`Catalog` as microservices.** Rejected because it requires data migration, dual-writing during cutover, admin-UI rebuild, and re-validation of every plugin that touches `IOrderService` / `IProductService`. None of this fits the assignment scope, and the rubric explicitly penalises *"impress by increasing the number of technologies rather than the quality of the design."*
- **Fat plugin only (no worker).** Rejected because retry, exponential backoff, and circuit-breaker behaviour need a process that is not the web request thread. Hosting all of that inside the nopCommerce process makes nopCommerce itself responsible for WMS availability — the exact failure mode this scenario is solving.
- **Anti-corruption layer inside `Nop.Services` directly.** Rejected because it requires modifying core libraries, which contradicts ADR-0002 and removes the explicit boundary that lets us reason about coordination separately from commerce.

## Triggers to revisit

Reopen this decision if any of the following becomes true:

- `Order` or `Catalog` becomes a measured throughput or latency bottleneck that cannot be addressed within the monolith.
- An independent-deploy requirement emerges (e.g., the team grows large enough that a single release cadence costs more than extraction would).
- A new scenario (regulatory, federation, multi-tenant) makes monolith ownership of order state unsafe.
- A separate team takes ownership of one of the subdomains and needs hard isolation.
