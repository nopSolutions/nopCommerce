using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.News;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.News
{
    [Validator(typeof(NewsItemValidator))]
    public partial class NewsItemModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Language")]
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Language")]
        [AllowHtml]
        public string LanguageName { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Short")]
        [AllowHtml]
        public string Short { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Full")]
        [AllowHtml]
        public string Full { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.AllowComments")]
        public bool AllowComments { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.Comments")]
        public int Comments { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

    }
}