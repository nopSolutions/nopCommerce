using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Recently viewed products service
    /// </summary>
    public partial class RecentlyViewedProductsService : IRecentlyViewedProductsService
    {
        #region Constants

        /// <summary>
        /// Recently viewed products cookie name
        /// </summary>
        private const string RECENTLY_VIEWED_PRODUCTS_COOKIE_NAME = ".Nop.RecentlyViewedProducts";

        #endregion

        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public RecentlyViewedProductsService(CatalogSettings catalogSettings,
            IHttpContextAccessor httpContextAccessor,
            IProductService productService)
        {
            this._catalogSettings = catalogSettings;
            this._httpContextAccessor = httpContextAccessor;
            this._productService = productService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a list of identifier of recently viewed products
        /// </summary>
        /// <returns>List of identifier</returns>
        protected List<int> GetRecentlyViewedProductsIds()
        {
            return GetRecentlyViewedProductsIds(int.MaxValue);
        }

        /// <summary>
        /// Gets a list of identifier of recently viewed products
        /// </summary>
        /// <param name="number">Number of products to load</param>
        /// <returns>List of identifier</returns>
        protected List<int> GetRecentlyViewedProductsIds(int number)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Request == null)
                return new List<int>();

            //try to get cookie
            if (!httpContext.Request.Cookies.TryGetValue(RECENTLY_VIEWED_PRODUCTS_COOKIE_NAME, out string productIdsCookie) || string.IsNullOrEmpty(productIdsCookie))
                return new List<int>();

            //get array of string product identifiers from cookie
            var productIds = productIdsCookie.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //return list of int product identifiers
            return productIds.Select(productId => int.Parse(productId)).Distinct().Take(number).ToList();
        }

        /// <summary>
        /// Add cookie value for the recently viewed products
        /// </summary>
        /// <param name="recentlyViewedProductIds">Collection of the recently viewed products identifiers</param>
        protected virtual void AddRecentlyViewedProductsCookie(IEnumerable<int> recentlyViewedProductIds)
        {
            //delete current cookie if exists
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(RECENTLY_VIEWED_PRODUCTS_COOKIE_NAME);

            //create cookie value
            var productIdsCookie = string.Join(",", recentlyViewedProductIds);

            //create cookie options 
            var cookieExpires = 24 * 10; //TODO make configurable
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(cookieExpires),
                HttpOnly = true
            };

            //add cookie
            _httpContextAccessor.HttpContext.Response.Cookies.Append(RECENTLY_VIEWED_PRODUCTS_COOKIE_NAME, productIdsCookie, cookieOptions);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a "recently viewed products" list
        /// </summary>
        /// <param name="number">Number of products to load</param>
        /// <returns>"recently viewed products" list</returns>
        public virtual IList<Product> GetRecentlyViewedProducts(int number)
        {
            //get list of recently viewed product identifiers
            var productIds = GetRecentlyViewedProductsIds(number);

            //return list of product
            return _productService.GetProductsByIds(productIds.ToArray())
                .Where(product => product.Published && !product.Deleted).ToList();
        }

        /// <summary>
        /// Adds a product to a recently viewed products list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public virtual void AddProductToRecentlyViewedList(int productId)
        {
            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.Response == null)
                return;

            //whether recently viewed products is enabled
            if (!_catalogSettings.RecentlyViewedProductsEnabled)
                return;

            //get list of recently viewed product identifiers
            var productIds = GetRecentlyViewedProductsIds();

            //whether product identifier to add already exist
            if (!productIds.Contains(productId))
                productIds.Insert(0, productId);

            //limit list based on the allowed number of the recently viewed products
            productIds = productIds.Take(_catalogSettings.RecentlyViewedProductsNumber).ToList();

            //set cookie
            AddRecentlyViewedProductsCookie(productIds);
        }

        #endregion
    }
}
