using ClosedXML.Excel;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Plugin.DropShipping.AliExpress.Domain;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;


namespace Nop.Plugin.DropShipping.AliExpress.Services;

/// <summary>
/// Interface for order tracking service
/// </summary>
public interface IAliExpressOrderTrackingService
{
    /// <summary>
    /// Gets AliExpress order by NopCommerce order ID
    /// </summary>
    Task<AliExpressOrder?> GetByOrderIdAsync(int orderId);

    /// <summary>
    /// Gets AliExpress order by AliExpress order ID
    /// </summary>
    Task<AliExpressOrder?> GetByAliExpressOrderIdAsync(long aliExpressOrderId);

    /// <summary>
    /// Updates order tracking information
    /// </summary>
    Task UpdateTrackingAsync(long aliExpressOrderId, string status, string? trackingNumber = null);

    /// <summary>
    /// Processes delivery notification from AliExpress
    /// </summary>
    Task ProcessDeliveryNotificationAsync(long aliExpressOrderId);

    /// <summary>
    /// Creates local shipment with Courier Guy
    /// </summary>
    Task CreateLocalShipmentAsync(int orderId);
}

/// <summary>
/// Service for tracking AliExpress orders and managing local shipments
/// </summary>
public class AliExpressOrderTrackingService : IAliExpressOrderTrackingService
{
    private readonly IRepository<AliExpressOrder> _aliExpressOrderRepository;
    private readonly IOrderService _orderService;
    private readonly IShipmentService _shipmentService;
    private readonly ILogger _logger;
    private readonly AliExpressSettings _settings;

    public AliExpressOrderTrackingService(
        IRepository<AliExpressOrder> aliExpressOrderRepository,
        IOrderService orderService,
        IShipmentService shipmentService,
        ILogger logger,
        AliExpressSettings settings)
    {
        _aliExpressOrderRepository = aliExpressOrderRepository;
        _orderService = orderService;
        _shipmentService = shipmentService;
        _logger = logger;
        _settings = settings;
    }

    public async Task<AliExpressOrder?> GetByOrderIdAsync(int orderId)
    {
        return await _aliExpressOrderRepository.Table
            .FirstOrDefaultAsync(ao => ao.OrderId == orderId);
    }

    public async Task<AliExpressOrder?> GetByAliExpressOrderIdAsync(long aliExpressOrderId)
    {
        return await _aliExpressOrderRepository.Table
            .FirstOrDefaultAsync(ao => ao.AliExpressOrderId == aliExpressOrderId);
    }

    public async Task UpdateTrackingAsync(long aliExpressOrderId, string status, string? trackingNumber = null)
    {
        var aliExpressOrder = await GetByAliExpressOrderIdAsync(aliExpressOrderId);
        if (aliExpressOrder == null)
        {
            await _logger.WarningAsync($"AliExpress order {aliExpressOrderId} not found");
            return;
        }

        aliExpressOrder.AliExpressOrderStatus = status;
        aliExpressOrder.UpdatedOnUtc = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(trackingNumber))
        {
            aliExpressOrder.AliExpressTrackingNumber = trackingNumber;
        }

        // Update status-specific timestamps
        if (status.Contains("shipped", StringComparison.OrdinalIgnoreCase) && !aliExpressOrder.ShippedOnUtc.HasValue)
        {
            aliExpressOrder.ShippedOnUtc = DateTime.UtcNow;
        }
        else if (status.Contains("delivered", StringComparison.OrdinalIgnoreCase) && !aliExpressOrder.DeliveredOnUtc.HasValue)
        {
            aliExpressOrder.DeliveredOnUtc = DateTime.UtcNow;
        }

        await _aliExpressOrderRepository.UpdateAsync(aliExpressOrder);

