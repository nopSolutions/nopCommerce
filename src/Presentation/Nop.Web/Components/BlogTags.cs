using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class BlogTagsViewComponent : NopViewComponent
    {
        private readonly IBlogModelFactory _blogModelFactory;
        private readonly BlogSettings _blogSettings;

        public BlogTagsViewComponent(IBlogModelFactory blogModelFactory, BlogSettings blogSettings)
        {
            this._blogModelFactory = blogModelFactory;
            this._blogSettings = blogSettings;
        }

        public IViewComponentResult Invoke(int currentCategoryId, int currentProductId)
        {
            if (!_blogSettings.Enabled)
                return Content("");

            var model = _blogModelFactory.PrepareBlogPostTagListModel();
            return View(model);
        }
    }
}
