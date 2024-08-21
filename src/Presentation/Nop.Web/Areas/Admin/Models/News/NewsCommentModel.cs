using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.News;

/// <summary>
/// Represents a news comment model
/// </summary>
public partial record NewsCommentModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.NewsItem")]
    public int NewsItemId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.NewsItem")]
    public string NewsItemTitle { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.Customer")]
    public int CustomerId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.Customer")]
    public string CustomerInfo { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.CommentTitle")]
    public string CommentTitle { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.CommentText")]
    public string CommentText { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.IsApproved")]
    public bool IsApproved { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.StoreName")]
    public int StoreId { get; set; }

    public string StoreName { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.Comments.Fields.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    #endregion
}