using System.Text.Json;
using System.Transactions;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Inventory.AllocationGate.Models;
using Nop.Plugin.Inventory.AllocationGate.OpenBoxes;
using Nop.Plugin.Messaging.RabbitMq.Domain;
using Nop.Plugin.Messaging.RabbitMq.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.ScheduleTasks;
using Nop.Services.Shipping;

namespace Nop.Plugin.Inventory.AllocationGate.Services;

public class OpenBoxesStatusPollerTask : IScheduleTask
{
    private const string IssuedStatus = "SHIPPED";

    private readonly IOpenBoxesClient _openBoxesClient;
    private readonly IOrderService _orderService;
    private readonly IShipmentService _shipmentService;
    private readonly IAddressService _addressService;
    private readonly IOutboxRepository _outboxRepository;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly ILogger _logger;

    public OpenBoxesStatusPollerTask(
        IOpenBoxesClient openBoxesClient,
        IOrderService orderService,
        IShipmentService shipmentService,
        IAddressService addressService,
        IOutboxRepository outboxRepository,
        ISettingService settingService,
        IStoreContext storeContext,
        ILogger logger)
    {
        _openBoxesClient = openBoxesClient;
        _orderService = orderService;
        _shipmentService = shipmentService;
        _addressService = addressService;
        _outboxRepository = outboxRepository;
        _settingService = settingService;
        _storeContext = storeContext;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var settings = await _settingService.LoadSettingAsync<AllocationSettings>(store.Id);

        var fulfillmentOrders = await _openBoxesClient.GetIssuedFulfillmentOrdersAsync(
            settings.PollerBatchSize, CancellationToken.None);

        if (fulfillmentOrders.Count == 0)
            return;

        foreach (var fulfillment in fulfillmentOrders)
        {
            if (!string.Equals(fulfillment.Status, IssuedStatus, StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                await ProcessIssuedFulfillmentAsync(fulfillment);
            }
            catch (Exception ex)
            {
                // never let one failure block the rest of the tick
                await _logger.ErrorAsync(
                    $"[OpenBoxesPoller] Failed to process fulfillment for OrderGuid={fulfillment.OrderGuid}", ex);
            }
        }
    }

    private async Task ProcessIssuedFulfillmentAsync(OpenBoxesFulfillmentOrder fulfillment)
    {
        var order = await _orderService.GetOrderByGuidAsync(fulfillment.OrderGuid);
        if (order is null)
        {
            await _logger.WarningAsync(
                $"[OpenBoxesPoller] No nopCommerce order found for OrderGuid={fulfillment.OrderGuid}");
            return;
        }

        // Idempotency guard: skip if the order is already complete or already has a shipment.
        if (order.OrderStatusId == (int)OrderStatus.Complete)
        {
            await _logger.WarningAsync(
                $"[OpenBoxesPoller] OrderId={order.Id} already marked as Complete for OrderGuid={fulfillment.OrderGuid}; skipping");
            return;
        }

        var existingShipments = await _shipmentService.GetShipmentsByOrderIdAsync(order.Id);
        if (existingShipments.Any())
        {
            await _logger.WarningAsync(
                $"[OpenBoxesPoller] OrderId={order.Id} already has shipments; skipping creation of duplicate shipment for OrderGuid={fulfillment.OrderGuid}");
            return;
        }

        var orderItems = await _orderService.GetOrderItemsAsync(order.Id, isShipEnabled: true);

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var shipment = new Shipment
        {
            OrderId = order.Id,
            TrackingNumber = string.Empty,
            AdminComment = $"Auto-created by OpenBoxesStatusPollerTask from FulfillmentId={fulfillment.FulfillmentId}",
            CreatedOnUtc = DateTime.UtcNow
        };
        await _shipmentService.InsertShipmentAsync(shipment);

        foreach (var orderItem in orderItems)
        {
            await _shipmentService.InsertShipmentItemAsync(new ShipmentItem
            {
                ShipmentId = shipment.Id,
                OrderItemId = orderItem.Id,
                Quantity = orderItem.Quantity,
                WarehouseId = 0
            });
        }

        order.OrderStatusId = (int)OrderStatus.Complete;
        await _orderService.UpdateOrderAsync(order);

        ShippingAddressPayload? shippingAddress = null;
        if (order.ShippingAddressId.HasValue)
        {
            var address = await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value);
            if (address is not null)
                shippingAddress = new ShippingAddressPayload(
                    address.FirstName,
                    address.LastName,
                    address.Address1,
                    address.Address2,
                    address.City,
                    address.ZipPostalCode,
                    address.CountryId);
        }

        var lineItems = orderItems
            .Select(i => new CarrierBookingLineItemPayload(i.ProductId, i.Quantity))
            .ToList();

        var bookingMessage = new CarrierBookingRequestedPayload(
            ShipmentId: shipment.Id,
            OrderId: order.Id,
            ShippingAddress: shippingAddress,
            Items: lineItems);

        await _outboxRepository.InsertAsync(new OutboxMessage
        {
            AggregateId = shipment.Id.ToString(),
            EventType = "carrier.booking.requested",
            Exchange = "verdemart.carrier.booking",
            Payload = JsonSerializer.Serialize(bookingMessage),
            Status = (int)OutboxMessageStatus.Pending,
            AttemptCount = 0,
            CreatedAtUtc = DateTime.UtcNow
        });

        scope.Complete();

        await _logger.InformationAsync(
            $"[OpenBoxesPoller] OrderGuid={fulfillment.OrderGuid} ISSUED → ShipmentId={shipment.Id}, order set to Complete, carrier.booking.requested queued");
    }
}
