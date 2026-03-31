using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Orders;

public partial class AutoCancelPaidOrdersTask : IScheduleTask
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

    public AutoCancelPaidOrdersTask(ICustomerService customerService,
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
        if (!_orderSettings.AutoCancelUnpaidOrdersEnabled)
            return;

        var orders = await _orderService.SearchOrdersAsync(
            osIds: [(int)OrderStatus.Pending],
            createdToUtc: DateTime.UtcNow.AddMinutes(-_orderSettings.AutoCancelUnpaidOrdersDelay));

        var ordersToCancel = orders
            .Where(_orderProcessingService.CanCancelOrder)
            .Where(o => !_orderSettings.AutoCancelIgnoredPaymentMethods.Contains(o.PaymentMethodSystemName))
            .GroupBy(o => o.CustomerId, o => o)
            .Select(g => g.OrderByDescending(o => o.CreatedOnUtc).First())
            .ToList();

        foreach (var order in ordersToCancel)
        {
            await _orderProcessingService.CancelOrderAsync(order, true);

            if (!_orderSettings.PutAutoCanceledOrderToShoppingCart)
                continue;

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            foreach (var item in await _orderService.GetOrderItemsAsync(order.Id))
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);

                var addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: customer,
                    product: product,
                    shoppingCartType: ShoppingCartType.ShoppingCart,
                    storeId: order.StoreId,
                    attributesXml: item.AttributesXml,
                    rentalStartDate: item.RentalStartDateUtc,
                    rentalEndDate: item.RentalEndDateUtc,
                    quantity: item.Quantity);

                if (addToCartWarnings?.Count > 0)
                    await _logger.WarningAsync(addToCartWarnings.Aggregate((c, n) => c + Environment.NewLine + n));
            }
        }
    }

    #endregion
}
