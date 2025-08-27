using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.News.Public.Components;

/// <summary>
/// Represents a view component for render RSS link in the HTML header
/// </summary>
public class NewsRssHeaderLinkViewComponent : NopViewComponent
{
    #region Fields

    protected readonly NewsSettings _newsSettings;

    #endregion

    #region Ctor

    public NewsRssHeaderLinkViewComponent(NewsSettings newsSettings)
    {
        _newsSettings = newsSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke the widget view component
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public IViewComponentResult Invoke()
    {
        if (!_newsSettings.Enabled || !_newsSettings.ShowHeaderRssUrl)
            return Content("");

        return View("~/Plugins/Misc.News/Public/Views/Components/NewsRssHeaderLink.cshtml");
    }

    #endregion
}