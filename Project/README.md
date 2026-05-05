# Project Plan — Architectural Evolution of nopCommerce

Compact orientation page. Detail lives in the canonical docs.

## Scenario and commitment

- **Scenario C — Omnichannel Commerce Core.** nopCommerce evolves from web storefront to commerce core surrounded by WMS + POS + worker, designed to stay useful under degradation and recover with traceability.
- Source briefs: [Group Assignment - Final Assignment.pdf](Group%20Assignment%20-%20Final%20Assignment.pdf), [Assignment 2 — Architectural Evolution of nopCommerce.pdf](Assignment%202%20%E2%80%94%20Architectural%20Evolution%20of%20nopCommerce.pdf).

## Dates

- **Part 1 (Architecture Checkpoint, 20%)** — 05/06 May 2026.
- **Part 2 (Implementation + Demo, 80%)** — 02/03 June 2026.

## Authoritative docs

- **[roadmap.md](roadmap.md)** — phased delivery plan (Phase 0–6) with verification gates.
- **[journal.md](journal.md)** — code-change journal (Part 2).
- **[docs/part1/architecture-checkpoint.md](docs/part1/architecture-checkpoint.md)** — design intent (3 ADD iterations, framework, target arch, ADR map).
- **[docs/part1/quality-attribute-scenarios.md](docs/part1/quality-attribute-scenarios.md)** — five QA scenarios with numeric measures.
- **[docs/architecture.md](docs/architecture.md)** — current-state analysis with source-line citations.
- **[docs/adr/](docs/adr/)** — 10 ADRs: 8 accepted decisions plus 2 explicit rejected decisions; new ADRs use [docs/adr/template.md](docs/adr/template.md).

## Technology stack

- nopCommerce: ASP.NET Core / .NET 10, modular monolith, plugins, in-process events, scheduled tasks.
- `Nop.Plugin.Misc.OmnichannelCore` — own tables via plugin migrations.
- RabbitMQ + `RabbitMQ.Client` — publisher confirms, manual acks, DLQ.
- `.NET Worker Service` — independent integration service.
- Polly — retry + circuit breaker for HTTP calls.
- Docker Compose — nopCommerce + SQL Server + RabbitMQ + worker + WMS sim + POS sim.

## Reference documentation

- [nopCommerce plugins](https://docs.nopcommerce.com/en/developer/plugins/index.html)
- [nopCommerce plugins with data access](https://docs.nopcommerce.com/en/developer/plugins/plugin-with-data-access.html)
- [nopCommerce scheduled tasks](https://docs.nopcommerce.com/en/developer/tutorials/scheduled-tasks.html)
- [RabbitMQ reliability](https://www.rabbitmq.com/docs/reliability)
- [Polly retry strategies](https://www.pollydocs.org/strategies/retry)
- [.NET Worker Services](https://learn.microsoft.com/en-us/dotnet/core/extensions/workers)

## Environment assumption

`dotnet` is not on the local PATH. Final validation runs through Docker Compose, which is the same environment as the demo.

## AI usage

See [ai.md](ai.md) for AI-tool usage transparency.
