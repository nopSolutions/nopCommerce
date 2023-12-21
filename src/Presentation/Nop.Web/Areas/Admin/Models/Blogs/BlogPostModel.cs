using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Blogs;

/// <summary>
/// Represents a blog post model
/// </summary>
public partial record BlogPostModel : BaseNopEntityModel, IStoreMappingSupportedModel
{
    #region Ctor

    public BlogPostModel()
    {
        AvailableLanguages = new List<SelectListItem>();
        SelectedStoreIds = new List<int>();
        AvailableStores = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Language")]
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.IncludeInSitemap")]
    public bool IncludeInSitemap { get; set; }

    public IList<SelectListItem> AvailableLanguages { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Language")]
    public string LanguageName { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Title")]
    public string Title { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Body")]
    public string Body { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.BodyOverview")]
    public string BodyOverview { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.AllowComments")]
    public bool AllowComments { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Tags")]
    public string Tags { get; set; }

    public string InitialBlogTags { get; set; }

    public int ApprovedComments { get; set; }

    public int NotApprovedComments { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.StartDate")]
    [UIHint("DateTimeNullable")]
    public DateTime? StartDateUtc { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.EndDate")]
    [UIHint("DateTimeNullable")]
    public DateTime? EndDateUtc { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.MetaKeywords")]
    public string MetaKeywords { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.MetaDescription")]
    public string MetaDescription { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.MetaTitle")]
    public string MetaTitle { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.SeName")]
    public string SeName { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }
    public IList<SelectListItem> AvailableStores { get; set; }

    #endregion
}