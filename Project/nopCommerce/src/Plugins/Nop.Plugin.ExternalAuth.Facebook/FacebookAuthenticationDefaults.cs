namespace Nop.Plugin.ExternalAuth.Facebook;

/// <summary>
/// Represents plugin constants
/// </summary>
public class FacebookAuthenticationDefaults
{
    /// <summary>
    /// Gets a plugin system name
    /// </summary>
    public static string SystemName => "ExternalAuth.Facebook";

    /// <summary>
    /// Gets a name of the route to the data deletion callback
    /// </summary>
    public static string DataDeletionCallbackRoute => "Plugin.ExternalAuth.Facebook.DataDeletionCallback";

    /// <summary>
    /// Gets a name of the route to the data deletion status check
    /// </summary>
    public static string DataDeletionStatusCheckRoute => "Plugin.ExternalAuth.Facebook.DataDeletionStatusCheck";

    /// <summary>
    /// Gets a name of error callback method
    /// </summary>
    public static string ErrorCallback => "ErrorCallback";
}