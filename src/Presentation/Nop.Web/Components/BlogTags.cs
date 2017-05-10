using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using System.Threading.Tasks;
using Nop.Core.Domain.Blogs;

namespace Nop.Web.Components
{
    public class BlogTagsViewComponent : ViewComponent
    {
        private readonly IBlogModelFactory _blogModelFactory;
        private readonly BlogSettings _blogSettings;

        public BlogTagsViewComponent(IBlogModelFactory blogModelFactory, BlogSettings blogSettings)
        {
            this._blogModelFactory = blogModelFactory;
            this._blogSettings = blogSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync(int currentCategoryId, int currentProductId)
        {
            if (!_blogSettings.Enabled)
                return Content("");

            var model = _blogModelFactory.PrepareBlogPostTagListModel();
            return View(model);
        }
    }
}
