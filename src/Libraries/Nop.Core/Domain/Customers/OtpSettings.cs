using Nop.Core.Configuration;

namespace Nop.Core.Domain.Customers;

/// <summary>
/// Represents settings for one-time password (OTP) authentication
/// </summary>
public partial class OtpSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether users are allowed to login using their phone number
    /// </summary>
    public bool LoginByPhoneEnabled { get; set; }

    /// <summary>
    /// Gets or sets the duration, in seconds, for which a one-time password (OTP) remains valid
    /// </summary>
    public int OtpTimeLife { get; set; }

    /// <summary>
    /// Gets or sets the number of attempts allowed to send an OTP code before the process is blocked or requires
    /// additional verification
    /// </summary>
    public int OtpCountAttemptsToSendCode { get; set; }

    /// <summary>
    /// Gets or sets the time interval, in minutes, before a one-time password (OTP) can be requested again
    /// </summary>
    public int OtpTimeToRepeat { get; set; }

    /// <summary>
    /// Gets or sets the number of digits to use when generating one-time passwords (OTPs)
    /// </summary>
    public int OtpLength { get; set; }

    /// <summary>
    /// Gets or sets system name of active sms provider
    /// </summary>
    public string ActiveSmsProviderSystemName { get; set; }

}
