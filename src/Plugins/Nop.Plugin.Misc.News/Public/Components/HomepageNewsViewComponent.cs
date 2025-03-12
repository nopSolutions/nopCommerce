using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.News.Public.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.News.Public.Components;

public partial class HomepageNewsViewComponent : NopViewComponent
{
    protected readonly NewsModelFactory _newsModelFactory;
    protected readonly NewsSettings _newsSettings;

    public HomepageNewsViewComponent(NewsModelFactory newsModelFactory, NewsSettings newsSettings)
    {
        _newsModelFactory = newsModelFactory;
        _newsSettings = newsSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!_newsSettings.Enabled || !_newsSettings.ShowNewsOnMainPage)
            return Content("");

        var model = await _newsModelFactory.PrepareHomepageNewsItemsModelAsync();
        return View("~/Plugins/Misc.News/Public/Views/Components/HomepageNews.cshtml", model);
    }
}