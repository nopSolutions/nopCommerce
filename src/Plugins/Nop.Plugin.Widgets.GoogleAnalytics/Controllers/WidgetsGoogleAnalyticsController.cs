using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.GoogleAnalytics.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Controllers
{
    public class WidgetsGoogleAnalyticsController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        private readonly ICategoryService _categoryService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ILocalizationService _localizationService;

        public WidgetsGoogleAnalyticsController(IWorkContext workContext,
            IStoreContext storeContext, 
            IStoreService storeService,
            ISettingService settingService, 
            IOrderService orderService, 
            ILogger logger, 
            ICategoryService categoryService,
            IProductAttributeParser productAttributeParser,
            ILocalizationService localizationService)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._orderService = orderService;
            this._logger = logger;
            this._categoryService = categoryService;
            this._productAttributeParser = productAttributeParser;
            this._localizationService = localizationService;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>(storeScope);
            var model = new ConfigurationModel();
            model.GoogleId = googleAnalyticsSettings.GoogleId;
            model.TrackingScript = googleAnalyticsSettings.TrackingScript;
            model.EcommerceScript = googleAnalyticsSettings.EcommerceScript;
            model.EcommerceDetailScript = googleAnalyticsSettings.EcommerceDetailScript;
            model.IncludingTax = googleAnalyticsSettings.IncludingTax;
            model.ZoneId = googleAnalyticsSettings.WidgetZone;
            model.AvailableZones.Add(new SelectListItem() { Text = "Before body end html tag", Value = "body_end_html_tag_before" });
            model.AvailableZones.Add(new SelectListItem() { Text = "Head html tag", Value = "head_html_tag" });

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.GoogleId_OverrideForStore = _settingService.SettingExists(googleAnalyticsSettings, x => x.GoogleId, storeScope);
                model.TrackingScript_OverrideForStore = _settingService.SettingExists(googleAnalyticsSettings, x => x.TrackingScript, storeScope);
                model.EcommerceScript_OverrideForStore = _settingService.SettingExists(googleAnalyticsSettings, x => x.EcommerceScript, storeScope);
                model.EcommerceDetailScript_OverrideForStore = _settingService.SettingExists(googleAnalyticsSettings, x => x.EcommerceDetailScript, storeScope);
                model.IncludingTax_OverrideForStore = _settingService.SettingExists(googleAnalyticsSettings, x => x.IncludingTax, storeScope);
                model.ZoneId_OverrideForStore = _settingService.SettingExists(googleAnalyticsSettings, x => x.WidgetZone, storeScope);
            }

            return View("~/Plugins/Widgets.GoogleAnalytics/Views/WidgetsGoogleAnalytics/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>(storeScope);
            googleAnalyticsSettings.GoogleId = model.GoogleId;
            googleAnalyticsSettings.TrackingScript = model.TrackingScript;
            googleAnalyticsSettings.EcommerceScript = model.EcommerceScript;
            googleAnalyticsSettings.EcommerceDetailScript = model.EcommerceDetailScript;
            googleAnalyticsSettings.IncludingTax = model.IncludingTax;
            googleAnalyticsSettings.WidgetZone = model.ZoneId;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(googleAnalyticsSettings, x => x.GoogleId, model.GoogleId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(googleAnalyticsSettings, x => x.TrackingScript, model.TrackingScript_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(googleAnalyticsSettings, x => x.EcommerceScript, model.EcommerceScript_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(googleAnalyticsSettings, x => x.EcommerceDetailScript, model.EcommerceDetailScript_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(googleAnalyticsSettings, x => x.IncludingTax, model.IncludingTax_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(googleAnalyticsSettings, x => x.WidgetZone, model.ZoneId_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            string globalScript = "";
            var routeData = ((System.Web.UI.Page)this.HttpContext.CurrentHandler).RouteData;

            try
            {
                var controller = routeData.Values["controller"];
                var action = routeData.Values["action"];

                if (controller == null || action == null)
                    return Content("");

                //Special case, if we are in last step of checkout, we can use order total for conversion value
                if (controller.ToString().Equals("checkout", StringComparison.InvariantCultureIgnoreCase) &&
                    action.ToString().Equals("completed", StringComparison.InvariantCultureIgnoreCase))
                {
                    var lastOrder = GetLastOrder();
                    globalScript += GetEcommerceScript(lastOrder);
                }
                else
                {
                    globalScript += GetTrackingScript();
                }
            }
            catch (Exception ex)
            {
                _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, "Error creating scripts for google ecommerce tracking", ex.ToString());
            }
            return Content(globalScript);
        }

        private Order GetLastOrder()
        {
            var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1).FirstOrDefault();
            return order;
        }
        
        //<script type="text/javascript"> 

        //var _gaq = _gaq || []; 
        //_gaq.push(['_setAccount', 'UA-XXXXX-X']); 
        //_gaq.push(['_trackPageview']); 

        //(function() { 
        //var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true; 
        //ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js'; 
        //var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s); 
        //})(); 

        //</script>
        private string GetTrackingScript()
        {
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>(_storeContext.CurrentStore.Id);
            var analyticsTrackingScript = googleAnalyticsSettings.TrackingScript + "\n";
            analyticsTrackingScript = analyticsTrackingScript.Replace("{GOOGLEID}", googleAnalyticsSettings.GoogleId);
            analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE}", "");
            return analyticsTrackingScript;
        }
        
        //<script type="text/javascript"> 

        //var _gaq = _gaq || []; 
        //_gaq.push(['_setAccount', 'UA-XXXXX-X']); 
        //_gaq.push(['_trackPageview']); 
        //_gaq.push(['_addTrans', 
        //'1234',           // order ID - required 
        //'Acme Clothing',  // affiliation or store name 
        //'11.99',          // total - required 
        //'1.29',           // tax 
        //'5',              // shipping 
        //'San Jose',       // city 
        //'California',     // state or province 
        //'USA'             // country 
        //]); 

        //// add item might be called for every item in the shopping cart 
        //// where your ecommerce engine loops through each item in the cart and 
        //// prints out _addItem for each 
        //_gaq.push(['_addItem', 
        //'1234',           // order ID - required 
        //'DD44',           // SKU/code - required 
        //'T-Shirt',        // product name 
        //'Green Medium',   // category or variation 
        //'11.99',          // unit price - required 
        //'1'               // quantity - required 
        //]); 
        //_gaq.push(['_trackTrans']); //submits transaction to the Analytics servers 

        //(function() { 
        //var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true; 
        //ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js'; 
        //var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s); 
        //})(); 

        //</script>
        private string GetEcommerceScript(Order order)
        {
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>(_storeContext.CurrentStore.Id);
            var usCulture = new CultureInfo("en-US");
            var analyticsTrackingScript = googleAnalyticsSettings.TrackingScript + "\n";
            analyticsTrackingScript = analyticsTrackingScript.Replace("{GOOGLEID}", googleAnalyticsSettings.GoogleId);

            if (order != null)
            {
                var analyticsEcommerceScript = googleAnalyticsSettings.EcommerceScript + "\n";
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{GOOGLEID}", googleAnalyticsSettings.GoogleId);
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{ORDERID}", order.Id.ToString());
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{SITE}", _storeContext.CurrentStore.Url.Replace("http://", "").Replace("/", ""));
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{TOTAL}", order.OrderTotal.ToString("0.00", usCulture));
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{TAX}", order.OrderTax.ToString("0.00", usCulture));
                var orderShipping = googleAnalyticsSettings.IncludingTax ? order.OrderShippingInclTax : order.OrderShippingExclTax;
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{SHIP}", orderShipping.ToString("0.00", usCulture));
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{CITY}", order.BillingAddress == null ? "" : FixIllegalJavaScriptChars(order.BillingAddress.City));
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{STATEPROVINCE}", order.BillingAddress == null || order.BillingAddress.StateProvince == null ? "" : FixIllegalJavaScriptChars(order.BillingAddress.StateProvince.Name));
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{COUNTRY}", order.BillingAddress == null || order.BillingAddress.Country == null ? "" : FixIllegalJavaScriptChars(order.BillingAddress.Country.Name));

                var sb = new StringBuilder();
                foreach (var item in order.OrderItems)
                {
                    string analyticsEcommerceDetailScript = googleAnalyticsSettings.EcommerceDetailScript;
                    //get category
                    string category = "";
                    var defaultProductCategory = _categoryService.GetProductCategoriesByProductId(item.ProductId).FirstOrDefault();
                    if (defaultProductCategory != null)
                        category = defaultProductCategory.Category.Name;
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{ORDERID}", item.OrderId.ToString());
                    //The SKU code is a required parameter for every item that is added to the transaction
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{PRODUCTSKU}", FixIllegalJavaScriptChars(item.Product.FormatSku(item.AttributesXml, _productAttributeParser)));
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{PRODUCTNAME}", FixIllegalJavaScriptChars(item.Product.Name));
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{CATEGORYNAME}", FixIllegalJavaScriptChars(category));
                    var unitPrice = googleAnalyticsSettings.IncludingTax ? item.UnitPriceInclTax : item.UnitPriceExclTax;
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{UNITPRICE}", unitPrice.ToString("0.00", usCulture));
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{QUANTITY}", item.Quantity.ToString());
                    sb.AppendLine(analyticsEcommerceDetailScript);
                }

                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{DETAILS}", sb.ToString());

                analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE}", analyticsEcommerceScript);

            }

            return analyticsTrackingScript;
        }

        private string FixIllegalJavaScriptChars(string text)
        {
            if (String.IsNullOrEmpty(text))
                return text;

            //replace ' with \' (http://stackoverflow.com/questions/4292761/need-to-url-encode-labels-when-tracking-events-with-google-analytics)
            text = text.Replace("'", "\\'");
            return text;
        }
    }
}