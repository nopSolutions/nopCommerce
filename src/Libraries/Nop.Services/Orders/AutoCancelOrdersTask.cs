using System.Text;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;
using Nop.Services.Stores;

namespace Nop.Services.Orders;

public partial class AutoCancelOrdersTask : IScheduleTask
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly ILogger _logger;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderService _orderService;
    protected readonly IProductService _productService;
    protected readonly ISettingService _settingService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public AutoCancelOrdersTask(ICustomerService customerService,
        ILogger logger,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IProductService productService,
        ISettingService settingService,
        IShoppingCartService shoppingCartService,
        IStoreService storeService)
    {
        _customerService = customerService;
        _logger = logger;
        _orderProcessingService = orderProcessingService;
        _orderService = orderService;
        _productService = productService;
        _settingService = settingService;
        _shoppingCartService = shoppingCartService;
        _storeService = storeService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
        var stores = await _storeService.GetAllStoresAsync();
        foreach (var store in stores)
        {
            var orderSettings = await _settingService.LoadSettingAsync<OrderSettings>(store.Id);

            if (!orderSettings.AutoCancelEnabled)
                continue;

            var orders = await _orderService.SearchOrdersAsync(
                storeId: store.Id,
                psIds: [(int)PaymentStatus.Pending],
                osIds: [(int)OrderStatus.Pending, (int)OrderStatus.Processing],
                createdFromUtc: orderSettings.AutoCancelIgnoreBeforeUtc,
                createdToUtc: DateTime.UtcNow.AddMinutes(-orderSettings.AutoCancelDelay));

            var ordersToCancel = orders
                .Where(_orderProcessingService.CanCancelOrder)
                .Where(o => !orderSettings.AutoCancelIgnoredPaymentMethods.Contains(o.PaymentMethodSystemName))
                .GroupBy(o => o.CustomerId, o => o)
                .Select(g => new { CustomerId = g.Key, Orders = g.OrderByDescending(o => o.CreatedOnUtc).ToList() })
                .ToList();

            foreach (var customerOrders in ordersToCancel)
            {
                var customer = await _customerService.GetCustomerByIdAsync(customerOrders.CustomerId);
                var shoppingCartIsRestored = false;

                foreach (var order in customerOrders.Orders)
                {
                    await _orderProcessingService.CancelOrderAsync(order, true);

                    if (!orderSettings.AutoCancelRestoreShoppingCart || shoppingCartIsRestored)
                        continue;

                    var warnings = new StringBuilder();

                    foreach (var item in await _orderService.GetOrderItemsAsync(order.Id))
                    {
                        var product = await _productService.GetProductByIdAsync(item.ProductId);

                        if (product is null)
                            continue;

                        var addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: customer,
                            product: product,
                            shoppingCartType: ShoppingCartType.ShoppingCart,
                            storeId: order.StoreId,
                            attributesXml: item.AttributesXml,
                            rentalStartDate: item.RentalStartDateUtc,
                            rentalEndDate: item.RentalEndDateUtc,
                            quantity: item.Quantity,
                            addRequiredProducts: false);

                        if (addToCartWarnings?.Count > 0)
                            warnings.AppendLine(addToCartWarnings.Aggregate((c, n) => c + Environment.NewLine + n));
                    }

                    if (warnings.Length > 0)
                        await _logger.WarningAsync($"Errors occurred during shopping cart restoration:{Environment.NewLine} {warnings}");

                    shoppingCartIsRestored = true;
                }
            }
        }
    }

    #endregion
}
