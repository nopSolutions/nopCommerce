using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.CyberSource.Domain;
using Nop.Plugin.Payments.CyberSource.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.CyberSource.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class CyberSourceController : BasePluginController
    {
        #region Fields

        protected readonly ILocalizationService _localizationService;
        protected readonly INotificationService _notificationService;
        protected readonly IPermissionService _permissionService;
        protected readonly IScheduleTaskService _scheduleTaskService;
        protected readonly ISettingService _settingService;
        protected readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public CyberSourceController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _scheduleTaskService = scheduleTaskService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<CyberSourceSettings>(storeId);

            var model = new ConfigurationModel
            {
                UseSandbox = settings.UseSandbox,
                MerchantId = settings.MerchantId,
                KeyId = settings.KeyId,
                SecretKey = settings.SecretKey,
                TokenizationEnabled = settings.TokenizationEnabled,
                PaymentConnectionMethodId = (int)settings.PaymentConnectionMethod,
                PayerAuthenticationEnabled = settings.PayerAuthenticationEnabled,
                PayerAuthenticationRequired = settings.PayerAuthenticationRequired,
                TransactionTypeId = (int)settings.TransactionType,
                CvvRequired = settings.CvvRequired,
                AvsActionTypeId = (int)settings.AvsActionType,
                CvnActionTypeId = (int)settings.CvnActionType,
                DecisionManagerEnabled = settings.DecisionManagerEnabled,
                ConversionDetailReportingEnabled = settings.ConversionDetailReportingEnabled,
                ConversionDetailReportingFrequency = settings.ConversionDetailReportingFrequency,
                ActiveStoreScopeConfiguration = storeId
            };

            if (storeId > 0)
            {
                model.UseSandbox_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.UseSandbox, storeId);
                model.MerchantId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.MerchantId, storeId);
                model.KeyId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.KeyId, storeId);
                model.SecretKey_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.SecretKey, storeId);
                model.TokenizationEnabled_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.TokenizationEnabled, storeId);
                model.PaymentConnectionMethodId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.PaymentConnectionMethod, storeId);
                model.PayerAuthenticationEnabled_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.PayerAuthenticationEnabled, storeId);
                model.PayerAuthenticationRequired_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.PayerAuthenticationRequired, storeId);
                model.TransactionTypeId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.TransactionType, storeId);
                model.CvvRequired_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.CvvRequired, storeId);
                model.AvsActionTypeId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.AvsActionType, storeId);
                model.CvnActionTypeId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.CvnActionType, storeId);
                model.DecisionManagerEnabled_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.DecisionManagerEnabled, storeId);
            }

            var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(CyberSourceDefaults.OrderStatusUpdateTask);
            if (scheduleTask is not null)
            {
                model.ConversionDetailReportingEnabled = scheduleTask.Enabled;
                model.ConversionDetailReportingFrequency = scheduleTask.Seconds / 60;
            }

            return View("~/Plugins/Payments.CyberSource/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<CyberSourceSettings>(storeId);

            settings.UseSandbox = model.UseSandbox;
            settings.MerchantId = model.MerchantId;
            settings.KeyId = model.KeyId;
            settings.SecretKey = model.SecretKey;
            settings.TokenizationEnabled = model.TokenizationEnabled;
            settings.PaymentConnectionMethod = (ConnectionMethodType)model.PaymentConnectionMethodId;
            settings.PayerAuthenticationEnabled = model.PayerAuthenticationEnabled;
            settings.PayerAuthenticationRequired = model.PayerAuthenticationRequired;
            settings.TransactionType = (TransactionType)model.TransactionTypeId;
            settings.CvvRequired = model.CvvRequired;
            settings.AvsActionType = (AvsActionType)model.AvsActionTypeId;
            settings.CvnActionType = (CvnActionType)model.CvnActionTypeId;
            settings.DecisionManagerEnabled = model.DecisionManagerEnabled;
            settings.ConversionDetailReportingEnabled = model.ConversionDetailReportingEnabled;
            settings.ConversionDetailReportingFrequency = model.ConversionDetailReportingFrequency;

            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.UseSandbox, model.UseSandbox_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.MerchantId, model.MerchantId_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.KeyId, model.KeyId_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.SecretKey, model.SecretKey_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.TokenizationEnabled, model.TokenizationEnabled_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.PaymentConnectionMethod, model.PaymentConnectionMethodId_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.PayerAuthenticationEnabled, model.PayerAuthenticationEnabled_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.PayerAuthenticationRequired, model.PayerAuthenticationRequired_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.TransactionType, model.TransactionTypeId_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.CvvRequired, model.CvvRequired_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.AvsActionType, model.AvsActionTypeId_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.CvnActionType, model.CvnActionTypeId_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.DecisionManagerEnabled, model.DecisionManagerEnabled_OverrideForStore, storeId, false);
            await _settingService.SaveSettingAsync(settings, setting => setting.ConversionDetailReportingEnabled, 0, false);
            await _settingService.SaveSettingAsync(settings, setting => setting.ConversionDetailReportingFrequency, 0, false);
            await _settingService.ClearCacheAsync();

            var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(CyberSourceDefaults.OrderStatusUpdateTask);
            if (scheduleTask is not null)
            {
                if (!scheduleTask.Enabled && settings.ConversionDetailReportingEnabled)
                    scheduleTask.LastEnabledUtc = DateTime.UtcNow;
                scheduleTask.Enabled = settings.ConversionDetailReportingEnabled;
                scheduleTask.Seconds = settings.ConversionDetailReportingFrequency * 60;
                await _scheduleTaskService.UpdateTaskAsync(scheduleTask);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}