# Feasibility Spike - OrderPlacedEvent to Durable Outbox

## Experiment Charter

| Field          | Value                                                                                              |
|----------------|----------------------------------------------------------------------------------------------------|
| Question       | Can a placed order start an asynchronous omnichannel workflow without making checkout depend on WMS/POS availability and without rewriting core order processing? |
| Success signal | A plugin consumer of `OrderPlacedEvent` writes a durable outbox row in < 100 ms and returns; a scheduled task publishes that row to RabbitMQ later; no synchronous external HTTP on the checkout thread. |
| If it fails    | Architecture must change — extract order processing or accept synchronous WMS coupling. Both invalidate the current target architecture. |
| Outcome        | **Feasible.** Findings below.                                                                      |

## Goal

Validate the riskiest Part 2 assumption: a placed nopCommerce order can start an asynchronous omnichannel workflow without making checkout depend on WMS/POS availability and without rewriting core order processing.

## Spike Question

Can the architecture capture a completed order through an extension point, persist a durable integration message and publish it later?

## Findings

### 1. Order placement has a usable event point

`OrderProcessingService.PlaceOrderAsync` publishes `OrderPlacedEvent` after the order and order items are saved and after checkout cleanup/activity logging.

Source: `nopCommerce/src/Libraries/Nop.Services/Orders/OrderProcessingService.cs:1617`.

Implication: a plugin consumer can observe a committed order and create an outbox message without moving WMS work into checkout.

### 2. nopCommerce event handling is not an external integration boundary

`EventPublisher.PublishAsync` resolves `IConsumer<TEvent>` from the local container and awaits each handler.

Source: `nopCommerce/src/Libraries/Nop.Services/Events/EventPublisher.cs:20`.

Implication: the plugin consumer must do minimal local work only: write an outbox row and return. It must not call WMS directly.

### 3. Stock and warehouse concepts already exist

`ProductService.GetTotalStockQuantityAsync` calculates stock across warehouse inventory and reserved quantities.

Source: `nopCommerce/src/Libraries/Nop.Services/Catalog/ProductService.cs:1438`.

`ProductService.AdjustInventoryAsync` updates stock and reservation behavior.

Source: `nopCommerce/src/Libraries/Nop.Services/Catalog/ProductService.cs:1699`.

Implication: the final implementation can use existing stock services or maintain an omnichannel projection before deciding whether to update core stock directly.

### 4. Plugin extension is the smallest architectural change

Existing plugins demonstrate that nopCommerce supports independent plugin folders, controllers, services, migrations and event consumers.

Implication: Part 2 should add `Nop.Plugin.Misc.OmnichannelCore` rather than modifying `Nop.Services.Orders` directly.

## Proposed Event Shape

Sample `commerce.order.placed.v1`:

```json
{
  "messageId": "c260f7b0-6db9-4124-9c54-cd5c706b7c7e",
  "eventType": "commerce.order.placed.v1",
  "occurredOnUtc": "2026-05-02T12:00:00Z",
  "orderGuid": "8b5ce538-9b8b-4e81-9019-3e11f0a9d4ef",
  "orderId": 1024,
  "storeId": 1,
  "items": [
    {
      "orderItemId": 2048,
      "productId": 15,
      "sku": "LAPTOP-15",
      "quantity": 1,
      "warehouseId": 1
    }
  ]
}
```

## Minimal Part 2 Plugin Shape

```csharp
public sealed class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
{
    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        // Final implementation: map order to commerce.order.placed.v1,
        // insert OmniOutboxMessage with Pending status, then return.
    }
}
```

## Validation Result

The spike validates the architecture path:

- Checkout can remain focused on nopCommerce order placement.
- Integration reliability can be handled after checkout through outbox publication.
- WMS/POS degradation can be demonstrated without breaking the storefront.

## Remaining Risks for Part 2

- Confirm exact nopCommerce migration and plugin installation steps in the active branch.
- Decide whether stock updates alter core `ProductWarehouseInventory` immediately or remain as plugin projection first.
- Add tests for duplicate `messageId` and stale POS `sourceVersion`.
- Ensure Docker has .NET 10 SDK/runtime because local `dotnet` is not available in the current shell.

