using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Services.Vendors;

namespace Nop.Web.Controllers
{
    //do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
    //they can create guest account(s), etc
    public partial class BackwardCompatibility2XController : Controller
    {
        #region Fields

        protected IBlogService BlogService { get; }
        protected ICategoryService CategoryService { get; }
        protected IManufacturerService ManufacturerService { get; }
        protected INewsService NewsService { get; }
        protected IProductTagService ProductTagService { get; }
        protected IProductService ProductService { get; }
        protected ITopicService TopicService { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IVendorService VendorService { get; }

        #endregion

        #region Ctor

        public BackwardCompatibility2XController(IBlogService blogService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            INewsService newsService,
            IProductTagService productTagService,
            IProductService productService,
            ITopicService topicService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService)
        {
            BlogService = blogService;
            CategoryService = categoryService;
            ManufacturerService = manufacturerService;
            NewsService = newsService;
            ProductTagService = productTagService;
            ProductService = productService;
            TopicService = topicService;
            UrlRecordService = urlRecordService;
            VendorService = vendorService;
        }

        #endregion

        #region Methods

        //in versions 2.00-2.65 we had ID in product URLs
        public virtual async Task<IActionResult> RedirectProductById(int productId)
        {
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Product", new { SeName = await UrlRecordService.GetSeNameAsync(product) });
        }

        //in versions 2.00-2.65 we had ID in category URLs
        public virtual async Task<IActionResult> RedirectCategoryById(int categoryId)
        {
            var category = await CategoryService.GetCategoryByIdAsync(categoryId);
            if (category == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Category", new { SeName = await UrlRecordService.GetSeNameAsync(category) });
        }

        //in versions 2.00-2.65 we had ID in manufacturer URLs
        public virtual async Task<IActionResult> RedirectManufacturerById(int manufacturerId)
        {
            var manufacturer = await ManufacturerService.GetManufacturerByIdAsync(manufacturerId);
            if (manufacturer == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Manufacturer", new { SeName = await UrlRecordService.GetSeNameAsync(manufacturer) });
        }

        //in versions 2.00-2.70 we had ID in news URLs
        public virtual async Task<IActionResult> RedirectNewsItemById(int newsItemId)
        {
            var newsItem = await NewsService.GetNewsByIdAsync(newsItemId);
            if (newsItem == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("NewsItem", new { SeName = await UrlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false) });
        }

        //in versions 2.00-2.70 we had ID in blog URLs
        public virtual async Task<IActionResult> RedirectBlogPostById(int blogPostId)
        {
            var blogPost = await BlogService.GetBlogPostByIdAsync(blogPostId);
            if (blogPost == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("BlogPost", new { SeName = await UrlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false) });
        }

        //in versions 2.00-3.20 we had SystemName in topic URLs
        public virtual async Task<IActionResult> RedirectTopicBySystemName(string systemName)
        {
            var topic = await TopicService.GetTopicBySystemNameAsync(systemName);
            if (topic == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Topic", new { SeName = await UrlRecordService.GetSeNameAsync(topic) });
        }

        //in versions 3.00-3.20 we had ID in vendor URLs
        public virtual async Task<IActionResult> RedirectVendorById(int vendorId)
        {
            var vendor = await VendorService.GetVendorByIdAsync(vendorId);
            if (vendor == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Vendor", new { SeName = await UrlRecordService.GetSeNameAsync(vendor) });
        }

        //in versions 3.00-4.00 we had ID in product tag URLs
        public virtual async Task<IActionResult> RedirectProductTagById(int productTagId)
        {
            var productTag = await ProductTagService.GetProductTagByIdAsync(productTagId);
            if (productTag == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("ProductsByTag", new { SeName = await UrlRecordService.GetSeNameAsync(productTag) });
        }

        #endregion
    }
}