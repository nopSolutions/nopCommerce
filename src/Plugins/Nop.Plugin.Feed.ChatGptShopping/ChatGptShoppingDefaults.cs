namespace Nop.Plugin.Feed.ChatGptShopping;

/// <summary>
/// Represents plugin constants
/// </summary>
public class ChatGptShoppingDefaults
{
    /// <summary>
    /// Gets a plugin system name
    /// </summary>
    public static string SystemName => "Feed.ChatGptShopping";

    /// <summary>
    /// Gets the configuration route name
    /// </summary>
    public static string ConfigurationRouteName => "Plugin.Feed.ChatGptShopping.Configure";

    /// <summary>
    /// Gets the product availability value In Stock
    /// </summary>
    public static string ProductAvailabilityInStock => "in_stock";

    /// <summary>
    /// Gets the product availability value Out of Stock
    /// </summary>
    public static string ProductAvailabilityOutOfStock => "out_of_stock";

    /// <summary>
    /// Gets the product availability value Pre-order
    /// </summary>
    public static string ProductAvailabilityPreOrder => "pre_order";

    /// <summary>
    /// Gets a maximum number of products per page when generating feed
    /// </summary>
    public static int PageSize => 500;

    /// <summary>
    /// Gets a name of the feed file
    /// </summary>
    public static string PathToFeedFile => "files/exportimport/{0}-chatgptshopping_products.jsonl.gz";

    /// <summary>
    /// Gets a name, type and period (in seconds) of the auto synchronization task
    /// </summary>
    public static (string Name, string Type, int Period) SynchronizationTask =>
        ("Synchronization (ChatGPT Shopping)", "Nop.Plugin.Feed.ChatGptShopping.Services.ChatGptShoppingSyncTask", 28800);
}
