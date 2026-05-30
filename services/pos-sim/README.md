# POS Simulator

Small HTTP simulator for the Pair A consistency track. It emits `pos.stock.changed.v1` events to the nopCommerce omnichannel plugin callback endpoint.

## Modes

| Mode | Behaviour | Expected plugin result |
|------|-----------|------------------------|
| `normal` | Sends one stock event with a new `messageId` and increasing `sourceVersion`. | `OmniInboxMessage` gets one processed row; `OmniStockSyncState` is inserted or updated. |
| `duplicate` | Sends the same event twice with the same `messageId`. | First request is applied; second request returns `duplicate` and does not change stock projection. |
| `stale` | Sends a current event, then sends a second event for the same product/warehouse with a lower `sourceVersion`. | First request is applied; second request returns `stale_ignored` and does not change `QuantityOnHand` or stored `SourceVersion`. |

The version rule is deliberately simple for the demo: the simulator owns a monotonic in-memory `sourceVersion` counter starting at `41`. A stale event is generated as `currentVersion - 1`.

## Configuration

| Setting | Default | Purpose |
|---------|---------|---------|
| `NopCommerce__BaseUrl` | `http://localhost` | nopCommerce base URL. |
| `NopCommerce__PosStockChangedPath` | `/omnichannel/callbacks/pos/stock-changed` | Plugin callback path. |
| `OmnichannelCore__DemoToken` | `omni-demo-token` | Value sent in `X-Demo-Token`. |

## Run with Docker

From the repository root:

```bash
docker build -t omni-pos-sim services/pos-sim
```

If nopCommerce is running on the host at `http://localhost`, run:

```bash
docker run --rm -p 5081:8080 \
  --add-host=host.docker.internal:host-gateway \
  -e NopCommerce__BaseUrl=http://host.docker.internal \
  -e OmnichannelCore__DemoToken=omni-demo-token \
  omni-pos-sim
```

Health check:

```bash
curl http://localhost:5081/health
```

Emit a normal update:

```bash
curl -X POST http://localhost:5081/emit \
  -H "Content-Type: application/json" \
  -d '{"mode":"normal","productId":15,"sku":"LAPTOP-15","warehouseId":2,"quantityOnHand":3}'
```

Emit a duplicate update:

```bash
curl -X POST http://localhost:5081/emit \
  -H "Content-Type: application/json" \
  -d '{"mode":"duplicate","productId":15,"sku":"LAPTOP-15","warehouseId":2,"quantityOnHand":4}'
```

Emit a stale update:

```bash
curl -X POST http://localhost:5081/emit \
  -H "Content-Type: application/json" \
  -d '{"mode":"stale","productId":15,"sku":"LAPTOP-15","warehouseId":2,"quantityOnHand":5}'
```

## Direct Plugin Callback

The simulator calls:

```text
POST /omnichannel/callbacks/pos/stock-changed
X-Demo-Token: omni-demo-token
Content-Type: application/json
```

Sample body:

```json
{
  "messageId": "e2251e6f-6e06-44f2-9c54-6d41f72d7fb8",
  "correlationId": "e2251e6f-6e06-44f2-9c54-6d41f72d7fb8",
  "eventType": "pos.stock.changed.v1",
  "occurredOnUtc": "2026-05-15T12:05:00Z",
  "source": "store-pos-porto",
  "sourceVersion": 42,
  "productId": 15,
  "sku": "LAPTOP-15",
  "warehouseId": 2,
  "quantityOnHand": 3
}
```
