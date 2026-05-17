﻿# VerdeMart — Omnichannel Commerce Core

Architectural evolution of [nopCommerce](https://www.nopcommerce.com/) into a reliable omnichannel commerce core, developed as a group assignment for the Software Architecture course (Masters, 1st year, 2nd semester).

## Scenario

VerdeMart is a retail business whose nopCommerce storefront is evolving into the commerce core of a wider enterprise ecosystem — integrating a warehouse (OpenBoxes), an ERP (ERPNext), an in-store POS, and a shipping carrier (WireMock), all coordinated through RabbitMQ.

The architectural challenge: keep checkout working and eventually deliver a consistent state across all systems when surrounding systems disagree, lag, or are temporarily unavailable.

## Repository Structure

```
.
├── docs/
│   └── architecture/
│       ├── 01-scenario.md               # Business scenario and strategic goals
│       ├── 02-current-state.md          # nopCommerce analysis — seams and pressure points
│       ├── 03-bounded-contexts.md       # Domain model and bounded context map
│       ├── 04-qas.md                    # Quality Attribute Scenarios (QAS-1 to QAS-5)
│       ├── 05-framework-add.md          # Why ADD was chosen as the architectural framework
│       ├── 07-adrs/                     # Architectural Decision Records (ADR-001 to ADR-010)
│       ├── 08-risk-and-validation-plan.md
│       ├── 09-evolution-roadmap.md      # Implementation phases 1–5
│       ├── 10-feasibility-spike.md
│       ├── add-iteration-1/             # ADD Iteration 1 — steps 1–7
│       ├── add-iteration-2/             # ADD Iteration 2 — steps 1–7
│       ├── add-iteration-3/             # ADD Iteration 3 — steps 1–7
│       ├── add-iteration-4/             # ADD Iteration 4 — steps 1–7
│       ├── add-iteration-5/             # ADD Iteration 5 — steps 1–7
│       └── diagrams/                    # Architecture diagrams
├── spike/                               # Feasibility spikes
├── src/
│   ├── Libraries/                       # nopCommerce core libraries
│   ├── Plugins/                         # nopCommerce plugins
│   ├── Presentation/
│   │   └── Nop.Web/                     # nopCommerce web application
│   └── NopCommerce.sln
├── docker-compose.yml                   # Runs nopCommerce + SQL Server + RabbitMQ
└── Dockerfile
```

## Quality Attribute Scenarios

| ID | Attribute | Requirement |
|----|-----------|-------------|
| QAS-1 | Reliability | Zero orders lost during OpenBoxes outage ≤ 30 min; delivered within 60 s of recovery |
| QAS-2 | Consistency | Zero oversell under concurrent web + POS load |
| QAS-3 | Availability | Checkout response ≤ 3 s regardless of surrounding system latency |
| QAS-4 | Recoverability | All queued orders processed within 5 min of consumer recovery; no operator action |
| QAS-5 | Visibility | Carrier tracking and OpenBoxes fulfillment state reflected within 30 s |

## Architecture Overview

The system uses **ADD (Attribute-Driven Design)** across 5 iterations, each traceable to a QAS:

| Phase | Driver | Status | What was built |
|-------|--------|--------|----------------|
| 1 | QAS-1 partial | Done | `Nop.Plugin.Messaging.RabbitMq` — publishes `order.placed` to RabbitMQ on checkout |
| 2 | QAS-1 full | Done | Transactional outbox — decouples checkout from RabbitMQ availability |
| 3 | QAS-2 | In progress | Allocation gate (pessimistic lock) + OpenBoxes Bridge (separate Docker service) |
| 4 | QAS-5 carrier | Designed | Carrier webhook plugin + `CarrierStatusPollerTask` |
| 5 | QAS-5 warehouse | Designed | `OpenBoxesStatusPollerTask` + Redis distributed lock and status cache |

Key architectural decisions are recorded in [`docs/architecture/07-adrs/`](docs/architecture/07-adrs/).

## Running Locally

**Prerequisites:** Docker Desktop

**1. Start everything:**
```bash
docker-compose up -d
```

> If you changed source code, force a rebuild: `docker-compose up -d --build`

**2. Open the store:** `http://localhost:80`

On first run, the installation wizard will appear. Use the raw connection string (check "Enter raw connection string"):
- Server name: `nopcommerce_mssql_server`
- Database name: `nopcommerce_mssql_server`
- Username: `sa`
- Password: `nopCommerce_db_password`

**RabbitMQ Management UI:** `http://localhost:15672` (guest / guest)

## Surrounding Systems

| System | Role |
|--------|------|
| nopCommerce | Commerce core — order state machine, customer-facing experience |
| ERPNext | Financial backbone — sales orders, stock ledger, accounting |
| OpenBoxes | Warehouse — pick/pack/ship, physical inventory truth |
| Open Source POS | In-store sales channel |
| RabbitMQ | Message broker — async coordination between all systems |
| WireMock | Shipping carrier simulator |
| Keycloak | Identity — single sign-on across all channels |
| Meilisearch | Product search |
