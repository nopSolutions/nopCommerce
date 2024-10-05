using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Orders;
public class OrderStatusEventConsumer : IConsumer<OrderPlacedEvent>
{
    #region Fields
    protected readonly IOrderService _orderService;
    protected readonly IProductService _productService;

    #endregion

    #region Ctor
    public OrderStatusEventConsumer(IOrderService orderService,
        IProductService productService)
    {
        _orderService = orderService;
        _productService = productService;
    }
    #endregion

    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        var order = eventMessage.Order;
        // Check if any product requires approval
        var orderItems = await _orderService.GetOrderItemsAsync(order.Id);
        var productTasks = orderItems.Select(x => _productService.GetProductByIdAsync(x.ProductId));
        var products = await Task.WhenAll(productTasks);

        var requiresApproval = products.Any(product => product.RequireApproval);
        if (requiresApproval)
        {
            order.OrderStatusId = (int)OrderStatus.PendingApproval;
            await _orderService.UpdateOrderAsync(order);
        }
    }
}
