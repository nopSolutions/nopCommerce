using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Components
{
    [ViewComponent(Name = "WidgetsGoogleAnalytics")]
    public class WidgetsGoogleAnalyticsViewComponent : NopViewComponent
    {
        #region Fields

        private readonly GoogleAnalyticsSettings _googleAnalyticsSettings;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public WidgetsGoogleAnalyticsViewComponent(GoogleAnalyticsSettings googleAnalyticsSettings,
            IWorkContext workContext,
            IStoreContext storeContext,
            ISettingService settingService,
            IOrderService orderService,
            ILogger logger)
        {
            this._googleAnalyticsSettings = googleAnalyticsSettings;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._settingService = settingService;
            this._orderService = orderService;
            this._logger = logger;
        }

        #endregion

        #region Methods

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var globalScript = GetEcommerceScript();
            return View("~/Plugins/Widgets.GoogleAnalytics/Views/PublicInfo.cshtml", globalScript);
        }

        //<script> 

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

        private string GetEcommerceScript()
        {
            var analyticsTrackingScript = _googleAnalyticsSettings.TrackingScript + "\n";
            analyticsTrackingScript = analyticsTrackingScript.Replace("{GOOGLEID}", _googleAnalyticsSettings.GoogleId);
            //remove {ECOMMERCE} (used in previous versions of the plugin)
            analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE}", "");

            //whether to include customer identifier
            var customerIdCode = string.Empty;
            if (_googleAnalyticsSettings.IncludeCustomerId && !_workContext.CurrentCustomer.IsGuest())
            {
                //is it an universal or ga script
                if (analyticsTrackingScript.Contains("google-analytics.com/ga.js"))
                    customerIdCode = $"_gaq.push(['_setCustomVar', 1, 'UserID', '{_workContext.CurrentCustomer.Id}', 1]);{Environment.NewLine}";
                else
                    customerIdCode = $"ga('set', 'userId', '{_workContext.CurrentCustomer.Id}');{Environment.NewLine}";
            }
            analyticsTrackingScript = analyticsTrackingScript.Replace("{CustomerID}", customerIdCode);

            return analyticsTrackingScript;
        }

        #endregion
    }
}