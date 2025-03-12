using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.News.Public.Components;

public partial class NewsRssHeaderLinkViewComponent : NopViewComponent
{
    protected readonly NewsSettings _newsSettings;

    public NewsRssHeaderLinkViewComponent(NewsSettings newsSettings)
    {
        _newsSettings = newsSettings;
    }

    public IViewComponentResult Invoke(int currentCategoryId, int currentProductId)
    {
        if (!_newsSettings.Enabled || !_newsSettings.ShowHeaderRssUrl)
            return Content("");

        return View("~/Plugins/Misc.News/Public/Views/Components/NewsRssHeaderLink.cshtml");
    }
}