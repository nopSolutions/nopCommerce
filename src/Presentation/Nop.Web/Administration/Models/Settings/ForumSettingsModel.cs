using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Settings
{
    public partial class ForumSettingsModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }


        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ForumsEnabled")]
        public bool ForumsEnabled { get; set; }
        public bool ForumsEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.RelativeDateTimeFormattingEnabled")]
        public bool RelativeDateTimeFormattingEnabled { get; set; }
        public bool RelativeDateTimeFormattingEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ShowCustomersPostCount")]
        public bool ShowCustomersPostCount { get; set; }
        public bool ShowCustomersPostCount_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.AllowGuestsToCreatePosts")]
        public bool AllowGuestsToCreatePosts { get; set; }
        public bool AllowGuestsToCreatePosts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.AllowGuestsToCreateTopics")]
        public bool AllowGuestsToCreateTopics { get; set; }
        public bool AllowGuestsToCreateTopics_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.AllowCustomersToEditPosts")]
        public bool AllowCustomersToEditPosts { get; set; }
        public bool AllowCustomersToEditPosts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.AllowCustomersToDeletePosts")]
        public bool AllowCustomersToDeletePosts { get; set; }
        public bool AllowCustomersToDeletePosts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.AllowPostVoting")]
        public bool AllowPostVoting { get; set; }
        public bool AllowPostVoting_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.MaxVotesPerDay")]
        public int MaxVotesPerDay { get; set; }
        public bool MaxVotesPerDay_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.AllowCustomersToManageSubscriptions")]
        public bool AllowCustomersToManageSubscriptions { get; set; }
        public bool AllowCustomersToManageSubscriptions_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.TopicsPageSize")]
        public int TopicsPageSize { get; set; }
        public bool TopicsPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.PostsPageSize")]
        public int PostsPageSize { get; set; }
        public bool PostsPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ForumEditor")]
        public int ForumEditor { get; set; }
        public bool ForumEditor_OverrideForStore { get; set; }
        public SelectList ForumEditorValues { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.SignaturesEnabled")]
        public bool SignaturesEnabled { get; set; }
        public bool SignaturesEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.AllowPrivateMessages")]
        public bool AllowPrivateMessages { get; set; }
        public bool AllowPrivateMessages_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ShowAlertForPM")]
        public bool ShowAlertForPM { get; set; }
        public bool ShowAlertForPM_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.NotifyAboutPrivateMessages")]
        public bool NotifyAboutPrivateMessages { get; set; }
        public bool NotifyAboutPrivateMessages_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ActiveDiscussionsFeedEnabled")]
        public bool ActiveDiscussionsFeedEnabled { get; set; }
        public bool ActiveDiscussionsFeedEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ActiveDiscussionsFeedCount")]
        public int ActiveDiscussionsFeedCount { get; set; }
        public bool ActiveDiscussionsFeedCount_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ForumFeedsEnabled")]
        public bool ForumFeedsEnabled { get; set; }
        public bool ForumFeedsEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ForumFeedCount")]
        public int ForumFeedCount { get; set; }
        public bool ForumFeedCount_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.SearchResultsPageSize")]
        public int SearchResultsPageSize { get; set; }
        public bool SearchResultsPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Forums.ActiveDiscussionsPageSize")]
        public int ActiveDiscussionsPageSize { get; set; }
        public bool ActiveDiscussionsPageSize_OverrideForStore { get; set; }

    }
}