using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Blogs;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Blogs
{
    [Validator(typeof(BlogPostValidator))]
    public partial class BlogPostModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Language")]
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Language")]
        [AllowHtml]
        public string LanguageName { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.AllowComments")]
        public bool AllowComments { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Tags")]
        [AllowHtml]
        public string Tags { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Comments")]
        public int Comments { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

    }
}