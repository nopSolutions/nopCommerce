using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.News;
using Nop.Services.Catalog;
using Nop.Services.Topics;

namespace Nop.Services.Seo
{
    /// <summary>
    /// Represents a sitemap generator
    /// </summary>
    public partial class SitemapGenerator : BaseSitemapGenerator, ISitemapGenerator
    {
        private readonly IStoreContext _storeContext;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ITopicService _topicService;
        private readonly CommonSettings _commonSettings;
        private readonly BlogSettings _blogSettings;
        private readonly NewsSettings _newsSettings;
        private readonly ForumSettings _forumSettings;

        public SitemapGenerator(IStoreContext storeContext,
            ICategoryService categoryService,
            IProductService productService, 
            IManufacturerService manufacturerService, 
            ITopicService topicService,
            CommonSettings commonSettings,
            BlogSettings blogSettings,
            NewsSettings newsSettings,
            ForumSettings forumSettings)
        {
            this._storeContext = storeContext;
            this._categoryService = categoryService;
            this._productService = productService;
            this._manufacturerService = manufacturerService;
            this._topicService = topicService;
            this._commonSettings = commonSettings;
            this._blogSettings = blogSettings;
            this._newsSettings = newsSettings;
            this._forumSettings = forumSettings;
        }

        /// <summary>
        /// Method that is overridden, that handles creation of child urls.
        /// Use the method WriteUrlLocation() within this method.
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        protected override void GenerateUrlNodes(UrlHelper urlHelper)
        {
            //home page
            var homePageUrl = urlHelper.RouteUrl("HomePage", null, "http");
            WriteUrlLocation(homePageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);
            //search products
            var productSearchUrl = urlHelper.RouteUrl("ProductSearch", null, "http");
            WriteUrlLocation(productSearchUrl, UpdateFrequency.Weekly, DateTime.UtcNow);
            //contact us
            var contactUsUrl = urlHelper.RouteUrl("ContactUs", null, "http");
            WriteUrlLocation(contactUsUrl, UpdateFrequency.Weekly, DateTime.UtcNow);
            //news
            if (_newsSettings.Enabled)
            {
                var url = urlHelper.RouteUrl("NewsArchive", null, "http");
                WriteUrlLocation(url, UpdateFrequency.Weekly, DateTime.UtcNow);
            }
            //blog
            if (_blogSettings.Enabled)
            {
                var url = urlHelper.RouteUrl("Blog", null, "http");
                WriteUrlLocation(url, UpdateFrequency.Weekly, DateTime.UtcNow);
            }
            //blog
            if (_forumSettings.ForumsEnabled)
            {
                var url = urlHelper.RouteUrl("Boards", null, "http");
                WriteUrlLocation(url, UpdateFrequency.Weekly, DateTime.UtcNow);
            }
            //categories
            if (_commonSettings.SitemapIncludeCategories)
            {
                WriteCategories(urlHelper, 0);
            }
            //manufacturers
            if (_commonSettings.SitemapIncludeManufacturers)
            {
                WriteManufacturers(urlHelper);
            }
            //products
            if (_commonSettings.SitemapIncludeProducts)
            {
                WriteProducts(urlHelper);
            }
            //topics
            WriteTopics(urlHelper);
        }

        private void WriteCategories(UrlHelper urlHelper, int parentCategoryId)
        {
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
            foreach (var category in categories)
            {
                var url = urlHelper.RouteUrl("Category", new { SeName = category.GetSeName() }, "http");
                WriteUrlLocation(url, UpdateFrequency.Weekly, category.UpdatedOnUtc);

                WriteCategories(urlHelper, category.Id);
            }
        }

        private void WriteManufacturers(UrlHelper urlHelper)
        {
            var manufacturers = _manufacturerService.GetAllManufacturers();
            foreach (var manufacturer in manufacturers)
            {
                var url = urlHelper.RouteUrl("Manufacturer", new { SeName = manufacturer.GetSeName() }, "http");
                WriteUrlLocation(url, UpdateFrequency.Weekly, manufacturer.UpdatedOnUtc);
            }
        }

        private void WriteProducts(UrlHelper urlHelper)
        {
            var products = _productService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                orderBy: ProductSortingEnum.CreatedOn);
            foreach (var product in products)
            {
                var url = urlHelper.RouteUrl("Product", new { SeName = product.GetSeName() }, "http");
                WriteUrlLocation(url, UpdateFrequency.Weekly, product.UpdatedOnUtc);
            }
        }

        private void WriteTopics(UrlHelper urlHelper)
        {
            var topics = _topicService.GetAllTopics(_storeContext.CurrentStore.Id)
                .Where(t => t.IncludeInSitemap)
                .ToList();
            foreach (var topic in topics)
            {
                var url = urlHelper.RouteUrl("Topic", new { SeName = topic.GetSeName() }, "http");
                WriteUrlLocation(url, UpdateFrequency.Weekly, DateTime.UtcNow);
            }
        }
    }
}
