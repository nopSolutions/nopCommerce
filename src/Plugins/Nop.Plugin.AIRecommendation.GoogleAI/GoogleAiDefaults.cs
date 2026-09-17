namespace Nop.Plugin.AIRecommendation.GoogleAI;

/// <summary>
/// Represents plugin constants
/// </summary>
public class GoogleAiDefaults
{
    /// <summary>
    /// Gets the system name of the plugin
    /// </summary>
    public static string SystemName => "AIRecommendation.GoogleAI";

    /// <summary>
    /// Gets the configuration route name
    /// </summary>
    public static string ConfigurationRouteName => "Plugin.AIRecommendation.GoogleAI.Configure";

    /// <summary>
    /// Gets a name of the cookies "client_id"
    /// </summary>
    public static string ClientIdCookiesName => "_ga";
}