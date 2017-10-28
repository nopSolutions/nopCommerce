using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Widgets.GoogleAnalytics.Api;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.GoogleAnalytics
{
    public class EventConsumer : IConsumer<OrderCancelledEvent>, IConsumer<OrderPaidEvent>
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWebHelper _webHelper;
        private readonly IWidgetService _widgetService;
        private readonly WidgetSettings _widgetSettings;

        public EventConsumer(ICategoryService categoryService,
            ILogger logger,
            IProductAttributeParser productAttributeParser,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWebHelper webHelper,
            IWidgetService widgetService,
            WidgetSettings widgetSetting)
        {
            this._logger = logger;
            this._categoryService = categoryService;
            this._productAttributeParser = productAttributeParser;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._webHelper = webHelper;
            this._widgetService = widgetService;
            this._widgetSettings = widgetSetting;
        }

        private string FixIllegalJavaScriptChars(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            //replace ' with \' (http://stackoverflow.com/questions/4292761/need-to-url-encode-labels-when-tracking-events-with-google-analytics)
            text = text.Replace("'", "\\'");
            return text;
        }

        private void ProcessOrderEvent(Order order, bool add)
        {
            //ensure the plugin is installed and active
            var plugin = _widgetService.LoadWidgetBySystemName("Widgets.GoogleAnalytics") as GoogleAnalyticPlugin;
            if (plugin == null ||
                !plugin.IsWidgetActive(_widgetSettings) || !plugin.PluginDescriptor.Installed)
                return;

            try
            {
                var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
                //settings per store
                var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>(store.Id);

                if (!googleAnalyticsSettings.EnableEcommerce)
                    return;
                
                var request = new GoogleRequest
                {
                    AccountCode = googleAnalyticsSettings.GoogleId,
                    Culture = "en-US",
                    HostName = new Uri(_webHelper.GetThisPageUrl(false)).Host,
                    PageTitle = add ? "AddTransaction" : "CancelTransaction"
                };

                var orderId = order.Id.ToString(); //pass custom order number? order.CustomOrderNumber
                var orderShipping = googleAnalyticsSettings.IncludingTax ? order.OrderShippingInclTax : order.OrderShippingExclTax;
                var orderTax = order.OrderTax;
                var orderTotal = order.OrderTotal;
                if (!add)
                {
                    orderShipping = -orderShipping;
                    orderTax = -orderTax;
                    orderTotal = -orderTotal;
                }
                var trans = new Transaction(orderId,
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
                    var category = "";
                    var defaultProductCategory = _categoryService.GetProductCategoriesByProductId(item.ProductId).FirstOrDefault();
                    if (defaultProductCategory != null)
                        category = defaultProductCategory.Category.Name;

                    var unitPrice = googleAnalyticsSettings.IncludingTax ? item.UnitPriceInclTax : item.UnitPriceExclTax;
                    var qty = item.Quantity;
                    if (!add)
                        qty = -qty;

                    var product = new TransactionItem(FixIllegalJavaScriptChars(orderId), 
                      FixIllegalJavaScriptChars(item.Product.FormatSku(item.AttributesXml, _productAttributeParser)),
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
        public void HandleEvent(OrderCancelledEvent eventMessage)
        {
            ProcessOrderEvent(eventMessage.Order, false);
        }

        /// <summary>
        /// Handles the event
        /// </summary>
        /// <param name="eventMessage">The event message</param>
        public void HandleEvent(OrderPaidEvent eventMessage)
        {
            ProcessOrderEvent(eventMessage.Order, true);
        }
    }
}