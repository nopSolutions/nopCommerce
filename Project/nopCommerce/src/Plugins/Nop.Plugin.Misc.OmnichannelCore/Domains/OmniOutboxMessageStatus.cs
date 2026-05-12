namespace Nop.Plugin.Misc.OmnichannelCore.Domains;

/// <summary>
/// Represents outbound integration message status
/// </summary>
public enum OmniOutboxMessageStatus
{
    /// <summary>
    /// Message is waiting to be published
    /// </summary>
    Pending = 10,

    /// <summary>
    /// Message has been published
    /// </summary>
    Published = 20,

    /// <summary>
    /// Message publishing failed and can be retried
    /// </summary>
    Failed = 30,

    /// <summary>
    /// Message was moved to dead-letter handling
    /// </summary>
    DeadLettered = 40
}
