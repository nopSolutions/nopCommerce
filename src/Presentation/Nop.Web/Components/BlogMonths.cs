using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Web.Factories;

namespace Nop.Web.Components
{
    public class BlogMonthsViewComponent : ViewComponent
    {
        private readonly IBlogModelFactory _blogModelFactory;
        private readonly BlogSettings _blogSettings;

        public BlogMonthsViewComponent(IBlogModelFactory blogModelFactory, BlogSettings blogSettings)
        {
            this._blogModelFactory = blogModelFactory;
            this._blogSettings = blogSettings;
        }

        public IViewComponentResult Invoke(int currentCategoryId, int currentProductId)
        {
            if (!_blogSettings.Enabled)
                return Content("");

            var model = _blogModelFactory.PrepareBlogPostYearModel();
            return View(model);
        }
    }
}
