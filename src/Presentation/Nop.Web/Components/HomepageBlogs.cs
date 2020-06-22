using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.News;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
namespace Nop.Web.Components
{
    public class HomepageBlogsViewComponent : NopViewComponent
    {
        private readonly IBlogModelFactory _blogModelFactory;
       private readonly BlogSettings _blogSettings;

        public HomepageBlogsViewComponent(IBlogModelFactory blogsModelFactory, BlogSettings blogSettings)
        {
            _blogModelFactory = blogsModelFactory;
            // _newsSettings = newsSettings;
            _blogSettings = blogSettings;
        }

        public IViewComponentResult Invoke()
        {

            if (_blogSettings.MainPageBlogPostsCount<1)
            return Content("");

            var model = _blogModelFactory.PrepareHomepageBlogPostsModel();
            return View(model);
        }
    }
}

/*


namespace Nop.Web.Components
{
    public class HomepageNewsViewComponent : NopViewComponent
    {
        private readonly INewsModelFactory _newsModelFactory;
        private readonly NewsSettings _newsSettings;

        public HomepageNewsViewComponent(INewsModelFactory newsModelFactory, NewsSettings newsSettings)
        {
            _newsModelFactory = newsModelFactory;
            _newsSettings = newsSettings;
        }

        public IViewComponentResult Invoke()
        {
            if (!_newsSettings.Enabled || !_newsSettings.ShowNewsOnMainPage)
                return Content("");

            var model = _newsModelFactory.PrepareHomepageNewsItemsModel();
            return View(model);
        }
    }
}

     
     
     */
