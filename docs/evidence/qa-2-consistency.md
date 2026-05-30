# QA-2 Evidence - POS Consistency

## Scenario

POS/store-originated stock updates arrive at nopCommerce through the omnichannel plugin callback:

```text
POST /omnichannel/callbacks/pos/stock-changed
X-Demo-Token: omni-demo-token
```

The plugin must handle at-least-once delivery safely:

- duplicate `messageId` is detected by `OmniInboxMessage`;
- stale `sourceVersion` is ignored by `OmniStockSyncState`;
- core nopCommerce stock is not overwritten in this iteration, per ADR-0007 projection-first stock.

## Implemented Controls

| Control | Implementation |
|---------|----------------|
| Internal callback auth | `X-Demo-Token` checked by `OmnichannelCallbackController`. |
| Inbox idempotency | `OmniInboxService.TryBeginProcessingAsync` checks `MessageId` before processing. |
| Duplicate handling | duplicate request returns `result = duplicate` and does not call stock projection logic. |
| Stale handling | `OmniStockSyncService` ignores updates where `sourceVersion <= stored SourceVersion`. |
| POS simulator | `services/pos-sim` supports `normal`, `duplicate`, and `stale` modes. |

## Run

Build and start nopCommerce first. Then build the simulator from the repository root:

```bash
docker build -t omni-pos-sim services/pos-sim
```

Run it against a nopCommerce instance exposed on the host:

```bash
docker run --rm -p 5081:8080 \
  --add-host=host.docker.internal:host-gateway \
  -e NopCommerce__BaseUrl=http://host.docker.internal \
  -e OmnichannelCore__DemoToken=omni-demo-token \
  omni-pos-sim
```

## Normal Update

```bash
curl -X POST http://localhost:5081/emit \
  -H "Content-Type: application/json" \
  -d '{"mode":"normal","productId":15,"sku":"LAPTOP-15","warehouseId":2,"quantityOnHand":3}'
```

Expected plugin result inside simulator response:

```json
{
  "result": "applied",
  "applied": true,
  "duplicate": false,
  "stale": false
}
```

Expected SQL:

```sql
SELECT TOP (20) *
FROM [dbo].[OmniInboxMessage]
ORDER BY [Id] DESC;

SELECT TOP (20) *
FROM [dbo].[OmniStockSyncState]
WHERE [ProductId] = 15 AND [WarehouseId] = 2
ORDER BY [Id] DESC;
```

## Duplicate Update

```bash
curl -X POST http://localhost:5081/emit \
  -H "Content-Type: application/json" \
  -d '{"mode":"duplicate","productId":15,"sku":"LAPTOP-15","warehouseId":2,"quantityOnHand":4}'
```

Expected simulator result:

- first post returns `result = applied`;
- second post returns `result = duplicate`;
- both posts use the same `messageId`.

Expected SQL:

```sql
DECLARE @MessageId UNIQUEIDENTIFIER = '<duplicate-message-id-from-response>';

SELECT COUNT(*) AS InboxRowsForMessage
FROM [dbo].[OmniInboxMessage]
WHERE [MessageId] = @MessageId;
```

Expected value:

```text
InboxRowsForMessage = 1
```

This proves at-least-once delivery does not create duplicate processing rows. POS stock events do not create fulfillment rows, so `OmniOrderFulfillment` remains unchanged by this scenario.

## Stale Update

```bash
curl -X POST http://localhost:5081/emit \
  -H "Content-Type: application/json" \
  -d '{"mode":"stale","productId":15,"sku":"LAPTOP-15","warehouseId":2,"quantityOnHand":5}'
```

Expected simulator result:

- first post returns `result = applied`;
- second post returns `result = stale_ignored`;
- second post has a lower `sourceVersion`.

Expected SQL:

```sql
SELECT TOP (1)
    [ProductId],
    [WarehouseId],
    [QuantityOnHand],
    [SourceVersion],
    [LastMessageId],
    [StatusId],
    [LastSeenOnUtc],
    [UpdatedOnUtc]
FROM [dbo].[OmniStockSyncState]
WHERE [ProductId] = 15 AND [WarehouseId] = 2
ORDER BY [Id] DESC;
```

Expected result:

- `SourceVersion` remains the newer version.
- `QuantityOnHand` remains the value from the newer version.
- `StatusId = 20`, meaning `OmniStockSyncStatus.StaleIgnored`.
- `LastMessageId` points to the stale message for traceability.

## Build/Smoke Status

Completed on 2026-05-15:

- `docker build --target build -t nopcommerce-omni-inbox-pos-check .` from `nopCommerce/` succeeds with 3 existing nopCommerce warnings and 0 errors.
- `docker build -t omni-pos-sim-check .` from `services/pos-sim/` succeeds with 0 warnings and 0 errors.
- `docker run --rm -d -p 5081:8080 --name omni-pos-sim-smoke omni-pos-sim-check` starts the simulator.
- `curl -sS http://localhost:5081/health` returns `{"status":"ok","simulator":"pos-sim","mode":"normal"}`.

## QA-2 Status

Current status: **implemented, runtime measurement pending**.

The runtime evidence to capture during demo rehearsal is:

- duplicate callback duration, target `<= 50 ms`;
- `COUNT(*) = 1` for duplicate `messageId` in `OmniInboxMessage`;
- stale `sourceVersion` does not overwrite stored `QuantityOnHand` or `SourceVersion`.
