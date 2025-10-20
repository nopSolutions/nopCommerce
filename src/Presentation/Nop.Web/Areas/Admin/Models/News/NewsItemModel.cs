﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.News;

/// <summary>
/// Represents a news item model
/// </summary>
public partial record NewsItemModel : BaseNopEntityModel, IStoreMappingSupportedModel, IMetaTagsSupportedModel
{
    #region Ctor

    public NewsItemModel()
    {
        AvailableLanguages = new List<SelectListItem>();

        SelectedStoreIds = new List<int>();
        AvailableStores = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Language")]
    public int LanguageId { get; set; }

    public IList<SelectListItem> AvailableLanguages { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Language")]
    public string LanguageName { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Title")]
    public string Title { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Short")]
    public string Short { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Full")]
    public string Full { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.AllowComments")]
    public bool AllowComments { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.StartDate")]
    [UIHint("DateTimeNullable")]
    public DateTime? StartDateUtc { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.EndDate")]
    [UIHint("DateTimeNullable")]
    public DateTime? EndDateUtc { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.MetaKeywords")]
    public string MetaKeywords { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.MetaDescription")]
    public string MetaDescription { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.MetaTitle")]
    public string MetaTitle { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.SeName")]
    public string SeName { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Published")]
    public bool Published { get; set; }

    public int ApprovedComments { get; set; }

    public int NotApprovedComments { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    #endregion
}