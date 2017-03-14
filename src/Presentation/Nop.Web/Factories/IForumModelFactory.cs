using Nop.Core.Domain.Forums;
using Nop.Web.Models.Boards;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the forum model factory
    /// </summary>
    public partial interface IForumModelFactory
    {
        /// <summary>
        /// Prepare the forum topic row model
        /// </summary>
        /// <param name="topic">Forum topic</param>
        /// <returns>Forum topic row model</returns>
        ForumTopicRowModel PrepareForumTopicRowModel(ForumTopic topic);

        /// <summary>
        /// Prepare the forum row model
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>Forum row model</returns>
        ForumRowModel PrepareForumRowModel(Forum forum);

        /// <summary>
        /// Prepare the forum group model
        /// </summary>
        /// <param name="forum">Forum group</param>
        /// <returns>Forum group model</returns>
        ForumGroupModel PrepareForumGroupModel(ForumGroup forumGroup);

        /// <summary>
        /// Prepare the boards index model
        /// </summary>
        /// <returns>Boards index model</returns>
        BoardsIndexModel PrepareBoardsIndexModel();

        /// <summary>
        /// Prepare the active discussions model
        /// </summary>
        /// <returns>Active discussions model</returns>
        ActiveDiscussionsModel PrepareActiveDiscussionsModel();

        /// <summary>
        /// Prepare the active discussions model
        /// </summary>
        /// <param name="forumId">Forum identifier</param>
        /// <param name="page">Number of forum topics page</param>
        /// <returns>Active discussions model</returns>
        ActiveDiscussionsModel PrepareActiveDiscussionsModel(int forumId, int page);

        /// <summary>
        /// Prepare the forum page model
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <param name="page">Number of forum topics page</param>
        /// <returns>Forum page model</returns>
        ForumPageModel PrepareForumPageModel(Forum forum, int page);

        /// <summary>
        /// Prepare the forum topic page model
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="page">Number of forum posts page</param>
        /// <returns>Forum topic page model</returns>
        ForumTopicPageModel PrepareForumTopicPageModel(ForumTopic forumTopic, int page);

        /// <summary>
        /// Prepare the topic move model
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>Topic move model</returns>
        TopicMoveModel PrepareTopicMove(ForumTopic forumTopic);

        /// <summary>
        /// Prepare the forum topic create model
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <param name="model">Edit forum topic model</param>
        void PrepareTopicCreateModel(Forum forum, EditForumTopicModel model);

        /// <summary>
        /// Prepare the forum topic edit model
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="model">Edit forum topic model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        void PrepareTopicEditModel(ForumTopic forumTopic, EditForumTopicModel model,
            bool excludeProperties);

        /// <summary>
        /// Prepare the forum post create model
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="quote">Identifier of the quoted post; pass null to load the empty text</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Edit forum post model</returns>
        EditForumPostModel PreparePostCreateModel(ForumTopic forumTopic, int? quote,
            bool excludeProperties);

        /// <summary>
        /// Prepare the forum post edit model
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Edit forum post model</returns>
        EditForumPostModel PreparePostEditModel(ForumPost forumPost, bool excludeProperties);

        /// <summary>
        /// Prepare the search model
        /// </summary>
        /// <param name="searchterms">Search terms</param>
        /// <param name="adv">Whether to use the advanced search</param>
        /// <param name="forumId">Forum identifier</param>
        /// <param name="within">String representation of int value of ForumSearchType</param>
        /// <param name="limitDays">Limit by the last number days; 0 to load all topics</param>
        /// <param name="page">Number of items page</param>
        /// <returns>Search model</returns>
        SearchModel PrepareSearchModel(string searchterms, bool? adv, string forumId,
            string within, string limitDays, int page);

        /// <summary>
        /// Prepare the last post model
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        /// <param name="showTopic">Whether to show topic</param>
        /// <returns>Last post model</returns>
        LastPostModel PrepareLastPostModel(ForumPost forumPost, bool showTopic);

        /// <summary>
        /// Prepare the forum breadcrumb model
        /// </summary>
        /// <param name="forumGroupId">Forum group identifier; pass null to load nothing</param>
        /// <param name="forumId">Forum identifier; pass null to load breadcrumbs up to forum group</param>
        /// <param name="forumTopicId">Forum topic identifier; pass null to load breadcrumbs up to forum</param>
        /// <returns>Forum breadcrumb model</returns>
        ForumBreadcrumbModel PrepareForumBreadcrumbModel(int? forumGroupId, int? forumId,
            int? forumTopicId);

        /// <summary>
        /// Prepare the customer forum subscriptions model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>customer forum subscriptions model</returns>
        CustomerForumSubscriptionsModel PrepareCustomerForumSubscriptionsModel(int? page);
    }
}
