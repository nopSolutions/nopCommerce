using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.ScheduleTasks;
using Nop.Services.Shipping;

namespace Nop.Plugin.Shipping.CarrierTracking.Services;

public class CarrierStatusPollerTask : IScheduleTask
{
    private static readonly HashSet<string> TerminalStatuses = new() { "DELIVERED", "RETURNED" };

    private readonly IShipmentService _shipmentService;
    private readonly IWireMockClient _wireMockClient;
    private readonly IExternalStatusMapper _statusMapper;
    private readonly IWorkflowMessageService _workflowMessageService;
    private readonly IOrderService _orderService;
    private readonly ILogger _logger;

    public CarrierStatusPollerTask(
        IShipmentService shipmentService,
        IWireMockClient wireMockClient,
        IExternalStatusMapper statusMapper,
        IWorkflowMessageService workflowMessageService,
        IOrderService orderService,
        ILogger logger)
    {
        _shipmentService = shipmentService;
        _wireMockClient = wireMockClient;
        _statusMapper = statusMapper;
        _workflowMessageService = workflowMessageService;
        _orderService = orderService;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var allShipments = await _shipmentService.GetAllShipmentsAsync(
            loadNotDelivered: true,
            pageSize: int.MaxValue);

        var openShipments = allShipments
            .Where(s => s.ExternalShipmentId is not null
                     && !TerminalStatuses.Contains(s.ExternalShippingStatus ?? string.Empty))
            .ToList();

        if (openShipments.Count == 0)
            return;

        foreach (var shipment in openShipments)
        {
            var result = await _wireMockClient.GetShipmentStatusAsync(shipment.ExternalShipmentId!);

            if (result.IsUnreachable)
            {
                await _logger.WarningAsync($"[CarrierTracking] WireMock unreachable for ShipmentId={shipment.Id} — skipping tick");
                continue;
            }

            if (result.Status == shipment.ExternalShippingStatus)
                continue;

            var previousStatus = shipment.ExternalShippingStatus ?? "(none)";
            shipment.ExternalShippingStatus = result.Status;
            shipment.LastStatusOccurredAtUtc = result.OccurredAtUtc;
            if (result.Status == "DELIVERED")
                shipment.DeliveryDateUtc = result.OccurredAtUtc;

            await _shipmentService.UpdateShipmentAsync(shipment);

            var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
            if (order is not null)
            {
                var internalStatus = _statusMapper.MapToInternal(result.Status);
                if (internalStatus.HasValue)
                {
                    order.ShippingStatusId = (int)internalStatus.Value;
                    await _orderService.UpdateOrderAsync(order);
                }

                await _workflowMessageService.SendShipmentSentCustomerNotificationAsync(shipment, order.CustomerLanguageId);
            }

            await _logger.InformationAsync(
                $"[CarrierTracking] ShipmentId={shipment.Id} status {previousStatus} → {result.Status}");
        }
    }
}