        await _logger.InformationAsync($"Updated tracking for AliExpress order {aliExpressOrderId}: {status}");
    }

    public async Task ProcessDeliveryNotificationAsync(long aliExpressOrderId)
    {
        try
        {
            var aliExpressOrder = await GetByAliExpressOrderIdAsync(aliExpressOrderId);
            if (aliExpressOrder == null)
            {
                await _logger.WarningAsync($"AliExpress order {aliExpressOrderId} not found for delivery notification");
                return;
            }

            // Mark as delivered
            aliExpressOrder.DeliveredOnUtc = DateTime.UtcNow;
            aliExpressOrder.AliExpressOrderStatus = "Delivered";
            aliExpressOrder.UpdatedOnUtc = DateTime.UtcNow;
            await _aliExpressOrderRepository.UpdateAsync(aliExpressOrder);

            // Update NopCommerce order status to Packing/Quality Control
            var order = await _orderService.GetOrderByIdAsync(aliExpressOrder.OrderId);
            if (order != null)
            {
                // Change order status to indicate it's ready for local shipping
                order.OrderStatusId = (int)OrderStatus.Processing;
                await _orderService.UpdateOrderAsync(order);

                await _logger.InformationAsync($"Order {order.Id} moved to Processing status after AliExpress delivery");

                // Create local shipment if enabled
                if (_settings.AutoCreateLocalShipments && !aliExpressOrder.LocalShippingCreated)
                {
                    await CreateLocalShipmentAsync(order.Id);
                }
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error processing delivery notification for AliExpress order {aliExpressOrderId}: {ex.Message}", ex);
        }
    }

    public async Task CreateLocalShipmentAsync(int orderId)
    {
        try
        {
            var aliExpressOrder = await GetByOrderIdAsync(orderId);
            if (aliExpressOrder == null)
            {
                await _logger.WarningAsync($"No AliExpress order found for order {orderId}");
                return;
            }

            if (aliExpressOrder.LocalShippingCreated)
            {
                await _logger.InformationAsync($"Local shipping already created for order {orderId}");
                return;
            }

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                await _logger.WarningAsync($"Order {orderId} not found");
                return;
            }

            // Get order items
            var orderItems = await _orderService.GetOrderItemsAsync(orderId);
            
            if (!orderItems.Any())
            {
                await _logger.WarningAsync($"No order items found for order {orderId}");
                return;
            }

            // Create shipment
            var shipment = new Shipment
            {
                OrderId = orderId,
                TrackingNumber = string.Empty, // Will be populated by Courier Guy plugin
                AdminComment = $"Shipment created automatically after AliExpress delivery (Order: {aliExpressOrder.AliExpressOrderId})",
                CreatedOnUtc = DateTime.UtcNow
            };

            // Add shipment items
            foreach (var orderItem in orderItems)
            {
                var shipmentItem = new ShipmentItem
                {
                    OrderItemId = orderItem.Id,
                    Quantity = orderItem.Quantity
                };
                await _shipmentService.InsertShipmentItemAsync(shipmentItem);
            }

            await _shipmentService.InsertShipmentAsync(shipment);

            // Mark as local shipping created
            aliExpressOrder.LocalShippingCreated = true;
            aliExpressOrder.UpdatedOnUtc = DateTime.UtcNow;
            await _aliExpressOrderRepository.UpdateAsync(aliExpressOrder);

            await _logger.InformationAsync($"Created local shipment {shipment.Id} for order {orderId}");

            // Note: The Courier Guy plugin will handle the actual courier booking
            // This shipment creation will trigger the Courier Guy event consumer if configured
        }
        catch (Exception ex)
        {
            var aliExpressOrder = await GetByOrderIdAsync(orderId);
            if (aliExpressOrder != null)
            {
                aliExpressOrder.LastErrorMessage = $"Error creating local shipment: {ex.Message}";
                await _aliExpressOrderRepository.UpdateAsync(aliExpressOrder);
            }

            await _logger.ErrorAsync($"Error creating local shipment for order {orderId}: {ex.Message}", ex);
        }
    }
}
