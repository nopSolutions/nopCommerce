using Nop.Core.Domain.Forums;

namespace Nop.Web.Models.Boards
{
    public class ForumModel
    {
        public ForumModel(Forum forum)
        {
            this.Forum = forum;
        }

        public Forum Forum { get; private set; }

        public string WatchForumText { get; set; }

        public Core.PagedList<ForumTopic> PagedList { get; set; }

        public bool IsCustomerAllowedToSubscribe { get; set; }

        public bool AllowViewingProfiles { get; set; }

        public bool ForumFeedsEnabled { get; set; }

        public int PostsPageSize { get; set; }

        public bool RelativeDateTimeFormattingEnabled { get; set; }
    }
}