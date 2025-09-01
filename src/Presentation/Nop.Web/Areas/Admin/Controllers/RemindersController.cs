using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Reminders;
using Nop.Services.Configuration;
using Nop.Services.Reminders;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Reminders;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class RemindersController : BaseAdminController
{
    #region Fields

    protected readonly IReminderModelFactory _reminderModelFactory;
    protected readonly IReminderService _reminderService;
    protected readonly ISettingService _settingService;
    protected readonly RemindersSettings _remindersSettings;


    #endregion

    #region Ctor

    public RemindersController(IReminderModelFactory reminderModelFactory, IReminderService reminderService, ISettingService settingService, RemindersSettings remindersSettings)
    {
        _reminderModelFactory = reminderModelFactory;
        _reminderService = reminderService;
        _settingService = settingService;
        _remindersSettings = remindersSettings;
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
            _remindersSettings.AbandonedCartEnabled = model.AbandonedCartEnabled;
            if (model.AbandonedCartEnabled)
            {
                foreach (var folowUp in model.AbandonedCartFollowUps)
                    await _reminderService.UpdateFollowUpAsync(folowUp.Name, folowUp.Enabled, folowUp.DelayBeforeSend, (MessageDelayPeriod)folowUp.DelayPeriodId);
            }

            _remindersSettings.PendingOrdersEnabled = model.PendingOrdersEnabled;
            if (model.PendingOrdersEnabled)
            {
                foreach (var folowUp in model.PendingOrdersFollowUps)
                    await _reminderService.UpdateFollowUpAsync(folowUp.Name, folowUp.Enabled, folowUp.DelayBeforeSend, (MessageDelayPeriod)folowUp.DelayPeriodId);
            }

            _remindersSettings.IncompleteRegistrationEnabled = model.IncompleteRegistrationEnabled;
            if (model.IncompleteRegistrationEnabled)
            {
                foreach (var folowUp in model.IncompleteRegistrationFollowUps)
                    await _reminderService.UpdateFollowUpAsync(folowUp.Name, folowUp.Enabled, folowUp.DelayBeforeSend, (MessageDelayPeriod)folowUp.DelayPeriodId);
            }

            await _settingService.SaveSettingAsync(_remindersSettings);
        }

        //prepare model
        model = await _reminderModelFactory.PrepareRemindersModelAsync();

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    #endregion
}
