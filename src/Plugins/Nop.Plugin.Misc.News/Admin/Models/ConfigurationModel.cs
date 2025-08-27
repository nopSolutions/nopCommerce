using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.News.Admin.Models;

/// <summary>
/// Represents a configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel, ISettingsModel
{
    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Configuration.Enabled")]
    public bool Enabled { get; set; }
    public bool Enabled_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Configuration.AllowNotRegisteredUsersToLeaveComments")]
    public bool AllowNotRegisteredUsersToLeaveComments { get; set; }
    public bool AllowNotRegisteredUsersToLeaveComments_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Configuration.NotifyAboutNewNewsComments")]
    public bool NotifyAboutNewNewsComments { get; set; }
    public bool NotifyAboutNewNewsComments_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Configuration.ShowNewsOnMainPage")]
    public bool ShowNewsOnMainPage { get; set; }
    public bool ShowNewsOnMainPage_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Configuration.MainPageNewsCount")]
    public int MainPageNewsCount { get; set; }
    public bool MainPageNewsCount_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Configuration.NewsArchivePageSize")]
    public int NewsArchivePageSize { get; set; }
    public bool NewsArchivePageSize_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Configuration.ShowHeaderRSSUrl")]
    public bool ShowHeaderRssUrl { get; set; }
    public bool ShowHeaderRssUrl_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Configuration.NewsCommentsMustBeApproved")]
    public bool NewsCommentsMustBeApproved { get; set; }
    public bool NewsCommentsMustBeApproved_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Configuration.ShowNewsCommentsPerStore")]
    public bool ShowNewsCommentsPerStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Configuration.SitemapIncludeNews")]
    public bool SitemapIncludeNews { get; set; }
    public bool SitemapIncludeNews_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Configuration.SitemapXmlIncludeNews")]
    public bool SitemapXmlIncludeNews { get; set; }
    public bool SitemapXmlIncludeNews_OverrideForStore { get; set; }

    //[NopResourceDisplayName("Plugins.Misc.News.Configuration.DisplayNewsFooterItem")]
    //public bool DisplayNewsFooterItem { get; set; }
    //public bool DisplayNewsFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.News.Configuration.ShowCaptchaOnNewsCommentPage")]
    public bool ShowCaptchaOnNewsCommentPage { get; set; }
    public bool ShowCaptchaOnNewsCommentPage_OverrideForStore { get; set; }

    #endregion
}