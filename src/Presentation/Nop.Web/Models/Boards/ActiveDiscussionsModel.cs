using System.Collections.Generic;
using Nop.Core.Domain.Forums;

namespace Nop.Web.Models.Boards
{
    public class ActiveDiscussionsModel
    {
        public ActiveDiscussionsModel(IEnumerable<ForumTopic> activeDiscussions)
        {
            ActiveDiscussions = activeDiscussions;
        }

        public IEnumerable<ForumTopic> ActiveDiscussions { get; private set; }

        public bool ViewAllLinkEnabled { get; set; }

        public bool ActiveDiscussionsFeedEnabled { get; set; }

        public int PostsPageSize { get; set; }

        public bool AllowViewingProfiles { get; set; }

        public bool RelativeDateTimeFormattingEnabled { get; set; }
    }
}