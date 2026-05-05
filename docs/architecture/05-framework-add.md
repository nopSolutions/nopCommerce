# Chosen Framework: ADD (Attribute-Driven Design)

## Why ADD Fits This Case

The work done in this document maps directly onto ADD's inputs and outputs:

| ADD concept | What it corresponds to in this project |
|---|---|
| Architectural drivers | The 7 pressure points identified in the nopCommerce analysis |
| Quality attribute scenarios | QAS-1 through QAS-5 |
| Decomposition elements | The 6 bounded contexts (Commerce, Warehouse, ERP, POS, Shipping, Identity) |
| Architectural tactics | Durable queues, async integration, outbox pattern, webhook ingestion |
| Constraints | nopCommerce as the fixed commerce core; RabbitMQ as the broker |

ADD is appropriate here for three specific reasons.

**First, the scenario is quality-attribute-driven.** The three strategic goals - reliable commerce core, cross-channel visibility, and resilience to degraded external systems - are not functional requirements. They are quality attributes. ADD is the only one of the three methods that treats quality attributes as the primary input to every design decision.

**Second, the scope is bounded.** VerdeMart is integrating a defined set of systems for a defined set of use cases. ADM is designed for enterprise-wide transformation programmes spanning years. ACDM introduces process overhead that adds no value when one or two people are designing and building the same system. ADD is lean enough to match the project scale.

**Third, ADD is traceable by construction.** Every architectural decision in ADD exists because a quality attribute scenario demanded it:

- QAS-1 demands durable queues → ADR-001: RabbitMQ as message broker
- QAS-3 demands async ERPNext integration → ADR-002: Outbox pattern
- QAS-1 + QAS-3 demand the integration stays outside the monolith core → ADR-003: Plugin architecture

This traceability makes the reasoning behind each design choice explicit and defensible.

## Why Not ADM/TOGAF or ACDM

**ADM/TOGAF** is designed for enterprise-wide transformation programmes spanning years, with multiple teams, governance layers, and organisational change management. VerdeMart is integrating a defined set of systems for a defined set of use cases. Applying ADM here would introduce disproportionate process overhead (governance artefacts, capability models, and phase gates that add no value at this scale).

**ACDM (Architecture-Centric Design Method)** has more steps, more artefacts, and more ceremony around stakeholder management than ADD. When one or two people are designing and building the same system, that overhead adds no value. ADD is lean enough to match the project scale while still providing the structure needed to make decisions traceable.

The key distinction is that neither ADM nor ACDM treats quality attributes as the **primary input** to every design decision. Both can produce good architectures, but they are not optimised for quality-attribute-driven scenarios. Since the entire problem here is defined by quality attributes, like reliability, consistency, availability, recoverability, and visibility, using a framework that puts them at the centre is the natural fit.
