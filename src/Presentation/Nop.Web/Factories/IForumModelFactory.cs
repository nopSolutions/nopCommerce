using Nop.Core.Domain.Forums;
using Nop.Web.Models.Boards;

namespace Nop.Web.Factories
{
    public partial interface IForumModelFactory
    {
        ForumTopicRowModel PrepareForumTopicRowModel(ForumTopic topic);

        ForumRowModel PrepareForumRowModel(Forum forum);

        ForumGroupModel PrepareForumGroupModel(ForumGroup forumGroup);

        BoardsIndexModel PrepareBoardsIndexModel();

        ActiveDiscussionsModel PrepareActiveDiscussionsModel();

        ActiveDiscussionsModel PrepareActiveDiscussionsModel(int forumId, int page);

        ForumPageModel PrepareForumPageModel(Forum forum, int page);

        ForumTopicPageModel PrepareForumTopicPageModel(ForumTopic forumTopic, int page);

        TopicMoveModel PrepareTopicMove(ForumTopic forumTopic);

        void PrepareTopicCreateModel(Forum forum, EditForumTopicModel model);

        void PrepareTopicEditModel(ForumTopic forumTopic, EditForumTopicModel model,
            bool excludeProperties);

        EditForumPostModel PreparePostCreateModel(ForumTopic forumTopic, int? quote,
            bool excludeProperties);

        EditForumPostModel PreparePostEditModel(ForumPost forumPost, bool excludeProperties);

        SearchModel PrepareSearchModel(string searchterms, bool? adv, string forumId,
            string within, string limitDays, int page);

        LastPostModel PrepareLastPostModel(ForumPost forumPost, bool showTopic);

        ForumBreadcrumbModel PrepareForumBreadcrumbModel(int? forumGroupId, int? forumId,
            int? forumTopicId);

        CustomerForumSubscriptionsModel PrepareCustomerForumSubscriptionsModel(int? page);
    }
}
