namespace Nop.Plugin.Misc.Forums.Domain;

/// <summary>
/// Represents a forum search type
/// </summary>
public enum ForumSearchType
{
    /// <summary>
    /// Topic titles and post text
    /// </summary>
    All = 0,

    /// <summary>
    /// Topic titles only
    /// </summary>
    TopicTitlesOnly = 10,

    /// <summary>
    /// Post text only
    /// </summary>
    PostTextOnly = 20
}