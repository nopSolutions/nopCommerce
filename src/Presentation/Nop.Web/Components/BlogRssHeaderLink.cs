using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Nop.Core.Domain.Blogs;

namespace Nop.Web.Components
{
    public class BlogRssHeaderLinkViewComponent : ViewComponent
    {
        private readonly BlogSettings _blogSettings;

        public BlogRssHeaderLinkViewComponent(BlogSettings blogSettings)
        {
            this._blogSettings = blogSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync(int currentCategoryId, int currentProductId)
        {
            if (!_blogSettings.Enabled || !_blogSettings.ShowHeaderRssUrl)
                return Content("");

            return View();
        }
    }
}
