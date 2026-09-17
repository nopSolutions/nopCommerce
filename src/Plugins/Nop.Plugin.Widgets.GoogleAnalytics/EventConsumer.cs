using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Stores;
using Nop.Core.Events;
using Nop.Plugin.Widgets.GoogleAnalytics.Api;
using Nop.Plugin.Widgets.GoogleAnalytics.Api.Models;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tax;

namespace Nop.Plugin.Widgets.GoogleAnalytics;

public class EventConsumer :
    IConsumer<OrderPlacedEvent>,
    IConsumer<OrderPaidEvent>,
    IConsumer<OrderRefundedEvent>,
    IConsumer<EntityInsertedEvent<ShoppingCartItem>>,
    IConsumer<EntityDeletedEvent<ShoppingCartItem>>
{
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly GoogleAnalyticsHttpClient _googleAnalyticsHttpClient;
    protected readonly ICategoryService _categoryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILogger _logger;
    protected readonly IOrderService _orderService;
    protected readonly IProductService _productService;
    protected readonly ISettingService _settingService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly ITaxService _taxService;
    protected readonly IWidgetPluginManager _widgetPluginManager;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public EventConsumer(
        CurrencySettings currencySettings,
        GoogleAnalyticsHttpClient googleAnalyticsHttpClient,
        ICategoryService categoryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        ILogger logger,
        IOrderService orderService,
        IProductService productService,
        ISettingService settingService,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IStoreService storeService,
        ITaxService taxService,
        IWidgetPluginManager widgetPluginManager,
        IWorkContext workContext)
    {
        _currencySettings = currencySettings;
        _googleAnalyticsHttpClient = googleAnalyticsHttpClient;
        _categoryService = categoryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _orderService = orderService;
        _productService = productService;
        _settingService = settingService;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _storeService = storeService;
        _taxService = taxService;
        _widgetPluginManager = widgetPluginManager;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Indicates whether plugin is enabled
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the value indicating whether plugin is enabled
    /// </returns>
    protected async Task<bool> IsPluginEnabledAsync()
    {
        return await _widgetPluginManager.IsPluginActiveAsync(GoogleAnalyticsDefaults.SystemName);
    }

    /// <summary>
    /// Saves cookies to order and generic attributes.
    /// </summary>
    /// <param name="order">Order to save cookies for</param>
    /// <param name="googleAnalyticsSettings">Google Analytics settings</param>
    /// <param name="store">Store to save cookies for</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task SaveCookiesAsync(Order order, GoogleAnalyticsSettings googleAnalyticsSettings, Store store)
    {
        //try to get cookie
        var httpContext = _httpContextAccessor.HttpContext;

        //client_id
        httpContext.Request.Cookies.TryGetValue(GoogleAnalyticsDefaults.ClientIdCookiesName, out var clientId);
        await _genericAttributeService.SaveAttributeAsync(order, GoogleAnalyticsDefaults.ClientIdAttribute, clientId, store.Id);

        //session_id
        var measurementId = googleAnalyticsSettings.GoogleId.Split('-')[1];
        var sessionCookieKey = $"{GoogleAnalyticsDefaults.SessionIdCookiesName}{measurementId}";
        httpContext.Request.Cookies.TryGetValue(sessionCookieKey, out var sessionId);
        await _genericAttributeService.SaveAttributeAsync(order, GoogleAnalyticsDefaults.SessionIdAttribute, sessionId, store.Id);
    }

    /// <summary>
    /// Process order event
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="googleAnalyticsSettings">Google Analytics settings</param>
    /// <param name="eventName">Event name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task ProcessOrderEventAsync(Order order, GoogleAnalyticsSettings googleAnalyticsSettings, string eventName)
    {
        try
        {
            var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
            var currency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            var orderId = order.CustomOrderNumber;
            var orderShipping = googleAnalyticsSettings.IncludingTax ? order.OrderShippingInclTax : order.OrderShippingExclTax;
            var orderTax = order.OrderTax;
            var orderTotal = order.OrderTotal;

            var gaRequest = new EventRequest
            {
                ClientId = await _genericAttributeService.GetAttributeAsync<string>(order, GoogleAnalyticsDefaults.ClientIdAttribute, store.Id),
                UserId = order.CustomerId.ToString(),
                TimestampMicros = (DateTimeOffset.UtcNow - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).Ticks / 10
            };

            var events = new List<Event>();
            var gaEvent = new Event
            {
                Name = eventName
            };
            events.Add(gaEvent);

            var gaParams = new Parameters
            {
                Currency = currency,
                TransactionId = orderId,
                EngagementTime = 100,
                SessionId = await _genericAttributeService.GetAttributeAsync<string>(order, GoogleAnalyticsDefaults.SessionIdAttribute, store.Id),
                Value = orderTotal,
                Tax = orderTax,
                Shipping = orderShipping
            };

            var items = new List<Item>();
            foreach (var item in await _orderService.GetOrderItemsAsync(order.Id))
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                var sku = await _productService.FormatSkuAsync(product, item.AttributesXml);

                if (string.IsNullOrEmpty(sku))
                    sku = product.Id.ToString();


                var unitPrice = googleAnalyticsSettings.IncludingTax ? item.UnitPriceInclTax : item.UnitPriceExclTax;

                var gaItem = new Item
                {
                    ItemId = sku,
                    ItemName = product.Name,
                    Affiliation = store.Name,
                    ItemCategory = await _categoryService.GetCategoryNameForProductAsync(product.Id),
                    Price = unitPrice,
                    Quantity = item.Quantity
                };

                items.Add(gaItem);
            }

            gaParams.Items = items;
            gaEvent.Params = gaParams;
            gaRequest.Events = events;

            await _googleAnalyticsHttpClient.RequestAsync(gaRequest, googleAnalyticsSettings);
        }
        catch (Exception ex)
        {
            await _logger.InsertLogAsync(LogLevel.Error, "Google Analytics. Error processing order from server side", ex.ToString());
        }
    }

    /// <summary>
    /// Process shopping cart event
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <param name="store">Store</param>
    /// <param name="googleAnalyticsSettings">Google Analytics settings</param>
    /// <param name="add">The flag indicating whether the item is added or removed</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task ProcessShoppingCartEventAsync(ShoppingCartItem shoppingCartItem, Store store, GoogleAnalyticsSettings googleAnalyticsSettings, bool add)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);
            var currency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

            var unitPrice = decimal.Zero;//ICustomerService.GetShoppingCartCustomerAsync()

            if (product is { CallForPrice: false })
            {
                var customer =
                    await _customerService.GetShoppingCartCustomerAsync(
                        new List<ShoppingCartItem> { shoppingCartItem });

                var sciUnitPrice = await _shoppingCartService.GetUnitPriceAsync(shoppingCartItem, true);
                var productPrice = await _taxService.GetProductPriceAsync(product, price: sciUnitPrice.unitPrice,
                    includingTax: googleAnalyticsSettings.IncludingTax, customer: customer);

                unitPrice = productPrice.price;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Request.Cookies.TryGetValue(GoogleAnalyticsDefaults.ClientIdCookiesName, out var clientId);

            var gaRequest = new EventRequest
            {
                ClientId = clientId,
                UserId = shoppingCartItem.CustomerId.ToString(),
                TimestampMicros = (DateTimeOffset.UtcNow - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).Ticks / 10
            };

            string eventName;

            if (shoppingCartItem.ShoppingCartType == ShoppingCartType.ShoppingCart)
                eventName = add ? GoogleAnalyticsDefaults.AddToCartEventName : GoogleAnalyticsDefaults.RemoveFromCartEventName;
            else
                eventName = add ? GoogleAnalyticsDefaults.AddToWishlistEventName : GoogleAnalyticsDefaults.RemoveFromWishlistEventName;

            var events = new List<Event>();
            var gaEvent = new Event
            {
                Name = eventName
            };
            events.Add(gaEvent);

            var measurementId = googleAnalyticsSettings.GoogleId.Split('-')[1];
            var sessionCookieKey = $"{GoogleAnalyticsDefaults.SessionIdCookiesName}{measurementId}";
            httpContext.Request.Cookies.TryGetValue(sessionCookieKey, out var sessionId);

            var items = new List<Item>();
            var sku = await _productService.FormatSkuAsync(product, shoppingCartItem.AttributesXml);

            if (string.IsNullOrEmpty(sku))
                sku = product.Id.ToString();

            var gaItem = new Item
            {
                ItemId = sku,
                ItemName = product.Name,
                Affiliation = store.Name,
                ItemCategory = await _categoryService.GetCategoryNameForProductAsync(shoppingCartItem.ProductId),
                Price = unitPrice,
                Quantity = shoppingCartItem.Quantity
            };

            items.Add(gaItem);

            var subTotal = await _shoppingCartService.GetSubTotalAsync(shoppingCartItem, true);

            var gaParams = new Parameters
            {
                Currency = currency,
                TransactionId = shoppingCartItem.Id.ToString(),
                EngagementTime = 100,
                SessionId = sessionId,
                Value = Math.Round(subTotal.subTotal, 2),
                
                Items = items
            };

            gaEvent.Params = gaParams;
            gaRequest.Events = events;

            await _googleAnalyticsHttpClient.RequestAsync(gaRequest, googleAnalyticsSettings);
        }
        catch (Exception ex)
        {
            await _logger.InsertLogAsync(LogLevel.Error, "Google Analytics. Error processing shopping cart from server side", ex.ToString());
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the event
    /// </summary>
    /// <param name="eventMessage">The event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderRefundedEvent eventMessage)
    {
        //ensure the plugin is installed and active
        if (!await IsPluginEnabledAsync())
            return;

        var order = eventMessage.Order;

        //settings per store
        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);

        //ecommerce is disabled
        if (!googleAnalyticsSettings.EnableEcommerce)
            return;

        //if we use HTTP requests to notify GA about new orders (only when they are paid), then we should notify GA about deleted AND paid orders
        var sendRequest = order.PaymentStatus == PaymentStatus.Paid;

        if (sendRequest)
            await ProcessOrderEventAsync(order, googleAnalyticsSettings, GoogleAnalyticsDefaults.OrderRefundedEventName);
    }

    /// <summary>
    /// Handles the event
    /// </summary>
    /// <param name="eventMessage">The event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPaidEvent eventMessage)
    {
        //ensure the plugin is installed and active
        if (!await IsPluginEnabledAsync())
            return;

        var order = eventMessage.Order;

        //settings per store
        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);

        //ecommerce is disabled
        if (!googleAnalyticsSettings.EnableEcommerce)
            return;

        //we use HTTP requests to notify GA about new orders (only when they are paid)
        await ProcessOrderEventAsync(order, googleAnalyticsSettings, GoogleAnalyticsDefaults.OrderPaidEventName);
    }

    /// <summary>
    /// Handles the event
    /// </summary>
    /// <param name="eventMessage">The event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        //ensure the plugin is installed and active
        if (!await IsPluginEnabledAsync())
            return;

        var order = eventMessage.Order;

        //settings per store
        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);

        //ecommerce is disabled
        if (!googleAnalyticsSettings.EnableEcommerce)
            return;

        await SaveCookiesAsync(order, googleAnalyticsSettings, store);
    }

    /// <summary>
    /// Handles the event
    /// </summary>
    /// <param name="eventMessage">The event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<ShoppingCartItem> eventMessage)
    {
        //ensure the plugin is installed and active
        if (!await IsPluginEnabledAsync())
            return;

        var shoppingCartItem = eventMessage.Entity;

        //settings per store
        var store = await _storeService.GetStoreByIdAsync(shoppingCartItem.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);

        if (eventMessage.Entity != null)
            await ProcessShoppingCartEventAsync(shoppingCartItem, store, googleAnalyticsSettings, false);
    }

    /// <summary>
    /// Handles the event
    /// </summary>
    /// <param name="eventMessage">The event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
    {
        //ensure the plugin is installed and active
        if (!await IsPluginEnabledAsync())
            return;

        var shoppingCartItem = eventMessage.Entity;

        //settings per store
        var store = await _storeService.GetStoreByIdAsync(shoppingCartItem.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);

        if (eventMessage.Entity != null)
            await ProcessShoppingCartEventAsync(shoppingCartItem, store, googleAnalyticsSettings, true);
    }

    #endregion
}