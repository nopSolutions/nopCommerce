using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Blogs
{
    public partial class BlogCommentListModel : BaseNopModel
    {
        public BlogCommentListModel()
        {
            AvailableApprovedOptions = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.Comments.List.CreatedOnFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.Comments.List.CreatedOnTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.Comments.List.SearchText")]
        [AllowHtml]
        public string SearchText { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.Comments.List.SearchApproved")]
        public int SearchApprovedId { get; set; }

        public IList<SelectListItem> AvailableApprovedOptions { get; set; }
    }
}