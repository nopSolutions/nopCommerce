namespace Nop.Plugin.Misc.OmnichannelCore.Domains;

/// <summary>
/// Represents omnichannel fulfillment status
/// </summary>
public enum OmniFulfillmentStatus
{
    /// <summary>
    /// Fulfillment request is waiting for external processing
    /// </summary>
    Pending = 10,

    /// <summary>
    /// External service is degraded and fulfillment is delayed
    /// </summary>
    Degraded = 20,

    /// <summary>
    /// External service accepted the fulfillment request
    /// </summary>
    Accepted = 30,

    /// <summary>
    /// Fulfillment is complete
    /// </summary>
    Completed = 40,

    /// <summary>
    /// Fulfillment was rejected
    /// </summary>
    Rejected = 50
}
