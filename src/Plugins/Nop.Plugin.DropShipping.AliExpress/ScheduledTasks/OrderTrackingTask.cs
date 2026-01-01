using Nop.Data;
using Nop.Plugin.DropShipping.AliExpress.Domain;
using Nop.Plugin.DropShipping.AliExpress.Services;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.DropShipping.AliExpress.ScheduledTasks;

/// <summary>
/// Scheduled task to poll AliExpress for order tracking updates
/// </summary>
public class OrderTrackingTask : IScheduleTask
{
    private readonly IRepository<AliExpressOrder> _aliExpressOrderRepository;
    private readonly IAliExpressService _aliExpressService;
    private readonly IAliExpressOrderTrackingService _trackingService;
    private readonly ILogger _logger;

    public OrderTrackingTask(
        IRepository<AliExpressOrder> aliExpressOrderRepository,
        IAliExpressService aliExpressService,
        IAliExpressOrderTrackingService trackingService,
        ILogger logger)
    {
        _aliExpressOrderRepository = aliExpressOrderRepository;
        _aliExpressService = aliExpressService;
        _trackingService = trackingService;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        try
        {
            await _logger.InformationAsync("Starting AliExpress order tracking task...");

            // Get all orders that are not yet delivered
            var pendingOrders = await _aliExpressOrderRepository.GetAllAsync(query =>
                query.Where(ao => ao.AliExpressOrderId.HasValue &&
                                 !ao.DeliveredOnUtc.HasValue &&
                                 ao.AliExpressOrderStatus != "Delivered"));

            var updatedCount = 0;
            var deliveredCount = 0;

            foreach (var aliExpressOrder in pendingOrders)
            {
                try
                {
                    if (!aliExpressOrder.AliExpressOrderId.HasValue)
                        continue;

                    // Get tracking information from AliExpress
                    var trackingInfo = await _aliExpressService.GetOrderTrackingAsync(aliExpressOrder.AliExpressOrderId.Value);

                    if (trackingInfo != null)
                    {
                        // Update status
                        var oldStatus = aliExpressOrder.AliExpressOrderStatus;
                        aliExpressOrder.AliExpressOrderStatus = trackingInfo.OrderStatus;
                        aliExpressOrder.AliExpressTrackingNumber = trackingInfo.TrackingNumber;
                        aliExpressOrder.UpdatedOnUtc = DateTime.UtcNow;

                        // Update logistics service
                        if (!string.IsNullOrEmpty(trackingInfo.LogisticsService))
                        {
                            aliExpressOrder.LogisticsServiceName = trackingInfo.LogisticsService;
                        }

                        // Update shipped date
                        if (trackingInfo.ShippedDate.HasValue && !aliExpressOrder.ShippedOnUtc.HasValue)
                        {
                            aliExpressOrder.ShippedOnUtc = trackingInfo.ShippedDate.Value;
                        }

                        // Check if delivered
                        if (trackingInfo.DeliveredDate.HasValue || 
                            trackingInfo.OrderStatus?.Contains("delivered", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            if (!aliExpressOrder.DeliveredOnUtc.HasValue)
                            {
                                aliExpressOrder.DeliveredOnUtc = trackingInfo.DeliveredDate ?? DateTime.UtcNow;
                                
                                // Process delivery notification
                                await _trackingService.ProcessDeliveryNotificationAsync(aliExpressOrder.AliExpressOrderId.Value);
                                
                                deliveredCount++;
                            }
                        }

                        await _aliExpressOrderRepository.UpdateAsync(aliExpressOrder);

                        if (oldStatus != trackingInfo.OrderStatus)
                        {
                            await _logger.InformationAsync(
                                $"Order {aliExpressOrder.OrderId} (AE: {aliExpressOrder.AliExpressOrderId}) status changed: {oldStatus} → {trackingInfo.OrderStatus}");
                            updatedCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync($"Error tracking AliExpress order {aliExpressOrder.AliExpressOrderId}: {ex.Message}", ex);
                    
                    aliExpressOrder.LastErrorMessage = $"Tracking error: {ex.Message}";
                    aliExpressOrder.UpdatedOnUtc = DateTime.UtcNow;
                    await _aliExpressOrderRepository.UpdateAsync(aliExpressOrder);
                }
            }

            await _logger.InformationAsync(
                $"AliExpress order tracking completed. Updated: {updatedCount}, Delivered: {deliveredCount}, Total checked: {pendingOrders.Count}");
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("Error in AliExpress order tracking task", ex);
        }
    }
}
