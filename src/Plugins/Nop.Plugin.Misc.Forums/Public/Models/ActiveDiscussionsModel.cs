using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Forums.Public.Models;

public record ActiveDiscussionsModel : BaseNopModel
{
    #region Properties

    public bool ViewAllLinkEnabled { get; set; }
    public bool ActiveDiscussionsFeedEnabled { get; set; }
    public int TopicPageSize { get; set; }
    public int TopicTotalRecords { get; set; }
    public int TopicPageIndex { get; set; }
    public int PostsPageSize { get; set; }
    public bool AllowPostVoting { get; set; }

    public List<ForumTopicRowModel> ForumTopics { get; protected set; } = new();

    #endregion
}