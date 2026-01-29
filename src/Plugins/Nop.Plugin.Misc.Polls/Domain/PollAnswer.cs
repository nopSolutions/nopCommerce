using Nop.Core;

namespace Nop.Plugin.Misc.Polls.Domain;

/// <summary>
/// Represents a poll answer
/// </summary>
public class PollAnswer : BaseEntity
{
    /// <summary>
    /// Gets or sets the poll identifier
    /// </summary>
    public int PollId { get; set; }

    /// <summary>
    /// Gets or sets the poll answer name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the current number of votes
    /// </summary>
    public int NumberOfVotes { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}