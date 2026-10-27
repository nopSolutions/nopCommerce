using Newtonsoft.Json;
using Nop.Services.Logging;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Nop.Plugin.Sms.Twilio.Services;

/// <summary>
/// Represents the plugin service
/// </summary>
public partial class TwilioService
{
    #region Fields

    protected readonly ILogger _logger;
    protected readonly TwilioHttpClient _twilioHttpClient;
    protected readonly TwilioSettings _twilioSettings;

    #endregion

    #region Ctor

    public TwilioService(ILogger logger, 
        TwilioHttpClient twilioHttpClient,
        TwilioSettings twilioSettings)
    {
        _logger = logger;
        _twilioHttpClient = twilioHttpClient;
        _twilioSettings = twilioSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sends an SMS message to the specified phone number
    /// </summary>
    /// <param name="phoneNumber">The destination phone number in E.164 format to which the SMS message will be sent.</param>
    /// <param name="body">The text content of the SMS message to send. The length and content must comply with Twilio's SMS message limitations.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the SMS message
    /// was sent successfully; otherwise, <see langword="false"/>.</returns>
    public async Task<bool> SendSmsMessageAsync(string phoneNumber, string body)
    {
        var twilioRestClient = _twilioHttpClient.GetClient();

        var message = MessageResource.Create(
            to: new PhoneNumber(phoneNumber),
            from: new PhoneNumber(_twilioSettings.PhoneNumber),
            body: body,
            client: twilioRestClient
        );

        if (message.ErrorCode.HasValue)
        {
            await _logger.ErrorAsync($"{TwilioDefaults.SystemName} - {JsonConvert.SerializeObject(message)}");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Retrieves the current account balance and currency from the Twilio service
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result is a account balance. Returns an empty string if the balance
    /// cannot be retrieved.</returns>
    public async Task<string> GetAccountBalanceAsync()
    {
        var twilioRestClient = _twilioHttpClient.GetClient();
        var balance = BalanceResource.Fetch(client: twilioRestClient);
        if (balance == null)
        {
            await _logger.ErrorAsync($"{TwilioDefaults.SystemName} - Failed to fetch account balance.");
            return "";
        }
        return string.Concat(balance.Balance, " ", balance.Currency);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Check whether the plugin is configured
    /// </summary>
    /// <returns>Result</returns>
    public bool IsConfigured => !string.IsNullOrEmpty(_twilioSettings.AccountSID) && !string.IsNullOrEmpty(_twilioSettings.AuthToken);

    #endregion
}
