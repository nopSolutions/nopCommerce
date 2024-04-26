using Nop.Core.Domain.Forums;
using Nop.Web.Models.Boards;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the interface of the forum model factory
/// </summary>
public partial interface IForumModelFactory
{
    /// <summary>
    /// Prepare the forum group model
    /// </summary>
    /// <param name="forumGroup">Forum group</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum group model
    /// </returns>
    Task<ForumGroupModel> PrepareForumGroupModelAsync(ForumGroup forumGroup);

    /// <summary>
    /// Prepare the boards index model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the boards index model
    /// </returns>
    Task<BoardsIndexModel> PrepareBoardsIndexModelAsync();

    /// <summary>
    /// Prepare the active discussions model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the active discussions model
    /// </returns>
    Task<ActiveDiscussionsModel> PrepareActiveDiscussionsModelAsync();

    /// <summary>
    /// Prepare the active discussions model
    /// </summary>
    /// <param name="forumId">Forum identifier</param>
    /// <param name="page">Number of forum topics page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the active discussions model
    /// </returns>
    Task<ActiveDiscussionsModel> PrepareActiveDiscussionsModelAsync(int forumId, int page);

    /// <summary>
    /// Prepare the forum page model
    /// </summary>
    /// <param name="forum">Forum</param>
    /// <param name="page">Number of forum topics page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum page model
    /// </returns>
    Task<ForumPageModel> PrepareForumPageModelAsync(Forum forum, int page);

    /// <summary>
    /// Prepare the forum topic page model
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <param name="page">Number of forum posts page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum topic page model
    /// </returns>
    Task<ForumTopicPageModel> PrepareForumTopicPageModelAsync(ForumTopic forumTopic, int page);

    /// <summary>
    /// Prepare the topic move model
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic move model
    /// </returns>
    Task<TopicMoveModel> PrepareTopicMoveAsync(ForumTopic forumTopic);

    /// <summary>
    /// Prepare the forum topic create model
    /// </summary>
    /// <param name="forum">Forum</param>
    /// <param name="model">Edit forum topic model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PrepareTopicCreateModelAsync(Forum forum, EditForumTopicModel model);

    /// <summary>
    /// Prepare the forum topic edit model
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <param name="model">Edit forum topic model</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PrepareTopicEditModelAsync(ForumTopic forumTopic, EditForumTopicModel model,
        bool excludeProperties);

    /// <summary>
    /// Prepare the forum post create model
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <param name="quote">Identifier of the quoted post; pass null to load the empty text</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the edit forum post model
    /// </returns>
    Task<EditForumPostModel> PreparePostCreateModelAsync(ForumTopic forumTopic, int? quote,
        bool excludeProperties);

    /// <summary>
    /// Prepare the forum post edit model
    /// </summary>
    /// <param name="forumPost">Forum post</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the edit forum post model
    /// </returns>
    Task<EditForumPostModel> PreparePostEditModelAsync(ForumPost forumPost, bool excludeProperties);

    /// <summary>
    /// Prepare the search model
    /// </summary>
    /// <param name="searchterms">Search terms</param>
    /// <param name="advs">Whether to use the advanced search</param>
    /// <param name="forumId">Forum identifier</param>
    /// <param name="within">String representation of int value of ForumSearchType</param>
    /// <param name="limitDays">Limit by the last number days; 0 to load all topics</param>
    /// <param name="page">Number of items page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search model
    /// </returns>
    Task<SearchModel> PrepareSearchModelAsync(string searchterms, bool? advs, string forumId,
        string within, string limitDays, int page);

    /// <summary>
    /// Prepare the last post model
    /// </summary>
    /// <param name="forumPost">Forum post</param>
    /// <param name="showTopic">Whether to show topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the last post model
    /// </returns>
    Task<LastPostModel> PrepareLastPostModelAsync(ForumPost forumPost, bool showTopic);

    /// <summary>
    /// Prepare the forum breadcrumb model
    /// </summary>
    /// <param name="forumGroupId">Forum group identifier; pass null to load nothing</param>
    /// <param name="forumId">Forum identifier; pass null to load breadcrumbs up to forum group</param>
    /// <param name="forumTopicId">Forum topic identifier; pass null to load breadcrumbs up to forum</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum breadcrumb model
    /// </returns>
    Task<ForumBreadcrumbModel> PrepareForumBreadcrumbModelAsync(int? forumGroupId, int? forumId,
        int? forumTopicId);

    /// <summary>
    /// Prepare the customer forum subscriptions model
    /// </summary>
    /// <param name="page">Number of items page; pass null to load the first page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer forum subscriptions model
    /// </returns>
    Task<CustomerForumSubscriptionsModel> PrepareCustomerForumSubscriptionsModelAsync(int? page);

    /// <summary>
    /// Prepare the forum topic row model
    /// </summary>
    /// <param name="topic">Forum topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum topic row model
    /// </returns>
    Task<ForumTopicRowModel> PrepareForumTopicRowModelAsync(ForumTopic topic);

    /// <summary>
    /// Prepare the forum row model
    /// </summary>
    /// <param name="forum">Forum</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum row model
    /// </returns>
    Task<ForumRowModel> PrepareForumRowModelAsync(Forum forum);
}