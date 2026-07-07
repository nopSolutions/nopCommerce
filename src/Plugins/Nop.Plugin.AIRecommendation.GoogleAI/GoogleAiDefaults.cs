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

    /// <summary>
    /// Gets the default page size to import products
    /// </summary>
    public static int ImportPageSize => 500;

    /// <summary>
    /// Gets the search request count
    /// </summary>
    /// <remarks>For search request, the maximum page size is 120. See https://cloud.google.com/retail/docs/reference/rpc/google.cloud.retail.v2#google.cloud.retail.v2.SearchRequest</remarks>
    public static int SearchRequestCount => 120;

    /// <summary>
    /// Gets the category separator
    /// </summary>
    public static string CategorySeparator => " > ";
}