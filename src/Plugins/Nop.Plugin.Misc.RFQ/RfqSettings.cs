using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.RFQ;

/// <summary>
/// Represents plugin settings
/// </summary>
public class RfqSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether RFQ functionality is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the "Create Request" page
    /// </summary>
    public bool ShowCaptchaOnRequestPage { get; set; }
}
