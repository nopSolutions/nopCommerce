namespace Nop.Plugin.Misc.OmnichannelCore;

/// <summary>
/// Represents omnichannel core plugin defaults
/// </summary>
public static class OmnichannelCoreDefaults
{
    /// <summary>
    /// Gets the plugin system name
    /// </summary>
    public const string SystemName = "Misc.OmnichannelCore";

    /// <summary>
    /// Gets the configuration page route
    /// </summary>
    public const string ConfigurationRoute = "Admin/OmnichannelCore/Configure";

    /// <summary>
    /// Gets the internal callback demo token header name
    /// </summary>
    public const string DemoTokenHeaderName = "X-Demo-Token";

    /// <summary>
    /// Gets the default demo token used by local simulators
    /// </summary>
    public const string DefaultDemoToken = "omni-demo-token";

    /// <summary>
    /// Gets the POS stock changed event type
    /// </summary>
    public const string PosStockChangedEventType = "pos.stock.changed.v1";

    /// <summary>
    /// Gets the order placed event type
    /// </summary>
    public const string OrderPlacedEventType = "commerce.order.placed.v1";
}
