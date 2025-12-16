using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.News.Admin.Models;

/// <summary>
/// Represents a news comment model
/// </summary>
public record NewsCommentModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Plugins.Misc.News.Comments.Fields.NewsItem")]
    public int NewsItemId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Comments.Fields.NewsItem")]
    public string NewsItemTitle { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Comments.Fields.Customer")]
    public int CustomerId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Comments.Fields.Customer")]
    public string CustomerInfo { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Comments.Fields.CommentTitle")]
    public string CommentTitle { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Comments.Fields.CommentText")]
    public string CommentText { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Comments.Fields.IsApproved")]
    public bool IsApproved { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Comments.Fields.StoreName")]
    public int StoreId { get; set; }

    public string StoreName { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Comments.Fields.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    #endregion
}