using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Nop.Core.Domain.News;

namespace Nop.Web.Components
{
    public class NewsRssHeaderLinkViewComponent : ViewComponent
    {
        private readonly NewsSettings _newsSettings;

        public NewsRssHeaderLinkViewComponent(NewsSettings newsSettings)
        {
            this._newsSettings = newsSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync(int currentCategoryId, int currentProductId)
        {
            if (!_newsSettings.Enabled || !_newsSettings.ShowHeaderRssUrl)
                return Content("");

            return View();
        }
    }
}
