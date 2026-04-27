# ADR-003: Order Integration Service Runtime — .NET Worker Service vs Python

**Status:** Accepted  
**Date:** 2026-04-26  
**Owner:** Sebastião  
**Deciders:** Full team

---

## Context

The Order Integration Service is a new independently deployable component. We need to choose its runtime. The primary candidates are a **.NET Worker Service** (same stack as nopCommerce) and a **Python service** (faster to prototype, common in integration work).

---

## Decision

**Use .NET Worker Service** (ASP.NET Core minimal API + `IHostedService`).

---

## Rationale

| Criterion | .NET Worker Service | Python (FastAPI / Celery) |
|-----------|--------------------|-----------------------------|
| Polly circuit breaker & retry | Native, mature, well-tested | Requires tenacity or custom — less mature |
| Team familiarity | High (C# is the project stack) | Lower |
| Type-safe message contracts | Shared C# models possible | Requires separate schema definition |
| Docker image size | ~200 MB (runtime) | ~150 MB (similar) |
| RabbitMQ client quality | `RabbitMQ.Client` — official, stable | `pika` — stable but async support patchy |
| Structured logging | Serilog — excellent | structlog — good |

The key driver is **Polly**: our circuit breaker and retry requirements are first-class in the .NET ecosystem. Polly's `CircuitBreakerPolicy` and `RetryPolicy` are battle-tested and well-documented, which reduces implementation risk for the most critical part of the assignment (the pressure point demo).

---

## Rejected Alternative: Python (FastAPI + tenacity)

Python would be faster to prototype the HTTP stubs but the Integration Service requires a robust circuit breaker implementation. Python's `tenacity` library handles retries but lacks a clean circuit breaker abstraction comparable to Polly. Given the team's C# experience and the reliability-critical nature of this service, Python introduces unnecessary risk.

---

## Consequences

- `services/order-integration-service/` is a .NET 10 project
- Dependencies: `RabbitMQ.Client`, `Polly`, `Serilog`, `Microsoft.Extensions.Hosting`
- Uses `IHostedService` for the consumer loop and reconciliation loop
- Exposes `/health` via `Microsoft.AspNetCore.Diagnostics.HealthChecks`
- Idempotency: incoming messages deduplicated on `eventId` using an in-memory `HashSet<Guid>` (sufficient for demo; production would use a Redis set or DB table)
