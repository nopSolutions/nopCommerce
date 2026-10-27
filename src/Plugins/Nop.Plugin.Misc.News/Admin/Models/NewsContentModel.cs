using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.News.Admin.Models;

/// <summary>
/// Represents a news content model
/// </summary>
public record NewsContentModel : BaseNopModel
{
    #region Ctor

    public NewsContentModel()
    {
        NewsItems = new NewsItemSearchModel();
        NewsComments = new NewsCommentSearchModel();
        SearchTitle = new NewsItemSearchModel().SearchTitle;
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Plugins.Misc.News.NewsItems.List.SearchTitle")]
    public string SearchTitle { get; set; }

    public NewsItemSearchModel NewsItems { get; set; }

    public NewsCommentSearchModel NewsComments { get; set; }

    #endregion
}