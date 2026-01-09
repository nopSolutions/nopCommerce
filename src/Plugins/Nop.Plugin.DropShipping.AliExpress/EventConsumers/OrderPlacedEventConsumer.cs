using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.DropShipping.AliExpress.Domain;
using Nop.Plugin.DropShipping.AliExpress.Services;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Orders;


namespace Nop.Plugin.DropShipping.AliExpress.EventConsumers;

/// <summary>
/// Event consumer for order placement
/// </summary>
public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IOrderService _orderService;
    private readonly IAliExpressService _aliExpressService;
    private readonly IAliExpressProductMappingService _mappingService;
    private readonly IRepository<AliExpressOrder> _aliExpressOrderRepository;
    private readonly ILogger _logger;
    private readonly AliExpressSettings _settings;

    public OrderPlacedEventConsumer(
        IOrderService orderService,
        IAliExpressService aliExpressService,
        IAliExpressProductMappingService mappingService,
        IRepository<AliExpressOrder> aliExpressOrderRepository,
        ILogger logger,
        AliExpressSettings settings)
    {
        _orderService = orderService;
        _aliExpressService = aliExpressService;
        _mappingService = mappingService;
        _aliExpressOrderRepository = aliExpressOrderRepository;
        _logger = logger;
        _settings = settings;
    }

    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        if (!_settings.AutoCreateOrders)
            return;

        try
        {
            var order = eventMessage.Order;
            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

            foreach (var orderItem in orderItems)
            {
                // Check if this product has an AliExpress mapping
                var mapping = await _mappingService.GetMappingByProductIdAsync(orderItem.ProductId);
                
                if (mapping == null)
                    continue;

                // Check if we already created an AliExpress order for this
                var existingAliExpressOrder = await _aliExpressOrderRepository.Table
                    .FirstOrDefaultAsync(ao => ao.OrderId == order.Id && ao.AliExpressProductId == mapping.AliExpressProductId);

                if (existingAliExpressOrder != null)
                    continue;

                // Create AliExpress order record
                var aliExpressOrder = new AliExpressOrder
                {
                    OrderId = order.Id,
                    AliExpressProductId = mapping.AliExpressProductId,
                    AliExpressOrderStatus = "Pending",
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                };

                await _aliExpressOrderRepository.InsertAsync(aliExpressOrder);

                // Attempt to create order on AliExpress
                try
                {
                    var aliExpressOrderId = await _aliExpressService.CreateOrderAsync(order.Id);
                    
                    if (aliExpressOrderId.HasValue)
                    {
                        aliExpressOrder.AliExpressOrderId = aliExpressOrderId.Value;
                        aliExpressOrder.AliExpressOrderStatus = "Created";
                        aliExpressOrder.PlacedOnUtc = DateTime.UtcNow;
                        await _aliExpressOrderRepository.UpdateAsync(aliExpressOrder);

                        await _logger.InformationAsync($"Created AliExpress order {aliExpressOrderId} for NopCommerce order {order.Id}");
                    }
                    else
                    {
                        aliExpressOrder.LastErrorMessage = "Failed to create order on AliExpress";
                        await _aliExpressOrderRepository.UpdateAsync(aliExpressOrder);
                        
                        await _logger.WarningAsync($"Failed to create AliExpress order for NopCommerce order {order.Id}");
                    }
                }
                catch (Exception ex)
                {
                    aliExpressOrder.LastErrorMessage = ex.Message;
                    await _aliExpressOrderRepository.UpdateAsync(aliExpressOrder);
                    
                    await _logger.ErrorAsync($"Error creating AliExpress order for NopCommerce order {order.Id}: {ex.Message}", ex);
                }
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error in OrderPlacedEventConsumer: {ex.Message}", ex);
        }
    }
}
