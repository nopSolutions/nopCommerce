using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
namespace Nop.Web.Components
{
    public partial class HomepageBlogPostsViewComponent : NopViewComponent{
        protected readonly IBlogModelFactory _blogModelFactory;
        protected readonly BlogSettings _blogSettings;
        public HomepageBlogPostsViewComponent(IBlogModelFactory blogModelFactory, BlogSettings blogSetting){
            _blogModelFactory = blogModelFactory;
            _blogSettings = blogSetting;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_blogSettings.Enabled || !_blogSettings.ShowBlogOnMainPage)
                return Content("");

            var model = await _blogModelFactory.PrepareHomepageBlogPostItemsModelAsync();
            return View(model);
        }
    }
}