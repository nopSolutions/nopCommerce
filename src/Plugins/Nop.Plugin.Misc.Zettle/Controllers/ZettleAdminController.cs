using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Misc.Zettle.Domain;
using Nop.Plugin.Misc.Zettle.Models;
using Nop.Plugin.Misc.Zettle.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.Zettle.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class ZettleAdminController : BasePluginController
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly TaxSettings _taxSettings;
        private readonly ZettleRecordService _zettleRecordService;
        private readonly ZettleService _zettleService;
        private readonly ZettleSettings _zettleSettings;

        #endregion

        #region Ctor

        public ZettleAdminController(CurrencySettings currencySettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IProductService productService,
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService,
            IStoreContext storeContext,
            TaxSettings taxSettings,
            ZettleRecordService zettleRecordService,
            ZettleService zettleService,
            ZettleSettings zettleSettings)
        {
            _currencySettings = currencySettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _currencyService = currencyService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _productService = productService;
            _scheduleTaskService = scheduleTaskService;
            _settingService = settingService;
            _storeContext = storeContext;
            _taxSettings = taxSettings;
            _zettleRecordService = zettleRecordService;
            _zettleService = zettleService;
            _zettleSettings = zettleSettings;
        }

        #endregion

        #region Methods

        #region Configuration

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                ClientId = _zettleSettings.ClientId,
                ApiKey = _zettleSettings.ApiKey,
                DisconnectOnUninstall = _zettleSettings.DisconnectOnUninstall,
                AutoSyncEnabled = _zettleSettings.AutoSyncEnabled,
                AutoSyncPeriod = _zettleSettings.AutoSyncPeriod,
                DeleteBeforeImport = _zettleSettings.DeleteBeforeImport,
                SyncEnabled = _zettleSettings.SyncEnabled,
                PriceSyncEnabled = _zettleSettings.PriceSyncEnabled,
                ImageSyncEnabled = _zettleSettings.ImageSyncEnabled,
                InventoryTrackingEnabled = _zettleSettings.InventoryTrackingEnabled,
                DefaultTaxEnabled = _zettleSettings.DefaultTaxEnabled,
                DiscountSyncEnabled = _zettleSettings.DiscountSyncEnabled,
            };

            if (ZettleService.IsConfigured(_zettleSettings))
            {
                //account info
                var (accountInfo, error) = await _zettleService.GetAccountInfoAsync();
                if (!string.IsNullOrEmpty(error) || accountInfo is null)
                {
                    var locale = await _localizationService.GetResourceAsync("Plugins.Misc.Zettle.Configuration.Error");
                    var errorMessage = string.Format(locale, error, Url.Action("List", "Log"));
                    _notificationService.ErrorNotification(errorMessage, false);

                    return View("~/Plugins/Misc.Zettle/Views/Configure.cshtml", model);
                }

                model.Connected = true;
                model.Account.Name = accountInfo.Name;
                model.Account.CustomerStatus = accountInfo.CustomerStatus?.ToLower() ?? "undefined";
                model.Account.Accepted = string.Equals(accountInfo.CustomerStatus, "ACCEPTED", StringComparison.InvariantCultureIgnoreCase);

                //ensure the same currencies are used
                var storeCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
                if (string.IsNullOrEmpty(accountInfo.Currency) ||
                    !accountInfo.Currency.Equals(storeCurrency.CurrencyCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    var locale = await _localizationService.GetResourceAsync("Plugins.Misc.Zettle.Account.Fields.Currency.Warning");
                    var warning = string.Format(locale, storeCurrency.CurrencyCode, accountInfo.Currency, Url.Action("List", "Currency"));
                    _notificationService.WarningNotification(warning, false);
                }
                model.Account.Currency = accountInfo.Currency;

                //ensure the same tax types are used
                var taxNone = string.Equals(accountInfo.TaxationType, "NONE", StringComparison.InvariantCultureIgnoreCase);
                var taxVat = string.Equals(accountInfo.TaxationType, "VAT", StringComparison.InvariantCultureIgnoreCase);
                var taxRates = string.Equals(accountInfo.TaxationType, "SALES_TAX", StringComparison.InvariantCultureIgnoreCase);
                if (_taxSettings.EuVatEnabled != taxVat)
                {
                    var locale = await _localizationService.GetResourceAsync("Plugins.Misc.Zettle.Account.Fields.TaxationType.Vat.Warning");
                    var warning = string.Format(locale, Url.Action("Tax", "Setting"));
                    _notificationService.WarningNotification(warning, false);
                }
                model.Account.TaxationType = accountInfo.TaxationType?.Replace('_', ' ');
                if (taxVat)
                {
                    _zettleSettings.DefaultTaxEnabled = true;
                    if (accountInfo.VatPercentage.HasValue)
                        model.Account.TaxationType = $"{model.Account.TaxationType} ({accountInfo.VatPercentage}%)";
                }
                if (taxRates)
                {
                    var (percentage, _) = await _zettleService.GetDefaultTaxRateAsync();
                    if (percentage.HasValue)
                        model.Account.TaxationType = $"{model.Account.TaxationType} ({percentage}% by default)";
                    else
                    {
                        var warning = await _localizationService.GetResourceAsync("Plugins.Misc.Zettle.Account.Fields.TaxationType.SalesTax.Warning");
                        _notificationService.WarningNotification(warning);
                    }
                }

                //ensure the same price types are used
                var pricesIncludeTax = string.Equals(accountInfo.TaxationMode, "INCLUSIVE", StringComparison.InvariantCultureIgnoreCase);
                if (_taxSettings.PricesIncludeTax != pricesIncludeTax || (_taxSettings.PricesIncludeTax && taxNone))
                {
                    var locale = await _localizationService.GetResourceAsync("Plugins.Misc.Zettle.Account.Fields.TaxationMode.Warning");
                    var warning = string.Format(locale, Url.Action("Tax", "Setting"));
                    _notificationService.WarningNotification(warning, false);
                }
                model.Account.TaxationMode = accountInfo.TaxationMode;

                //ensure the webhook is created
                var store = await _storeContext.GetCurrentStoreAsync();
                var webhookUrl = $"{store.Url.TrimEnd('/')}{Url.RouteUrl(ZettleDefaults.WebhookRouteName)}".ToLowerInvariant();
                var (webhook, _) = await _zettleService.CreateWebhookAsync(webhookUrl);

                _zettleSettings.WebhookUrl = webhook?.Destination;
                _zettleSettings.WebhookKey = webhook?.SigningKey;
                await _settingService.SaveSettingAsync(_zettleSettings);

                if (string.IsNullOrEmpty(_zettleSettings.WebhookUrl))
                {
                    var locale = await _localizationService.GetResourceAsync("Plugins.Misc.Zettle.Configuration.Webhook.Warning");
                    var warning = string.Format(locale, Url.Action("List", "Log"));
                    _notificationService.WarningNotification(warning, false);
                }

                //last import details
                if (!string.IsNullOrEmpty(_zettleSettings.ImportId))
                {
                    var (import, _) = await _zettleService.GetImportAsync();
                    if (import is not null)
                    {
                        model.Import.StartDate = import.Created;
                        model.Import.EndDate = import.Finished;
                        model.Import.State = import.State?.Replace('_', ' ');
                        model.Import.Items = import.Items?.ToString();
                        model.Import.Active = string.Equals(import.State, "IMPORTING", StringComparison.InvariantCultureIgnoreCase);
                    }
                }
            }

            var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(ZettleDefaults.SynchronizationTask.Type);
            if (scheduleTask is not null)
            {
                model.AutoSyncEnabled = scheduleTask.Enabled;
                model.AutoSyncPeriod = scheduleTask.Seconds / 60;
            }

            return View("~/Plugins/Misc.Zettle/Views/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("credentials")]
        public async Task<IActionResult> SaveCredentials(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            if (!model.ClientId?.Equals(_zettleSettings.ClientId) ?? true)
            {
                //credentials are changed
                await _zettleRecordService.ClearRecordsAsync();

                _zettleSettings.WebhookUrl = string.Empty;
                _zettleSettings.WebhookKey = string.Empty;
                _zettleSettings.ImportId = string.Empty;
                _zettleSettings.InventoryTrackingIds = new();

                if (ZettleService.IsConfigured(_zettleSettings))
                {
                    if (_zettleSettings.DisconnectOnUninstall)
                        await _zettleService.DisconnectAsync();
                    else if (!string.IsNullOrEmpty(_zettleSettings.WebhookUrl))
                        await _zettleService.DeleteWebhookAsync();
                }
            }

            _zettleSettings.ClientId = model.ClientId;
            _zettleSettings.ApiKey = model.ApiKey;
            _zettleSettings.DisconnectOnUninstall = model.DisconnectOnUninstall;
            await _settingService.SaveSettingAsync(_zettleSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("sync")]
        public async Task<IActionResult> SaveSync(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            _zettleSettings.AutoSyncEnabled = model.AutoSyncEnabled;
            _zettleSettings.AutoSyncPeriod = model.AutoSyncPeriod;
            _zettleSettings.DeleteBeforeImport = model.DeleteBeforeImport;
            _zettleSettings.SyncEnabled = model.SyncEnabled;
            _zettleSettings.PriceSyncEnabled = model.PriceSyncEnabled;
            _zettleSettings.ImageSyncEnabled = model.ImageSyncEnabled;
            _zettleSettings.InventoryTrackingEnabled = model.InventoryTrackingEnabled;
            _zettleSettings.DefaultTaxEnabled = model.DefaultTaxEnabled;
            _zettleSettings.DiscountSyncEnabled = model.DiscountSyncEnabled;
            await _settingService.SaveSettingAsync(_zettleSettings);

            var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(ZettleDefaults.SynchronizationTask.Type);
            if (scheduleTask is not null)
            {
                if (!scheduleTask.Enabled && _zettleSettings.AutoSyncEnabled)
                    scheduleTask.LastEnabledUtc = DateTime.UtcNow;
                scheduleTask.Enabled = _zettleSettings.AutoSyncEnabled;
                scheduleTask.Seconds = _zettleSettings.AutoSyncPeriod * 60;
                await _scheduleTaskService.UpdateTaskAsync(scheduleTask);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("revoke")]
        public async Task<IActionResult> RevokeAccess()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            if (!ZettleService.IsConfigured(_zettleSettings))
                return await Configure();

            if (!string.IsNullOrEmpty(_zettleSettings.WebhookUrl))
                await _zettleService.DeleteWebhookAsync();

            var (_, error) = await _zettleService.DisconnectAsync();
            if (!string.IsNullOrEmpty(error))
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Misc.Zettle.Configuration.Error");
                var errorMessage = string.Format(locale, error, Url.Action("List", "Log"));
                _notificationService.ErrorNotification(errorMessage, false);
                return await Configure();
            }

            await _zettleRecordService.ClearRecordsAsync();

            _zettleSettings.ClientId = string.Empty;
            _zettleSettings.ApiKey = string.Empty;
            _zettleSettings.WebhookUrl = string.Empty;
            _zettleSettings.WebhookKey = string.Empty;
            _zettleSettings.ImportId = string.Empty;
            _zettleSettings.InventoryTrackingIds = new();
            await _settingService.SaveSettingAsync(_zettleSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Zettle.Credentials.AccessRevoked"));

            return await Configure();
        }

        #endregion

        #region Synchronization

        [HttpPost]
        public async Task<IActionResult> SyncRecordList(SyncRecordSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            var records = await _zettleRecordService.GetAllRecordsAsync(productOnly: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
            var products = await _productService.GetProductsByIdsAsync(records.Select(record => record.ProductId).Distinct().ToArray());
            var model = await new SyncRecordListModel().PrepareToGridAsync(searchModel, records, () =>
            {
                return records.SelectAwait(async record => new SyncRecordModel
                {
                    Id = record.Id,
                    Active = record.Active,
                    ProductId = record.ProductId,
                    ProductName = products.FirstOrDefault(product => product.Id == record.ProductId)?.Name ?? "Not found",
                    PriceSyncEnabled = record.PriceSyncEnabled,
                    ImageSyncEnabled = record.ImageSyncEnabled,
                    InventoryTrackingEnabled = record.InventoryTrackingEnabled,
                    UpdatedDate = record.UpdatedOnUtc.HasValue
                        ? await _dateTimeHelper.ConvertToUserTimeAsync(record.UpdatedOnUtc.Value, DateTimeKind.Utc)
                        : null
                });
            });

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> SyncRecordUpdate(SyncRecordModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            var productRecord = await _zettleRecordService.GetRecordByIdAsync(model.Id)
                ?? throw new ArgumentException("No record found");

            var records = (await _zettleRecordService.GetAllRecordsAsync(productUuid: productRecord.Uuid)).ToList();
            foreach (var record in records)
            {
                record.Active = model.Active;
                record.PriceSyncEnabled = model.PriceSyncEnabled;
                record.ImageSyncEnabled = model.ImageSyncEnabled;
                record.InventoryTrackingEnabled = model.InventoryTrackingEnabled;
                record.OperationType = OperationType.Update;
            }
            await _zettleRecordService.UpdateRecordsAsync(records);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> SyncRecordDelete(List<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            if (!selectedIds?.Any() ?? true)
                return NoContent();

            foreach (var id in selectedIds)
            {
                var productRecord = await _zettleRecordService.GetRecordByIdAsync(id);
                if (productRecord is null)
                    continue;

                var records = (await _zettleRecordService.GetAllRecordsAsync(productUuid: productRecord.Uuid)).ToList();
                await _zettleRecordService.DeleteRecordsAsync(records.Select(record => record.Id).ToList());
            }

            return new NullJsonResult();
        }

        public async Task<IActionResult> ProductToSync()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new AddProductToSyncSearchModel();
            await _baseAdminModelFactory.PrepareProductTypesAsync(model.AvailableProductTypes);
            await _baseAdminModelFactory.PrepareCategoriesAsync(model.AvailableCategories);
            await _baseAdminModelFactory.PrepareManufacturersAsync(model.AvailableManufacturers);
            await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);
            await _baseAdminModelFactory.PrepareVendorsAsync(model.AvailableVendors);
            model.SetPopupGridPageSize();

            return View("~/Plugins/Misc.Zettle/Views/ProductToSync.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductListToSync(AddProductToSyncSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            var products = await _productService.SearchProductsAsync(showHidden: true,
                keywords: searchModel.SearchProductName,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new AddProductToSyncListModel().PrepareToGrid(searchModel, products, () =>
            {
                return products.Select(product => new ProductModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Sku = product.Sku,
                    Price = product.Price,
                    Published = product.Published
                }).ToList();
            });

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public async Task<IActionResult> ProductToSync(AddProductToSyncModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var invalidProducts = await _zettleRecordService.AddRecordsAsync(model.SelectedProductIds?.ToList());
            if (invalidProducts > 0)
            {
                var warning = await _localizationService.GetResourceAsync("Plugins.Misc.Zettle.Sync.AddProduct.Warning");
                _notificationService.WarningNotification(string.Format(warning, invalidProducts));
            }
            else
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Zettle.Sync.AddProduct.Success"));

            ViewBag.RefreshPage = true;

            return View("~/Plugins/Misc.Zettle/Views/ProductToSync.cshtml", new AddProductToSyncSearchModel());
        }

        public async Task<IActionResult> SyncStart()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            await _zettleService.ImportAsync();

            return new NullJsonResult();
        }

        public async Task<IActionResult> SyncUpdate()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            if (string.IsNullOrEmpty(_zettleSettings.ImportId))
                return ErrorJson("Synchronization not found");

            var (import, _) = await _zettleService.GetImportAsync();
            if (string.IsNullOrEmpty(import?.State))
                return ErrorJson("Synchronization error");

            if (string.Equals(import.State, "IMPORTING", StringComparison.InvariantCultureIgnoreCase))
                return Json(new { StartDate = import.Created, Items = import.Items?.ToString(), State = import.State });

            return Json(new { Completed = true });
        }

        #endregion

        #endregion
    }
}