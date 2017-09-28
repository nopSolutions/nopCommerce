using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Orders;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Components
{
    [ViewComponent(Name = "WidgetsGoogleAnalytics")]
    public class WidgetsGoogleAnalyticsViewComponent : ViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;

        public WidgetsGoogleAnalyticsViewComponent(
            IWorkContext workContext,
            IStoreContext storeContext,
            ISettingService settingService,
            IOrderService orderService,
            ILogger logger)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._settingService = settingService;
            this._orderService = orderService;
            this._logger = logger;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            string globalScript = "";
            var routeData = Url.ActionContext.RouteData;

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
                    globalScript += GetEcommerceScript(null);
                }
            }
            catch (Exception ex)
            {
                _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, "Error creating scripts for Google ecommerce tracking", ex.ToString());
            }
            return View("~/Plugins/Widgets.GoogleAnalytics/Views/PublicInfo.cshtml", globalScript);
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
            var analyticsTrackingScript = googleAnalyticsSettings.TrackingScript + "\n";
            analyticsTrackingScript = analyticsTrackingScript.Replace("{GOOGLEID}", googleAnalyticsSettings.GoogleId);
            //remove {ECOMMERCE} (used in previous versions of the plugin)
            analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE}", "");

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