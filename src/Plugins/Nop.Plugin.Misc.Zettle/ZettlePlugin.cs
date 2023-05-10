using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Media;
using Nop.Plugin.Misc.Zettle.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.Zettle
{
    /// <summary>
    /// Represents Zettle plugin
    /// </summary>
    public class ZettlePlugin : BasePlugin, IAdminMenuPlugin, IMiscPlugin
    {
        #region Fields

        protected readonly IActionContextAccessor _actionContextAccessor;
        protected readonly ILocalizationService _localizationService;
        protected readonly IScheduleTaskService _scheduleTaskService;
        protected readonly ISettingService _settingService;
        protected readonly IUrlHelperFactory _urlHelperFactory;
        protected readonly MediaSettings _mediaSettings;
        protected readonly ZettleService _zettleService;

        #endregion

        #region Ctor

        public ZettlePlugin(IActionContextAccessor actionContextAccessor,
            ILocalizationService localizationService,
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService,
            IUrlHelperFactory urlHelperFactory,
            MediaSettings mediaSettings,
            ZettleService zettleService)
        {
            _actionContextAccessor = actionContextAccessor;
            _localizationService = localizationService;
            _scheduleTaskService = scheduleTaskService;
            _settingService = settingService;
            _urlHelperFactory = urlHelperFactory;
            _mediaSettings = mediaSettings;
            _zettleService = zettleService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(ZettleDefaults.ConfigurationRouteName);
        }

        /// <summary>
        /// Manage sitemap. You can use "SystemName" of menu items to manage existing sitemap or add a new menu item.
        /// </summary>
        /// <param name="rootNode">Root node of the sitemap.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var configurationItem = rootNode.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Configuration"));
            if (configurationItem is null)
                return;

            var shippingItem = configurationItem.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Shipping"));
            var widgetsItem = configurationItem.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Widgets"));
            if (shippingItem is null && widgetsItem is null)
                return;

            var index = shippingItem is not null ? configurationItem.ChildNodes.IndexOf(shippingItem) : -1;
            if (index < 0)
                index = widgetsItem is not null ? configurationItem.ChildNodes.IndexOf(widgetsItem) : -1;
            if (index < 0)
                return;

            configurationItem.ChildNodes.Insert(index + 1, new SiteMapNode
            {
                Visible = true,
                SystemName = "POS plugins",
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.Zettle.Menu.Pos"),
                IconClass = "far fa-dot-circle",
                ChildNodes = new List<SiteMapNode>
                {
                    new()
                    {
                        Visible = true,
                        SystemName = PluginDescriptor.SystemName,
                        Title = PluginDescriptor.FriendlyName,
                        ControllerName = "ZettleAdmin",
                        ActionName = "Configure",
                        IconClass = "far fa-circle",
                        RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
                    }
                }
            });
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //ensure MediaSettings.UseAbsoluteImagePath is enabled (used for images uploading)
            await _settingService.SetSettingAsync($"{nameof(MediaSettings)}.{nameof(MediaSettings.UseAbsoluteImagePath)}", true, clearCache: false);

            await _settingService.SaveSettingAsync(new ZettleSettings
            {
                SyncEnabled = true,
                DefaultTaxEnabled = true,
                AutoSyncEnabled = false,
                AutoSyncPeriod = ZettleDefaults.SynchronizationTask.Period / 60,
                RequestTimeout = ZettleDefaults.RequestTimeout,
                ImportProductsNumber = ZettleDefaults.ImportProductsNumber,
                AutoAddRecordsEnabled = true,
                LogSyncMessages = true,
                CategorySyncEnabled = true,
                ClearRecordsOnChangeCredentials = true
            });

            if (await _scheduleTaskService.GetTaskByTypeAsync(ZettleDefaults.SynchronizationTask.Type) is null)
            {
                await _scheduleTaskService.InsertTaskAsync(new()
                {
                    Enabled = false,
                    StopOnError = false,
                    LastEnabledUtc = DateTime.UtcNow,
                    Name = ZettleDefaults.SynchronizationTask.Name,
                    Type = ZettleDefaults.SynchronizationTask.Type,
                    Seconds = ZettleDefaults.SynchronizationTask.Period
                });
            }

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Misc.Zettle.Menu.Pos"] = "POS",

                ["Plugins.Misc.Zettle.Credentials"] = "Credentials",
                ["Plugins.Misc.Zettle.Credentials.AccessRevoked"] = "Access to PayPal Zettle organization has been revoked. You need to reconfigure the plugin.",
                ["Plugins.Misc.Zettle.Credentials.Connected"] = "Connected",
                ["Plugins.Misc.Zettle.Credentials.Disconnected"] = "Disconnected",
                ["Plugins.Misc.Zettle.Credentials.Revoke"] = "Revoke Access",
                ["Plugins.Misc.Zettle.Credentials.SignUp"] = "Sign Up",
                ["Plugins.Misc.Zettle.Credentials.Status"] = "Status",

                ["Plugins.Misc.Zettle.Configuration.Error"] = "Error: {0} (see details in the <a href=\"{1}\" target=\"_blank\">log</a>)",
                ["Plugins.Misc.Zettle.Configuration.Fields.ClientId"] = "Client ID",
                ["Plugins.Misc.Zettle.Configuration.Fields.ClientId.Hint"] = "Enter the Client ID. The Client ID is used together with the API key to identify the merchant.",
                ["Plugins.Misc.Zettle.Configuration.Fields.ApiKey"] = "API Key",
                ["Plugins.Misc.Zettle.Configuration.Fields.ApiKey.Hint"] = "Enter the API Key. The API key contains merchant identity information, and is valid until the merchant revokes it.",
                ["Plugins.Misc.Zettle.Configuration.Fields.ApiKey.Required"] = "API key is required",
                ["Plugins.Misc.Zettle.Configuration.Fields.DisconnectOnUninstall"] = "Disconnect on uninstall",
                ["Plugins.Misc.Zettle.Configuration.Fields.DisconnectOnUninstall.Hint"] = "Determine whether to disconnect an app (Revoke Access) from an associated PayPal Zettle organisation when the plugin is uninstalled. In this case you will need to go through the signing-up process again. If disabled, you can use the same credentials after reinstalling the plugin.",
                ["Plugins.Misc.Zettle.Configuration.Fields.AutoSyncEnabled"] = "Enable auto synchronization",
                ["Plugins.Misc.Zettle.Configuration.Fields.AutoSyncEnabled.Hint"] = "Determine whether to enable auto synchronization. This will automatically synchronize changes for the selected products and add new ones. If disabled, synchronization must be started manually on this page.",
                ["Plugins.Misc.Zettle.Configuration.Fields.AutoSyncPeriod"] = "Auto synchronization period",
                ["Plugins.Misc.Zettle.Configuration.Fields.AutoSyncPeriod.Hint"] = "Set the period (in minutes) for auto synchronization.",
                ["Plugins.Misc.Zettle.Configuration.Fields.AutoSyncPeriod.Invalid"] = "Period is invalid",
                ["Plugins.Misc.Zettle.Configuration.Fields.DeleteBeforeImport"] = "Delete products before sync",
                ["Plugins.Misc.Zettle.Configuration.Fields.DeleteBeforeImport.Hint"] = "Determine whether to delete existing library items before importing products to PayPal Zettle. It may be required on the first synchronization if you want to keep only products from nopCommerce catalog. After that, it's recommended to disable this setting.",
                ["Plugins.Misc.Zettle.Configuration.Fields.SyncEnabled"] = "Sync enabled",
                ["Plugins.Misc.Zettle.Configuration.Fields.SyncEnabled.Hint"] = "Determine whether to synchronize the selected products by default. If disabled, the selected products will be inactive and will not be imported until you enable them.",
                ["Plugins.Misc.Zettle.Configuration.Fields.PriceSyncEnabled"] = "Price sync enabled",
                ["Plugins.Misc.Zettle.Configuration.Fields.PriceSyncEnabled.Hint"] = "Determine whether to synchronize prices of the selected products by default. If disabled, prices will have to be set manually in PayPal Zettle.",
                ["Plugins.Misc.Zettle.Configuration.Fields.ImageSyncEnabled"] = "Image sync enabled",
                ["Plugins.Misc.Zettle.Configuration.Fields.ImageSyncEnabled.Hint"] = "Determine whether to synchronize images of the selected products by default. A product in PayPal Zettle library can have a single image assigned.",
                ["Plugins.Misc.Zettle.Configuration.Fields.InventoryTrackingEnabled"] = "Inventory tracking enabled",
                ["Plugins.Misc.Zettle.Configuration.Fields.InventoryTrackingEnabled.Hint"] = "Determine whether to track inventory balance for products by default. The product in nopCommerce catalog must also have the appropriate inventory method selected. Tracking is enabled on sync. Then when tracking is enabled for a product, the inventory data on both sides is updated immediately and doesn't require synchronization.",
                ["Plugins.Misc.Zettle.Configuration.Fields.DefaultTaxEnabled"] = "Use default tax",
                ["Plugins.Misc.Zettle.Configuration.Fields.DefaultTaxEnabled.Hint"] = "Determine whether to use the country's default VAT rate or default sales tax rate for new products. If disabled, taxes will have to be set manually in PayPal Zettle.",
                ["Plugins.Misc.Zettle.Configuration.Fields.DiscountSyncEnabled"] = "Add discounts",
                ["Plugins.Misc.Zettle.Configuration.Fields.DiscountSyncEnabled.Hint"] = "Determine whether to import discounts from nopCommerce catalog (assigned to order subtotal only) to PayPal Zettle.",
                ["Plugins.Misc.Zettle.Configuration.Webhook.Warning"] = "Webhook was not created (see details in the <a href=\"{0}\" target=\"_blank\">log</a>), so some functions may not work correctly. Please ensure that your store is under SSL, PayPal Zettle doesn't send requests to unsecured sites.",

                ["Plugins.Misc.Zettle.Account.Pending"] = "Before your account is activated, you will go through a process to verify your identity. This process may take some time. See details <a href=\"{1}\" target=\"_blank\">here</a>. The current customer status is {0}.",
                ["Plugins.Misc.Zettle.Account.Fields.Name"] = "Name",
                ["Plugins.Misc.Zettle.Account.Fields.Name.Hint"] = "Displays the name of the connected account.",
                ["Plugins.Misc.Zettle.Account.Fields.Currency"] = "Currency",
                ["Plugins.Misc.Zettle.Account.Fields.Currency.Hint"] = "Displays the currency of the connected account.",
                ["Plugins.Misc.Zettle.Account.Fields.Currency.Warning"] = "The <a href=\"{2}\" target=\"_blank\">primary store currency</a> ({0}) doesn't match the currency of the merchant's PayPal Zettle account ({1}). In this case, prices will not be synchronized and will have to be set manually",
                ["Plugins.Misc.Zettle.Account.Fields.TaxationType"] = "Tax type",
                ["Plugins.Misc.Zettle.Account.Fields.TaxationType.Hint"] = "Displays the taxation type of the connected account. This setting determines which VAT/tax model is used for products and their prices.",
                ["Plugins.Misc.Zettle.Account.Fields.TaxationType.Vat.Warning"] = "The VAT settings don't match the settings of the merchant's PayPal Zettle account. See <a href=\"{0}\" target=\"_blank\">Tax settings</a>",
                ["Plugins.Misc.Zettle.Account.Fields.TaxationType.SalesTax.Warning"] = "Default sales tax rate not set. Please complete your PayPal Zettle account setup before continuing, otherwise, no sales tax information will be applied to the imported products.",
                ["Plugins.Misc.Zettle.Account.Fields.TaxationMode"] = "Price type",
                ["Plugins.Misc.Zettle.Account.Fields.TaxationMode.Hint"] = "Displays the taxation mode of the connected account. Product prices may be inclusive taxes (net prices) or exclusive taxes (gross prices). This setting only provides information how product prices are stored.",
                ["Plugins.Misc.Zettle.Account.Fields.TaxationMode.Warning"] = "The store price type doesn't match the price type of the merchant's PayPal Zettle account. See 'Prices include tax' in <a href=\"{0}\" target=\"_blank\">Tax settings</a>",

                ["Plugins.Misc.Zettle.Import.Fields.StartDate"] = "Start date",
                ["Plugins.Misc.Zettle.Import.Fields.StartDate.Hint"] = "Displays the start date and time of the import.",
                ["Plugins.Misc.Zettle.Import.Fields.EndDate"] = "End date",
                ["Plugins.Misc.Zettle.Import.Fields.EndDate.Hint"] = "Displays the end date and time of the import.",
                ["Plugins.Misc.Zettle.Import.Fields.State"] = "State",
                ["Plugins.Misc.Zettle.Import.Fields.State.Hint"] = "Displays the current state of the import.",
                ["Plugins.Misc.Zettle.Import.Fields.Items"] = "Imported items",
                ["Plugins.Misc.Zettle.Import.Fields.Items.Hint"] = "Displays the number of imported items.",

                ["Plugins.Misc.Zettle.Sync"] = "Synchronization",
                ["Plugins.Misc.Zettle.Sync.AddProduct"] = "Add product",
                ["Plugins.Misc.Zettle.Sync.AddProduct.Success"] = "Products successfully added",
                ["Plugins.Misc.Zettle.Sync.AddProduct.Warning"] = "{0} products were not added because their SKU are not specified",
                ["Plugins.Misc.Zettle.Sync.DeleteSelected"] = "Delete selected",
                ["Plugins.Misc.Zettle.Sync.Fields.Active"] = "Active",
                ["Plugins.Misc.Zettle.Sync.Fields.Product"] = "Product",
                ["Plugins.Misc.Zettle.Sync.Fields.PriceSyncEnabled"] = "Price sync enabled",
                ["Plugins.Misc.Zettle.Sync.Fields.ImageSyncEnabled"] = "Image sync enabled",
                ["Plugins.Misc.Zettle.Sync.Fields.InventoryTrackingEnabled"] = "Inventory tracking enabled",
                ["Plugins.Misc.Zettle.Sync.Fields.UpdatedDate"] = "Updated",
                ["Plugins.Misc.Zettle.Sync.Last"] = "Last import",
                ["Plugins.Misc.Zettle.Sync.Start"] = "Start Synchronization",
                ["Plugins.Misc.Zettle.Sync.Start.Confirm"] = @"
                    <p>
                        You want to start synchronization with the connected account.
                    </p>
                    <ol>
                        <li>Discounts assigned to order subtotal will be added if the setting is enabled.</li>
                        <li>Existing library items will be removed before products are imported if the setting is enabled.</li>
                        <li>Products removed from the catalog will be removed.</li>
                        <li>Updated images will be replaced.</li>
                        <li>Added and updated products will be imported with the appropriate settings (prices, images, inventory tracking, default tax).</li>
                    </ol>
                    <p>
                        Synchronization may take some time.
                    </p>",
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            var zettleSettings = await _settingService.LoadSettingAsync<ZettleSettings>();
            if (zettleSettings.DisconnectOnUninstall)
                await _zettleService.DisconnectAsync();
            else if (!string.IsNullOrEmpty(zettleSettings.WebhookUrl))
                await _zettleService.DeleteWebhookAsync();

            await _settingService.DeleteSettingAsync<ZettleSettings>();
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.Zettle");

            var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(ZettleDefaults.SynchronizationTask.Type);
            if (scheduleTask is not null)
                await _scheduleTaskService.DeleteTaskAsync(scheduleTask);

            await base.UninstallAsync();
        }

        #endregion
    }
}