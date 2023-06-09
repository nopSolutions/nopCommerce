using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record ActiveDiscussionsModel : BaseNopModel
    {
        public ActiveDiscussionsModel()
        {
            ForumTopics = new List<ForumTopicRowModel>();
        }

        public IList<ForumTopicRowModel> ForumTopics { get; protected set; }

        public bool ViewAllLinkEnabled { get; set; }

        public bool ActiveDiscussionsFeedEnabled { get; set; }

        public int TopicPageSize { get; set; }
        public int TopicTotalRecords { get; set; }
        public int TopicPageIndex { get; set; }

        public int PostsPageSize { get; set; }

        public bool AllowPostVoting { get; set; }
    }
}