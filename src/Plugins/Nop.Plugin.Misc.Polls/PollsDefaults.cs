using Nop.Core.Caching;

namespace Nop.Plugin.Misc.Polls;

/// <summary>
/// Represents plugin constants
/// </summary>
public class PollsDefaults
{
    /// <summary>
    /// Gets a plugin system name
    /// </summary>
    public static string SystemName => "Misc.Polls";

    /// <summary>
    /// Gets a URL of the polls docs page
    /// </summary>
    public static string DocumentationUrl => "https://docs.nopcommerce.com/running-your-store/content-management/polls.html";

    #region Routes

    public static class Routes
    {
        public static string ConfigurationRouteName => "Plugin.Misc.Polls.Route.Configure";
        public static string PollVoteRouteName => "Plugin.Misc.Polls.Route.PollVote";
    }

    #endregion

    #region Caching defaults

    /// <summary>
    /// Key for left column polls
    /// </summary>
    /// <remarks>
    /// {0} : language ID
    /// {1} : current store ID
    /// </remarks>
    public static CacheKey LeftColumnPollsModelKey => new("Nop.pres.poll.leftcolumn-{0}-{1}");

    /// <summary>
    /// Key for home page polls
    /// </summary>
    /// <remarks>
    /// {0} : language ID
    /// {1} : current store ID
    /// </remarks>
    public static CacheKey HomepagePollsModelKey => new("Nop.pres.poll.homepage-{0}-{1}");

    public static string PollsPrefixCacheKey => "Nop.pres.poll";

    #endregion

    #region Permissions

    public static class Permissions
    {
        public const string POLLS_VIEW = "Polls.View";
        public const string POLLS_MANAGE = "Polls.Manage";
    }

    #endregion
}
