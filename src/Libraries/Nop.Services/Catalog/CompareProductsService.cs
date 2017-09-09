using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Compare products service
    /// </summary>
    public partial class CompareProductsService : ICompareProductsService
    {
        #region Constants

        /// <summary>
        /// Compare products cookie name
        /// </summary>
        private const string COMPARE_PRODUCTS_COOKIE_NAME = ".Nop.CompareProducts";

        #endregion

        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public CompareProductsService(CatalogSettings catalogSettings,
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
        /// Get a list of identifier of compared products
        /// </summary>
        /// <returns>List of identifier</returns>
        protected virtual List<int> GetComparedProductIds()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Request == null)
                return new List<int>();

            //try to get cookie
            if (!httpContext.Request.Cookies.TryGetValue(COMPARE_PRODUCTS_COOKIE_NAME, out string productIdsCookie) || string.IsNullOrEmpty(productIdsCookie))
                return new List<int>();

            //get array of string product identifiers from cookie
            var productIds = productIdsCookie.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //return list of int product identifiers
            return productIds.Select(productId => int.Parse(productId)).Distinct().ToList();
        }

        /// <summary>
        /// Add cookie value for the compared products
        /// </summary>
        /// <param name="comparedProductIds">Collection of compared products identifiers</param>
        protected virtual void AddCompareProductsCookie(IEnumerable<int> comparedProductIds)
        {
            //delete current cookie if exists
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(COMPARE_PRODUCTS_COOKIE_NAME);

            //create cookie value
            var comparedProductIdsCookie = string.Join(",", comparedProductIds);

            //create cookie options 
            var cookieExpires = 24 * 10; //TODO make configurable
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(cookieExpires),
                HttpOnly = true
            };

            //add cookie
            _httpContextAccessor.HttpContext.Response.Cookies.Append(COMPARE_PRODUCTS_COOKIE_NAME, comparedProductIdsCookie, cookieOptions);
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Clears a "compare products" list
        /// </summary>
        public virtual void ClearCompareProducts()
        {
            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.Response == null)
                return;

            //sets an expired cookie
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(COMPARE_PRODUCTS_COOKIE_NAME);
        }

        /// <summary>
        /// Gets a "compare products" list
        /// </summary>
        /// <returns>"Compare products" list</returns>
        public virtual IList<Product> GetComparedProducts()
        {
            //get list of compared product identifiers
            var productIds = GetComparedProductIds();

            //return list of product
            return _productService.GetProductsByIds(productIds.ToArray())
                .Where(product => product.Published && !product.Deleted).ToList();
        }

        /// <summary>
        /// Removes a product from a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public virtual void RemoveProductFromCompareList(int productId)
        {
            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.Response == null)
                return;

            //get list of compared product identifiers
            var comparedProductIds = GetComparedProductIds();

            //whether product identifier to remove exists
            if (!comparedProductIds.Contains(productId))
                return;

            //it exists, so remove it from list
            comparedProductIds.Remove(productId);

            //set cookie
            AddCompareProductsCookie(comparedProductIds);
        }

        /// <summary>
        /// Adds a product to a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public virtual void AddProductToCompareList(int productId)
        {
            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.Response == null)
                return;

            //get list of compared product identifiers
            var comparedProductIds = GetComparedProductIds();

            //whether product identifier to add already exist
            if (!comparedProductIds.Contains(productId))
                comparedProductIds.Insert(0, productId);

            //limit list based on the allowed number of products to be compared
            comparedProductIds = comparedProductIds.Take(_catalogSettings.CompareProductsNumber).ToList();

            //set cookie
            AddCompareProductsCookie(comparedProductIds);
        }

        #endregion
    }
}
