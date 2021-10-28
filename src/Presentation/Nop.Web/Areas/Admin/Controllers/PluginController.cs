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

        protected ExternalAuthenticationSettings ExternalAuthenticationSettings { get; }
        protected IAuthenticationPluginManager AuthenticationPluginManager { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IMultiFactorAuthenticationPluginManager MultiFactorAuthenticationPluginManager { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IPaymentPluginManager PaymentPluginManager { get; }
        protected IPickupPluginManager PickupPluginManager { get; }
        protected IPluginModelFactory PluginModelFactory { get; }
        protected IPluginService PluginService { get; }
        protected ISettingService SettingService { get; }
        protected IShippingPluginManager ShippingPluginManager { get; }
        protected IUploadService UploadService { get; }
        protected IWebHelper WebHelper { get; }
        protected IWidgetPluginManager WidgetPluginManager { get; }
        protected IWorkContext WorkContext { get; }
        protected MultiFactorAuthenticationSettings MultiFactorAuthenticationSettings { get; }
        protected PaymentSettings PaymentSettings { get; }
        protected ShippingSettings ShippingSettings { get; }
        protected TaxSettings TaxSettings { get; }
        protected WidgetSettings WidgetSettings { get; }

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
            ExternalAuthenticationSettings = externalAuthenticationSettings;
            AuthenticationPluginManager = authenticationPluginManager;
            CustomerActivityService = customerActivityService;
            EventPublisher = eventPublisher;
            LocalizationService = localizationService;
            MultiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
            NotificationService = notificationService;
            PermissionService = permissionService;
            PaymentPluginManager = paymentPluginManager;
            PickupPluginManager = pickupPluginManager;
            PluginModelFactory = pluginModelFactory;
            PluginService = pluginService;
            SettingService = settingService;
            ShippingPluginManager = shippingPluginManager;
            UploadService = uploadService;
            WebHelper = webHelper;
            WidgetPluginManager = widgetPluginManager;
            WorkContext = workContext;
            MultiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
            PaymentSettings = paymentSettings;
            ShippingSettings = shippingSettings;
            TaxSettings = taxSettings;
            WidgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = await PluginModelFactory.PreparePluginSearchModelAsync(new PluginSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ListSelect(PluginSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await PluginModelFactory.PreparePluginListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> AdminNavigationPlugins()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return Json(new List<string>());

            //prepare models
            var models = await (await PluginModelFactory.PrepareAdminNavigationPluginModelsAsync()).SelectAwait(async model => new
            {
                title = model.FriendlyName,
                link = model.ConfigurationUrl,
                parent = await LocalizationService.GetResourceAsync("Admin.Configuration.Plugins.Local"),
                grandParent = string.Empty,
                rate = -50 //negative rate is set to move plugins to the end of list
            }).ToListAsync();

            return Json(models);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UploadPluginsAndThemes(IFormFile archivefile)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                if (archivefile == null || archivefile.Length == 0)
                    throw new NopException(await LocalizationService.GetResourceAsync("Admin.Common.UploadFile"));

                var descriptors = UploadService.UploadPluginsAndThemes(archivefile);
                var pluginDescriptors = descriptors.OfType<PluginDescriptor>().ToList();
                var themeDescriptors = descriptors.OfType<ThemeDescriptor>().ToList();

                //activity log
                foreach (var descriptor in pluginDescriptors)
                {
                    await CustomerActivityService.InsertActivityAsync("UploadNewPlugin",
                        string.Format(await LocalizationService.GetResourceAsync("ActivityLog.UploadNewPlugin"), descriptor.FriendlyName));
                }

                foreach (var descriptor in themeDescriptors)
                {
                    await CustomerActivityService.InsertActivityAsync("UploadNewTheme",
                        string.Format(await LocalizationService.GetResourceAsync("ActivityLog.UploadNewTheme"), descriptor.FriendlyName));
                }

                //events
                if (pluginDescriptors.Any())
                    await EventPublisher.PublishAsync(new PluginsUploadedEvent(pluginDescriptors));

                if (themeDescriptors.Any())
                    await EventPublisher.PublishAsync(new ThemesUploadedEvent(themeDescriptors));

                var message = string.Format(await LocalizationService.GetResourceAsync("Admin.Configuration.Plugins.Uploaded"), pluginDescriptors.Count, themeDescriptors.Count);
                NotificationService.SuccessNotification(message);

                return View("RestartApplication", Url.Action("List", "Plugin"));
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired(FormValueRequirement.StartsWith, "install-plugin-link-")]
        public virtual async Task<IActionResult> Install(IFormCollection form)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                //get plugin system name
                string systemName = null;
                foreach (var formValue in form.Keys)
                    if (formValue.StartsWith("install-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
                        systemName = formValue["install-plugin-link-".Length..];

                var pluginDescriptor = await PluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is not installed
                if (pluginDescriptor.Installed)
                    return RedirectToAction("List");

                await PluginService.PreparePluginToInstallAsync(pluginDescriptor.SystemName, await WorkContext.GetCurrentCustomerAsync());
                pluginDescriptor.ShowInPluginsList = false;
                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired(FormValueRequirement.StartsWith, "uninstall-plugin-link-")]
        public virtual async Task<IActionResult> Uninstall(IFormCollection form)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                //get plugin system name
                string systemName = null;
                foreach (var formValue in form.Keys)
                    if (formValue.StartsWith("uninstall-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
                        systemName = formValue["uninstall-plugin-link-".Length..];

                var pluginDescriptor = await PluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is installed
                if (!pluginDescriptor.Installed)
                    return RedirectToAction("List");

                await PluginService.PreparePluginToUninstallAsync(pluginDescriptor.SystemName);
                pluginDescriptor.ShowInPluginsList = false;
                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired(FormValueRequirement.StartsWith, "delete-plugin-link-")]
        public virtual async Task<IActionResult> Delete(IFormCollection form)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                //get plugin system name
                string systemName = null;
                foreach (var formValue in form.Keys)
                    if (formValue.StartsWith("delete-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
                        systemName = formValue["delete-plugin-link-".Length..];

                var pluginDescriptor = await PluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);

                //check whether plugin is not installed
                if (pluginDescriptor.Installed)
                    return RedirectToAction("List");

                await PluginService.PreparePluginToDeleteAsync(pluginDescriptor.SystemName);
                pluginDescriptor.ShowInPluginsList = false;
                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Plugins.ChangesApplyAfterReboot"));
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("plugin-reload-grid")]
        public virtual async Task<IActionResult> ReloadList()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            await PluginService.UninstallPluginsAsync();
            await PluginService.DeletePluginsAsync();

            return View("RestartApplication", Url.Action("List", "Plugin"));
        }

        public virtual async Task<IActionResult> UninstallAndDeleteUnusedPlugins(string[] names)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            foreach (var name in names)
                await PluginService.PreparePluginToUninstallAsync(name);

            await PluginService.UninstallPluginsAsync();

            foreach (var name in names)
                await PluginService.PreparePluginToDeleteAsync(name);

            await PluginService.DeletePluginsAsync();

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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            PluginService.ResetChanges();

            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> EditPopup(string systemName)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a plugin with the specified system name
            var pluginDescriptor = await PluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                return RedirectToAction("List");

            //prepare model
            var model = await PluginModelFactory.PreparePluginModelAsync(null, pluginDescriptor);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> EditPopup(PluginModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a plugin with the specified system name
            var pluginDescriptor = await PluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(model.SystemName, LoadPluginsMode.All);
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
                await EventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

                //locales
                var pluginInstance = pluginDescriptor.Instance<IPlugin>();
                foreach (var localized in model.Locales)
                {
                    await LocalizationService.SaveLocalizedFriendlyNameAsync(pluginInstance, localized.LanguageId, localized.FriendlyName);
                }

                //enabled/disabled
                if (!pluginDescriptor.Installed)
                    return View(model);

                var pluginIsActive = false;
                switch (pluginInstance)
                {
                    case IPaymentMethod paymentMethod:
                        pluginIsActive = PaymentPluginManager.IsPluginActive(paymentMethod);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            PaymentSettings.ActivePaymentMethodSystemNames.Remove(pluginDescriptor.SystemName);
                            await SettingService.SaveSettingAsync(PaymentSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            PaymentSettings.ActivePaymentMethodSystemNames.Add(pluginDescriptor.SystemName);
                            await SettingService.SaveSettingAsync(PaymentSettings);
                        }

                        break;
                    case IShippingRateComputationMethod shippingRateComputationMethod:
                        pluginIsActive = ShippingPluginManager.IsPluginActive(shippingRateComputationMethod);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            ShippingSettings.ActiveShippingRateComputationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                            await SettingService.SaveSettingAsync(ShippingSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            ShippingSettings.ActiveShippingRateComputationMethodSystemNames.Add(pluginDescriptor.SystemName);
                            await SettingService.SaveSettingAsync(ShippingSettings);
                        }

                        break;
                    case IPickupPointProvider pickupPointProvider:
                        pluginIsActive = PickupPluginManager.IsPluginActive(pickupPointProvider);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            ShippingSettings.ActivePickupPointProviderSystemNames.Remove(pluginDescriptor.SystemName);
                            await SettingService.SaveSettingAsync(ShippingSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            ShippingSettings.ActivePickupPointProviderSystemNames.Add(pluginDescriptor.SystemName);
                            await SettingService.SaveSettingAsync(ShippingSettings);
                        }

                        break;
                    case ITaxProvider taxProvider:
                        if (!model.IsEnabled)
                        {
                            //mark as disabled
                            TaxSettings.ActiveTaxProviderSystemName = string.Empty;
                            await SettingService.SaveSettingAsync(TaxSettings);
                            break;
                        }

                        //mark as enabled
                        TaxSettings.ActiveTaxProviderSystemName = model.SystemName;
                        await SettingService.SaveSettingAsync(TaxSettings);
                        break;
                    case IExternalAuthenticationMethod externalAuthenticationMethod:
                        pluginIsActive = AuthenticationPluginManager.IsPluginActive(externalAuthenticationMethod);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            ExternalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                            await SettingService.SaveSettingAsync(ExternalAuthenticationSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            ExternalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(pluginDescriptor.SystemName);
                            await SettingService.SaveSettingAsync(ExternalAuthenticationSettings);
                        }

                        break;
                    case IMultiFactorAuthenticationMethod multiFactorAuthenticationMethod:
                        pluginIsActive = MultiFactorAuthenticationPluginManager.IsPluginActive(multiFactorAuthenticationMethod);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            MultiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(pluginDescriptor.SystemName);
                            await SettingService.SaveSettingAsync(MultiFactorAuthenticationSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            MultiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(pluginDescriptor.SystemName);
                            await SettingService.SaveSettingAsync(MultiFactorAuthenticationSettings);
                        }

                        break;
                    case IWidgetPlugin widgetPlugin:
                        pluginIsActive = WidgetPluginManager.IsPluginActive(widgetPlugin);
                        if (pluginIsActive && !model.IsEnabled)
                        {
                            //mark as disabled
                            WidgetSettings.ActiveWidgetSystemNames.Remove(pluginDescriptor.SystemName);
                            await SettingService.SaveSettingAsync(WidgetSettings);
                            break;
                        }

                        if (!pluginIsActive && model.IsEnabled)
                        {
                            //mark as enabled
                            WidgetSettings.ActiveWidgetSystemNames.Add(pluginDescriptor.SystemName);
                            await SettingService.SaveSettingAsync(WidgetSettings);
                        }

                        break;
                }

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditPlugin",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditPlugin"), pluginDescriptor.FriendlyName));

                return View(model);
            }

            //prepare model
            model = await PluginModelFactory.PreparePluginModelAsync(model, pluginDescriptor, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> OfficialFeed()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //prepare model
            var model = await PluginModelFactory.PrepareOfficialFeedPluginSearchModelAsync(new OfficialFeedPluginSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OfficialFeedSelect(OfficialFeedPluginSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await PluginModelFactory.PrepareOfficialFeedPluginListModelAsync(searchModel);

            return Json(model);
        }

        #endregion
    }
}