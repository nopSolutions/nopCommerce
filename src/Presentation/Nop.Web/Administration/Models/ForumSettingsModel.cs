using System.Web.Mvc;
using Nop.Core.Domain.Forums;
using Nop.Web.Framework;

namespace Nop.Admin.Models
{
    public class ForumSettingsModel
    {
        [NopResourceDisplayName("Admin.ContentManagement.Forums.Settings.Fields.ForumsEnabled")]
        public bool ForumsEnabled { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Settings.Fields.RelativeDateTimeFormattingEnabled")]
        public bool RelativeDateTimeFormattingEnabled { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Settings.Fields.ShowCustomersPostCount")]
        public bool ShowCustomersPostCount { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Settings.Fields.AllowGuestsToCreatePosts")]
        public bool AllowGuestsToCreatePosts { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Settings.Fields.AllowGuestsToCreateTopics")]
        public bool AllowGuestsToCreateTopics { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Settings.Fields.AllowCustomersToEditPosts")]
        public bool AllowCustomersToEditPosts { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Settings.Fields.AllowCustomersToDeletePosts")]
        public bool AllowCustomersToDeletePosts { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Settings.Fields.AllowCustomersToManageSubscriptions")]
        public bool AllowCustomersToManageSubscriptions { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Settings.Fields.TopicsPageSize")]
        public int TopicsPageSize { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Settings.Fields.PostsPageSize")]
        public int PostsPageSize { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Settings.Fields.ForumEditor")]
        public EditorType ForumEditor { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Forums.Settings.Fields.SignaturesEnabled")]
        public bool SignaturesEnabled { get; set; }

        public SelectList ForumEditorValues { get; set; }
    }
}