using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;

namespace Nop.Plugin.Misc.Forums.Public.Models;

public record ProfilePostsModel : BaseNopModel
{
    #region Properties

    public string CustomerName { get; set; }
    public int CustomerId { get; set; }
    public PagerModel PagerModel { get; set; }
    public List<PostsModel> Posts { get; set; } = new();

    #endregion
}

public record PostsModel : BaseNopModel
{
    #region Properties

    public int ForumTopicId { get; set; }
    public string ForumTopicTitle { get; set; }
    public string ForumTopicSlug { get; set; }
    public string ForumPostText { get; set; }
    public string Posted { get; set; }

    #endregion
}