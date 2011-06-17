using System.Collections.Generic;
using Nop.Core.Domain.Forums;

namespace Nop.Web.Models.Boards
{
    public class BoardsIndexModel
    {
        public BoardsIndexModel(IEnumerable<ForumGroup> forumGroup, ActiveDiscussionsModel activeDiscusssionsModel)
        {
            ForumGroup = forumGroup;
            ActiveDiscussionsModel = activeDiscusssionsModel;
        }

        public ActiveDiscussionsModel ActiveDiscussionsModel { get; set; }

        public IEnumerable<ForumGroup> ForumGroup { get; private set; }

        public bool AllowViewingProfiles { get; set; }

        public bool RelativeDateTimeFormattingEnabled { get; set; }
    }
}