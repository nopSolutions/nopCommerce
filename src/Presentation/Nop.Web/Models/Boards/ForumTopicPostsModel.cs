using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Forums;

namespace Nop.Web.Models.Boards
{
    public class ForumTopicPostsModel
    {
        public ForumTopicPostsModel(ForumTopic forumTopic)
        {
            this.ForumTopic = forumTopic;
            this.ForumPostModels = new List<ForumPostModel>();
        }

        public ForumTopic ForumTopic { get; private set; }

        public string WatchTopicText { get; set; }

        public PagedList<ForumPost> PagedList { get; set; }

        public ForumBreadcrumbModel ForumBreadcrumbModel { get; set; }

        public bool IsCustomerAllowedToEditTopic { get; set; }

        public bool IsCustomerAllowedToDeleteTopic { get; set; }

        public bool IsCustomerAllowedToMoveTopic { get; set; }

        public bool IsCustomerAllowedToSubscribe { get; set; }

        public List<ForumPostModel> ForumPostModels { get; set; }
    }
}