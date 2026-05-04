# ADR-0009 - Reject Microservices Extraction of Order/Catalog

## Status

**Rejected.**

## Context

A common architectural response to "make nopCommerce a commerce core" is to extract `Order` and/or `Catalog` services from the monolith. This was considered as a primary structural move and rejected. Recording it explicitly so it does not quietly return as a proposal during Part 2 implementation.

The assignment brief itself states: *"This is not a 'rewrite nopCommerce into microservices' assignment."* The architectural problem of Scenario C is **coordination under degradation**, not service decomposition.

## Decision

We will **not** extract `Order`, `Catalog`, or any other nopCommerce service from the monolith. Commerce state ownership stays inside nopCommerce (ADR-0002). All omnichannel coordination lives in a plugin + worker boundary outside the monolith, communicating through events (ADR-0003) and HTTP callbacks across an explicit boundary (ADR-0005).

## Reasons

- Scenario C is satisfied by adding integration discipline (outbox + worker + idempotency), not by relocating order state.
- Extraction implies data migration, dual-writing during cutover, admin-UI rebuild, and re-validation of every plugin that touches `IOrderService` / `IProductService`. None of this fits the assignment scope.
- The rubric explicitly penalises *"impress by increasing the number of technologies rather than the quality of the design."*
- Existing nopCommerce extension seams (plugin events, scheduled tasks) are sufficient — proven by the feasibility spike.

## Consequences

- Order, Catalog, and Stock remain in the nopCommerce DB owned by nopCommerce services.
- The plugin + worker design treats nopCommerce as a *customer/supplier* peer over versioned event contracts, not as a substrate to be carved up.
- Architectural discussion focuses on integration quality (resilience, consistency, traceability) rather than decomposition mechanics.

## Tradeoffs

- We cannot scale `Order` or `Catalog` independently of the rest of nopCommerce.
- Future moves like CQRS read-model split or independent-deploy of order services are blocked until this decision is revisited.
- Some performance characteristics of nopCommerce (in-process plugin calls, shared SQL connection pool) are inherited as constraints rather than challenged.

## Triggers to revisit

Reopen this decision if any of the following becomes true:

- `Order` or `Catalog` becomes a measured throughput or latency bottleneck that cannot be addressed within the monolith.
- An independent-deploy requirement emerges (e.g., the team grows large enough that a single release cadence costs more than extraction would).
- A new scenario (regulatory, federation, multi-tenant) makes monolith ownership of order state unsafe.
- A separate team takes ownership of one of the subdomains and needs hard isolation.
