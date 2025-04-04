using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.News.Public.Components;

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

    public IViewComponentResult Invoke()
    {
        if (!_newsSettings.Enabled || !_newsSettings.ShowHeaderRssUrl)
            return Content("");

        return View("~/Plugins/Misc.News/Public/Views/Components/NewsRssHeaderLink.cshtml");
    }

    #endregion
}