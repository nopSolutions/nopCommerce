using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Reminders;
using Nop.Services.Configuration;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Models.Reminders;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the reminder model factory
/// </summary>
public partial class ReminderModelFactory : IReminderModelFactory
{
    #region Fields

    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public ReminderModelFactory(IMessageTemplateService messageTemplateService,
        ISettingService settingService,
        IStoreContext storeContext)
    {
        _messageTemplateService = messageTemplateService;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare follow up model
    /// </summary>
    /// <param name="messageTemplateName">Message template name</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the follow up model
    /// </returns>
    protected virtual async Task<FollowUpModel> PrepareFollowUpModelAsync(string messageTemplateName, int storeId)
    {
        //we use only the first message template
        var messageTemplates = await _messageTemplateService.GetMessageTemplatesByNameAsync(messageTemplateName, storeId);
        var messageTemplate = messageTemplates.FirstOrDefault();

        return new FollowUpModel
        {
            Name = messageTemplate.Name,
            Enabled = messageTemplate?.IsActive ?? false,
            DelayBeforeSend = messageTemplate?.DelayBeforeSend ?? 0,
            DelayPeriodId = messageTemplate?.DelayPeriodId ?? 0
        };
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
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var reminderSettings = await _settingService.LoadSettingAsync<ReminderSettings>(storeId);

        var model = new RemindersModel
        {
            AbandonedCartEnabled = reminderSettings.AbandonedCartEnabled,
            PendingOrdersEnabled = reminderSettings.PendingOrdersEnabled,
            IncompleteRegistrationEnabled = reminderSettings.IncompleteRegistrationEnabled
        };

        model.AbandonedCartFollowUps.Add(await PrepareFollowUpModelAsync(MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_1_MESSAGE, storeId));
        model.AbandonedCartFollowUps.Add(await PrepareFollowUpModelAsync(MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_2_MESSAGE, storeId));
        model.AbandonedCartFollowUps.Add(await PrepareFollowUpModelAsync(MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_3_MESSAGE, storeId));

        model.PendingOrdersFollowUps.Add(await PrepareFollowUpModelAsync(MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_1_MESSAGE, storeId));
        model.PendingOrdersFollowUps.Add(await PrepareFollowUpModelAsync(MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_2_MESSAGE, storeId));

        model.IncompleteRegistrationFollowUps.Add(await PrepareFollowUpModelAsync(MessageTemplateSystemNames.REMINDER_REGISTRATION_FOLLOW_UP_MESSAGE, storeId));

        if (storeId > 0)
        {
            model.AbandonedCartEnabled_OverrideForStore = await _settingService.SettingExistsAsync(reminderSettings, settings => settings.AbandonedCartEnabled, storeId);
            model.PendingOrdersEnabled_OverrideForStore = await _settingService.SettingExistsAsync(reminderSettings, settings => settings.PendingOrdersEnabled, storeId);
            model.IncompleteRegistrationEnabled_OverrideForStore = await _settingService.SettingExistsAsync(reminderSettings, settings => settings.IncompleteRegistrationEnabled, storeId);
        }

        return model;
    }

    #endregion
}