using Nop.Core.Caching;

namespace Nop.Plugin.Misc.Forums;

/// <summary>
/// Represents plugin constants
/// </summary>
public class ForumDefaults
{
    /// <summary>
    /// Gets a plugin system name
    /// </summary>
    public static string SystemName => "Misc.Forums";

    /// <summary>
    /// Represents system name of the "Forums" menu item in the admin area
    /// </summary>
    public static string ForumsMenuSystemName => "Manage forums";

    /// <summary>
    /// Gets a system name of 'forum moderators' customer role
    /// </summary>
    public static string ForumModeratorsRoleName => "ForumModerators";

    /// <summary>
    /// Gets a URL of the forums docs page
    /// </summary>
    public static string ForumsDocsUrl => "https://docs.nopcommerce.com/running-your-store/content-management/forums.html";

    /// <summary>
    /// Gets a name of generic attribute to store the value of 'ForumPostCount'
    /// </summary>
    public static string ForumPostCountAttribute => "ForumPostCount";

    /// <summary>
    /// Gets a name of generic attribute to store the value of 'Signature'
    /// </summary>
    public static string SignatureAttribute => "Signature";

    /// <summary>
    /// Generic attribute name to hide common settings block on the plugin configuration page
    /// </summary>
    public static string HideCommonBlockAttributeName => "ForumSettingsPage.HideCommonBlock";

    /// <summary>
    /// Generic attribute name to hide permissions settings block on the plugin configuration page
    /// </summary>
    public static string HidePermissionsBlockAttributeName => "ForumSettingsPage.HidePermissionsBlock";

    /// <summary>
    /// Generic attribute name to hide page sizes settings block on the plugin configuration page
    /// </summary>
    public static string HidePageSizesBlockAttributeName => "ForumSettingsPage.HidePageSizesBlock";

    /// <summary>
    /// Generic attribute name to hide forum feeds settings block on the plugin configuration page
    /// </summary>
    public static string HideFeedsBlockAttributeName => "ForumSettingsPage.HideFeedsBlock";

    /// <summary>
    /// Gets a max length of forum topic slug name
    /// </summary>
    /// <remarks>For long URLs we can get the following error: 
    /// "the specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters", 
    /// that's why we limit it to 100</remarks>
    public static int ForumTopicLength => 100;

    /// <summary>
    /// Gets the tab id of the Forum subscriptions menu item
    /// </summary>
    public static int ForumCustomerNavigationTab => 90;

    #region Caching defaults

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : forum group ID
    /// </remarks>
    public static CacheKey ForumByForumGroupCacheKey => new("Nop.forum.byforumgroup.{0}");

    #endregion

    #region Message templates

    /// <summary>
    /// Represents system name of notification about new forum topic
    /// </summary>
    public const string NEW_FORUM_TOPIC_MESSAGE = "Forums.NewForumTopic";

    /// <summary>
    /// Represents system name of notification about new forum post
    /// </summary>
    public const string NEW_FORUM_POST_MESSAGE = "Forums.NewForumPost";

    #endregion

    #region Activity log types

    public static class ActivityLogTypeSystemNames
    {
        /// <summary>
        /// Gets a system name of the "Add forum topic"
        /// </summary>
        public static string AddForumTopic => "PublicStore.AddForumTopic";

        /// <summary>
        /// Gets a system name of the "Edit forum topic"
        /// </summary>
        public static string EditForumTopic => "PublicStore.EditForumTopic";

        /// <summary>
        /// Gets a system name of the "Delete forum topic"
        /// </summary>
        public static string DeleteForumTopic => "PublicStore.DeleteForumTopic";

        /// <summary>
        /// Gets a system name of the "Add forum post"
        /// </summary>
        public static string AddForumPost => "PublicStore.AddForumPost";

        /// <summary>
        /// Gets a system name of the "Edit forum post"
        /// </summary>
        public static string EditForumPost => "PublicStore.EditForumPost";

        /// <summary>
        /// Gets a system name of the "Delete forum post"
        /// </summary>
        public static string DeleteForumPost => "PublicStore.DeleteForumPost";
    }

    #endregion

    #region Routes

    public static class Routes
    {
        private const string ROUTE_PREFIX = "Plugin.Misc.Forums.Route.";

        public static class Admin
        {
            public static string CONFIGURATION => ROUTE_PREFIX + "Configure";
        }

        public static class Public
        {
            /// <summary>
            /// Gets the forums route name
            /// </summary>
            public const string BOARDS = ROUTE_PREFIX + "Boards";

