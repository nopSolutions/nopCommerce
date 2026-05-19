# ADR-0005 - No Shared Database Across Service Boundaries

## Status

Accepted.

## Context

The assignment explicitly forbids using a shared database as a shortcut across extracted service boundaries. The omnichannel worker must not become coupled to nopCommerce internals.

## Decision

The worker, WMS simulator and POS simulator must not read or write nopCommerce database tables.

All cross-boundary communication happens through RabbitMQ events and plugin HTTP endpoints.

## Consequences

- Service boundaries are explicit and defendable.
- Integration behavior can be tested through contracts rather than database side effects.
- Some data duplication is required in projections and idempotency records.

## Tradeoffs

- Projection state can drift from core stock — visible drift is the acceptable cost (ADR-0007).
- Cross-boundary queries that would be one SQL join become two service calls or a projection lookup.
- We pay storage cost for inbox/outbox/projection tables that would be unnecessary in a shared-DB shortcut.

## Rejected Alternatives

- Giving the worker SQL access to nopCommerce was rejected because it hides coupling and violates the assignment constraint.
- Sharing plugin tables with the worker was rejected for the same reason.

