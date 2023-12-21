using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.News;

/// <summary>
/// Represents a news comment search model
/// </summary>
public partial record NewsCommentSearchModel : BaseSearchModel
{
    #region Ctor

    public NewsCommentSearchModel()
    {
        AvailableApprovedOptions = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    public int? NewsItemId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.Comments.List.CreatedOnFrom")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnFrom { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.Comments.List.CreatedOnTo")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnTo { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.Comments.List.SearchText")]
    public string SearchText { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.Comments.List.SearchApproved")]
    public int SearchApprovedId { get; set; }

    public IList<SelectListItem> AvailableApprovedOptions { get; set; }

    #endregion
}