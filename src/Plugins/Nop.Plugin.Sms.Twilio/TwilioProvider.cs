using Nop.Core;
using Nop.Plugin.Sms.Twilio.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Sms.Twilio;

/// <summary>
/// Represents Twilio sms provider
/// </summary>
public class TwilioProvider : BasePlugin, ISmsProvider
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly TwilioService _twilioService;

    #endregion

    #region Ctor

    public TwilioProvider(ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        ISettingService settingService,
        IStoreContext storeContext,
        TwilioService twilioService)
    {
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _settingService = settingService;
        _storeContext = storeContext;
        _twilioService = twilioService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {        
        return _nopUrlHelper.RouteUrl(TwilioDefaults.ConfigurationRouteName);
    }

    /// <summary>
    /// Sends an SMS message to the specified phone number
    /// </summary>
    /// <param name="phone">The destination phone number for the SMS message</param>
    /// <param name="text">The text content of the SMS message</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the SMS message
    /// was sent successfully; otherwise, <see langword="false"/>.</returns>
    public async Task<bool> SendSmsAsync(string phone, string text)
    {
        if (!_twilioService.IsConfigured)
            return false;
        return await _twilioService.SendSmsMessageAsync(phone, text);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //settings
        await _settingService.SaveSettingAsync(new TwilioSettings());
        
        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            //admin config
            ["Plugins.Sms.Twilio.Credentials"] = "Credentials",
            ["Plugins.Sms.Twilio.Credentials.Fields.AuthToken"] = "Auth token",
            ["Plugins.Sms.Twilio.Credentials.Fields.AuthToken.Hint"] = "Enter the Auth token. You can find the Auth Token in the Account Info pane of the Console Dashboard page.",
            ["Plugins.Sms.Twilio.Credentials.Fields.AccountSID"] = "Account SID",
            ["Plugins.Sms.Twilio.Credentials.Fields.AccountSID.Hint"] = "Enter the Account SID. A Twilio Account SID is a 34-character alphanumeric identifier that begins with the letters “AC\" and can be found on the dashboard when logging into the Twilio Console. It is a unique key that is used to identify a specific Twilio Parent Account or Subaccount and is a credential that acts as a username.",
            ["Plugins.Sms.Twilio.Credentials.Fields.PhoneNumber"] = "Phone number",
            ["Plugins.Sms.Twilio.Credentials.Fields.PhoneNumber.Hint"] = "Enter the Phone number. Twilio account phone number.",
            ["Plugins.Sms.Twilio.Credentials.Fields.BalanceInfo"] = "Balance info",
            ["Plugins.Sms.Twilio.Credentials.Fields.BalanceInfo.Hint"] = "Check an Account Balance",
            ["Plugins.Sms.Twilio.Credentials.Fields.BalanceInfo.Text"] = "Update to check balance.",
            ["Plugins.Sms.Twilio.Credentials.CheckBalance"] = "Check balance",
            
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        //settings
        await _settingService.DeleteSettingAsync<TwilioSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Sms.Twilio");

        await base.UninstallAsync();
    }

    #endregion
}
