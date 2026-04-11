using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Orders;

/// <summary>
/// Represents a task for automatically cancelling unpaid orders
/// </summary>
public partial class CancelUnpaidOrdersTask : IScheduleTask
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly ILogger _logger;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderService _orderService;
    protected readonly IProductService _productService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly OrderSettings _orderSettings;

    #endregion

    #region Ctor

    public CancelUnpaidOrdersTask(ICustomerService customerService,
        ILogger logger,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IProductService productService,
        IShoppingCartService shoppingCartService,
        OrderSettings orderSettings)
    {
        _customerService = customerService;
        _logger = logger;
        _orderProcessingService = orderProcessingService;
        _orderService = orderService;
        _productService = productService;
        _shoppingCartService = shoppingCartService;
        _orderSettings = orderSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
        // Exit if the feature is disabled
        if (!_orderSettings.AutoCancelUnpaidOrdersEnabled)
            return;

        // Calculate the cutoff time
        var delay = _orderSettings.AutoCancelUnpaidOrdersDelay;
        if (delay <= 0)
            delay = 600; // Default to 600 minutes if invalid

        var cutoffTimeUtc = DateTime.UtcNow.AddMinutes(-delay);

        // Parse ignored payment methods
        var ignoredPaymentMethods = new List<string>();
        if (!string.IsNullOrWhiteSpace(_orderSettings.IgnorePaymentMethods))
        {
            ignoredPaymentMethods = _orderSettings.IgnorePaymentMethods
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
        }

        // Search for orders with pending payment status created before the cutoff time
        var pendingOrders = await _orderService.SearchOrdersAsync(
            createdToUtc: cutoffTimeUtc,
            psIds: new List<int> { (int)PaymentStatus.Pending },
            osIds: null, // Don't filter by order status initially
            pageIndex: 0,
            pageSize: int.MaxValue);

        var cancelledCount = 0;
        var failedCount = 0;

        foreach (var order in pendingOrders)
        {
            try
            {
                // Skip already cancelled orders
                if (order.OrderStatus == OrderStatus.Cancelled)
                    continue;

                // Skip orders with ignored payment methods
                if (ignoredPaymentMethods.Any() &&
                    !string.IsNullOrWhiteSpace(order.PaymentMethodSystemName) &&
                    ignoredPaymentMethods.Contains(order.PaymentMethodSystemName, StringComparer.OrdinalIgnoreCase))
                    continue;

                // Cancel the order and notify the customer
                await _orderProcessingService.CancelOrderAsync(order, notifyCustomer: true);

                // Restore shopping cart if enabled
                if (_orderSettings.RestoreCartAfterCancellation)
                {
                    await RestoreShoppingCartAsync(order);
                }

                cancelledCount++;
            }
            catch (Exception ex)
            {
                failedCount++;
                await _logger.ErrorAsync($"Error cancelling unpaid order #{order.Id}", ex);
            }
        }

        // Log summary
        if (cancelledCount > 0 || failedCount > 0)
        {
            await _logger.InformationAsync($"Auto-cancel unpaid orders task completed. Cancelled: {cancelledCount}, Failed: {failedCount}");
        }
    }

    /// <summary>
    /// Restores shopping cart items from a cancelled order
    /// </summary>
    /// <param name="order">The cancelled order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task RestoreShoppingCartAsync(Order order)
    {
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
        if (customer == null)
            return;

        var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

        foreach (var orderItem in orderItems)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                if (product == null || product.Deleted)
                    continue;

                // Add the product back to the shopping cart
                await _shoppingCartService.AddToCartAsync(
                    customer: customer,
                    product: product,
                    shoppingCartType: ShoppingCartType.ShoppingCart,
                    storeId: order.StoreId,
                    attributesXml: orderItem.AttributesXml,
                    quantity: orderItem.Quantity,
                    customerEnteredPrice: orderItem.UnitPriceInclTax,
                    rentalStartDate: orderItem.RentalStartDateUtc,
                    rentalEndDate: orderItem.RentalEndDateUtc);
            }
            catch (Exception ex)
            {
                // Log the error but continue processing other items
                await _logger.WarningAsync($"Failed to restore order item #{orderItem.Id} to shopping cart for customer #{customer.Id}", ex);
            }
        }
    }

    #endregion
}
