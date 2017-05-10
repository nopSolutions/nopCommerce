using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using System.Threading.Tasks;
using Nop.Core.Domain.News;

namespace Nop.Web.Components
{
    public class HomepageNewsViewComponent : ViewComponent
    {
        private readonly INewsModelFactory _newsModelFactory;
        private readonly NewsSettings _newsSettings;

        public HomepageNewsViewComponent(INewsModelFactory newsModelFactory, NewsSettings newsSettings)
        {
            this._newsModelFactory = newsModelFactory;
            this._newsSettings = newsSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_newsSettings.Enabled || !_newsSettings.ShowNewsOnMainPage)
                return Content("");

            var model = _newsModelFactory.PrepareHomePageNewsItemsModel();
            return View(model);
        }
    }
}
