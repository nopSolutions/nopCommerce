namespace Nop.Plugin.Misc.OmnichannelCore.Domains;

/// <summary>
/// Represents inbound integration message status
/// </summary>
public enum OmniInboxMessageStatus
{
    /// <summary>
    /// Message has been received
    /// </summary>
    Received = 10,

    /// <summary>
    /// Message has been processed
    /// </summary>
    Processed = 20,

    /// <summary>
    /// Message was detected as duplicate
    /// </summary>
    Duplicate = 30,

    /// <summary>
    /// Message processing failed
    /// </summary>
    Failed = 40
}
