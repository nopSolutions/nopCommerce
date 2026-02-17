using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Forums.Public.Models;

public record ForumPageModel : BaseNopEntityModel
{
    #region Properties

    public string Name { get; set; }
    public string SeName { get; set; }
    public string Description { get; set; }
    public string WatchForumText { get; set; }
    public int TopicPageSize { get; set; }
    public int TopicTotalRecords { get; set; }
    public int TopicPageIndex { get; set; }
    public bool IsCustomerAllowedToSubscribe { get; set; }
    public bool ForumFeedsEnabled { get; set; }
    public int PostsPageSize { get; set; }
    public bool AllowPostVoting { get; set; }

    public List<ForumTopicRowModel> ForumTopics { get; set; } = new();

    #endregion
}