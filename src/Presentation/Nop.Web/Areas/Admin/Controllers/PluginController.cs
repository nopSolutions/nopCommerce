using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Tax;
using Nop.Services.Themes;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Framework.Controllers;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class PluginController : BaseAdminController
    {
        #region Fields

        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly IPaymentService _paymentService;
        private readonly IPermissionService _permissionService;
        private readonly IPluginFinder _pluginFinder;
        private readonly IPluginModelFactory _pluginModelFactory;
        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;
        private readonly IUploadService _uploadService;
        private readonly IWebHelper _webHelper;
        private readonly IWidgetService _widgetService;
        private readonly PaymentSettings _paymentSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public PluginController(ExternalAuthenticationSettings externalAuthenticationSettings,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            IExternalAuthenticationService externalAuthenticationService,
            ILocalizationService localizationService,
            IPaymentService paymentService,
            IPermissionService permissionService,
            IPluginFinder pluginFinder,
            IPluginModelFactory pluginModelFactory,
            ISettingService settingService,
            IShippingService shippingService,
            IUploadService uploadService,
            IWebHelper webHelper,
            IWidgetService widgetService,
            PaymentSettings paymentSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            WidgetSettings widgetSettings)
        {
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._customerActivityService = customerActivityService;
            this._eventPublisher = eventPublisher;
            this._externalAuthenticationService = externalAuthenticationService;
            this._localizationService = localizationService;
            this._paymentService = paymentService;
            this._permissionService = permissionService;
            this._pluginFinder = pluginFinder;
            this._pluginModelFactory = pluginModelFactory;
            this._settingService = settingService;
            this._shippingService = shippingService;
            this._uploadService = uploadService;
            this._webHelper = webHelper;
            this._widgetService = widgetService;
            this._paymentSettings = paymentSettings;
            this._shippingSettings = shippingSettings;
            this._taxSettings = taxSettings;
            this._widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = _pluginModelFactory.PreparePluginsConfigurationModel(new PluginsConfigurationModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ListSelect(PluginSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _pluginModelFactory.PreparePluginListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult SearchList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Json(new System.Collections.Generic.List<string>());

            //prepare model
            var model = _pluginModelFactory.PreparePluginListModel(
                new PluginSearchModel { PageSize = int.MaxValue });

            //negative rate is set to move plugins to the end of list
            var filtredPlugins = model.Data
                .Where(m => !string.IsNullOrEmpty(m.ConfigurationUrl))
                .Select(m => new
                {
                    title = m.FriendlyName,
                    link = m.ConfigurationUrl,
                    parent = "Plugins",
                    grandParent = "",
                    rate = -50
                })
                .ToList();

            return Json(filtredPlugins);
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
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
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
                SuccessNotification(message);

                //restart application
                _webHelper.RestartAppDomain();
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
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

                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is not installed
                if (pluginDescriptor.Installed)
                    return RedirectToAction("List");

                //install plugin
                pluginDescriptor.Instance().Install();

                //activity log
                _customerActivityService.InsertActivity("InstallNewPlugin",
                    string.Format(_localizationService.GetResource("ActivityLog.InstallNewPlugin"), pluginDescriptor.FriendlyName));

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Plugins.Installed"));

                //restart application
                _webHelper.RestartAppDomain();
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
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

                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is installed
                if (!pluginDescriptor.Installed)
                    return RedirectToAction("List");

                //uninstall plugin
                pluginDescriptor.Instance().Uninstall();

                //activity log
                _customerActivityService.InsertActivity("UninstallPlugin",
                    string.Format(_localizationService.GetResource("ActivityLog.UninstallPlugin"), pluginDescriptor.FriendlyName));

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Plugins.Uninstalled"));

                //restart application
                _webHelper.RestartAppDomain();
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
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

                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
                if (!PluginManager.DeletePlugin(pluginDescriptor))
                    return RedirectToAction("List");

                //activity log
                _customerActivityService.InsertActivity("DeletePlugin",
                    string.Format(_localizationService.GetResource("ActivityLog.DeletePlugin"), pluginDescriptor.FriendlyName));

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Plugins.Deleted"));

                //restart application
                _webHelper.RestartAppDomain();
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("plugin-reload-grid")]
        public virtual IActionResult ReloadList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //restart application
            _webHelper.RestartAppDomain();

            return RedirectToAction("List");
        }

        public virtual IActionResult EditPopup(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a plugin with the specified system name
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
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
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(model.SystemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
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
                PluginManager.SavePluginDescriptor(pluginDescriptor);

                //reset plugin cache
                _pluginFinder.ReloadPlugins(pluginDescriptor);

                //locales
                foreach (var localized in model.Locales)
                {
                    _localizationService.SaveLocalizedFriendlyName(pluginDescriptor.Instance(), localized.LanguageId, localized.FriendlyName);
                }

                //enabled/disabled
                if (pluginDescriptor.Installed)
                {
                    var pluginInstance = pluginDescriptor.Instance();
                    switch (pluginInstance)
                    {
                        case IPaymentMethod paymentMethod:
                            if (_paymentService.IsPaymentMethodActive(paymentMethod) && !model.IsEnabled)
                            {
                                //mark as disabled
                                _paymentSettings.ActivePaymentMethodSystemNames.Remove(pluginDescriptor.SystemName);
                                _settingService.SaveSetting(_paymentSettings);
                                break;
                            }

                            if (!_paymentService.IsPaymentMethodActive(paymentMethod) && model.IsEnabled)
                            {
                                //mark as enabled
                                _paymentSettings.ActivePaymentMethodSystemNames.Add(pluginDescriptor.SystemName);
                                _settingService.SaveSetting(_paymentSettings);
                            }

                            break;
                        case IShippingRateComputationMethod shippingRateComputationMethod:
                            if (_shippingService.IsShippingRateComputationMethodActive(shippingRateComputationMethod) && !model.IsEnabled)
                            {
                                //mark as disabled
                                _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                                _settingService.SaveSetting(_shippingSettings);
                                break;
                            }

                            if (!_shippingService.IsShippingRateComputationMethodActive(shippingRateComputationMethod) && model.IsEnabled)
                            {
                                //mark as enabled
                                _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add(pluginDescriptor.SystemName);
                                _settingService.SaveSetting(_shippingSettings);
                            }

                            break;
                        case IPickupPointProvider pickupPointProvider:
                            if (_shippingService.IsPickupPointProviderActive(pickupPointProvider) && !model.IsEnabled)
                            {
                                //mark as disabled
                                _shippingSettings.ActivePickupPointProviderSystemNames.Remove(pluginDescriptor.SystemName);
                                _settingService.SaveSetting(_shippingSettings);
                                break;
                            }

                            if (!_shippingService.IsPickupPointProviderActive(pickupPointProvider) && model.IsEnabled)
                            {
                                //mark as enabled
                                _shippingSettings.ActivePickupPointProviderSystemNames.Add(pluginDescriptor.SystemName);
                                _settingService.SaveSetting(_shippingSettings);
                            }

                            break;
                        case ITaxProvider _:
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
                            if (_externalAuthenticationService.IsExternalAuthenticationMethodActive(externalAuthenticationMethod) && !model.IsEnabled)
                            {
                                //mark as disabled
                                _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                                _settingService.SaveSetting(_externalAuthenticationSettings);
                                break;
                            }

                            if (!_externalAuthenticationService.IsExternalAuthenticationMethodActive(externalAuthenticationMethod) && model.IsEnabled)
                            {
                                //mark as enabled
                                _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(pluginDescriptor.SystemName);
                                _settingService.SaveSetting(_externalAuthenticationSettings);
                            }

                            break;
                        case IWidgetPlugin widgetPlugin:
                            if (_widgetService.IsWidgetActive(widgetPlugin) && !model.IsEnabled)
                            {
                                //mark as disabled
                                _widgetSettings.ActiveWidgetSystemNames.Remove(pluginDescriptor.SystemName);
                                _settingService.SaveSetting(_widgetSettings);
                                break;
                            }

                            if (!_widgetService.IsWidgetActive(widgetPlugin) && model.IsEnabled)
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
                }

                ViewBag.RefreshPage = true;

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
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _pluginModelFactory.PrepareOfficialFeedPluginListModel(searchModel);

            return Json(model);
        }

        #endregion
    }
}