            /// <summary>
            /// Gets the post edit route name
            /// </summary>
            public const string POST_EDIT = ROUTE_PREFIX + "PostEdit";

            /// <summary>
            /// Gets the post delete route name
            /// </summary>
            public const string POST_DELETE = ROUTE_PREFIX + "PostDelete";

            /// <summary>
            /// Gets the post create route name
            /// </summary>
            public const string POST_CREATE = ROUTE_PREFIX + "PostCreate";

            /// <summary>
            /// Gets the post create quote route name
            /// </summary>
            public const string POST_CREATE_QUOTE = ROUTE_PREFIX + "PostCreateQuote";

            /// <summary>
            /// Gets the topic edit route name
            /// </summary>
            public const string TOPIC_EDIT = ROUTE_PREFIX + "TopicEdit";

            /// <summary>
            /// Gets the topic delete route name
            /// </summary>
            public const string TOPIC_DELETE = ROUTE_PREFIX + "TopicDelete";

            /// <summary>
            /// Gets the topic create route name
            /// </summary>
            public const string TOPIC_CREATE = ROUTE_PREFIX + "TopicCreate";

            /// <summary>
            /// Gets the topic move route name
            /// </summary>
            public const string TOPIC_MOVE = ROUTE_PREFIX + "TopicMove";

            /// <summary>
            /// Gets the topic slug route name
            /// </summary>
            public const string TOPIC_SLUG = ROUTE_PREFIX + "TopicSlug";

            /// <summary>
            /// Gets the topic slug paged route name
            /// </summary>
            public const string TOPIC_SLUG_PAGED = ROUTE_PREFIX + "TopicSlugPaged";

            /// <summary>
            /// Gets the forums RSS (file result) route name
            /// </summary>
            public const string FORUM_RSS = ROUTE_PREFIX + "ForumRSS";

            /// <summary>
            /// Gets the forum slug route name
            /// </summary>
            public const string FORUM_SLUG = ROUTE_PREFIX + "ForumSlug";

            /// <summary>
            /// Gets the forum slug paged route name
            /// </summary>
            public const string FORUM_SLUG_PAGED = ROUTE_PREFIX + "ForumSlugPaged";

            /// <summary>
            /// Gets the forum group slug route name
            /// </summary>
            public const string FORUM_GROUP_SLUG = ROUTE_PREFIX + "ForumGroupSlug";

            /// <summary>
            /// Gets the forum search route name
            /// </summary>
            public const string BOARDS_SEARCH = ROUTE_PREFIX + "Search";

            /// <summary>
            /// Gets the post vote route name
            /// </summary>
            public const string POST_VOTE = ROUTE_PREFIX + "PostVote";

            /// <summary>
            /// Gets the topic watch route name
            /// </summary>
            public const string TOPIC_WATCH = ROUTE_PREFIX + "TopicWatch";

            /// <summary>
            /// Gets the forum watch route name
            /// </summary>
            public const string FORUM_WATCH = ROUTE_PREFIX + "ForumWatch";

            /// <summary>
            /// Gets the forums RSS (file result) route name
            /// </summary>
            public const string ACTIVE_DISCUSSIONS_RSS = ROUTE_PREFIX + "ActiveDiscussionsRSS";

            /// <summary>
            /// Gets the active discussions route name
            /// </summary>
            public const string ACTIVE_DISCUSSIONS = ROUTE_PREFIX + "ActiveDiscussions";

            /// <summary>
            /// Gets the active discussions paged route name
            /// </summary>
            public const string ACTIVE_DISCUSSIONS_PAGED = ROUTE_PREFIX + "ActiveDiscussionsPaged";

            /// <summary>
            /// Gets the customer forum subscriptions route name
            /// </summary>
            public const string CUSTOMER_FORUM_SUBSCRIPTIONS = ROUTE_PREFIX + "CustomerForumSubscriptions";

            /// <summary>
            /// Gets the customer's forum posts route name
            /// </summary>
            public const string FORUM_PROFILE_POSTS = ROUTE_PREFIX + "ProfileForumPosts";

            /// <summary>
            /// Gets the customer's forum posts route name (paged)
            /// </summary>
            public const string FORUM_PROFILE_POSTS_PAGED = ROUTE_PREFIX + "ProfileForumPostsPaged";
        }
    }

    #endregion

    #region Permissions

    public static class Permissions
    {
        public const string FORUMS_VIEW = "Forums.View";
        public const string FORUMS_MANAGE = "Forums.Manage";
    }

    #endregion
}