using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Azure;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Plugin.Widgets.CustomCustomProductReviews.Services;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Stores;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.CustomProductReviews
{
    /// <summary>
    /// Rename this file and change to the correct type
    /// </summary>
    public class CustomProductReviewsPlugin : BasePlugin,IWidgetPlugin
    {


        #region Fields

        private readonly CustomProductReviewsSettings _customProdutReviewSettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IWebHelper _webHelper;


        #endregion

        #region Ctor

        public CustomProductReviewsPlugin(CustomProductReviewsSettings customProdutReviewSettings,
            IActionContextAccessor actionContextAccessor,
            ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IUrlHelperFactory urlHelperFactory, IStoreContext storeContext, IWebHelper webHelper)
        {
            _customProdutReviewSettings = customProdutReviewSettings;
            _actionContextAccessor = actionContextAccessor;
            _localizationService = localizationService;
            _settingService = settingService;
            _storeService = storeService;
            _urlHelperFactory = urlHelperFactory;
            _storeContext = storeContext;
            _webHelper=webHelper;
            
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            //return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(AccessiBeDefaults.ConfigurationRouteName);
            return "";
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget zones
        /// </returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { _customProdutReviewSettings.WidgetZone,PublicWidgetZones.ProductDetailsBottom });
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone == null)
                throw new ArgumentNullException(nameof(widgetZone));

            return "CustomProductReviews";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {

            Uri myUri = new Uri(_webHelper.GetStoreLocation());
            string host = myUri.Host;


            host = EncryptService.Encrypt(host);
            var version = "1.0.0";
            version = "CustomProductReviews" + " " + version;
            version= EncryptService.Encrypt(version);
            //settings
            await _settingService.SaveSettingAsync(new CustomProductReviewsSettings
            {
                WidgetZone = PublicWidgetZones.ProductReviewsPageTop,
                data = host+","+version,
                MaximumFile = 5,
                MaximumSize = 1073741824

            });
         
         


            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Widgets.CustomProductReviews.Fields.Enabled"] = "Enable",
                ["Plugins.Widgets.CustomProductReviews.Fields.Enabled.Hint"] = "Check to activate this widget.",
                ["Plugins.Widgets.CustomProductReviews.Fields.Script"] = "Installation script",
                ["Plugins.Widgets.CustomProductReviews.Fields.Script.Hint"] = "Find your unique installation script on the Installation tab in your account and then copy it into this field.",
                ["Plugins.Widgets.CustomProductReviews.Fields.Script.Required"] = "Installation script is required",
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<CustomProductReviewsSettings>();

            var stores = await _storeService.GetAllStoresAsync();
            var storeIds = new List<int> { 0 }.Union(stores.Select(store => store.Id));
            foreach (var storeId in storeIds)
            {
                var widgetSettings = await _settingService.LoadSettingAsync<WidgetSettings>(storeId);
                widgetSettings.ActiveWidgetSystemNames.Remove(CustomProductReviewsDefaults.SystemName);
                await _settingService.SaveSettingAsync(widgetSettings);
            }

            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.CustomProductReviews");

            await base.UninstallAsync();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;

        #endregion


    }
}
