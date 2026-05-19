# Phase 1 Evidence - Plugin Scaffold and Tables

## Scope

This phase implements only the first roadmap step: the nopCommerce plugin foundation and its database schema. It does not implement RabbitMQ publication, a worker service, WMS/POS simulators, retries, circuit breakers, DLQ handling or a real order workflow.

## Implemented Artefacts

| Area | Artefact | Purpose |
|------|----------|---------|
| Plugin lifecycle | `nopCommerce/src/Plugins/Nop.Plugin.Misc.OmnichannelCore/Nop.Plugin.Misc.OmnichannelCore.csproj`, `plugin.json`, `OmnichannelCorePlugin.cs` | Makes the omnichannel module a standard nopCommerce plugin. |
| Schema migration | `Data/Migrations/SchemaMigration.cs` | Creates and drops the four Phase 1 plugin tables during plugin install/uninstall. |
| Table mapping | `Data/Mapping/*` | Keeps table names explicit and defines key column sizes/indexes. |
| Domain model | `Domains/*` | Defines outbox, inbox, fulfillment and stock projection entities needed by later ADD iterations. |
| Admin shell | `Controllers/OmnichannelCoreController.cs`, `Views/Configure.cshtml` | Shows table counts and makes clear that this phase is foundation-only. |
| Registration | `Infrastructure/PluginNopStartup.cs`, `Services/EventConsumer.cs`, solution entry | Registers the read service and adds the plugin to the nopCommerce admin plugin menu. |

## Schema Created

### `OmniOutboxMessage`

Purpose: durable outbound event ledger for `commerce.order.placed.v1` and later integration events.

Key columns:

- `MessageId`: integration message identifier, indexed for traceability.
- `EventType`: versioned event name.
- `CorrelationId`: cross-component correlation key.
- `OrderGuid` / `OrderId`: nopCommerce order link.
- `Payload`: serialized event body.
- `StatusId`, `RetryCount`, `LastError`, `PublishedOnUtc`, `NextAttemptOnUtc`: later publisher/retry state.

### `OmniInboxMessage`

Purpose: inbound message ledger for idempotency when the worker or POS simulator calls back into nopCommerce.

Key columns:

- `MessageId`: external message identifier, indexed for duplicate detection.
- `EventType`, `CorrelationId`, `Source`: message envelope metadata.
- `StatusId`, `LastError`, `ReceivedOnUtc`, `ProcessedOnUtc`: processing state.

### `OmniOrderFulfillment`

Purpose: plugin-owned fulfillment projection, avoiding changes to nopCommerce core order enums.

Key columns:

- `OrderGuid` / `OrderId`: link back to the nopCommerce order.
- `MessageId`: integration event link.
- `ExternalRequestId`: future WMS request identifier.
- `StatusId`, `TrackingNumber`, `Reason`: fulfillment state visible to the demo/admin view.

### `OmniStockSyncState`

Purpose: projection-first cross-channel stock visibility, matching ADR-0007.

Key columns:

- `ProductId`, `Sku`, `WarehouseId`: stock item identity.
- `QuantityOnHand`: latest accepted source quantity.
- `SourceVersion`: stale update guard for POS-originated events.
- `LastMessageId`, `Source`, `StatusId`, `LastSeenOnUtc`: traceability and reconciliation state.

## Why This Phase Exists

The assignment asks for architectural evolution, not a rewrite. This phase creates a small plugin-owned boundary where later phases can add asynchronous integration without changing nopCommerce checkout, order status enums or catalog ownership. It supports:

- ADR-0003: keep checkout/order/catalog inside the monolith.
- ADR-0004 and ADR-0006: use outbox + asynchronous messaging instead of synchronous WMS calls.
- ADR-0007: store POS stock visibility as a projection first.
- ADR-0008: preserve traceability fields from the start.

## Explicitly Excluded

- No `OrderPlacedEvent` consumer yet.
- No scheduled outbox publisher yet.
- No RabbitMQ dependency yet.
- No worker service yet.
- No WMS/POS simulators yet.
- No retry, circuit breaker or DLQ behavior yet.
- No modification to nopCommerce core order, checkout or catalog services.

## Verification Status

Static checks completed:

- Plugin project is present under `nopCommerce/src/Plugins/Nop.Plugin.Misc.OmnichannelCore`.
- `plugin.json` uses system name `Misc.OmnichannelCore`.
- `NopCommerce.sln` references the plugin project.
- Migration creates and drops the four planned tables.
- `jq . nopCommerce/src/Plugins/Nop.Plugin.Misc.OmnichannelCore/plugin.json` succeeds.
- `rg "OmnichannelCore|OmniOutboxMessage|OmniInboxMessage|OmniOrderFulfillment|OmniStockSyncState"` confirms the plugin, solution and evidence references.

Build checks completed:

- Host `dotnet --info` fails in the current shell with `command not found`.
- `docker build --target build -t nopcommerce-omni-phase1-check .` from `nopCommerce/` succeeds.
- The Docker build emits 3 existing nopCommerce warnings and 0 errors.
- The build output includes `Nop.Plugin.Misc.OmnichannelCore.dll`.

Manual runtime checks completed:

- Plugin installed successfully in the local nopCommerce instance.
- Admin page is visible at `http://localhost/Admin/OmnichannelCore/Configure`.
- The four `Omni%` tables are visible from DBeaver in the nopCommerce database.

Runtime checks still required:

- Uninstall the plugin and confirm the four tables are removed.

## Database Inspection with DBeaver

The local Docker setup currently uses the default `nopCommerce/docker-compose.yml`, which starts SQL Server, not PostgreSQL. The active database container is `nopcommerce_mssql_server`.

Use these DBeaver settings:

| Field | Value |
|-------|-------|
| Driver | Microsoft SQL Server |
| Host | Container IP, for example `172.29.0.2` |
| Port | `1433` |
| Database | `nopcommerce` |
| Username | `sa` |
| Password | `nopCommerce_db_password` |
| SSL option | Trust server certificate, or disable encryption if needed |

The container IP can change after recreating containers. Confirm the current IP with:

```bash
docker inspect -f '{{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}}' nopcommerce_mssql_server
```

If the team wants a stable `localhost` connection from DBeaver, expose SQL Server in `nopCommerce/docker-compose.yml`:

```yaml
nopcommerce_database:
    image: "mcr.microsoft.com/mssql/server:2019-latest"
    container_name: nopcommerce_mssql_server
    ports:
        - "1433:1433"
    environment:
        SA_PASSWORD: "nopCommerce_db_password"
        ACCEPT_EULA: "Y"
        MSSQL_PID: "Express"
```

After recreating the containers, DBeaver can use:

| Field | Value |
|-------|-------|
| Host | `localhost` |
| Port | `1433` |
| Database | `nopcommerce` |
| Username | `sa` |
| Password | `nopCommerce_db_password` |

Useful verification queries:

```sql
SELECT TABLE_SCHEMA, TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME LIKE 'Omni%';
```

```sql
SELECT TOP (50) *
FROM [dbo].[OmniOutboxMessage];

SELECT TOP (50) *
FROM [dbo].[OmniInboxMessage];

SELECT TOP (50) *
FROM [dbo].[OmniOrderFulfillment];

SELECT TOP (50) *
FROM [dbo].[OmniStockSyncState];
```

The tables are expected to be empty in Phase 1, because no event consumer, publisher, worker, WMS simulator or POS simulator exists yet.

## Go/No-Go

Current status: **In review**.

The implementation builds in Docker and the install/table/admin checks have been observed locally. The phase should move to **Done** only after the uninstall database gate passes.
