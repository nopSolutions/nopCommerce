using System.Web.Mvc;
using Nop.Core.Domain.Forums;
using Nop.Web.Framework;

namespace Nop.Admin.Models
{
    public class BlogSettingsModel
    {
        [NopResourceDisplayName("Admin.ContentManagement.Blog.Settings.Fields.Enabled")]
        public bool Enabled { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.Settings.Fields.PostsPageSize")]
        public int PostsPageSize { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.Settings.Fields.AllowNotRegisteredUsersToLeaveComments")]
        public bool AllowNotRegisteredUsersToLeaveComments { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Blog.Settings.Fields.NotifyAboutNewBlogComments")]
        public bool NotifyAboutNewBlogComments { get; set; }
    }
}