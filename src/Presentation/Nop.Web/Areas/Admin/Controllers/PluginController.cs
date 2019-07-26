using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Authentication.External;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Tax;
using Nop.Services.Themes;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Areas.Admin.Models.Plugins.Marketplace;
using Nop.Web.Framework.Controllers;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class PluginController : BaseAdminController
    {
        #region Fields

        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPickupPluginManager _pickupPluginManager;
        private readonly IPluginModelFactory _pluginModelFactory;
        private readonly IPluginService _pluginService;
        private readonly ISettingService _settingService;
        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly IUploadService _uploadService;
        private readonly IWebHelper _webHelper;
        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IWorkContext _workContext;
        private readonly PaymentSettings _paymentSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public PluginController(ExternalAuthenticationSettings externalAuthenticationSettings,
            IAuthenticationPluginManager authenticationPluginManager,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPaymentPluginManager paymentPluginManager,
            IPickupPluginManager pickupPluginManager,
            IPluginModelFactory pluginModelFactory,
            IPluginService pluginService,
            ISettingService settingService,
            IShippingPluginManager shippingPluginManager,
            IUploadService uploadService,
            IWebHelper webHelper,
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext,
            PaymentSettings paymentSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            WidgetSettings widgetSettings)
        {
            _externalAuthenticationSettings = externalAuthenticationSettings;
            _authenticationPluginManager = authenticationPluginManager;
            _customerActivityService = customerActivityService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _paymentPluginManager = paymentPluginManager;
            _pickupPluginManager = pickupPluginManager;
            _pluginModelFactory = pluginModelFactory;
            _pluginService = pluginService;
            _settingService = settingService;
            _shippingPluginManager = shippingPluginManager;
            _uploadService = uploadService;
            _webHelper = webHelper;
            _widgetPluginManager = widgetPluginManager;
            _workContext = workContext;
            _paymentSettings = paymentSettings;
            _shippingSettings = shippingSettings;
            _taxSettings = taxSettings;
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = _pluginModelFactory.PreparePluginSearchModel(new PluginSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ListSelect(PluginSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _pluginModelFactory.PreparePluginListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult AdminNavigationPlugins()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Json(new List<string>());

            //prepare models
            var models = _pluginModelFactory.PrepareAdminNavigationPluginModels().Select(model => new
            {
                title = model.FriendlyName,
                link = model.ConfigurationUrl,
                parent = _localizationService.GetResource("Admin.Configuration.Plugins.Local"),
                grandParent = string.Empty,
                rate = -50 //negative rate is set to move plugins to the end of list
            }).ToList();

            return Json(models);
        }

        [HttpPost]
        public virtual IActionResult UploadPluginsAndThemes(IFormFile archivefile)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                if (archivefile == null || archivefile.Length == 0)
                {
                    _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }

                var descriptors = _uploadService.UploadPluginsAndThemes(archivefile);
                var pluginDescriptors = descriptors.OfType<PluginDescriptor>().ToList();
                var themeDescriptors = descriptors.OfType<ThemeDescriptor>().ToList();

                //activity log
                foreach (var descriptor in pluginDescriptors)
                {
                    _customerActivityService.InsertActivity("UploadNewPlugin",
                        string.Format(_localizationService.GetResource("ActivityLog.UploadNewPlugin"), descriptor.FriendlyName));
                }

                foreach (var descriptor in themeDescriptors)
                {
                    _customerActivityService.InsertActivity("UploadNewTheme",
                        string.Format(_localizationService.GetResource("ActivityLog.UploadNewTheme"), descriptor.FriendlyName));
                }

                //events
                if (pluginDescriptors.Any())
                    _eventPublisher.Publish(new PluginsUploadedEvent(pluginDescriptors));

                if (themeDescriptors.Any())
                    _eventPublisher.Publish(new ThemesUploadedEvent(themeDescriptors));

                var message = string.Format(_localizationService.GetResource("Admin.Configuration.Plugins.Uploaded"), pluginDescriptors.Count, themeDescriptors.Count);
                _notificationService.SuccessNotification(message);

                //restart application
                _webHelper.RestartAppDomain();
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired(FormValueRequirement.StartsWith, "install-plugin-link-")]
        public virtual IActionResult Install(IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                //get plugin system name
                string systemName = null;
                foreach (var formValue in form.Keys)
                    if (formValue.StartsWith("install-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
                        systemName = formValue.Substring("install-plugin-link-".Length);

                var pluginDescriptor = _pluginService.GetPluginDescriptorBySystemName<IPlugin>(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is not installed
                if (pluginDescriptor.Installed)
                    return RedirectToAction("List");

                _pluginService.PreparePluginToInstall(pluginDescriptor.SystemName, _workContext.CurrentCustomer);
                pluginDescriptor.ShowInPluginsList = false;
                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired(FormValueRequirement.StartsWith, "uninstall-plugin-link-")]
        public virtual IActionResult Uninstall(IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                //get plugin system name
                string systemName = null;
                foreach (var formValue in form.Keys)
                    if (formValue.StartsWith("uninstall-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
                        systemName = formValue.Substring("uninstall-plugin-link-".Length);

                var pluginDescriptor = _pluginService.GetPluginDescriptorBySystemName<IPlugin>(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is installed
                if (!pluginDescriptor.Installed)
                    return RedirectToAction("List");

                _pluginService.PreparePluginToUninstall(pluginDescriptor.SystemName);
                pluginDescriptor.ShowInPluginsList = false;
                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired(FormValueRequirement.StartsWith, "delete-plugin-link-")]
        public virtual IActionResult Delete(IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                //get plugin system name
                string systemName = null;
                foreach (var formValue in form.Keys)
                    if (formValue.StartsWith("delete-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
                        systemName = formValue.Substring("delete-plugin-link-".Length);

                var pluginDescriptor = _pluginService.GetPluginDescriptorBySystemName<IPlugin>(systemName, LoadPluginsMode.All);

                //check whether plugin is not installed
                if (pluginDescriptor.Installed)
                    return RedirectToAction("List");

                _pluginService.PreparePluginToDelete(pluginDescriptor.SystemName);
                pluginDescriptor.ShowInPluginsList = false;
                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("plugin-reload-grid")]
        public virtual IActionResult ReloadList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            _pluginService.UninstallPlugins();
            _pluginService.DeletePlugins();

            //restart application
            _webHelper.RestartAppDomain();

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("plugin-apply-changes")]
        public virtual IActionResult ApplyChanges()
        {
            return ReloadList();
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("plugin-discard-changes")]
        public virtual IActionResult DiscardChanges()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            _pluginService.ResetChanges();

            return RedirectToAction("List");
        }

        public virtual IActionResult EditPopup(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a plugin with the specified system name
            var pluginDescriptor = _pluginService.GetPluginDescriptorBySystemName<IPlugin>(systemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                return RedirectToAction("List");

            //prepare model
            var model = _pluginModelFactory.PreparePluginModel(null, pluginDescriptor);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult EditPopup(PluginModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a plugin with the specified system name
            var pluginDescriptor = _pluginService.GetPluginDescriptorBySystemName<IPlugin>(model.SystemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                ViewBag.RefreshPage = true;

                //we allow editing of 'friendly name', 'display order', store mappings
                pluginDescriptor.FriendlyName = model.FriendlyName;
                pluginDescriptor.DisplayOrder = model.DisplayOrder;
                pluginDescriptor.LimitedToStores.Clear();
                if (model.SelectedStoreIds.Any())
                    pluginDescriptor.LimitedToStores = model.SelectedStoreIds;
                pluginDescriptor.LimitedToCustomerRoles.Clear();
                if (model.SelectedCustomerRoleIds.Any())
                    pluginDescriptor.LimitedToCustomerRoles = model.SelectedCustomerRoleIds;

                //update the description file
                pluginDescriptor.Save();

                //raise event
                _eventPublisher.Publish(new PluginUpdatedEvent(pluginDescriptor));

                //locales
                var pluginInstance = pluginDescriptor.Instance<IPlugin>();
                foreach (var localized in model.Locales)
                {
                    _localizationService.SaveLocalizedFriendlyName(pluginInstance, localized.LanguageId, localized.FriendlyName);
                }

                //enabled/disabled
                if (!pluginDescriptor.Installed)
                    return View(model);

                var pluginIsActive = false;
                switch (pluginInstance)
                {
                    case IPaymentMethod paymentMethod:
                        pluginIsActive = _paymentPluginManager.IsPluginActive(paymentMethod);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            _paymentSettings.ActivePaymentMethodSystemNames.Remove(pluginDescriptor.SystemName);
                            _settingService.SaveSetting(_paymentSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            _paymentSettings.ActivePaymentMethodSystemNames.Add(pluginDescriptor.SystemName);
                            _settingService.SaveSetting(_paymentSettings);
                        }

                        break;
                    case IShippingRateComputationMethod shippingRateComputationMethod:
                        pluginIsActive = _shippingPluginManager.IsPluginActive(shippingRateComputationMethod);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                            _settingService.SaveSetting(_shippingSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add(pluginDescriptor.SystemName);
                            _settingService.SaveSetting(_shippingSettings);
                        }

                        break;
                    case IPickupPointProvider pickupPointProvider:
                        pluginIsActive = _pickupPluginManager.IsPluginActive(pickupPointProvider);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            _shippingSettings.ActivePickupPointProviderSystemNames.Remove(pluginDescriptor.SystemName);
                            _settingService.SaveSetting(_shippingSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            _shippingSettings.ActivePickupPointProviderSystemNames.Add(pluginDescriptor.SystemName);
                            _settingService.SaveSetting(_shippingSettings);
                        }

                        break;
                    case ITaxProvider taxProvider:
                        if (!model.IsEnabled)
                        {
                            //mark as disabled
                            _taxSettings.ActiveTaxProviderSystemName = string.Empty;
                            _settingService.SaveSetting(_taxSettings);
                            break;
                        }

                        //mark as enabled
                        _taxSettings.ActiveTaxProviderSystemName = model.SystemName;
                        _settingService.SaveSetting(_taxSettings);
                        break;
                    case IExternalAuthenticationMethod externalAuthenticationMethod:
                        pluginIsActive = _authenticationPluginManager.IsPluginActive(externalAuthenticationMethod);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                            _settingService.SaveSetting(_externalAuthenticationSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(pluginDescriptor.SystemName);
                            _settingService.SaveSetting(_externalAuthenticationSettings);
                        }

                        break;
                    case IWidgetPlugin widgetPlugin:
                        pluginIsActive = _widgetPluginManager.IsPluginActive(widgetPlugin);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            _widgetSettings.ActiveWidgetSystemNames.Remove(pluginDescriptor.SystemName);
                            _settingService.SaveSetting(_widgetSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            _widgetSettings.ActiveWidgetSystemNames.Add(pluginDescriptor.SystemName);
                            _settingService.SaveSetting(_widgetSettings);
                        }

                        break;
                }

                //activity log
                _customerActivityService.InsertActivity("EditPlugin",
                    string.Format(_localizationService.GetResource("ActivityLog.EditPlugin"), pluginDescriptor.FriendlyName));

                return View(model);
            }

            //prepare model
            model = _pluginModelFactory.PreparePluginModel(model, pluginDescriptor, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult OfficialFeed()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //prepare model
            var model = _pluginModelFactory.PrepareOfficialFeedPluginSearchModel(new OfficialFeedPluginSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult OfficialFeedSelect(OfficialFeedPluginSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _pluginModelFactory.PrepareOfficialFeedPluginListModel(searchModel);

            return Json(model);
        }

        #endregion
    }
}