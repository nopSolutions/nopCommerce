using System.Collections.Generic;

namespace Nop.Web.Models.Boards
{
    public class ActiveDiscussionsModel
    {
        public ActiveDiscussionsModel()
        {
            ForumTopics = new List<ForumTopicRowModel>();
        }

        public IList<ForumTopicRowModel> ForumTopics { get; private set; }

        public bool ViewAllLinkEnabled { get; set; }

        public bool ActiveDiscussionsFeedEnabled { get; set; }

        public int PostsPageSize { get; set; }
    }
}