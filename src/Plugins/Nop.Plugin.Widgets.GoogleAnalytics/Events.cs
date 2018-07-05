using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Events;
using Nop.Plugin.Widgets.GoogleAnalytics.Api;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.GoogleAnalytics
{
    public class EventConsumer : IConsumer<OrderCancelledEvent>, IConsumer<OrderPaidEvent>, IConsumer<EntityDeletedEvent<Order>>
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWebHelper _webHelper;
        private readonly IWidgetService _widgetService;

        public EventConsumer(ICategoryService categoryService,
            ILogger logger,
            IProductService productService,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWebHelper webHelper,
            IWidgetService widgetService)
        {
            this._logger = logger;
            this._categoryService = categoryService;
            this._productService = productService;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._webHelper = webHelper;
            this._widgetService = widgetService;
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
            var plugin = _widgetService.LoadWidgetBySystemName("Widgets.GoogleAnalytics") as GoogleAnalyticsPlugin;
            return plugin != null && _widgetService.IsWidgetActive(plugin) && plugin.PluginDescriptor.Installed;
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
                var trans = new Transaction(FixIllegalJavaScriptChars(orderId),
                    order.BillingAddress == null ? "" : FixIllegalJavaScriptChars(order.BillingAddress.City),
                    order.BillingAddress == null || order.BillingAddress.Country == null ? "" : FixIllegalJavaScriptChars(order.BillingAddress.Country.Name),
                    order.BillingAddress == null || order.BillingAddress.StateProvince == null ? "" : FixIllegalJavaScriptChars(order.BillingAddress.StateProvince.Name),
                    store.Name,
                    orderShipping,
                    orderTax,
                    orderTotal);

                foreach (var item in order.OrderItems)
                {
                    //get category
                    var category = _categoryService.GetProductCategoriesByProductId(item.ProductId).FirstOrDefault()?.Category?.Name;

                    var unitPrice = googleAnalyticsSettings.IncludingTax ? item.UnitPriceInclTax : item.UnitPriceExclTax;
                    var qty = item.Quantity;
                    if (!add)
                        qty = -qty;

                    var sku = _productService.FormatSku(item.Product, item.AttributesXml);
                    if (String.IsNullOrEmpty(sku))
                        sku = item.Product.Id.ToString();
                    var product = new TransactionItem(FixIllegalJavaScriptChars(orderId), 
                      FixIllegalJavaScriptChars(sku),
                      FixIllegalJavaScriptChars(item.Product.Name),
                      unitPrice,
                      qty,
                      FixIllegalJavaScriptChars(category));

                    trans.Items.Add(product);
                }

                request.SendRequest(trans);
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
            bool sendRequest = googleAnalyticsSettings.UseJsToSendEcommerceInfo || order.PaymentStatus == PaymentStatus.Paid;

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
            bool sendRequest = !googleAnalyticsSettings.UseJsToSendEcommerceInfo;

            if (sendRequest)
                ProcessOrderEvent(order, true);
        }
    }
}