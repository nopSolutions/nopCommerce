using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Plugin.Widgets.FacebookPixel.Domain;
using Nop.Plugin.Widgets.FacebookPixel.Models;
using Nop.Plugin.Widgets.FacebookPixel.Services;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.FacebookPixel.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class FacebookPixelController : BasePluginController
    {
        #region Fields

        protected readonly FacebookPixelService _facebookPixelService;
        protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly ILocalizationService _localizationService;
        protected readonly INotificationService _notificationService;
        protected readonly IPermissionService _permissionService;
        protected readonly IStoreContext _storeContext;
        protected readonly IStoreService _storeService;
        protected readonly IWorkContext _workContext;
        protected readonly StoreInformationSettings _storeInformationSettings;

        #endregion

        #region Ctor

        public FacebookPixelController(FacebookPixelService facebookPixelService,
            IBaseAdminModelFactory baseAdminModelFactory,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWorkContext workContext,
            StoreInformationSettings storeInformationSettings)
        {
            _facebookPixelService = facebookPixelService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _storeContext = storeContext;
            _storeService = storeService;
            _workContext = workContext;
            _storeInformationSettings = storeInformationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare list of available public widget zones
        /// </summary>
        /// <returns>Available widget zones</returns>
        public IList<SelectListItem> PreparePublicWidgetZones()
        {
            return typeof(PublicWidgetZones)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(property => property.PropertyType == typeof(string))
                .Select(property => property.GetValue(null) is string value ? new SelectListItem(value, value) : null)
                .Where(item => item != null)
                .ToList();
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //prepare plugin configuration model
            var model = new ConfigurationModel();
            await _baseAdminModelFactory.PrepareStoresAsync(model.FacebookPixelSearchModel.AvailableStores);
            model.FacebookPixelSearchModel.HideStoresList = model.FacebookPixelSearchModel.AvailableStores.SelectionIsNotPossible();
            model.FacebookPixelSearchModel.HideSearchBlock = await _genericAttributeService
                .GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), FacebookPixelDefaults.HideSearchBlockAttribute);
            model.FacebookPixelSearchModel.SetGridPageSize();
            model.HideList = !(await _facebookPixelService.GetPagedConfigurationsAsync()).Any();

            return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Configure.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(FacebookPixelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return await AccessDeniedDataTablesJson();

            var configurations = await _facebookPixelService.GetPagedConfigurationsAsync(searchModel.StoreId, searchModel.Page - 1, searchModel.PageSize);
            var model = await new FacebookPixelListModel().PrepareToGridAsync(searchModel, configurations, () =>
            {
                //fill in model values from the configuration
                return configurations.SelectAwait(async configuration => new FacebookPixelModel
                {
                    Id = configuration.Id,
                    PixelId = configuration.PixelId,
                    AccessToken = configuration.AccessToken,
                    PixelScriptEnabled = configuration.PixelScriptEnabled,
                    ConversionsApiEnabled = configuration.ConversionsApiEnabled,
                    StoreId = configuration.StoreId,
                    StoreName = (await _storeService.GetStoreByIdAsync(configuration.StoreId))?.Name
                });
            });

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //set default values
            var model = new FacebookPixelModel
            {
                TrackAddToCart = true,
                TrackPageView = true,
                TrackPurchase = true,
                TrackViewContent = true,
                UseAdvancedMatching = true
            };

            //prepare other model properties
            await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores, false);
            model.HideStoresList = model.AvailableStores.SelectionIsNotPossible();

            return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Create.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(FacebookPixelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //save configuration
                var store = await _storeContext.GetCurrentStoreAsync();
                model.StoreId = model.StoreId > 0 ? model.StoreId : store.Id;
                var configuration = model.ToEntity<FacebookPixelConfiguration>();
                await _facebookPixelService.InsertConfigurationAsync(configuration);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

                return RedirectToAction("Configure", "FacebookPixel");
            }

            //if we got this far, something failed, redisplay form
            await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores, false);
            model.HideStoresList = model.AvailableStores.SelectionIsNotPossible();

            return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Create.cshtml", model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var configuration = await _facebookPixelService.GetConfigurationByIdAsync(id);
            if (configuration == null)
                return RedirectToAction("Configure", "FacebookPixel");

            //fill in model values from the configuration
            var model = configuration.ToModel<FacebookPixelModel>();

            //prepare other model properties
            await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores, false);
            model.HideStoresList = model.AvailableStores.SelectionIsNotPossible();
            model.HideCustomEventsSearch = !(await _facebookPixelService.GetCustomEventsAsync(configuration.Id)).Any();
            model.CustomEventSearchModel.ConfigurationId = configuration.Id;
            model.CustomEventSearchModel.AddCustomEvent.AvailableWidgetZones = PreparePublicWidgetZones();
            model.CustomEventSearchModel.SetGridPageSize();

            return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Edit.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit(FacebookPixelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var configuration = await _facebookPixelService.GetConfigurationByIdAsync(model.Id);
            if (configuration == null)
                return RedirectToAction("Configure", "FacebookPixel");

            if (ModelState.IsValid)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                //save configuration
                var customEvents = configuration.CustomEvents;
                model.StoreId = model.StoreId > 0 ? model.StoreId : store.Id;
                configuration = model.ToEntity<FacebookPixelConfiguration>();
                configuration.CustomEvents = customEvents;
                await _facebookPixelService.UpdateConfigurationAsync(configuration);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

                return RedirectToAction("Configure", "FacebookPixel");
            }

            //if we got this far, something failed, redisplay form
            await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores, false);
            model.HideStoresList = model.AvailableStores.SelectionIsNotPossible();
            model.HideCustomEventsSearch = !(await _facebookPixelService.GetCustomEventsAsync(configuration.Id)).Any();
            model.CustomEventSearchModel.ConfigurationId = configuration.Id;
            model.CustomEventSearchModel.AddCustomEvent.AvailableWidgetZones = PreparePublicWidgetZones();
            model.CustomEventSearchModel.SetGridPageSize();

            return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Edit.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var configuration = await _facebookPixelService.GetConfigurationByIdAsync(id);
            if (configuration == null)
                return RedirectToAction("Configure", "FacebookPixel");

            //delete configuration
            await _facebookPixelService.DeleteConfigurationAsync(configuration);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return RedirectToAction("Configure", "FacebookPixel");
        }

        [HttpPost]
        public virtual async Task<IActionResult> CustomEventList(CustomEventSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return await AccessDeniedDataTablesJson();

            var configuration = await _facebookPixelService.GetConfigurationByIdAsync(searchModel.ConfigurationId)
                ?? throw new ArgumentException("No configuration found with the specified id", nameof(searchModel.ConfigurationId));

            var customEvents = (await _facebookPixelService.GetCustomEventsAsync(configuration.Id, searchModel.WidgetZone)).ToPagedList(searchModel);
            var model = new CustomEventListModel().PrepareToGrid(searchModel, customEvents, () =>
            {
                //fill in model values from the configuration
                return customEvents.Select(customEvent => new CustomEventModel
                {
                    ConfigurationId = configuration.Id,
                    EventName = customEvent.EventName,
                    WidgetZonesName = string.Join(", ", customEvent.WidgetZones)
                });
            });

            return Json(model);
        }

        //ValidateAttribute is used to force model validation
        [HttpPost]
        public virtual async Task<IActionResult> CustomEventAdd(int configurationId, [Validate] CustomEventModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //save custom events configuration
            await _facebookPixelService.SaveCustomEventsAsync(configurationId, model.EventName, model.WidgetZones);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> CustomEventDelete(int configurationId, string id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //save custom events configuration
            var eventName = id;
            await _facebookPixelService.SaveCustomEventsAsync(configurationId, eventName, null);

            return new NullJsonResult();
        }

        public async Task<IActionResult> CookieSettingsWarning(bool disableForUsersNotAcceptingCookieConsent)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            if (!disableForUsersNotAcceptingCookieConsent || _storeInformationSettings.DisplayEuCookieLawWarning)
                return Json(new { Result = string.Empty });

            //display a warning when "DisplayEuCookieLawWarning" setting is disabled, 
            //but merchant want to disable Facebook Pixel for users not accepting Cookie Consent
            var url = Url.Action("GeneralCommon", "Setting");
            var warning = string.Format(await _localizationService.GetResourceAsync("Plugins.Widgets.FacebookPixel.Configuration.CookieSettingsWarning"), url);
            return Json(new { Result = warning });
        }

        #endregion
    }
}