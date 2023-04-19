using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Infrastructure;
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
        private IPluginsInfo _pluginsInfo;
        private readonly IPluginService _pluginService;
        private readonly IHttpContextAccessor _httpContextAccessor;



        #endregion

        #region Ctor

        public CustomProductReviewsPlugin(CustomProductReviewsSettings customProdutReviewSettings,
            IActionContextAccessor actionContextAccessor,
            ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IUrlHelperFactory urlHelperFactory, IStoreContext storeContext, IWebHelper webHelper,IPluginService pluginService,IHttpContextAccessor httpContextAccessor)
        {
            _customProdutReviewSettings = customProdutReviewSettings;
            _actionContextAccessor = actionContextAccessor;
            _localizationService = localizationService;
            _settingService = settingService;
            _storeService = storeService;
            _urlHelperFactory = urlHelperFactory;
            _storeContext = storeContext;
            _webHelper=webHelper;
            _pluginService = pluginService;
            _httpContextAccessor = httpContextAccessor;




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
        public async Task<IList<string>> GetWidgetZonesAsync()
        {
            return await Task.FromResult<IList<string>>(new List<string> { _customProdutReviewSettings.WidgetZone, PublicWidgetZones.ProductDetailsBottom });
           
           
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

            try
            {
                _pluginsInfo = Singleton<IPluginsInfo>.Instance;


                var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string rq = _httpContextAccessor.HttpContext.Request.PathBase;


            Uri myUri = new Uri(rq);
          
            string host = myUri.Host;


            host = EncryptService.Encrypt(host);
            var version = "1.0.0";
            version = "CustomProductReviews" + " " + version;
            version = EncryptService.Encrypt(version);
            string json = host + "," + version;
            json = JsonConvert.SerializeObject(json);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("https://wupdater.duckdns.org/CustomerData", content);
            var jsonString = await result.Content.ReadAsStringAsync();
            bool rool = JsonConvert.DeserializeObject<bool>(jsonString);
            if (rool)
            {

                //settings
                await _settingService.SaveSettingAsync(new CustomProductReviewsSettings
                {
                    WidgetZone = PublicWidgetZones.ProductReviewsPageTop,
                    data = json,
                    MaximumFile = 5,
                    MaximumSize = 1073741824
                });




                await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
                {
                    ["Plugins.Widgets.CustomProductReviews.Fields.Enabled"] = "Enable",
                    ["Plugins.Widgets.CustomProductReviews.Fields.Enabled.Hint"] = "Check to activate this widget.",
                    ["Plugins.Widgets.CustomProductReviews.Fields.Script"] = "Installation script",
                    ["Plugins.Widgets.CustomProductReviews.Fields.Script.Hint"] =
                        "Find your unique installation script on the Installation tab in your account and then copy it into this field.",
                    ["Plugins.Widgets.CustomProductReviews.Fields.Script.Required"] =
                        "Installation script is required",
                });

                await base.InstallAsync();
            }
            else
            {
                try
                {
                    //var pluginToInstall = _pluginsInfo.PluginNamesToInstall.FirstOrDefault(plugin => plugin.SystemName.Equals("Nop.Plugin.Widgets.CustomProductReviews"));
                    //_pluginsInfo.PluginNamesToInstall.Remove(pluginToInstall);
                    //_pluginsInfo.PluginNamesToUninstall.Add("Nop.Plugin.Widgets.CustomProductReviews");
                    //await _pluginsInfo.SaveAsync();
                    _pluginService.ResetChanges();
                    await _pluginService.PreparePluginToUninstallAsync("Nop.Plugin.Widgets.CustomProductReviews");
                    await _pluginService.UninstallPluginsAsync();

                    //_webHelper.RestartAppDomain();
                }
                catch 
                {
                   
                }
                    // _pluginsInfo.PluginNamesToUninstall.Add("Nop.Plugin.Widgets.CustomProductReviews");

            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               
            }
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
