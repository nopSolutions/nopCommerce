using Nop.Core.Caching;

namespace Nop.Plugin.Misc.News;

/// <summary>
/// Represents plugin constants
/// </summary>
public class NewsDefaults
{
    /// <summary>
    /// Gets a plugin system name
    /// </summary>
    public static string SystemName => "Misc.News";

    /// <summary>
    /// Represents system name of notification store owner about new news comment
    /// </summary>
    public static string NewsCommentStoreOwnerNotification => "News.NewsComment";

    /// <summary>
    /// Represents system name of the "News items" menu item in the admin area
    /// </summary>
    public static string NewsItemsMenuSystemName => "News items";

    /// <summary>
    /// Represents system name of the "News comments" menu item in the admin area
    /// </summary>
    public static string NewsCommentsMenuSystemName => "News comments";

    /// <summary>
    /// Gets a URL of the news docs page
    /// </summary>
    public static string NewsDocsUrl => "https://docs.nopcommerce.com/running-your-store/content-management/news.html";

    public static string ReservedUrlRecord => "news";

    #region Caching defaults

    /// <summary>
    /// Key for home page news
    /// </summary>
    /// <remarks>
    /// {0} : language ID
    /// {1} : current store ID
    /// </remarks>
    public static CacheKey HomepageNewsModelKey => new("Nop.pres.news.homepage-{0}-{1}");
    public static string NewsPrefixCacheKey => "Nop.pres.news";

    /// <summary>
    /// Key for number of news comments
    /// </summary>
    /// <remarks>
    /// {0} : news item ID
    /// {1} : store ID
    /// {2} : are only approved comments?
    /// </remarks>
    public static CacheKey NewsCommentsNumberCacheKey => new("Nop.newsitem.comments.number.{0}-{1}-{2}");

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    /// <remarks>
    /// {0} : news item ID
    /// </remarks>
    public static string NewsCommentsNumberPrefix => "Nop.newsitem.comments.number.{0}";

    #endregion

    #region Activity log types

    public static class ActivityLogTypeSystemNames
    {
        /// <summary>
        /// Gets a system name of the "Add a new news"
        /// </summary>
        public static string AddNewNews => "AddNewNews";

        /// <summary>
        /// Gets a system name of the "Delete a news"
        /// </summary>
        public static string DeleteNews => "DeleteNews";

        /// <summary>
        /// Gets a system name of the "Edit a news"
        /// </summary>
        public static string EditNews => "EditNews";

        /// <summary>
        /// Gets a system name of the "Delete a news comment"
        /// </summary>
        public static string DeleteNewsComment => "DeleteNewsComment";

        /// <summary>
        /// Gets a system name of the "Edit a news comment"
        /// </summary>
        public static string EditNewsComment => "EditNewsComment";

        /// <summary>
        /// Gets a system name of the "Public store. Add news comment"
        /// </summary>
        public static string PublicStoreAddNewsComment => "PublicStore.AddNewsComment";
    }

    #endregion

    #region Routes

    public static class Routes
    {
        private const string ROUTE_PREFIX = "Plugin.Misc.News.Route.";

        public static class Admin
        {
            public static string ConfigurationRouteName => ROUTE_PREFIX + "Configure";
            public static string NewsItemsRouteName => ROUTE_PREFIX + "NewsItems";
            public static string NewsItemEditRouteName => ROUTE_PREFIX + "NewsItemEdit";
            public static string NewsCommentsRouteName => ROUTE_PREFIX + "NewsComments";
        }

        public static class Public
        {
            public static string NewsItemRouteName => ROUTE_PREFIX + "NewsItem";
            public static string NewsArchive => ROUTE_PREFIX + "NewsArchive";
            public static string NewsRSS => ROUTE_PREFIX + "NewsRSS";
        }

        /// <summary>
        /// Gets default key for newsitem id route value
        /// </summary>
        public static string NewsItemIdRouteValue => "newsItemId";
    }

    #endregion

    #region Permissions

    public static class Permissions
    {
        public const string NEWS_VIEW = "News.View";
        public const string NEWS_MANAGE = "News.Manage";
        public const string NEWS_COMMENTS_VIEW = "News.CommentsView";
        public const string NEWS_COMMENTS_MANAGE = "News.CommentsManage";
    }

    #endregion
}