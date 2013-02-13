using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
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
        private readonly IWebHelper _webHelper;

        public SitemapGenerator(IStoreContext storeContext,
            ICategoryService categoryService, IProductService productService, 
            IManufacturerService manufacturerService, ITopicService topicService, 
            CommonSettings commonSettings, IWebHelper webHelper)
        {
            this._storeContext = storeContext;
            this._categoryService = categoryService;
            this._productService = productService;
            this._manufacturerService = manufacturerService;
            this._topicService = topicService;
            this._commonSettings = commonSettings;
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Method that is overridden, that handles creation of child urls.
        /// Use the method WriteUrlLocation() within this method.
        /// </summary>
        protected override void GenerateUrlNodes()
        {
            if (_commonSettings.SitemapIncludeCategories)
            {
                WriteCategories(0);
            }

            if (_commonSettings.SitemapIncludeManufacturers)
            {
                WriteManufacturers();
            }

            if (_commonSettings.SitemapIncludeProducts)
            {
                WriteProducts();
            }

            if (_commonSettings.SitemapIncludeTopics)
            {
                WriteTopics();
            }
        }

        private void WriteCategories(int parentCategoryId)
        {
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
            foreach (var category in categories)
            {
                //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                var url = string.Format("{0}{1}", _webHelper.GetStoreLocation(false), category.GetSeName());
                var updateFrequency = UpdateFrequency.Weekly;
                var updateTime = category.UpdatedOnUtc;
                WriteUrlLocation(url, updateFrequency, updateTime);

                WriteCategories(category.Id);
            }
        }

        private void WriteManufacturers()
        {
            var manufacturers = _manufacturerService.GetAllManufacturers(false);
            foreach (var manufacturer in manufacturers)
            {
                //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                var url = string.Format("{0}{1}", _webHelper.GetStoreLocation(false),manufacturer.GetSeName());
                var updateFrequency = UpdateFrequency.Weekly;
                var updateTime = manufacturer.UpdatedOnUtc;
                WriteUrlLocation(url, updateFrequency, updateTime);
            }
        }

        private void WriteProducts()
        {
            var products = _productService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,
                orderBy: ProductSortingEnum.CreatedOn);
            foreach (var product in products)
            {
                //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                var url = string.Format("{0}{1}", _webHelper.GetStoreLocation(false), product.GetSeName());
                var updateFrequency = UpdateFrequency.Weekly;
                var updateTime = product.UpdatedOnUtc;
                WriteUrlLocation(url, updateFrequency, updateTime);
            }
        }

        private void WriteTopics()
        {
            var topics = _topicService.GetAllTopics(_storeContext.CurrentStore.Id).ToList().FindAll(t => t.IncludeInSitemap);
            foreach (var topic in topics)
            {
                //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                var url = string.Format("{0}t/{1}", _webHelper.GetStoreLocation(false), topic.SystemName.ToLowerInvariant());
                var updateFrequency = UpdateFrequency.Weekly;
                var updateTime = DateTime.UtcNow;
                WriteUrlLocation(url, updateFrequency, updateTime);
            }
        }
    }
}
