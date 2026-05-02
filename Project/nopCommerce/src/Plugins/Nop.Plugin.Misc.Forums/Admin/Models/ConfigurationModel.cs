using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Forums.Admin.Models;

/// <summary>
/// Represents a configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel, ISettingsModel
{
    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.ForumsEnabled")]
    public bool ForumsEnabled { get; set; }
    public bool ForumsEnabled_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.RelativeDateTimeFormattingEnabled")]
    public bool RelativeDateTimeFormattingEnabled { get; set; }
    public bool RelativeDateTimeFormattingEnabled_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.ShowCustomersPostCount")]
    public bool ShowCustomersPostCount { get; set; }
    public bool ShowCustomersPostCount_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.AllowGuestsToCreatePosts")]
    public bool AllowGuestsToCreatePosts { get; set; }
    public bool AllowGuestsToCreatePosts_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.AllowGuestsToCreateTopics")]
    public bool AllowGuestsToCreateTopics { get; set; }
    public bool AllowGuestsToCreateTopics_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.AllowCustomersToEditPosts")]
    public bool AllowCustomersToEditPosts { get; set; }
    public bool AllowCustomersToEditPosts_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.AllowCustomersToDeletePosts")]
    public bool AllowCustomersToDeletePosts { get; set; }
    public bool AllowCustomersToDeletePosts_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.AllowPostVoting")]
    public bool AllowPostVoting { get; set; }
    public bool AllowPostVoting_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.MaxVotesPerDay")]
    public int MaxVotesPerDay { get; set; }
    public bool MaxVotesPerDay_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.AllowCustomersToManageSubscriptions")]
    public bool AllowCustomersToManageSubscriptions { get; set; }
    public bool AllowCustomersToManageSubscriptions_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.TopicsPageSize")]
    public int TopicsPageSize { get; set; }
    public bool TopicsPageSize_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.PostsPageSize")]
    public int PostsPageSize { get; set; }
    public bool PostsPageSize_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.ForumEditor")]
    public int ForumEditor { get; set; }
    public bool ForumEditor_OverrideForStore { get; set; }
    public SelectList ForumEditorValues { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.SignaturesEnabled")]
    public bool SignaturesEnabled { get; set; }
    public bool SignaturesEnabled_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.ActiveDiscussionsFeedEnabled")]
    public bool ActiveDiscussionsFeedEnabled { get; set; }
    public bool ActiveDiscussionsFeedEnabled_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.ActiveDiscussionsFeedCount")]
    public int ActiveDiscussionsFeedCount { get; set; }
    public bool ActiveDiscussionsFeedCount_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.ForumFeedsEnabled")]
    public bool ForumFeedsEnabled { get; set; }
    public bool ForumFeedsEnabled_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.ForumFeedCount")]
    public int ForumFeedCount { get; set; }
    public bool ForumFeedCount_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.SearchResultsPageSize")]
    public int SearchResultsPageSize { get; set; }
    public bool SearchResultsPageSize_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.ActiveDiscussionsPageSize")]
    public int ActiveDiscussionsPageSize { get; set; }
    public bool ActiveDiscussionsPageSize_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Forums.Configuration.ShowCaptcha")]
    public bool ShowCaptcha { get; set; }
    public bool ShowCaptcha_OverrideForStore { get; set; }

    public bool HideCommonBlock { get; set; }
    public bool HidePermissionsBlock { get; set; }
    public bool HidePageSizesBlock { get; set; }
    public bool HideFeedsBlock { get; set; }

    #endregion
}