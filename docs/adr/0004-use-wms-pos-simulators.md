# ADR-0004 - Use WMS and POS Simulators

## Status

Accepted.

## Context

The assignment allows surrounding systems to be represented by simulators, stubs or mocks when they preserve architectural pressure. The final demo needs reliable control over normal, slow, unavailable and contradictory states.

## Decision

Use two focused simulators:

- WMS simulator for fulfillment acceptance, delay, unavailability and contradiction.
- POS simulator for store-originated stock updates.

## Consequences

- The demo can deterministically show degradation and recovery.
- The team avoids spending effort installing and configuring full ERP/WMS/POS products.
- The architecture remains honest because the simulators still create the required external pressure.

## Tradeoffs

- Real-system quirks (auth, schema drift, vendor latency profiles) are not exercised — the contract may need tightening for production.
- Reviewers may question whether observed behavior generalises beyond the simulator; mitigated by versioned contracts.
- We invest in simulator code that has no production value beyond the demo.

## Rejected Alternatives

- Full ERPNext/Odoo/OpenBoxes setup was rejected as too much operational scope for the assignment.
- Pure static mocks were rejected because they would not demonstrate runtime pressure and recovery.

