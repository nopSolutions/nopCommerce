using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.News.Public.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.News.Public.Components;

/// <summary>
/// Represents a view component for displaying news on the home page
/// </summary>
public class HomepageNewsViewComponent : NopViewComponent
{
    #region Fields

    protected readonly NewsModelFactory _newsModelFactory;
    protected readonly NewsSettings _newsSettings;

    #endregion

    #region Ctor

    public HomepageNewsViewComponent(NewsModelFactory newsModelFactory, NewsSettings newsSettings)
    {
        _newsModelFactory = newsModelFactory;
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
    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!_newsSettings.Enabled || !_newsSettings.ShowNewsOnMainPage)
            return Content("");

        var model = await _newsModelFactory.PrepareHomepageNewsItemsModelAsync();
        return View("~/Plugins/Misc.News/Public/Views/Components/HomepageNews.cshtml", model);
    }

    #endregion
}