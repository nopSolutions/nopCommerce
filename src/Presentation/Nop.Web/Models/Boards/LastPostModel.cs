using Nop.Core.Domain.Forums;

namespace Nop.Web.Models.Boards
{
    public class LastPostModel
    {
        public LastPostModel(ForumPost forumPost, bool showTopic, 
            bool allowViewingProfiles, bool relativeDateTimeFormattingEnabled)
        {
            this.forumPost = forumPost;
            this.ShowTopic = showTopic;
            this.AllowViewingProfiles = allowViewingProfiles;
            this.RelativeDateTimeFormattingEnabled = relativeDateTimeFormattingEnabled;
        }

        public ForumPost forumPost { get; private set; }

        public bool ShowTopic { get; private set; }

        public bool AllowViewingProfiles { get; set; }

        public bool RelativeDateTimeFormattingEnabled { get; set; }
    }
}