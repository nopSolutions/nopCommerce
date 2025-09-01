using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Reminders;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Reminders;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Reminders;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ReminderController : BaseAdminController
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IReminderModelFactory _reminderModelFactory;
    protected readonly IReminderService _reminderService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;


    #endregion

    #region Ctor

    public ReminderController(ILocalizationService localizationService,
        INotificationService notificationService,
        IReminderModelFactory reminderModelFactory,
        IReminderService reminderService,
        ISettingService settingService,
        IStoreContext storeContext)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _reminderModelFactory = reminderModelFactory;
        _reminderService = reminderService;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> Index()
    {
        return View(await _reminderModelFactory.PrepareRemindersModelAsync());
    }

    [HttpPost]
    public virtual async Task<IActionResult> Index(RemindersModel model)
    {
        if (ModelState.IsValid)
        {
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var reminderSettings = await _settingService.LoadSettingAsync<ReminderSettings>(storeId);

            reminderSettings.AbandonedCartEnabled = model.AbandonedCartEnabled;
            if (model.AbandonedCartEnabled)
                await updateFollowUpsAsync(model.AbandonedCartFollowUps);

            reminderSettings.PendingOrdersEnabled = model.PendingOrdersEnabled;
            if (model.PendingOrdersEnabled)
                await updateFollowUpsAsync(model.PendingOrdersFollowUps);

            reminderSettings.IncompleteRegistrationEnabled = model.IncompleteRegistrationEnabled;
            if (model.IncompleteRegistrationEnabled)
                await updateFollowUpsAsync(model.IncompleteRegistrationFollowUps);

            await _settingService.SaveSettingOverridablePerStoreAsync(reminderSettings, settings => settings.AbandonedCartEnabled, model.AbandonedCartEnabled_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(reminderSettings, settings => settings.PendingOrdersEnabled, model.PendingOrdersEnabled_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(reminderSettings, settings => settings.IncompleteRegistrationEnabled, model.IncompleteRegistrationEnabled_OverrideForStore, storeId, false);
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            async Task updateFollowUpsAsync(List<FollowUpModel> followUpModels)
            {
                foreach (var folowUp in followUpModels)
                    await _reminderService.UpdateFollowUpAsync(folowUp.Name, storeId, folowUp.Enabled, folowUp.DelayBeforeSend, (MessageDelayPeriod)folowUp.DelayPeriodId);
            }
        }

        model = await _reminderModelFactory.PrepareRemindersModelAsync();

        return View(model);
    }

    #endregion
}