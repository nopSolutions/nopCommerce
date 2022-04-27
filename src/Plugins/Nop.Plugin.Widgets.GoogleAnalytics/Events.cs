using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Plugin.Widgets.GoogleAnalytics.Api;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.GoogleAnalytics
{
    public class EventConsumer : IConsumer<OrderCancelledEvent>, IConsumer<OrderPaidEvent>, IConsumer<EntityDeletedEvent<Order>>
    {
        private readonly IAddressService _addressService;
        private readonly ICategoryService _categoryService;
        private readonly ICountryService _countryService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWebHelper _webHelper;
        private readonly IWidgetPluginManager _widgetPluginManager;

        public EventConsumer(IAddressService addressService,
            ICategoryService categoryService,
            ICountryService countryService,
            IHttpClientFactory httpClientFactory,
            ILogger logger,
            IOrderService orderService,
            IProductService productService,
            ISettingService settingService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWebHelper webHelper,
            IWidgetPluginManager widgetPluginManager)
        {
            _addressService = addressService;
            _categoryService = categoryService;
            _countryService = countryService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _orderService = orderService;
            _productService = productService;
            _settingService = settingService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _storeService = storeService;
            _webHelper = webHelper;
            _widgetPluginManager = widgetPluginManager;
        }

        private string FixIllegalJavaScriptChars(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            //replace ' with \' (http://stackoverflow.com/questions/4292761/need-to-url-encode-labels-when-tracking-events-with-google-analytics)
            text = text.Replace("'", "\\'");
            return text;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<bool> IsPluginEnabledAsync()
        {
            return await _widgetPluginManager.IsPluginActiveAsync("Widgets.GoogleAnalytics");
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ProcessOrderEventAsync(Order order, bool add)
        {
            try
            {
                //settings per store
                var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
                var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);

                var request = new GoogleRequest
                {
                    AccountCode = googleAnalyticsSettings.GoogleId,
                    Culture = "en-US",
                    HostName = new Uri(_webHelper.GetThisPageUrl(false)).Host,
                    PageTitle = add ? "AddTransaction" : "CancelTransaction"
                };

                var orderId = order.CustomOrderNumber;
                var orderShipping = googleAnalyticsSettings.IncludingTax ? order.OrderShippingInclTax : order.OrderShippingExclTax;
                var orderTax = order.OrderTax;
                var orderTotal = order.OrderTotal;
                if (!add)
                {
                    orderShipping = -orderShipping;
                    orderTax = -orderTax;
                    orderTotal = -orderTotal;
                }

                var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

                var trans = new Transaction(FixIllegalJavaScriptChars(orderId),
                    FixIllegalJavaScriptChars(billingAddress.City),
                    await _countryService.GetCountryByAddressAsync(billingAddress) is Country country ? FixIllegalJavaScriptChars(country.Name) : string.Empty,
                    await _stateProvinceService.GetStateProvinceByAddressAsync(billingAddress) is StateProvince stateProvince ? FixIllegalJavaScriptChars(stateProvince.Name) : string.Empty,
                    store.Name,
                    orderShipping,
                    orderTax,
                    orderTotal);

                foreach (var item in await _orderService.GetOrderItemsAsync(order.Id))
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    //get category
                    var category = (await _categoryService.GetCategoryByIdAsync((await _categoryService.GetProductCategoriesByProductIdAsync(product.Id)).FirstOrDefault()?.CategoryId ?? 0))?.Name;
                    if (string.IsNullOrEmpty(category))
                        category = "No category";

                    var unitPrice = googleAnalyticsSettings.IncludingTax ? item.UnitPriceInclTax : item.UnitPriceExclTax;
                    var qty = item.Quantity;
                    if (!add)
                        qty = -qty;

                    var sku = await _productService.FormatSkuAsync(product, item.AttributesXml);
                    if (string.IsNullOrEmpty(sku))
                        sku = product.Id.ToString();

                    var productItem = new TransactionItem(FixIllegalJavaScriptChars(orderId),
                      FixIllegalJavaScriptChars(sku),
                      FixIllegalJavaScriptChars(product.Name),
                      unitPrice,
                      qty,
                      FixIllegalJavaScriptChars(category));

                    trans.Items.Add(productItem);
                }

                await request.SendRequest(trans, _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient));
            }
            catch (Exception ex)
            {
                await _logger.InsertLogAsync(LogLevel.Error, "Google Analytics. Error canceling transaction from server side", ex.ToString());
            }
        }

        /// <summary>
        /// Handles the event
        /// </summary>
        /// <param name="eventMessage">The event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Order> eventMessage)
        {
            //ensure the plugin is installed and active
            if (!await IsPluginEnabledAsync())
                return;

            var order = eventMessage.Entity;

            //settings per store
            var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
            var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);

            //ecommerce is disabled
            if (!googleAnalyticsSettings.EnableEcommerce)
                return;

            bool sendRequest;
            if (googleAnalyticsSettings.UseJsToSendEcommerceInfo)
            {
                //if we use JS to notify GA about new orders (even when they are placed), then we should always notify GA about deleted orders
                //but ignore already cancelled orders (do not duplicate request to GA)
                sendRequest = order.OrderStatus != OrderStatus.Cancelled;
            }
            else
            {
                //if we use HTTP requests to notify GA about new orders (only when they are paid), then we should notify GA about deleted AND paid orders
                sendRequest = order.PaymentStatus == PaymentStatus.Paid;
            }

            if (sendRequest)
                await ProcessOrderEventAsync(order, false);
        }

        /// <summary>
        /// Handles the event
        /// </summary>
        /// <param name="eventMessage">The event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderCancelledEvent eventMessage)
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

            //if we use JS to notify GA about new orders (even when they are placed), then we should always notify GA about deleted orders
            //if we use HTTP requests to notify GA about new orders (only when they are paid), then we should notify GA about deleted AND paid orders
            var sendRequest = googleAnalyticsSettings.UseJsToSendEcommerceInfo || order.PaymentStatus == PaymentStatus.Paid;

            if (sendRequest)
                await ProcessOrderEventAsync(order, false);
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
            var sendRequest = !googleAnalyticsSettings.UseJsToSendEcommerceInfo;

            if (sendRequest)
                await ProcessOrderEventAsync(order, true);
        }
    }
}