using Nop.Core;

namespace Nop.Plugin.Misc.Zettle;

/// <summary>
/// Represents plugin constants
/// </summary>
public class ZettleDefaults
{
    /// <summary>
    /// Gets the plugin system name
    /// </summary>
    public static string SystemName => "Misc.Zettle";

    /// <summary>
    /// Gets the user agent used to request third-party services
    /// </summary>
    public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";

    /// <summary>
    /// Gets the application name
    /// </summary>
    public static string ApplicationName => "nopCommerce-integration";

    /// <summary>
    /// Gets the partner identifier
    /// </summary>
    public static string PartnerIdentifier => "nopCommerce";

    /// <summary>
    /// Gets the partner affiliation header used for each request to APIs
    /// </summary>
    public static (string Name, string Value) PartnerHeader => ("X-iZettle-Application-Id", "f4954821-e7e4-4fca-854e-e36060b5748d");

    /// <summary>
    /// Gets the webhook request signature header
    /// </summary>
    public static string SignatureHeader => "X-iZettle-Signature";

    /// <summary>
    /// Gets a default period (in seconds) before the request times out
    /// </summary>
    public static int RequestTimeout => 15;

    /// <summary>
    /// Gets a default number of products to import in one request
    /// </summary>
    public static int ImportProductsNumber => 500;

    /// <summary>
    /// Gets webhook event names to subscribe
    /// </summary>
    public static List<string> WebhookEventNames =>
    [
    "ProductCreated",
    "InventoryBalanceChanged",
    "InventoryTrackingStopped",
    "ApplicationConnectionRemoved"
    ];

    /// <summary>
    /// Gets the configuration route name
    /// </summary>
    public static string ConfigurationRouteName => "Plugin.Misc.Zettle.Configure";

    /// <summary>
    /// Gets the webhook route name
    /// </summary>
    public static string WebhookRouteName => "Plugin.Misc.Zettle.Webhook";

    /// <summary>
    /// Gets a name, type and period (in seconds) of the auto synchronization task
    /// </summary>
    public static (string Name, string Type, int Period) SynchronizationTask =>
        ("Synchronization (PayPal Zettle plugin)", "Nop.Plugin.Misc.Zettle.Services.ZettleSyncTask", 28800);
}