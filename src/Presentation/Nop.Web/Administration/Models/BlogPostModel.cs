using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models
{
    [Validator(typeof(BlogPostValidator))]
    public class BlogPostModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Language")]
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Language")]
        [AllowHtml]
        public string LanguageName { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Title")]
        [AllowHtml]
        public virtual string Title { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Body")]
        [AllowHtml]
        public virtual string Body { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.AllowComments")]
        public virtual bool AllowComments { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Tags")]
        [AllowHtml]
        public virtual string Tags { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.Comments")]
        public virtual int Comments { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.CreatedOn")]
        public virtual DateTime CreatedOn { get; set; }

    }
}