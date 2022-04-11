using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Components
{
    public class WidgetsGoogleAnalyticsViewComponent : NopViewComponent
    {
        #region Fields

        private const string ORDER_ALREADY_PROCESSED_ATTRIBUTE_NAME = "GoogleAnalytics.OrderAlreadyProcessed";

        private readonly CurrencySettings _currencySettings;
        private readonly GoogleAnalyticsSettings _googleAnalyticsSettings;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WidgetsGoogleAnalyticsViewComponent(CurrencySettings currencySettings,
            GoogleAnalyticsSettings googleAnalyticsSettings,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILogger logger,
            IOrderService orderService,
            IProductService productService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _currencySettings = currencySettings;
            _googleAnalyticsSettings = googleAnalyticsSettings;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _logger = logger;
            _orderService = orderService;
            _productService = productService;
            _settingService = settingService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        private string FixIllegalJavaScriptChars(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            //replace ' with \' (http://stackoverflow.com/questions/4292761/need-to-url-encode-labels-when-tracking-events-with-google-analytics)
            text = text.Replace("'", "\\'");
            return text;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<Order> GetLastOrderAsync()
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var order = (await _orderService.SearchOrdersAsync(storeId: store.Id,
                customerId: customer.Id, pageSize: 1)).FirstOrDefault();
            return order;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<string> GetEcommerceScriptAsync(Order order)
        {
            var analyticsTrackingScript = _googleAnalyticsSettings.TrackingScript + "\n";
            analyticsTrackingScript = analyticsTrackingScript.Replace("{GOOGLEID}", _googleAnalyticsSettings.GoogleId);
            //remove {ECOMMERCE} (used in previous versions of the plugin)
            analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE}", "");
            //remove {CustomerID} (used in previous versions of the plugin)
            analyticsTrackingScript = analyticsTrackingScript.Replace("{CustomerID}", "");

            //whether to include customer identifier
            var customerIdCode = string.Empty;
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (_googleAnalyticsSettings.IncludeCustomerId && !await _customerService.IsGuestAsync(customer))
                customerIdCode = $"gtag('set', {{'user_id': '{customer.Id}'}});{Environment.NewLine}";
            analyticsTrackingScript = analyticsTrackingScript.Replace("{CUSTOMER_TRACKING}", customerIdCode);

            //ecommerce info
            var store = await _storeContext.GetCurrentStoreAsync();
            var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);
            //ensure that ecommerce tracking code is renderred only once (avoid duplicated data in Google Analytics)
            if (order != null && !await _genericAttributeService.GetAttributeAsync<bool>(order, ORDER_ALREADY_PROCESSED_ATTRIBUTE_NAME))
            {
                var usCulture = new CultureInfo("en-US");

                var analyticsEcommerceScript = @"gtag('event', 'purchase', {
                    'transaction_id': '{ORDERID}',
                    'affiliation': '{SITE}',
                    'value': {TOTAL},
                    'currency': '{CURRENCY}',
                    'tax': {TAX},
                    'shipping': {SHIP},
                    'items': [
                    {DETAILS}
                    ]
                });";
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{ORDERID}", FixIllegalJavaScriptChars(order.CustomOrderNumber));
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{SITE}", FixIllegalJavaScriptChars(store.Name));
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{TOTAL}", order.OrderTotal.ToString("0.00", usCulture));
                var currencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{CURRENCY}", currencyCode);
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{TAX}", order.OrderTax.ToString("0.00", usCulture));
                var orderShipping = googleAnalyticsSettings.IncludingTax ? order.OrderShippingInclTax : order.OrderShippingExclTax;
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{SHIP}", orderShipping.ToString("0.00", usCulture));

                var sb = new StringBuilder();
                var listingPosition = 1;
                foreach (var item in await _orderService.GetOrderItemsAsync(order.Id))
                {
                    if (!string.IsNullOrEmpty(sb.ToString()))
                        sb.AppendLine(",");

                    var analyticsEcommerceDetailScript = @"{
                    'id': '{PRODUCTSKU}',
                    'name': '{PRODUCTNAME}',
                    'category': '{CATEGORYNAME}',
                    'list_position': {LISTPOSITION},
                    'quantity': {QUANTITY},
                    'price': '{UNITPRICE}'
                    }
                    ";

                    var product = await _productService.GetProductByIdAsync(item.ProductId);

                    var sku = await _productService.FormatSkuAsync(product, item.AttributesXml);

                    if (string.IsNullOrEmpty(sku))
                        sku = product.Id.ToString();

                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{PRODUCTSKU}", FixIllegalJavaScriptChars(sku));
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{PRODUCTNAME}", FixIllegalJavaScriptChars(product.Name));
                    var category = (await _categoryService.GetCategoryByIdAsync((await _categoryService.GetProductCategoriesByProductIdAsync(item.ProductId)).FirstOrDefault()?.CategoryId ?? 0))?.Name;
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{CATEGORYNAME}", FixIllegalJavaScriptChars(category));
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{LISTPOSITION}", listingPosition.ToString());
                    var unitPrice = googleAnalyticsSettings.IncludingTax ? item.UnitPriceInclTax : item.UnitPriceExclTax;
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{QUANTITY}", item.Quantity.ToString());
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{UNITPRICE}", unitPrice.ToString("0.00", usCulture));
                    sb.AppendLine(analyticsEcommerceDetailScript);

                    listingPosition++;
                }

                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{DETAILS}", sb.ToString());

                analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE_TRACKING}", analyticsEcommerceScript);

                await _genericAttributeService.SaveAttributeAsync(order, ORDER_ALREADY_PROCESSED_ATTRIBUTE_NAME, true);
            }
            else
            {
                analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE_TRACKING}", "");
            }

            return analyticsTrackingScript;
        }

        #endregion

        #region Methods

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var script = "";
            var routeData = Url.ActionContext.RouteData;

            try
            {
                var controller = routeData.Values["controller"];
                var action = routeData.Values["action"];

                if (controller == null || action == null)
                    return Content("");

                //Special case, if we are in last step of checkout, we can use order total for conversion value
                var isOrderCompletedPage = controller.ToString().Equals("checkout", StringComparison.InvariantCultureIgnoreCase) &&
                    action.ToString().Equals("completed", StringComparison.InvariantCultureIgnoreCase);
                if (isOrderCompletedPage && _googleAnalyticsSettings.EnableEcommerce && _googleAnalyticsSettings.UseJsToSendEcommerceInfo)
                {
                    var lastOrder = await GetLastOrderAsync();
                    script += await GetEcommerceScriptAsync(lastOrder);
                }
                else
                {
                    script += await GetEcommerceScriptAsync(null);
                }
            }
            catch (Exception ex)
            {
                await _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Error, "Error creating scripts for Google eCommerce tracking", ex.ToString());
            }
            return View("~/Plugins/Widgets.GoogleAnalytics/Views/PublicInfo.cshtml", script);
        }

        #endregion
    }
}