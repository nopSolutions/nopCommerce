using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nop.Plugin.Messaging.RabbitMq.Services;
using Nop.Plugin.Shipping.CarrierTracking.Models;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nop.Plugin.Shipping.CarrierTracking.Services;

public class CarrierBookingConsumer : BackgroundService
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CarrierBookingConsumer(
        IRabbitMqConnectionFactory connectionFactory,
        IServiceScopeFactory scopeFactory,
        ILogger logger)
    {
        _connectionFactory = connectionFactory;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _logger.InformationAsync("[CarrierTracking] CarrierBookingConsumer starting");

        IChannel channel;
        try
        {
            channel = await _connectionFactory.CreateChannelAsync();
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"[CarrierTracking] Cannot connect to RabbitMQ: {ex.Message}", ex);
            return;
        }

        await using (channel)
        {
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (_, args) =>
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var shipmentService = scope.ServiceProvider.GetRequiredService<IShipmentService>();
                var wireMockClient = scope.ServiceProvider.GetRequiredService<IWireMockClient>();

                CarrierBookingRequestedMessage? message = null;
                try
                {
                    var json = Encoding.UTF8.GetString(args.Body.Span);
                    message = JsonSerializer.Deserialize<CarrierBookingRequestedMessage>(json, JsonOptions);
                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync($"[CarrierTracking] Failed to deserialize message — sending to DLQ: {ex.Message}", ex);
                    await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                    return;
                }

                if (message is null)
                {
                    await _logger.ErrorAsync("[CarrierTracking] Deserialized message was null — sending to DLQ");
                    await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                    return;
                }

                var shipment = await shipmentService.GetShipmentByIdAsync(message.ShipmentId);
                if (shipment is null)
                {
                    await _logger.WarningAsync($"[CarrierTracking] Shipment {message.ShipmentId} not found — sending to DLQ");
                    await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                    return;
                }

                if (shipment.ExternalShipmentId is not null)
                {
                    await _logger.InformationAsync($"[CarrierTracking] ShipmentId={message.ShipmentId} already booked — ack and skip");
                    await channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                    return;
                }

                var result = await wireMockClient.BookShipmentAsync(message.ShipmentId, stoppingToken);

                switch (result)
                {
                    case BookingResult.Success success:
                        shipment.ExternalShipmentId = success.CarrierTrackingId;
                        shipment.ExternalCarrierCode = "WIREMOCK";
                        shipment.TrackingNumber = success.CarrierTrackingId;
                        shipment.ShippedDateUtc ??= DateTime.UtcNow;
                        await shipmentService.UpdateShipmentAsync(shipment);
                        await channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                        await _logger.InformationAsync($"[CarrierTracking] Booked ShipmentId={message.ShipmentId} → TrackingId={success.CarrierTrackingId}");
                        break;

                    case BookingResult.TransientFailure transient:
                        await _logger.WarningAsync($"[CarrierTracking] Transient failure for ShipmentId={message.ShipmentId}: {transient.Reason} — requeuing");
                        await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                        break;

                    case BookingResult.PermanentFailure permanent:
                        await _logger.ErrorAsync($"[CarrierTracking] Permanent failure for ShipmentId={message.ShipmentId}: {permanent.Reason} — sending to DLQ");
                        await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                        break;
                }
            };

            await channel.BasicConsumeAsync(
                queue: "verdemart.carrier.booking.requested",
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            await _logger.InformationAsync("[CarrierTracking] Consuming verdemart.carrier.booking.requested");

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                await _logger.InformationAsync("[CarrierTracking] CarrierBookingConsumer stopping");
            }
        }
    }
}
