# Architectural Evolution of nopCommerce

This folder contains the architecture artefacts for Assignment 2. Scenario C — Omnichannel Commerce Core. Method: ADD primary, with ACDM go/no-go and ADM Phase-F migration borrowed.

**Start here**: [roadmap.md](../roadmap.md) — seven-phase delivery plan from checkpoint to final demo.

**During Part 2**: every code change goes in [journal.md](../journal.md) with its driver (ADR/QA) and verification.

**Current implementation evidence**: [Phase 1 plugin scaffold](evidence/phase-1-plugin-scaffold.md) records the code scope, schema created, exclusions, and remaining runtime verification.

## Part 1 checkpoint

- [Architecture checkpoint](part1/architecture-checkpoint.md) — primary artefact, organised by 3 ADD iterations.
- [Quality attribute scenarios](part1/quality-attribute-scenarios.md) — five SEI 6-part scenarios with numeric measures.
- [Context map and C4 diagrams](part1/diagrams.md)
- [Current-state analysis](architecture.md) — pressure points feeding Iteration 1.
- [Presentation script](part1/presentation-script.md) — to be revised after content lock.
- [Feasibility spike (experiment charter)](evidence/feasibility-spike.md)
- [ADRs](adr/) — **10 ADRs**, all Accepted. Each ADR documents Status, Context, Decision, Consequences, Tradeoffs, and Rejected Alternatives; cross-cutting decisions (ADR-0009 plugin+worker boundary, ADR-0010 structured-log observability) additionally record Triggers to revisit. Use [template.md](adr/template.md) for new ADRs.

## Final delivery placeholders

The Part 1 decision is to implement a focused omnichannel evolution later:

- nopCommerce remains the commerce core.
- Fulfillment and stock propagation move through asynchronous messaging.
- WMS and POS are represented by simulators for the final demo.
- Failure and recovery are part of the runtime evidence, not only documentation.
