using Nop.Plugin.Misc.OmnichannelCore.Domains;

namespace Nop.Plugin.Misc.OmnichannelCore.Models.Callbacks;

/// <summary>
/// Represents inbox registration result
/// </summary>
public record InboxProcessingResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the message already existed
    /// </summary>
    public bool IsDuplicate { get; set; }

    /// <summary>
    /// Gets or sets the inbox message
    /// </summary>
    public OmniInboxMessage InboxMessage { get; set; }
}
