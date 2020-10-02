using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly FacebookPixelService _facebookPixelService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly StoreInformationSettings _storeInformationSettings;

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

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //prepare plugin configuration model
            var model = new ConfigurationModel();
            _baseAdminModelFactory.PrepareStores(model.FacebookPixelSearchModel.AvailableStores);
            model.FacebookPixelSearchModel.HideStoresList = model.FacebookPixelSearchModel.AvailableStores.SelectionIsNotPossible();
            model.FacebookPixelSearchModel.HideSearchBlock = _genericAttributeService
                .GetAttribute<bool>(_workContext.CurrentCustomer, FacebookPixelDefaults.HideSearchBlockAttribute);
            model.FacebookPixelSearchModel.SetGridPageSize();
            model.HideList = !_facebookPixelService.GetPagedConfigurations().Any();

            return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Configure.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult List(FacebookPixelSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedDataTablesJson();

            var configurations = _facebookPixelService.GetPagedConfigurations(searchModel.StoreId, searchModel.Page - 1, searchModel.PageSize);
            var model = new FacebookPixelListModel().PrepareToGrid(searchModel, configurations, () =>
            {
                //fill in model values from the configuration
                return configurations.Select(configuration => new FacebookPixelModel
                {
                    Id = configuration.Id,
                    PixelId = configuration.PixelId,
                    Enabled = configuration.Enabled,
                    StoreId = configuration.StoreId,
                    StoreName = _storeService.GetStoreById(configuration.StoreId)?.Name
                });
            });

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
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
            _baseAdminModelFactory.PrepareStores(model.AvailableStores, false);
            model.HideStoresList = model.AvailableStores.SelectionIsNotPossible();

            return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Create.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult Create(FacebookPixelModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //save configuration
                model.StoreId = model.StoreId > 0 ? model.StoreId : _storeContext.CurrentStore.Id;
                var configuration = model.ToEntity<FacebookPixelConfiguration>();
                _facebookPixelService.InsertConfiguration(configuration);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

                return RedirectToAction("Configure", "FacebookPixel");
            }

            //if we got this far, something failed, redisplay form
            _baseAdminModelFactory.PrepareStores(model.AvailableStores, false);
            model.HideStoresList = model.AvailableStores.SelectionIsNotPossible();

            return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Create.cshtml", model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var configuration = _facebookPixelService.GetConfigurationById(id);
            if (configuration == null)
                return RedirectToAction("Configure", "FacebookPixel");

            //fill in model values from the configuration
            var model = configuration.ToModel<FacebookPixelModel>();

            //prepare other model properties
            _baseAdminModelFactory.PrepareStores(model.AvailableStores, false);
            model.HideStoresList = model.AvailableStores.SelectionIsNotPossible();
            model.HideCustomEventsSearch = !_facebookPixelService.GetCustomEvents(configuration.Id).Any();
            model.CustomEventSearchModel.ConfigurationId = configuration.Id;
            model.CustomEventSearchModel.AddCustomEvent.AvailableWidgetZones = PreparePublicWidgetZones();
            model.CustomEventSearchModel.SetGridPageSize();

            return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Edit.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult Edit(FacebookPixelModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var configuration = _facebookPixelService.GetConfigurationById(model.Id);
            if (configuration == null)
                return RedirectToAction("Configure", "FacebookPixel");

            if (ModelState.IsValid)
            {
                //save configuration
                var customEvents = configuration.CustomEvents;
                model.StoreId = model.StoreId > 0 ? model.StoreId : _storeContext.CurrentStore.Id;
                configuration = model.ToEntity<FacebookPixelConfiguration>();
                configuration.CustomEvents = customEvents;
                _facebookPixelService.UpdateConfiguration(configuration);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

                return RedirectToAction("Configure", "FacebookPixel");
            }

            //if we got this far, something failed, redisplay form
            _baseAdminModelFactory.PrepareStores(model.AvailableStores, false);
            model.HideStoresList = model.AvailableStores.SelectionIsNotPossible();
            model.HideCustomEventsSearch = !_facebookPixelService.GetCustomEvents(configuration.Id).Any();
            model.CustomEventSearchModel.ConfigurationId = configuration.Id;
            model.CustomEventSearchModel.AddCustomEvent.AvailableWidgetZones = PreparePublicWidgetZones();
            model.CustomEventSearchModel.SetGridPageSize();

            return View("~/Plugins/Widgets.FacebookPixel/Views/Configuration/Edit.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var configuration = _facebookPixelService.GetConfigurationById(id);
            if (configuration == null)
                return RedirectToAction("Configure", "FacebookPixel");

            //delete configuration
            _facebookPixelService.DeleteConfiguration(configuration);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return RedirectToAction("Configure", "FacebookPixel");
        }

        [HttpPost]
        public virtual IActionResult CustomEventList(CustomEventSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedDataTablesJson();

            var configuration = _facebookPixelService.GetConfigurationById(searchModel.ConfigurationId)
                ?? throw new ArgumentException("No configuration found with the specified id", nameof(searchModel.ConfigurationId));

            var customEvents = _facebookPixelService.GetCustomEvents(configuration.Id, searchModel.WidgetZone).ToPagedList(searchModel);
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
        public virtual IActionResult CustomEventAdd(int configurationId, [Validate] CustomEventModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //save custom events configuration
            _facebookPixelService.SaveCustomEvents(configurationId, model.EventName, model.WidgetZones);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult CustomEventDelete(int configurationId, string id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //save custom events configuration
            var eventName = id;
            _facebookPixelService.SaveCustomEvents(configurationId, eventName, null);

            return new NullJsonResult();
        }

        public IActionResult CookieSettingsWarning(bool disableForUsersNotAcceptingCookieConsent)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            if (!disableForUsersNotAcceptingCookieConsent || _storeInformationSettings.DisplayEuCookieLawWarning)
                return Json(new { Result = string.Empty });

            //display a warning when "DisplayEuCookieLawWarning" setting is disabled, 
            //but merchant want to disable Facebook Pixel for users not accepting Cookie Consent
            var url = Url.Action("GeneralCommon", "Setting");
            var warning = string.Format(_localizationService.GetResource("Plugins.Widgets.FacebookPixel.Configuration.CookieSettingsWarning"), url);
            return Json(new { Result = warning });
        }

        #endregion
    }
}