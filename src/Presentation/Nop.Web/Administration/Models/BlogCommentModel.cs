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
    public class BlogCommentModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.BlogPost")]
        public int BlogPostId { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.BlogPost")]
        [AllowHtml]
        public string BlogPostTitle { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.Customer")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.IPAddress")]
        public string IpAddress { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.Comment")]
        public string Comment { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.Comments.Fields.CreatedOn")]
        public string CreatedOn { get; set; }

    }
}