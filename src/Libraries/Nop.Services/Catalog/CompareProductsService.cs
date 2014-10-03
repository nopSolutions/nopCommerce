using System;
using System.Collections.Generic;
using System.Web;
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
        private const string COMPARE_PRODUCTS_COOKIE_NAME = "nop.CompareProducts";

        #endregion
        
        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly IProductService _productService;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="productService">Product service</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public CompareProductsService(HttpContextBase httpContext, IProductService productService,
            CatalogSettings catalogSettings)
        {
            this._httpContext = httpContext;
            this._productService = productService;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a "compare products" identifier list
        /// </summary>
        /// <returns>"compare products" identifier list</returns>
        protected virtual List<int> GetComparedProductIds()
        {
            var productIds = new List<int>();
            HttpCookie compareCookie = _httpContext.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE_NAME);
            if (compareCookie == null)
                return productIds;
            string[] values = compareCookie.Values.GetValues("CompareProductIds");
            if (values == null)
                return productIds;
            foreach (string productId in values)
            {
                int prodId = int.Parse(productId);
                if (!productIds.Contains(prodId))
                    productIds.Add(prodId);
            }

            return productIds;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears a "compare products" list
        /// </summary>
        public virtual void ClearCompareProducts()
        {
            var compareCookie = _httpContext.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE_NAME);
            if (compareCookie != null)
            {
                compareCookie.Values.Clear();
                compareCookie.Expires = DateTime.Now.AddYears(-1);
                _httpContext.Response.Cookies.Set(compareCookie);
            }
        }

        /// <summary>
        /// Gets a "compare products" list
        /// </summary>
        /// <returns>"Compare products" list</returns>
        public virtual IList<Product> GetComparedProducts()
        {
            var products = new List<Product>();
            var productIds = GetComparedProductIds();
            foreach (int productId in productIds)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.Published && !product.Deleted)
                    products.Add(product);
            }
            return products;
        }

        /// <summary>
        /// Removes a product from a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public virtual void RemoveProductFromCompareList(int productId)
        {
            var oldProductIds = GetComparedProductIds();
            var newProductIds = new List<int>();
            newProductIds.AddRange(oldProductIds);
            newProductIds.Remove(productId);

            var compareCookie = _httpContext.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE_NAME);
            if (compareCookie == null)
                return;
            compareCookie.Values.Clear();
            foreach (int newProductId in newProductIds)
                compareCookie.Values.Add("CompareProductIds", newProductId.ToString());
            compareCookie.Expires = DateTime.Now.AddDays(10.0);
            _httpContext.Response.Cookies.Set(compareCookie);
        }

        /// <summary>
        /// Adds a product to a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public virtual void AddProductToCompareList(int productId)
        {
            var oldProductIds = GetComparedProductIds();
            var newProductIds = new List<int>();
            newProductIds.Add(productId);
            foreach (int oldProductId in oldProductIds)
                if (oldProductId != productId)
                    newProductIds.Add(oldProductId);

            var compareCookie = _httpContext.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE_NAME);
            if (compareCookie == null)
            {
                compareCookie = new HttpCookie(COMPARE_PRODUCTS_COOKIE_NAME);
                compareCookie.HttpOnly = true;
            }
            compareCookie.Values.Clear();
            int i = 1;
            foreach (int newProductId in newProductIds)
            {
                compareCookie.Values.Add("CompareProductIds", newProductId.ToString());
                if (i == _catalogSettings.CompareProductsNumber)
                    break;
                i++;
            }
            compareCookie.Expires = DateTime.Now.AddDays(10.0);
            _httpContext.Response.Cookies.Set(compareCookie);
        }

        #endregion
    }
}
