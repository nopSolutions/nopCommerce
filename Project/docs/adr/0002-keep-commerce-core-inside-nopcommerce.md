# ADR-0002 - Keep Commerce Core Inside nopCommerce

## Status

Accepted.

## Context

nopCommerce already owns checkout, order processing, catalog, product stock, warehouses and shipments. The assignment explicitly warns against rewriting nopCommerce into microservices.

## Decision

Keep order, checkout, catalog and core stock behavior inside the nopCommerce monolith.

Add omnichannel behavior through a plugin and one independently deployable worker service.

## Consequences

- The final implementation remains selective and defensible.
- Existing nopCommerce services remain the source of truth for commerce state.
- The worker coordinates external systems but does not own orders or products.

## Rejected Alternatives

- Extracting order service and catalog service was rejected because it would create high migration risk and weak scope control.
- Modifying `OrderProcessingService` directly was rejected because the plugin/event seam is enough for this scenario.

