using System.Web.Mvc;
using Nop.Core.Domain.Forums;
using Nop.Web.Framework;

namespace Nop.Admin.Models
{
    public class ForumSettingsModel
    {
        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ForumsEnabled")]
        public bool ForumsEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.RelativeDateTimeFormattingEnabled")]
        public bool RelativeDateTimeFormattingEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ShowCustomersPostCount")]
        public bool ShowCustomersPostCount { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.AllowGuestsToCreatePosts")]
        public bool AllowGuestsToCreatePosts { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.AllowGuestsToCreateTopics")]
        public bool AllowGuestsToCreateTopics { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.AllowCustomersToEditPosts")]
        public bool AllowCustomersToEditPosts { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.AllowCustomersToDeletePosts")]
        public bool AllowCustomersToDeletePosts { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.AllowCustomersToManageSubscriptions")]
        public bool AllowCustomersToManageSubscriptions { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.TopicsPageSize")]
        public int TopicsPageSize { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.PostsPageSize")]
        public int PostsPageSize { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ForumEditor")]
        public EditorType ForumEditor { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.SignaturesEnabled")]
        public bool SignaturesEnabled { get; set; }

        public SelectList ForumEditorValues { get; set; }
    }
}