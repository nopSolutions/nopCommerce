using Nop.Core.Configuration;

namespace Nop.Plugin.Sms.Twilio;

/// <summary>
/// Represents plugin settings
/// </summary>
public class TwilioSettings : ISettings
{
    /// <summary>
    /// Gets or sets the authentication token used to authorize API requests
    /// </summary>
    public string AuthToken { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the account associated with the service
    /// </summary>
    public string AccountSID { get; set; }

    /// <summary>
    /// Gets or sets the phone number associated with the contact
    /// </summary>
    public string PhoneNumber { get; set; }

}
