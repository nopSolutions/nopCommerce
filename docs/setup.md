# Setup & Run

Build-and-run instructions for the Scenario C omnichannel demo stack.

> **Status: skeleton (plan.md Phase 1 → filled through Phases 2/3/6).** Section
> headers and the compose entrypoint are in place; per-scenario run steps are
> filled in as each phase lands.

## Prerequisites

- Docker + Docker Compose v2
- (local dev only) .NET SDK `10.0.100` — see `nopCommerce/global.json`
- (local dev only) Python 3.12 for `services/wms-sim`

## Components

| Service | Path | Port (host) | Role |
|---------|------|-------------|------|
| nopCommerce | `nopCommerce/` | 8080 | commerce core (storefront + admin + OmnichannelCore plugin) |
| SQL Server | (image) | 1433 | nopCommerce database |
| RabbitMQ | (image) | 5672 / 15672 | async order→WMS transport + management UI |
| Worker | `services/worker/` | — | consumes `commerce.order.placed.v1`, calls WMS, posts fulfillment status |
| WMS sim | `services/wms-sim/` | 8081 | warehouse boundary; modes normal/slow/unavailable/contradictory |
| POS sim | `services/pos-sim/` | 8082 | POS stock events; modes normal/duplicate/stale |
| Contracts | `services/contracts/` | — | shared message envelope (referenced by worker) |

## Quick start

```bash
docker compose up --build
```

Then:

1. Open the storefront at <http://localhost:8080> and complete the nopCommerce
   install wizard, pointing the database at host `sqlserver`, user `sa`,
   password `Omni_Demo_Pass1` (see `docker-compose.yml`). _(Phase 6: document
   exact wizard values / pre-seeded settings.)_
2. Install the **Misc.OmnichannelCore** plugin from Admin → Configuration →
   Plugins.
3. RabbitMQ management UI: <http://localhost:15672> (guest/guest).

## Demo scenarios

_(Filled in Phases 2–6; one block per scenario.)_

- **Normal order flow (Phase 2)** — place an order → `OmniOutboxMessage` row →
  worker → WMS → `OmniOrderFulfillment` state `Accepted`.
- **WMS pressure + recovery (Phase 3, QA-1)** —
  `curl -X POST http://localhost:8081/mode/unavailable`, place orders, observe
  worker retry → circuit breaker → backlog drain after
  `curl -X POST http://localhost:8081/mode/normal`. Other WMS modes:
  `curl -X POST http://localhost:8081/mode/slow` and
  `curl -X POST http://localhost:8081/mode/contradictory`.
- **POS consistency (Phase 4, QA-2)** — `services/pos-sim` `normal` / `duplicate`
  / `stale`; see `docs/evidence/qa-2-consistency.md`.
- **Traceability (Phase 5, QA-3)** — look up an `OrderGuid` in the plugin admin
  view; see `docs/evidence/qa-3-traceability.md`.

## Baseline measurement

See `docs/evidence/baseline.md` (captured before plugin install; referenced by
the QA-1 "≤ 1.5× baseline" gate).
