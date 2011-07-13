using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Settings
{
    public class BlogSettingsModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Configuration.Settings.Blog.Enabled")]
        public bool Enabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Blog.PostsPageSize")]
        public int PostsPageSize { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Blog.AllowNotRegisteredUsersToLeaveComments")]
        public bool AllowNotRegisteredUsersToLeaveComments { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Blog.NotifyAboutNewBlogComments")]
        public bool NotifyAboutNewBlogComments { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Blog.NumberOfTags")]
        public int NumberOfTags { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Blog.ShowHeaderRSSUrl")]
        public bool ShowHeaderRssUrl { get; set; }
    }
}