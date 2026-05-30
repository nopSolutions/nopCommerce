using Nop.Core.Configuration;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator;

/// <summary>
/// Represents settings of the GoogleAuthenticatorMethod
/// </summary>
public class GoogleAuthenticatorSettings : ISettings
{
    /// <summary>
    /// Gets or sets the number of pixels per module
    /// </summary>
    public int QRPixelsPerModule { get; set; }

    /// <summary>
    /// Gets or sets business prefix
    /// </summary>
    public string BusinessPrefix { get; set; }
}