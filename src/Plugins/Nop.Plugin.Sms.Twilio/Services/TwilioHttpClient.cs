using Twilio.Clients;
using Twilio.Http;
using HttpClient = System.Net.Http.HttpClient;

namespace Nop.Plugin.Sms.Twilio.Services;

/// <summary>
/// Represents HTTP client to request third-party services
/// </summary>
public class TwilioHttpClient
{
    #region Fields

    protected readonly HttpClient _httpClient;
    protected readonly TwilioSettings _twilioSettings;

    #endregion

    #region Ctor

    public TwilioHttpClient(HttpClient httpClient, TwilioSettings twilioSettings)
    {
        _httpClient = httpClient;
        _twilioSettings = twilioSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates and returns a new instance of the TwilioRestClient configured with the current Twilio account credentials
    /// </summary>
    /// <returns>A TwilioRestClient instance initialized with the configured account SID and authentication token.</returns>
    public TwilioRestClient GetClient()
    {
        var accountSid = _twilioSettings.AccountSID;
        var authToken = _twilioSettings.AuthToken;

        var twilioRestClient = new TwilioRestClient(
            accountSid,
            authToken,
            httpClient: new SystemNetHttpClient(_httpClient)
        );

        return twilioRestClient;
    }

    #endregion
}
