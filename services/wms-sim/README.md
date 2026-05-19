# WMS Simulator

Controlled HTTP boundary for the warehouse management system in Scenario C.
The omnichannel worker will call this service when it receives
`commerce.order.placed.v1` messages from RabbitMQ.

This is the Phase 1 scaffold: it supports the happy-path `normal` mode only.
Failure modes are intentionally left for Phase 3.

## Run

```bash
pip install -r requirements.txt
uvicorn app.main:app --reload --host 0.0.0.0 --port 8080
```

Or with Docker:

```bash
docker build -t wms-sim .
docker run --rm -p 8080:8080 wms-sim
```

## Smoke Check

Run these from `services/wms-sim/`:

```bash
curl http://localhost:8080/health
curl http://localhost:8080/mode
curl -X POST http://localhost:8080/fulfillments \
  -H 'Content-Type: application/json' \
  --data-binary @../../docs/evidence/sample-commerce-order-placed-v1.json
```

## API

### `GET /health`

Returns liveness plus the active simulator mode.

### `GET /mode`

Returns the active mode and supported modes.

### `POST /fulfillments`

Accepts the order-placed contract documented in
`docs/evidence/sample-commerce-order-placed-v1.json`.

Required envelope fields:

- `messageId`
- `correlationId`
- `eventType` = `commerce.order.placed.v1`
- `occurredOnUtc`
- `orderGuid`

Required order fields:

- `orderId`
- `storeId`
- non-empty `items`

Successful requests return HTTP `202`:

```json
{
  "externalRequestId": "WMS-REQ-1024",
  "status": "Accepted",
  "orderGuid": "8b5ce538-9b8b-4e81-9019-3e11f0a9d4ef",
  "messageId": "c260f7b0-6db9-4124-9c54-cd5c706b7c7e"
}
```

The response is deliberately smaller than the plugin callback event. Translating
the WMS response into `fulfillment.status.changed.v1` remains the worker's job.

## Configuration

- `WMS_MODE`: defaults to `normal`. This scaffold supports only `normal`; any
  other value fails startup loudly.
- `PORT`: defaults to `8080` in the Docker image.

## Future Modes

- `slow`: delay the fulfillment response.
- `unavailable`: return HTTP 503.
- `contradictory`: return a domain contradiction response for Phase 3 demo
  pressure.
