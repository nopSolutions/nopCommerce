using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Reminders;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Models.Reminders;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the reminder model factory
/// </summary>
public partial class RemindersModelFactory : IReminderModelFactory
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly RemindersSettings _remindersSettings;

    #endregion

    #region Ctor

    public RemindersModelFactory(ILocalizationService localizationService, IMessageTemplateService messageTemplateService, RemindersSettings remindersSettings)
    {
        _localizationService = localizationService;
        _messageTemplateService = messageTemplateService;
        _remindersSettings = remindersSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare reminders model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the reminders model
    /// </returns>
    public virtual async Task<RemindersModel> PrepareRemindersModelAsync()
    {
        var model = new RemindersModel();

        model.AbandonedCartEnabled = _remindersSettings.AbandonedCartEnabled;
        model.PendingOrdersEnabled = _remindersSettings.PendingOrdersEnabled;
        model.IncompleteRegistrationEnabled = _remindersSettings.IncompleteRegistrationEnabled;

        var abandonedCartFollowUp1 = (await _messageTemplateService.GetMessageTemplatesByNameAsync(MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_1_MESSAGE)).FirstOrDefault();

        model.AbandonedCartFollowUps.Add(new FollowUpModel
        {
            Name = abandonedCartFollowUp1.Name,
            Enabled = abandonedCartFollowUp1?.IsActive ?? false,
            DelayBeforeSend = abandonedCartFollowUp1?.DelayBeforeSend ?? 0,
            DelayPeriodId = abandonedCartFollowUp1?.DelayPeriodId ?? 0
        });

        var abandonedCartFollowUp2 = (await _messageTemplateService.GetMessageTemplatesByNameAsync(MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_2_MESSAGE))
            .FirstOrDefault();

        model.AbandonedCartFollowUps.Add(new FollowUpModel()
        {
            Name = abandonedCartFollowUp2.Name,
            Enabled = abandonedCartFollowUp2?.IsActive ?? false,
            DelayBeforeSend = abandonedCartFollowUp2?.DelayBeforeSend ?? 0,
            DelayPeriodId = abandonedCartFollowUp2?.DelayPeriodId ?? 0
        });

        var abandonedCartFollowUp3 = (await _messageTemplateService.GetMessageTemplatesByNameAsync(MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_3_MESSAGE))
            .FirstOrDefault();

        model.AbandonedCartFollowUps.Add(new FollowUpModel()
        {
            Name = abandonedCartFollowUp3.Name,
            Enabled = abandonedCartFollowUp3?.IsActive ?? false,
            DelayBeforeSend = abandonedCartFollowUp3?.DelayBeforeSend ?? 0,
            DelayPeriodId = abandonedCartFollowUp3?.DelayPeriodId ?? 0
        });

        var pendingOrdersFollowUp1 = (await _messageTemplateService.GetMessageTemplatesByNameAsync(MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_1_MESSAGE))
            .FirstOrDefault();

        model.PendingOrdersFollowUps.Add(new FollowUpModel()
        {
            Name = pendingOrdersFollowUp1.Name,
            Enabled = pendingOrdersFollowUp1?.IsActive ?? false,
            DelayBeforeSend = pendingOrdersFollowUp1?.DelayBeforeSend ?? 0,
            DelayPeriodId = pendingOrdersFollowUp1?.DelayPeriodId ?? 0
        });

        var pendingOrdersFollowUp2 = (await _messageTemplateService.GetMessageTemplatesByNameAsync(MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_2_MESSAGE))
            .FirstOrDefault();

        model.PendingOrdersFollowUps.Add(new FollowUpModel()
        {
            Name = pendingOrdersFollowUp2.Name,
            Enabled = pendingOrdersFollowUp2?.IsActive ?? false,
            DelayBeforeSend = pendingOrdersFollowUp2?.DelayBeforeSend ?? 0,
            DelayPeriodId = pendingOrdersFollowUp2?.DelayPeriodId ?? 0
        });

        var incompleteRegistrationsReminder = (await _messageTemplateService.GetMessageTemplatesByNameAsync(MessageTemplateSystemNames.REMINDER_REGISTRATION_FOLLOW_UP_MESSAGE))
            .FirstOrDefault();

        model.IncompleteRegistrationFollowUps.Add(new FollowUpModel()
        {
            Name = incompleteRegistrationsReminder.Name,
            Enabled = incompleteRegistrationsReminder?.IsActive ?? false,
            DelayBeforeSend = incompleteRegistrationsReminder?.DelayBeforeSend ?? 0,
            DelayPeriodId = incompleteRegistrationsReminder?.DelayPeriodId ?? 0
        });

        return model;

    }

    #endregion
}
