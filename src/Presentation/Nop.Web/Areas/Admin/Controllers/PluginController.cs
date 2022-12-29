using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Cms;
using Nop.Services.Configuration;
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
        private readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
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
        private readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;
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
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
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
            MultiFactorAuthenticationSettings multiFactorAuthenticationSettings,
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
            _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
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
            _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
            _paymentSettings = paymentSettings;
            _shippingSettings = shippingSettings;
            _taxSettings = taxSettings;
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Utils

        //TODO: make CommonModelFactory.PreparePluginsWarningModelAsync method public and delete this one
        protected virtual async Task<List<string>> GetPluginsWarningModelAsync()
        {
            var warnings = new List<string>();

            var pluginInfo = Singleton<IPluginsInfo>.Instance;

            foreach (var pluginName in pluginInfo.IncompatiblePlugins) 
                warnings.Add(string.Format($"<b>{pluginName.Key}</b> plugin: {pluginName.Value}"));

            var assemblyCollisions = _pluginService.GetAssemblyCollisions();

            if (!assemblyCollisions.Any())
                return warnings;

            var warningFormat = await _localizationService
                .GetResourceAsync("Admin.System.Warnings.PluginRequiredAssembly");

            //check whether there are any collision of loaded assembly
            foreach (var assembly in assemblyCollisions)
            {
                //get plugin references message
                var message = assembly.Collisions
                    .Select(item => string.Format(warningFormat, item.PluginName, item.AssemblyName))
                    .Aggregate("", (current, all) => all + ", " + current).TrimEnd(',', ' ');

                warnings.Add(string.Format(
                    await _localizationService.GetResourceAsync("Admin.System.Warnings.AssemblyHasCollision"),
                    assembly.ShortName, assembly.AssemblyFullNameInMemory, message));
            }

            return warnings;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = await _pluginModelFactory.PreparePluginSearchModelAsync(new PluginSearchModel());

            var warnings = await GetPluginsWarningModelAsync();
            if (warnings.Any())
                _notificationService.WarningNotification(string.Join("<br />", warnings), false);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ListSelect(PluginSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _pluginModelFactory.PreparePluginListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> AdminNavigationPlugins()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return Json(new List<string>());

            //prepare models
            var models = await (await _pluginModelFactory.PrepareAdminNavigationPluginModelsAsync()).SelectAwait(async model => new
            {
                title = model.FriendlyName,
                link = model.ConfigurationUrl,
                parent = await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.Local"),
                grandParent = string.Empty,
                rate = -50 //negative rate is set to move plugins to the end of list
            }).ToListAsync();

            return Json(models);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UploadPluginsAndThemes(IFormFile archivefile)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                if (archivefile == null || archivefile.Length == 0)
                    throw new NopException(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));

                var descriptors = await _uploadService.UploadPluginsAndThemesAsync(archivefile);
                var pluginDescriptors = descriptors.OfType<PluginDescriptor>().ToList();
                var themeDescriptors = descriptors.OfType<ThemeDescriptor>().ToList();

                //activity log
                foreach (var descriptor in pluginDescriptors)
                {
                    await _customerActivityService.InsertActivityAsync("UploadNewPlugin",
                        string.Format(await _localizationService.GetResourceAsync("ActivityLog.UploadNewPlugin"), descriptor.FriendlyName));
                }

                foreach (var descriptor in themeDescriptors)
                {
                    await _customerActivityService.InsertActivityAsync("UploadNewTheme",
                        string.Format(await _localizationService.GetResourceAsync("ActivityLog.UploadNewTheme"), descriptor.FriendlyName));
                }

                //events
                if (pluginDescriptors.Any())
                    await _eventPublisher.PublishAsync(new PluginsUploadedEvent(pluginDescriptors));

                if (themeDescriptors.Any())
                    await _eventPublisher.PublishAsync(new ThemesUploadedEvent(themeDescriptors));

                var message = string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.Uploaded"), pluginDescriptors.Count, themeDescriptors.Count);
                _notificationService.SuccessNotification(message);

                if (themeDescriptors.Any())
                    return View("RestartApplication", Url.Action("List", "Plugin"));
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired(FormValueRequirement.StartsWith, "install-plugin-link-")]
        public virtual async Task<IActionResult> Install(IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                //get plugin system name
                string systemName = null;
                foreach (var formValue in form.Keys)
                    if (formValue.StartsWith("install-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
                        systemName = formValue["install-plugin-link-".Length..];

                var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is not installed
                if (pluginDescriptor.Installed)
                    return RedirectToAction("List");

                await _pluginService.PreparePluginToInstallAsync(pluginDescriptor.SystemName, await _workContext.GetCurrentCustomerAsync());
                pluginDescriptor.ShowInPluginsList = false;
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired(FormValueRequirement.StartsWith, "uninstall-plugin-link-")]
        public virtual async Task<IActionResult> Uninstall(IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                //get plugin system name
                string systemName = null;
                foreach (var formValue in form.Keys)
                    if (formValue.StartsWith("uninstall-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
                        systemName = formValue["uninstall-plugin-link-".Length..];

                var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is installed
                if (!pluginDescriptor.Installed)
                    return RedirectToAction("List");

                await _pluginService.PreparePluginToUninstallAsync(pluginDescriptor.SystemName);
                pluginDescriptor.ShowInPluginsList = false;
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired(FormValueRequirement.StartsWith, "delete-plugin-link-")]
        public virtual async Task<IActionResult> Delete(IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                //get plugin system name
                string systemName = null;
                foreach (var formValue in form.Keys)
                    if (formValue.StartsWith("delete-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
                        systemName = formValue["delete-plugin-link-".Length..];

                var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);

                //check whether plugin is not installed
                if (pluginDescriptor.Installed)
                    return RedirectToAction("List");

                await _pluginService.PreparePluginToDeleteAsync(pluginDescriptor.SystemName);
                pluginDescriptor.ShowInPluginsList = false;
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("plugin-reload-grid")]
        public virtual async Task<IActionResult> ReloadList()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            await _pluginService.UninstallPluginsAsync();
            await _pluginService.DeletePluginsAsync();

            return View("RestartApplication", Url.Action("List", "Plugin"));
        }

        public virtual async Task<IActionResult> UninstallAndDeleteUnusedPlugins(string[] names)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            foreach (var name in names)
                await _pluginService.PreparePluginToUninstallAsync(name);

            await _pluginService.UninstallPluginsAsync();

            foreach (var name in names)
                await _pluginService.PreparePluginToDeleteAsync(name);

            await _pluginService.DeletePluginsAsync();

            return View("RestartApplication", Url.Action("Warnings", "Common"));
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("plugin-apply-changes")]
        public virtual async Task<IActionResult> ApplyChanges()
        {
            return await ReloadList();
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("plugin-discard-changes")]
        public virtual async Task<IActionResult> DiscardChanges()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            _pluginService.ResetChanges();

            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> EditPopup(string systemName)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a plugin with the specified system name
            var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _pluginModelFactory.PreparePluginModelAsync(null, pluginDescriptor);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> EditPopup(PluginModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a plugin with the specified system name
            var pluginDescriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(model.SystemName, LoadPluginsMode.All);
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
                await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

                //locales
                var pluginInstance = pluginDescriptor.Instance<IPlugin>();
                foreach (var localized in model.Locales)
                {
                    await _localizationService.SaveLocalizedFriendlyNameAsync(pluginInstance, localized.LanguageId, localized.FriendlyName);
                }

                //enabled/disabled
                if (!pluginDescriptor.Installed)
                    return View(model);

                bool pluginIsActive;
                switch (pluginInstance)
                {
                    case IPaymentMethod paymentMethod:
                        pluginIsActive = _paymentPluginManager.IsPluginActive(paymentMethod);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            _paymentSettings.ActivePaymentMethodSystemNames.Remove(pluginDescriptor.SystemName);
                            await _settingService.SaveSettingAsync(_paymentSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            _paymentSettings.ActivePaymentMethodSystemNames.Add(pluginDescriptor.SystemName);
                            await _settingService.SaveSettingAsync(_paymentSettings);
                        }

                        break;
                    case IShippingRateComputationMethod shippingRateComputationMethod:
                        pluginIsActive = _shippingPluginManager.IsPluginActive(shippingRateComputationMethod);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                            await _settingService.SaveSettingAsync(_shippingSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add(pluginDescriptor.SystemName);
                            await _settingService.SaveSettingAsync(_shippingSettings);
                        }

                        break;
                    case IPickupPointProvider pickupPointProvider:
                        pluginIsActive = _pickupPluginManager.IsPluginActive(pickupPointProvider);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            _shippingSettings.ActivePickupPointProviderSystemNames.Remove(pluginDescriptor.SystemName);
                            await _settingService.SaveSettingAsync(_shippingSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            _shippingSettings.ActivePickupPointProviderSystemNames.Add(pluginDescriptor.SystemName);
                            await _settingService.SaveSettingAsync(_shippingSettings);
                        }

                        break;
                    case ITaxProvider taxProvider:
                        if (!model.IsEnabled)
                        {
                            //mark as disabled
                            _taxSettings.ActiveTaxProviderSystemName = string.Empty;
                            await _settingService.SaveSettingAsync(_taxSettings);
                            break;
                        }

                        //mark as enabled
                        _taxSettings.ActiveTaxProviderSystemName = model.SystemName;
                        await _settingService.SaveSettingAsync(_taxSettings);
                        break;
                    case IExternalAuthenticationMethod externalAuthenticationMethod:
                        pluginIsActive = _authenticationPluginManager.IsPluginActive(externalAuthenticationMethod);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                            await _settingService.SaveSettingAsync(_externalAuthenticationSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(pluginDescriptor.SystemName);
                            await _settingService.SaveSettingAsync(_externalAuthenticationSettings);
                        }

                        break;
                    case IMultiFactorAuthenticationMethod multiFactorAuthenticationMethod:
                        pluginIsActive = _multiFactorAuthenticationPluginManager.IsPluginActive(multiFactorAuthenticationMethod);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            _multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                            await _settingService.SaveSettingAsync(_multiFactorAuthenticationSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            _multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(pluginDescriptor.SystemName);
                            await _settingService.SaveSettingAsync(_multiFactorAuthenticationSettings);
                        }

                        break;
                    case IWidgetPlugin widgetPlugin:
                        pluginIsActive = _widgetPluginManager.IsPluginActive(widgetPlugin);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            _widgetSettings.ActiveWidgetSystemNames.Remove(pluginDescriptor.SystemName);
                            await _settingService.SaveSettingAsync(_widgetSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            _widgetSettings.ActiveWidgetSystemNames.Add(pluginDescriptor.SystemName);
                            await _settingService.SaveSettingAsync(_widgetSettings);
                        }

                        break;
                }

                //activity log
                await _customerActivityService.InsertActivityAsync("EditPlugin",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditPlugin"), pluginDescriptor.FriendlyName));

                return View(model);
            }

            //prepare model
            model = await _pluginModelFactory.PreparePluginModelAsync(model, pluginDescriptor, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> OfficialFeed()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //prepare model
            var model = await _pluginModelFactory.PrepareOfficialFeedPluginSearchModelAsync(new OfficialFeedPluginSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OfficialFeedSelect(OfficialFeedPluginSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _pluginModelFactory.PrepareOfficialFeedPluginListModelAsync(searchModel);

            return Json(model);
        }

        #endregion
    }
}