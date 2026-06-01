using System.Text.Json;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Messaging.RabbitMq.Domain;
using Nop.Plugin.Messaging.RabbitMq.Services;
using Nop.Plugin.Shipping.CarrierTracking.Models;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Orders;

namespace Nop.Plugin.Shipping.CarrierTracking.Services;

public class ShipmentSentEventConsumer : IConsumer<ShipmentSentEvent>
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IOrderService _orderService;
    private readonly IAddressService _addressService;
    private readonly ILogger _logger;

    public ShipmentSentEventConsumer(
        IOutboxRepository outboxRepository,
        IOrderService orderService,
        IAddressService addressService,
        ILogger logger)
    {
        _outboxRepository = outboxRepository;
        _orderService = orderService;
        _addressService = addressService;
        _logger = logger;
    }

    public async Task HandleEventAsync(ShipmentSentEvent eventMessage)
    {
        var shipment = eventMessage.Shipment;

        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
        if (order is null)
        {
            await _logger.WarningAsync($"[CarrierTracking] Order {shipment.OrderId} not found for ShipmentId={shipment.Id}; skipping booking.");
            return;
        }

        ShippingAddressMessage? shippingAddress = null;
        if (order.ShippingAddressId.HasValue)
        {
            var address = await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value);
            if (address is not null)
                shippingAddress = new ShippingAddressMessage(
                    address.FirstName,
                    address.LastName,
                    address.Address1,
                    address.Address2,
                    address.City,
                    address.ZipPostalCode,
                    address.CountryId);
        }

        var orderItems = await _orderService.GetOrderItemsAsync(order.Id);
        var lineItems = orderItems
            .Select(i => new CarrierBookingLineItemMessage(i.ProductId, i.Quantity))
            .ToList();

        var message = new CarrierBookingRequestedMessage(
            ShipmentId: shipment.Id,
            OrderId: order.Id,
            ShippingAddress: shippingAddress,
            Items: lineItems);

        var outboxMessage = new OutboxMessage
        {
            AggregateId = shipment.Id.ToString(),
            EventType = "carrier.booking.requested",
            Exchange = "verdemart.carrier.booking",
            Payload = JsonSerializer.Serialize(message),
            Status = (int)OutboxMessageStatus.Pending,
            AttemptCount = 0,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _outboxRepository.InsertAsync(outboxMessage);

        await _logger.InformationAsync($"[CarrierTracking] Queued carrier.booking.requested for ShipmentId={shipment.Id}, OrderId={order.Id}");
    }
}
