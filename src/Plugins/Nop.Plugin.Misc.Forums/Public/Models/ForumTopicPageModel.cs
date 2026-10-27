using Nop.Plugin.Misc.Forums.Domain;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Forums.Public.Models;

public record ForumTopicPageModel : BaseNopEntityModel
{
    #region Properties

    public string Subject { get; set; }
    public string SeName { get; set; }
    public string WatchTopicText { get; set; }
    public bool IsCustomerAllowedToEditTopic { get; set; }
    public bool IsCustomerAllowedToDeleteTopic { get; set; }
    public bool IsCustomerAllowedToMoveTopic { get; set; }
    public bool IsCustomerAllowedToSubscribe { get; set; }
    public EditorType ForumEditor { get; set; }
    public int PostsPageIndex { get; set; }
    public int PostsPageSize { get; set; }
    public int PostsTotalRecords { get; set; }
    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }
    public string JsonLd { get; set; }

    public List<ForumPostModel> ForumPostModels { get; set; } = new();

    #endregion
}