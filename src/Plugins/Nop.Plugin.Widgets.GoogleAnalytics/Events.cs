using System;
using System.Linq;
using System.Net.Http;
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

        private bool IsPluginEnabled()
        {
            return _widgetPluginManager.IsPluginActive("Widgets.GoogleAnalytics");
        }

        private void ProcessOrderEvent(Order order, bool add)
        {
            try
            {
                //settings per store
                var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
                var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>(store.Id);

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

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                var trans = new Transaction(FixIllegalJavaScriptChars(orderId),
                    FixIllegalJavaScriptChars(billingAddress.City),
                    _countryService.GetCountryByAddress(billingAddress) is Country country ? FixIllegalJavaScriptChars(country.Name) : string.Empty,
                    _stateProvinceService.GetStateProvinceByAddress(billingAddress) is StateProvince stateProvince ? FixIllegalJavaScriptChars(stateProvince.Name) : string.Empty,
                    store.Name,
                    orderShipping,
                    orderTax,
                    orderTotal);

                foreach (var item in _orderService.GetOrderItems(order.Id))
                {
                    var product = _productService.GetProductById(item.ProductId);
                    //get category
                    var category = _categoryService.GetCategoryById(_categoryService.GetProductCategoriesByProductId(product.Id).FirstOrDefault()?.CategoryId ?? 0)?.Name;
                    var unitPrice = googleAnalyticsSettings.IncludingTax ? item.UnitPriceInclTax : item.UnitPriceExclTax;
                    var qty = item.Quantity;
                    if (!add)
                        qty = -qty;

                    var sku = _productService.FormatSku(product, item.AttributesXml);
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

                request.SendRequest(trans, _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient));
            }
            catch (Exception ex)
            {
                _logger.InsertLog(LogLevel.Error, "Google Analytics. Error canceling transaction from server side", ex.ToString());
            }
        }

        /// <summary>
        /// Handles the event
        /// </summary>
        /// <param name="eventMessage">The event message</param>
        public void HandleEvent(EntityDeletedEvent<Order> eventMessage)
        {
            //ensure the plugin is installed and active
            if (!IsPluginEnabled())
                return;

            var order = eventMessage.Entity;

            //settings per store
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>(store.Id);

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
                ProcessOrderEvent(order, false);
        }

        /// <summary>
        /// Handles the event
        /// </summary>
        /// <param name="eventMessage">The event message</param>
        public void HandleEvent(OrderCancelledEvent eventMessage)
        {
            //ensure the plugin is installed and active
            if (!IsPluginEnabled())
                return;

            var order = eventMessage.Order;

            //settings per store
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>(store.Id);

            //ecommerce is disabled
            if (!googleAnalyticsSettings.EnableEcommerce)
                return;

            //if we use JS to notify GA about new orders (even when they are placed), then we should always notify GA about deleted orders
            //if we use HTTP requests to notify GA about new orders (only when they are paid), then we should notify GA about deleted AND paid orders
            var sendRequest = googleAnalyticsSettings.UseJsToSendEcommerceInfo || order.PaymentStatus == PaymentStatus.Paid;

            if (sendRequest)
                ProcessOrderEvent(order, false);
        }

        /// <summary>
        /// Handles the event
        /// </summary>
        /// <param name="eventMessage">The event message</param>
        public void HandleEvent(OrderPaidEvent eventMessage)
        {
            //ensure the plugin is installed and active
            if (!IsPluginEnabled())
                return;

            var order = eventMessage.Order;

            //settings per store
            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>(store.Id);

            //ecommerce is disabled
            if (!googleAnalyticsSettings.EnableEcommerce)
                return;

            //we use HTTP requests to notify GA about new orders (only when they are paid)
            var sendRequest = !googleAnalyticsSettings.UseJsToSendEcommerceInfo;

            if (sendRequest)
                ProcessOrderEvent(order, true);
        }
    }
}