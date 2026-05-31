# WMS Simulator

Controlled HTTP boundary for the warehouse management system in Scenario C.
The omnichannel worker will call this service when it receives
`commerce.order.placed.v1` messages from RabbitMQ.

The simulator supports the happy-path `normal` mode plus the Phase 3 pressure
modes, so QA-1 can toggle WMS behavior without restarting the stack.

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

Returns liveness plus the active simulator mode. This endpoint stays healthy
even when the simulator mode is `unavailable`, so Docker health checks do not
fight the pressure scenario.

### `GET /mode`

Returns the active mode, supported modes, and the configured `slow` delay.

### `POST /mode/{mode}`

Changes the active mode at runtime. Supported values:

- `normal`: return HTTP `202` with `status = Accepted`.
- `slow`: wait `WMS_SLOW_DELAY_SECONDS`, then return the normal accepted
  response.
- `unavailable`: return HTTP `503` with `error = wms_unavailable`.
- `contradictory`: return HTTP `409` with `error = inventory_contradiction`.

Examples:

```bash
curl -X POST http://localhost:8080/mode/slow
curl -X POST http://localhost:8080/mode/unavailable
curl -X POST http://localhost:8080/mode/contradictory
curl -X POST http://localhost:8080/mode/normal
```

`POST /mode` with a JSON body is also accepted:

```bash
curl -X POST http://localhost:8080/mode \
  -H 'Content-Type: application/json' \
  -d '{"mode":"unavailable"}'
```

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

- `WMS_MODE`: defaults to `normal`. Startup fails if the value is not one of the
  supported modes.
- `WMS_SLOW_DELAY_SECONDS`: defaults to `3`; used by `slow` mode.
- `PORT`: defaults to `8080` in the Docker image.
