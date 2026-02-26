using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Sms.Twilio.Models;
using Nop.Plugin.Sms.Twilio.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Sms.Twilio.Controllers;

[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public class TwilioController : BasePluginController
{
    #region Fields

    protected readonly TwilioService _twilioService;
    protected readonly TwilioSettings _twilioSettings;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public TwilioController(TwilioService twilioService,
        TwilioSettings googleAuthenticatorSettings,
        ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService)
    {
        _twilioService = twilioService;
        _twilioSettings = googleAuthenticatorSettings;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
    }
    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure()
    {
        //prepare model
        var model = new ConfigurationModel
        {
            AccountSID = _twilioSettings.AccountSID,
            AuthToken = _twilioSettings.AuthToken,
            PhoneNumber = _twilioSettings.PhoneNumber,
            IsConfigured = _twilioService.IsConfigured,
            BalanceInfo = await _localizationService.GetResourceAsync("Plugins.Sms.Twilio.Credentials.Fields.BalanceInfo.Text")
        };

        if (_twilioService.IsConfigured)
        {
            try
            {
                var balance = await _twilioService.GetAccountBalanceAsync();
                model.BalanceInfo = balance;
            }
            catch (Exception ex)
            {
                _notificationService.ErrorNotification(ex.Message);
            }
        }

        return View("~/Plugins/Sms.Twilio/Views/Configure.cshtml", model);
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("credentials")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> SaveCredentials(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        //set new settings values
        _twilioSettings.AccountSID = model.AccountSID;
        _twilioSettings.AuthToken = model.AuthToken;
        _twilioSettings.PhoneNumber = model.PhoneNumber;
        await _settingService.SaveSettingAsync(_twilioSettings);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("checkbalance")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> CheckBalance()
    {
        return await Configure();
    }

    #endregion
}