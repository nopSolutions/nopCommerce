﻿using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.News;

/// <summary>
/// News settings
/// </summary>
public class NewsSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether news are enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether not registered user can leave comments
    /// </summary>
    public bool AllowNotRegisteredUsersToLeaveComments { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to notify about new news comments
    /// </summary>
    public bool NotifyAboutNewNewsComments { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show news on the main page
    /// </summary>
    public bool ShowNewsOnMainPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating news count displayed on the main page
    /// </summary>
    public int MainPageNewsCount { get; set; }

    /// <summary>
    /// Gets or sets the page size for news archive
    /// </summary>
    public int NewsArchivePageSize { get; set; }

    /// <summary>
    /// Enable the news RSS feed link in customers browser address bar
    /// </summary>
    public bool ShowHeaderRssUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether news comments must be approved
    /// </summary>
    public bool NewsCommentsMustBeApproved { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether news comments will be filtered per store
    /// </summary>
    public bool ShowNewsCommentsPerStore { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include news to sitemap.xml
    /// </summary>
    public bool SitemapXmlIncludeNews { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include news to sitemap
    /// </summary>
    public bool SitemapIncludeNews { get; set; }

    ///// <summary>
    ///// Gets or sets a value indicating whether to display "news" footer item
    ///// </summary>
    //public bool DisplayNewsFooterItem { get; set; } 

    /// <summary>
    /// A value indicating whether CAPTCHA should be displayed on the "comment news" page
    /// </summary>
    public bool ShowCaptchaOnNewsCommentPage { get; set; }
}