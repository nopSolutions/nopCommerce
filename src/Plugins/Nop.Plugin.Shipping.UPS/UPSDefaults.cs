using Nop.Core;

namespace Nop.Plugin.Shipping.UPS;

/// <summary>
/// Represents plugin constants
/// </summary>
public class UPSDefaults
{
    /// <summary>
    /// Gets the plugin system name
    /// </summary>
    public static string SystemName => "Shipping.UPS";

    /// <summary>
    /// Gets a default period (in seconds) before the request times out
    /// </summary>
    public static int RequestTimeout => 15;

    /// <summary>
    /// Gets the user agent used to request third-party services
    /// </summary>
    public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";

    /// <summary>
    /// Gets the production API URL
    /// </summary>
    public static string ApiUrl => "https://onlinetools.ups.com/api";
}