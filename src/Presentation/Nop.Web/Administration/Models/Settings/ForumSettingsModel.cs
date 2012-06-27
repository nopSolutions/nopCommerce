using System.Web.Mvc;
using Nop.Core.Domain.Forums;
using Nop.Web.Framework;

namespace Nop.Admin.Models.Settings
{
    public partial class ForumSettingsModel
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

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.AllowPrivateMessages")]
        public bool AllowPrivateMessages { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ShowAlertForPM")]
        public bool ShowAlertForPM { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.NotifyAboutPrivateMessages")]
        public bool NotifyAboutPrivateMessages { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ActiveDiscussionsFeedEnabled")]
        public bool ActiveDiscussionsFeedEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ActiveDiscussionsFeedCount")]
        public int ActiveDiscussionsFeedCount { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ForumFeedsEnabled")]
        public bool ForumFeedsEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ForumFeedCount")]
        public int ForumFeedCount { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.SearchResultsPageSize")]
        public int SearchResultsPageSize { get; set; }

        public SelectList ForumEditorValues { get; set; }
    }
}