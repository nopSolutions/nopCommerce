using Nop.Core.Domain.Forums;

namespace Nop.Web.Models.Boards
{
    public class ForumGroupModel
    {
        public ForumGroupModel(ForumGroup forumGroup)
        {
            this.ForumGroup = forumGroup;
        }

        public ForumGroup ForumGroup { get; private set; }

        public bool AllowViewingProfiles { get; set; }

        public bool RelativeDateTimeFormattingEnabled { get; set; }
    }
}