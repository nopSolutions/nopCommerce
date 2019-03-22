using Microsoft.AspNetCore.Mvc;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Services.Vendors;

namespace Nop.Web.Controllers
{
    public partial class BackwardCompatibility2XController : BasePublicController
    {
        #region Fields

        private readonly IBlogService _blogService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly INewsService _newsService;
        private readonly IProductTagService _productTagService;
        private readonly IProductService _productService;
        private readonly ITopicService _topicService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;

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
            _blogService = blogService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _newsService = newsService;
            _productTagService = productTagService;
            _productService = productService;
            _topicService = topicService;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
        }

        #endregion

        #region Methods

        //in versions 2.00-2.65 we had ID in product URLs
        public virtual IActionResult RedirectProductById(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Product", new { SeName = _urlRecordService.GetSeName(product) });
        }

        //in versions 2.00-2.65 we had ID in category URLs
        public virtual IActionResult RedirectCategoryById(int categoryId)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Category", new { SeName = _urlRecordService.GetSeName(category) });
        }

        //in versions 2.00-2.65 we had ID in manufacturer URLs
        public virtual IActionResult RedirectManufacturerById(int manufacturerId)
        {
            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Manufacturer", new { SeName = _urlRecordService.GetSeName(manufacturer) });
        }

        //in versions 2.00-2.70 we had ID in news URLs
        public virtual IActionResult RedirectNewsItemById(int newsItemId)
        {
            var newsItem = _newsService.GetNewsById(newsItemId);
            if (newsItem == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("NewsItem", new { SeName = _urlRecordService.GetSeName(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false) });
        }

        //in versions 2.00-2.70 we had ID in blog URLs
        public virtual IActionResult RedirectBlogPostById(int blogPostId)
        {
            var blogPost = _blogService.GetBlogPostById(blogPostId);
            if (blogPost == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("BlogPost", new { SeName = _urlRecordService.GetSeName(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false) });
        }

        //in versions 2.00-3.20 we had SystemName in topic URLs
        public virtual IActionResult RedirectTopicBySystemName(string systemName)
        {
            var topic = _topicService.GetTopicBySystemName(systemName);
            if (topic == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Topic", new { SeName = _urlRecordService.GetSeName(topic) });
        }

        //in versions 3.00-3.20 we had ID in vendor URLs
        public virtual IActionResult RedirectVendorById(int vendorId)
        {
            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("Vendor", new { SeName = _urlRecordService.GetSeName(vendor) });
        }

        //in versions 3.00-4.00 we had ID in product tag URLs
        public virtual IActionResult RedirectProductTagById(int productTagId)
        {
            var productTag = _productTagService.GetProductTagById(productTagId);
            if (productTag == null)
                return RedirectToRoutePermanent("Homepage");

            return RedirectToRoutePermanent("ProductsByTag", new { SeName = _urlRecordService.GetSeName(productTag) });
        }

        #endregion
    }
}