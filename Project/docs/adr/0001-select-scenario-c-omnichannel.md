# ADR-0001 - Select Scenario C: Omnichannel Commerce Core

## Status

Accepted.

## Context

The assignment offers three scenarios. The team needs one focused architectural problem that can be demonstrated in a runnable environment with normal operation, degradation and recovery.

## Decision

Select **Scenario C - Omnichannel Commerce Core**.

The project will evolve nopCommerce so it remains the commerce core while integrating with external warehouse and store/POS capabilities.

## Consequences

- The mandatory demo will focus on order fulfillment, stock visibility and recovery from degraded external systems.
- The design can use existing nopCommerce order, catalog, warehouse and shipment concepts.
- The solution must represent at least WMS and POS capabilities, even if delivered as simulators.

## Rejected Alternatives

- Scenario A was rejected because federated business units would require more organization and identity modeling than the team can credibly implement for the final demo.
- Scenario B was rejected because regulated commerce would require careful policy and evidence modeling that is less directly supported by existing nopCommerce concepts.

