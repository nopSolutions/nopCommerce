using System;
using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Shipping.ShipStation.Models;
using Nop.Plugin.Shipping.ShipStation.Services;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Shipping.ShipStation.Controllers
{
    public class ShipStationController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IShipStationService _shipStationService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;

        public ShipStationController(ILocalizationService localizationService,
            INotificationService notificationService,
            ISettingService settingService,
            IShipStationService shipStationService,
            IStoreContext storeContext,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _settingService = settingService;
            _shipStationService = shipStationService;
            _storeContext = storeContext;
            _webHelper = webHelper;
        }
        
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var shipStationSettings = _settingService.LoadSetting<ShipStationSettings>(storeScope);

            var model = new ShipStationModel
            {
                ApiKey = shipStationSettings.ApiKey,
                ApiSecret = shipStationSettings.ApiSecret,
                PackingPackageVolume = shipStationSettings.PackingPackageVolume,
                PackingType = Convert.ToInt32(shipStationSettings.PackingType),
                PackingTypeValues = shipStationSettings.PackingType.ToSelectList(),
                PassDimensions = shipStationSettings.PassDimensions,
                ActiveStoreScopeConfiguration = storeScope,
                UserName = shipStationSettings.UserName,
                Password = shipStationSettings.Password,
                WebhookURL = $"{_webHelper.GetStoreLocation()}Plugins/ShipStation/Webhook"
            };

            if (storeScope <= 0)
                return View("~/Plugins/Shipping.ShipStation/Views/Configure.cshtml", model);

            model.ApiKey_OverrideForStore = _settingService.SettingExists(shipStationSettings, x => x.ApiKey, storeScope);
            model.ApiSecret_OverrideForStore = _settingService.SettingExists(shipStationSettings, x => x.ApiSecret, storeScope);
            model.PackingPackageVolume_OverrideForStore = _settingService.SettingExists(shipStationSettings, x => x.PackingPackageVolume, storeScope);
            model.PackingType_OverrideForStore = _settingService.SettingExists(shipStationSettings, x => x.PackingType, storeScope);
            model.PassDimensions_OverrideForStore = _settingService.SettingExists(shipStationSettings, x => x.PassDimensions, storeScope);
            model.Password_OverrideForStore = _settingService.SettingExists(shipStationSettings, x => x.Password, storeScope);
            model.UserName_OverrideForStore = _settingService.SettingExists(shipStationSettings, x => x.UserName, storeScope);

            return View("~/Plugins/Shipping.ShipStation/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public ActionResult Configure(ShipStationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var shipStationSettings = _settingService.LoadSetting<ShipStationSettings>(storeScope);

            //save settings
            shipStationSettings.ApiKey = model.ApiKey;
            shipStationSettings.ApiSecret = model.ApiSecret;
            shipStationSettings.PackingPackageVolume = model.PackingPackageVolume;
            shipStationSettings.PackingType = (PackingType)model.PackingType;
            shipStationSettings.PassDimensions = model.PassDimensions;
            shipStationSettings.Password = model.Password;
            shipStationSettings.UserName = model.UserName;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(shipStationSettings, x => x.ApiKey, model.ApiKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shipStationSettings, x => x.ApiSecret, model.ApiSecret_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shipStationSettings, x => x.PackingPackageVolume, model.PackingPackageVolume_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shipStationSettings, x => x.PackingType, model.PackingType_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shipStationSettings, x => x.PassDimensions, model.PassDimensions_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shipStationSettings, x => x.Password, model.Password_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(shipStationSettings, x => x.UserName, model.UserName_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

			_notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            
            return Configure();
        }

        public IActionResult Webhook()
        {
            var userName = _webHelper.QueryString<string>("SS-UserName");
            var password = _webHelper.QueryString<string>("SS-Password");

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var shipStationSettings = _settingService.LoadSetting<ShipStationSettings>(storeScope);

            if (!userName.Equals(shipStationSettings.UserName) || !password.Equals(shipStationSettings.Password))
                return Content(string.Empty);

            var action = _webHelper.QueryString<string>("action") ?? string.Empty;

            if (Request.Method == WebRequestMethods.Http.Post &&
                action.Equals("shipnotify", StringComparison.InvariantCultureIgnoreCase))
            {
                var orderNumber = _webHelper.QueryString<string>("order_number");
                var carrier = _webHelper.QueryString<string>("carrier");
                var service = _webHelper.QueryString<string>("service");
                var trackingNumber = _webHelper.QueryString<string>("tracking_number");

                _shipStationService.CreateOrUpadeteShipping(orderNumber, carrier, service, trackingNumber);

                //nothing should be rendered to visitor
                return Content(string.Empty);
            }

            if (!action.Equals("export", StringComparison.InvariantCultureIgnoreCase))
                return Content(string.Empty);

            var startDateParam = _webHelper.QueryString<string>("start_date");
            var endDateParam = _webHelper.QueryString<string>("end_date");
            var pageIndex = _webHelper.QueryString<int>("page");

            if (pageIndex > 0)
                pageIndex -= 1;

            var startDate = string.IsNullOrEmpty(startDateParam) ? (DateTime?)null : DateTime.ParseExact(startDateParam, _shipStationService.DateFormat, CultureInfo.InvariantCulture);
            var endDate = string.IsNullOrEmpty(endDateParam) ? (DateTime?)null : DateTime.ParseExact(endDateParam, _shipStationService.DateFormat, CultureInfo.InvariantCulture);
            
            return Content(_shipStationService.GetXmlOrders(startDate, endDate, pageIndex, 200), "text/xml");
        }
    }
}
