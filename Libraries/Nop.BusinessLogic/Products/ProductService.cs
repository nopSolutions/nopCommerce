//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Utils.Html;
using System.Data.Objects;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.BusinessLogic.Products
{
    /// <summary>
    /// Product service
    /// </summary>
    public partial class ProductService : IProductService
    {
        #region Constants
        private const string PRODUCTS_BY_ID_KEY = "Nop.product.id-{0}";
        private const string PRODUCTVARIANTS_ALL_KEY = "Nop.productvariant.all-{0}-{1}";
        private const string PRODUCTVARIANTS_BY_ID_KEY = "Nop.productvariant.id-{0}";
        private const string TIERPRICES_ALLBYPRODUCTVARIANTID_KEY = "Nop.tierprice.allbyproductvariantid-{0}";
        private const string CUSTOMERROLEPRICES_ALL_KEY = "Nop.customerroleproductprice.all-{0}";
        private const string PRODUCTS_PATTERN_KEY = "Nop.product.";
        private const string PRODUCTVARIANTS_PATTERN_KEY = "Nop.productvariant.";
        private const string TIERPRICES_PATTERN_KEY = "Nop.tierprice.";
        private const string CUSTOMERROLEPRICES_PATTERN_KEY = "Nop.customerroleproductprice.";
        #endregion

        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        protected readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        protected readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public ProductService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods

        #region Products

        /// <summary>
        /// Marks a product as deleted
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public void MarkProductAsDeleted(int productId)
        {
            if (productId == 0)
                return;

            var product = GetProductById(productId);
            if (product != null)
            {
                product.Deleted = true;
                UpdateProduct(product);

                foreach (var productVariant in product.ProductVariants)
                    MarkProductVariantAsDeleted(productVariant.ProductVariantId);
            }
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProducts()
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetAllProducts(showHidden);
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProducts(bool showHidden)
        {
            
            var query = from p in _context.Products
                        orderby p.Name
                        where (showHidden || p.Published) &&
                        !p.Deleted
                        select p;
            var products = query.ToList();
            return products;
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProducts(int pageSize, int pageIndex, 
            out int totalRecords)
        {
            return GetAllProducts(0, 0, 0, null, null, null,
                string.Empty, false, pageSize, pageIndex, null,
                ProductSortingEnum.Position, out totalRecords);
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="productTagId">Product tag identifier</param>
        /// <param name="featuredProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProducts(int categoryId, 
            int manufacturerId, int productTagId, bool? featuredProducts, 
            int pageSize, int pageIndex, out int totalRecords)
        {
            return GetAllProducts(categoryId, manufacturerId,
                productTagId, featuredProducts, null, null,
                string.Empty, false, pageSize, pageIndex, null,
                ProductSortingEnum.Position, out totalRecords);
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search in descriptions</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProducts(string keywords, 
            bool searchDescriptions, int pageSize, int pageIndex, out int totalRecords)
        {
            return GetAllProducts(0, 0, 0, null, null, null,
                keywords, searchDescriptions, pageSize, pageIndex, null,
                ProductSortingEnum.Position, out totalRecords);
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="productTagId">Product tag identifier</param>
        /// <param name="featuredProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search in descriptions</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="filteredSpecs">Filtered product specification identifiers</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProducts(int categoryId,
            int manufacturerId, int productTagId, bool? featuredProducts, 
            string keywords, bool searchDescriptions, int pageSize,
            int pageIndex, List<int> filteredSpecs, out int totalRecords)
        {
            return GetAllProducts(categoryId, manufacturerId,
                productTagId, featuredProducts, null, null,
                keywords, searchDescriptions, pageSize, pageIndex,
                filteredSpecs, ProductSortingEnum.Position, out totalRecords);
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="productTagId">Product tag identifier</param>
        /// <param name="featuredProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
        /// <param name="priceMin">Minimum price</param>
        /// <param name="priceMax">Maximum price</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="filteredSpecs">Filtered product specification identifiers</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProducts(int categoryId,
            int manufacturerId, int productTagId, bool? featuredProducts, 
            decimal? priceMin, decimal? priceMax, int pageSize, 
            int pageIndex, List<int> filteredSpecs, out int totalRecords)
        {
            return GetAllProducts(categoryId, manufacturerId,
                productTagId, featuredProducts, 
                priceMin, priceMax, string.Empty, false, pageSize, pageIndex, 
                filteredSpecs, ProductSortingEnum.Position, out totalRecords);
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="productTagId">Product tag identifier</param>
        /// <param name="featuredProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
        /// <param name="priceMin">Minimum price</param>
        /// <param name="priceMax">Maximum price</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search in descriptions</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="filteredSpecs">Filtered product specification identifiers</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProducts(int categoryId,
            int manufacturerId, int productTagId, bool? featuredProducts,
            decimal? priceMin, decimal? priceMax, string keywords, 
            bool searchDescriptions, int pageSize, int pageIndex,
            List<int> filteredSpecs, out int totalRecords)
        {
            return GetAllProducts(categoryId, manufacturerId,
                productTagId, featuredProducts, priceMin, 
                priceMax, keywords, searchDescriptions,
                pageSize, pageIndex, filteredSpecs,
                ProductSortingEnum.Position, out totalRecords);
        }
        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="productTagId">Product tag identifier</param>
        /// <param name="featuredProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
        /// <param name="priceMin">Minimum price</param>
        /// <param name="priceMax">Maximum price</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search in descriptions</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="filteredSpecs">Filtered product specification identifiers</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProducts(int categoryId,
            int manufacturerId, int productTagId, bool? featuredProducts,
            decimal? priceMin, decimal? priceMax, string keywords,
            bool searchDescriptions, int pageSize, int pageIndex,
            List<int> filteredSpecs, ProductSortingEnum orderBy, out int totalRecords)
        {
            int languageId = NopContext.Current.WorkingLanguage.LanguageId;

            return GetAllProducts(categoryId, manufacturerId, productTagId,
                featuredProducts, priceMin, priceMax, keywords, searchDescriptions,
                pageSize, pageIndex, filteredSpecs, languageId,
                orderBy, out totalRecords);
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="productTagId">Product tag identifier</param>
        /// <param name="featuredProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
        /// <param name="priceMin">Minimum price</param>
        /// <param name="priceMax">Maximum price</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search in descriptions</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="filteredSpecs">Filtered product specification identifiers</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProducts(int categoryId,
            int manufacturerId, int productTagId, bool? featuredProducts,
            decimal? priceMin, decimal? priceMax, 
            string keywords, bool searchDescriptions, int pageSize,
            int pageIndex, List<int> filteredSpecs, int languageId,
            ProductSortingEnum orderBy, out int totalRecords)
        {
            return GetAllProducts(categoryId,
                manufacturerId, productTagId, featuredProducts,
                priceMin, priceMax, 0,
                keywords, searchDescriptions, pageSize,
                pageIndex, filteredSpecs, languageId,
                orderBy, out totalRecords);
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="categoryId">Category identifier; 0 to load all recordss</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all recordss</param>
        /// <param name="productTagId">Product tag identifier; 0 to load all recordss</param>
        /// <param name="featuredProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
        /// <param name="priceMin">Minimum price</param>
        /// <param name="priceMax">Maximum price</param>
        /// <param name="relatedToProductId">Filter by related product; 0 to load all recordss</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search in descriptions</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="filteredSpecs">Filtered product specification identifiers</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProducts(int categoryId,
            int manufacturerId, int productTagId, bool? featuredProducts,
            decimal? priceMin, decimal? priceMax,
            int relatedToProductId, string keywords, bool searchDescriptions, int pageSize,
            int pageIndex, List<int> filteredSpecs, int languageId,
            ProductSortingEnum orderBy, out int totalRecords)
        {
            if (pageSize <= 0)
                pageSize = 10;
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            if (pageIndex < 0)
                pageIndex = 0;
            if (pageIndex == int.MaxValue)
                pageIndex = int.MaxValue - 1;

            bool showHidden = NopContext.Current.IsAdmin;

            string commaSeparatedSpecIds = string.Empty;
            if (filteredSpecs != null)
            {
                filteredSpecs.Sort();
                for (int i = 0; i < filteredSpecs.Count; i++)
                {
                    commaSeparatedSpecIds += filteredSpecs[i].ToString();
                    if (i != filteredSpecs.Count - 1)
                    {
                        commaSeparatedSpecIds += ",";
                    }
                }
            }

            
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));
            var products = _context.Sp_ProductLoadAllPaged(categoryId,
               manufacturerId, productTagId, featuredProducts,
               priceMin, priceMax, relatedToProductId,
               keywords, searchDescriptions, showHidden, pageIndex, pageSize, commaSeparatedSpecIds,
               languageId, (int)orderBy, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);

            return products;
        }

        /// <summary>
        /// Gets all products displayed on the home page
        /// </summary>
        /// <returns>Product collection</returns>
        public List<Product> GetAllProductsDisplayedOnHomePage()
        {
            bool showHidden = NopContext.Current.IsAdmin;
            int languageId = NopContext.Current.WorkingLanguage.LanguageId;
                        
            var query = from p in _context.Products
                        orderby p.Name
                        where (showHidden || p.Published) &&
                        !p.Deleted &&
                        p.ShowOnHomePage
                        select p;
            var products = query.ToList();
            return products;
        }
        
        /// <summary>
        /// Gets product
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product</returns>
        public Product GetProductById(int productId)
        {
            if (productId == 0)
                return null;

            string key = string.Format(PRODUCTS_BY_ID_KEY, productId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (Product)obj2;
            }

            
            var query = from p in _context.Products
                        where p.ProductId == productId
                        select p;
            var product = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, product);
            }
            return product;
        }
        
        /// <summary>
        /// Inserts a product
        /// </summary>
        /// <param name="product">Product</param>
        public void InsertProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");
            
            product.Name = CommonHelper.EnsureNotNull(product.Name);
            product.Name = CommonHelper.EnsureMaximumLength(product.Name, 400);
            product.ShortDescription = CommonHelper.EnsureNotNull(product.ShortDescription);
            product.FullDescription = CommonHelper.EnsureNotNull(product.FullDescription);
            product.AdminComment = CommonHelper.EnsureNotNull(product.AdminComment);
            product.MetaKeywords = CommonHelper.EnsureNotNull(product.MetaKeywords);
            product.MetaKeywords = CommonHelper.EnsureMaximumLength(product.MetaKeywords, 400);
            product.MetaDescription = CommonHelper.EnsureNotNull(product.MetaDescription);
            product.MetaDescription = CommonHelper.EnsureMaximumLength(product.MetaDescription, 4000);
            product.MetaTitle = CommonHelper.EnsureNotNull(product.MetaTitle);
            product.MetaTitle = CommonHelper.EnsureMaximumLength(product.MetaTitle, 400);
            product.SEName = CommonHelper.EnsureNotNull(product.SEName);
            product.SEName = CommonHelper.EnsureMaximumLength(product.SEName, 100);

            
            
            _context.Products.AddObject(product);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                _cacheManager.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
            
            //raise event             
            EventContext.Current.OnProductCreated(null,
                new ProductEventArgs() { Product = product });
        }

        /// <summary>
        /// Updates the product
        /// </summary>
        /// <param name="product">Product</param>
        public void UpdateProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            product.Name = CommonHelper.EnsureNotNull(product.Name);
            product.Name = CommonHelper.EnsureMaximumLength(product.Name, 400);
            product.ShortDescription = CommonHelper.EnsureNotNull(product.ShortDescription);
            product.FullDescription = CommonHelper.EnsureNotNull(product.FullDescription);
            product.AdminComment = CommonHelper.EnsureNotNull(product.AdminComment);
            product.MetaKeywords = CommonHelper.EnsureNotNull(product.MetaKeywords);
            product.MetaKeywords = CommonHelper.EnsureMaximumLength(product.MetaKeywords, 400);
            product.MetaDescription = CommonHelper.EnsureNotNull(product.MetaDescription);
            product.MetaDescription = CommonHelper.EnsureMaximumLength(product.MetaDescription, 4000);
            product.MetaTitle = CommonHelper.EnsureNotNull(product.MetaTitle);
            product.MetaTitle = CommonHelper.EnsureMaximumLength(product.MetaTitle, 400);
            product.SEName = CommonHelper.EnsureNotNull(product.SEName);
            product.SEName = CommonHelper.EnsureMaximumLength(product.SEName, 100);

            
            if (!_context.IsAttached(product))
                _context.Products.Attach(product);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                _cacheManager.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }

            //raise event             
            EventContext.Current.OnProductUpdated(null,
                new ProductEventArgs() { Product = product });
        }

        /// <summary>
        /// Gets localized product by id
        /// </summary>
        /// <param name="productLocalizedId">Localized product identifier</param>
        /// <returns>Product content</returns>
        public ProductLocalized GetProductLocalizedById(int productLocalizedId)
        {
            if (productLocalizedId == 0)
                return null;

            
            var query = from pl in _context.ProductLocalized
                        where pl.ProductLocalizedId == productLocalizedId
                        select pl;
            var productLocalized = query.SingleOrDefault();
            return productLocalized;
        }

        /// <summary>
        /// Gets localized product by product id
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product content</returns>
        public List<ProductLocalized> GetProductLocalizedByProductId(int productId)
        {
            if (productId == 0)
                return new List<ProductLocalized>();

            
            var query = from pl in _context.ProductLocalized
                        where pl.ProductId == productId
                        select pl;
            var content = query.ToList();
            return content;
        }

        /// <summary>
        /// Gets localized product by product id and language id
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Product content</returns>
        public ProductLocalized GetProductLocalizedByProductIdAndLanguageId(int productId, int languageId)
        {
            if (productId == 0 || languageId == 0)
                return null;

            
            var query = from pl in _context.ProductLocalized
                        orderby pl.ProductLocalizedId
                        where pl.ProductId == productId &&
                        pl.LanguageId == languageId
                        select pl;
            var productLocalized = query.FirstOrDefault();
            return productLocalized;
        }

        /// <summary>
        /// Inserts a localized product
        /// </summary>
        /// <param name="productLocalized">Product content</param>
        public void InsertProductLocalized(ProductLocalized productLocalized)
        {
            if (productLocalized == null)
                throw new ArgumentNullException("productLocalized");
            
            productLocalized.Name = CommonHelper.EnsureNotNull(productLocalized.Name);
            productLocalized.Name = CommonHelper.EnsureMaximumLength(productLocalized.Name, 400);
            productLocalized.ShortDescription = CommonHelper.EnsureNotNull(productLocalized.ShortDescription);
            productLocalized.FullDescription = CommonHelper.EnsureNotNull(productLocalized.FullDescription);
            productLocalized.MetaKeywords = CommonHelper.EnsureNotNull(productLocalized.MetaKeywords);
            productLocalized.MetaKeywords = CommonHelper.EnsureMaximumLength(productLocalized.MetaKeywords, 400);
            productLocalized.MetaDescription = CommonHelper.EnsureNotNull(productLocalized.MetaDescription);
            productLocalized.MetaDescription = CommonHelper.EnsureMaximumLength(productLocalized.MetaDescription, 4000);
            productLocalized.MetaTitle = CommonHelper.EnsureNotNull(productLocalized.MetaTitle);
            productLocalized.MetaTitle = CommonHelper.EnsureMaximumLength(productLocalized.MetaTitle, 400);
            productLocalized.SEName = CommonHelper.EnsureNotNull(productLocalized.SEName);
            productLocalized.SEName = CommonHelper.EnsureMaximumLength(productLocalized.SEName, 100);

            

            _context.ProductLocalized.AddObject(productLocalized);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                _cacheManager.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Update a localized product
        /// </summary>
        /// <param name="productLocalized">Product content</param>
        public void UpdateProductLocalized(ProductLocalized productLocalized)
        {
            if (productLocalized == null)
                throw new ArgumentNullException("productLocalized");

            productLocalized.Name = CommonHelper.EnsureNotNull(productLocalized.Name);
            productLocalized.Name = CommonHelper.EnsureMaximumLength(productLocalized.Name, 400);
            productLocalized.ShortDescription = CommonHelper.EnsureNotNull(productLocalized.ShortDescription);
            productLocalized.FullDescription = CommonHelper.EnsureNotNull(productLocalized.FullDescription);
            productLocalized.MetaKeywords = CommonHelper.EnsureNotNull(productLocalized.MetaKeywords);
            productLocalized.MetaKeywords = CommonHelper.EnsureMaximumLength(productLocalized.MetaKeywords, 400);
            productLocalized.MetaDescription = CommonHelper.EnsureNotNull(productLocalized.MetaDescription);
            productLocalized.MetaDescription = CommonHelper.EnsureMaximumLength(productLocalized.MetaDescription, 4000);
            productLocalized.MetaTitle = CommonHelper.EnsureNotNull(productLocalized.MetaTitle);
            productLocalized.MetaTitle = CommonHelper.EnsureMaximumLength(productLocalized.MetaTitle, 400);
            productLocalized.SEName = CommonHelper.EnsureNotNull(productLocalized.SEName);
            productLocalized.SEName = CommonHelper.EnsureMaximumLength(productLocalized.SEName, 100);

            bool allFieldsAreEmpty = string.IsNullOrEmpty(productLocalized.Name) &&
               string.IsNullOrEmpty(productLocalized.ShortDescription) &&
               string.IsNullOrEmpty(productLocalized.FullDescription) &&
               string.IsNullOrEmpty(productLocalized.MetaKeywords) &&
               string.IsNullOrEmpty(productLocalized.MetaDescription) &&
               string.IsNullOrEmpty(productLocalized.MetaTitle) &&
               string.IsNullOrEmpty(productLocalized.SEName);

            
            if (!_context.IsAttached(productLocalized))
                _context.ProductLocalized.Attach(productLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                _context.DeleteObject(productLocalized);
                _context.SaveChanges();
            }
            else
            {
                _context.SaveChanges();
            }

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                _cacheManager.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a list of products purchased by other customers who purchased the above
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product collection</returns>
        public List<Product> GetProductsAlsoPurchasedById(int productId)
        {
            int totalRecords = 0;
            var products = GetProductsAlsoPurchasedById(productId, 
                this.ProductsAlsoPurchasedNumber, 0, out totalRecords);
            return products;
        }

        /// <summary>
        /// Gets a list of products purchased by other customers who purchased the above
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Product collection</returns>
        public List<Product> GetProductsAlsoPurchasedById(int productId,
            int pageSize, int pageIndex, out int totalRecords)
        {
            if (pageSize <= 0)
                pageSize = 10;
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            if (pageIndex < 0)
                pageIndex = 0;
            if (pageIndex == int.MaxValue)
                pageIndex = int.MaxValue - 1;

            bool showHidden = NopContext.Current.IsAdmin;

            
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));
            var products = _context.Sp_ProductAlsoPurchasedLoadByProductID(productId,
               showHidden, pageIndex, pageSize, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);
            return products;
        }

        /// <summary>
        /// Sets a product rating
        /// </summary>
        /// <param name="productId">Product identifer</param>
        /// <param name="rating">Rating</param>
        public void SetProductRating(int productId, int rating)
        {
            if (NopContext.Current.User == null || (NopContext.Current.User.IsGuest && !IoC.Resolve<ICustomerService>().AllowAnonymousUsersToSetProductRatings))
            {
                return;
            }

            var product = GetProductById(productId);
            if (product == null)
                throw new NopException("Product could not be loaded");

            if (rating < 1 || rating > 5)
                rating = 1;
            var ratedOn = DateTime.UtcNow;

            //delete previous helpfulness

            var oldPr = (from pr in _context.ProductRatings
                         where pr.ProductId == productId &&
                         pr.CustomerId == NopContext.Current.User.CustomerId
                         select pr).FirstOrDefault();
            if (oldPr != null)
            {
                _context.DeleteObject(oldPr);
            }
            _context.SaveChanges();

            //insert new rating
            var newPr = _context.ProductRatings.CreateObject();
            newPr.ProductId = productId;
            newPr.CustomerId = NopContext.Current.User.CustomerId;
            newPr.Rating = rating;
            newPr.RatedOn = ratedOn;
            _context.ProductRatings.AddObject(newPr);
            _context.SaveChanges();

            //new totals
            int ratingSum = Convert.ToInt32((from pr in _context.ProductRatings
                                             where pr.ProductId == productId
                                             select pr).Sum(p => (int?)p.Rating));
            int totalRatingVotes = (from pr in _context.ProductRatings
                                    where pr.ProductId == productId
                                    select pr).Count();

            product.RatingSum = ratingSum;
            product.TotalRatingVotes = totalRatingVotes;
            UpdateProduct(product);
        }

        /// <summary>
        /// Clears a "compare products" list
        /// </summary>
        public void ClearCompareProducts()
        {
            HttpCookie compareCookie = HttpContext.Current.Request.Cookies.Get("NopCommerce.CompareProducts");
            if (compareCookie != null)
            {
                compareCookie.Values.Clear();
                compareCookie.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.Cookies.Set(compareCookie);
            }
        }

        /// <summary>
        /// Gets a "compare products" list
        /// </summary>
        /// <returns>"Compare products" list</returns>
        public List<Product> GetCompareProducts()
        {
            var products = new List<Product>();
            var productIds = GetCompareProductsIds();
            foreach (int productId in productIds)
            {
                var product = GetProductById(productId);
                if (product != null && product.Published && !product.Deleted)
                    products.Add(product);
            }
            return products;
        }

        /// <summary>
        /// Gets a "compare products" identifier list
        /// </summary>
        /// <returns>"compare products" identifier list</returns>
        public List<int> GetCompareProductsIds()
        {
            var productIds = new List<int>();
            HttpCookie compareCookie = HttpContext.Current.Request.Cookies.Get("NopCommerce.CompareProducts");
            if ((compareCookie == null) || (compareCookie.Values == null))
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

        /// <summary>
        /// Removes a product from a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifer</param>
        public void RemoveProductFromCompareList(int productId)
        {
            var oldProductIds = GetCompareProductsIds();
            var newProductIds = new List<int>();
            newProductIds.AddRange(oldProductIds);
            newProductIds.Remove(productId);

            HttpCookie compareCookie = HttpContext.Current.Request.Cookies.Get("NopCommerce.CompareProducts");
            if ((compareCookie == null) || (compareCookie.Values == null))
                return;
            compareCookie.Values.Clear();
            foreach (int newProductId in newProductIds)
                compareCookie.Values.Add("CompareProductIds", newProductId.ToString());
            compareCookie.Expires = DateTime.Now.AddDays(10.0);
            HttpContext.Current.Response.Cookies.Set(compareCookie);
        }

        /// <summary>
        /// Adds a product to a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifer</param>
        public void AddProductToCompareList(int productId)
        {
            if (!this.CompareProductsEnabled)
                return;

            var oldProductIds = GetCompareProductsIds();
            var newProductIds = new List<int>();
            newProductIds.Add(productId);
            foreach (int oldProductId in oldProductIds)
                if (oldProductId != productId)
                    newProductIds.Add(oldProductId);

            HttpCookie compareCookie = HttpContext.Current.Request.Cookies.Get("NopCommerce.CompareProducts");
            if (compareCookie == null)
                compareCookie = new HttpCookie("NopCommerce.CompareProducts");
            compareCookie.Values.Clear();
            int maxProducts = 4;
            int i = 1;
            foreach (int newProductId in newProductIds)
            {
                compareCookie.Values.Add("CompareProductIds", newProductId.ToString());
                if (i == maxProducts)
                    break;
                i++;
            }
            compareCookie.Expires = DateTime.Now.AddDays(10.0);
            HttpContext.Current.Response.Cookies.Set(compareCookie);
        }

        /// <summary>
        /// Gets a "recently viewed products" list
        /// </summary>
        /// <param name="number">Number of products to load</param>
        /// <returns>"recently viewed products" list</returns>
        public List<Product> GetRecentlyViewedProducts(int number)
        {
            var products = new List<Product>();
            var productIds = GetRecentlyViewedProductsIds(number);
            foreach (int productId in productIds)
            {
                Product product = GetProductById(productId);
                if (product != null && product.Published && !product.Deleted)
                    products.Add(product);
            }
            return products;
        }

        /// <summary>
        /// Gets a "recently viewed products" identifier list
        /// </summary>
        /// <returns>"recently viewed products" list</returns>
        public List<int> GetRecentlyViewedProductsIds()
        {
            return GetRecentlyViewedProductsIds(int.MaxValue);
        }

        /// <summary>
        /// Gets a "recently viewed products" identifier list
        /// </summary>
        /// <param name="number">Number of products to load</param>
        /// <returns>"recently viewed products" list</returns>
        public List<int> GetRecentlyViewedProductsIds(int number)
        {
            var productIds = new List<int>();
            HttpCookie recentlyViewedCookie = HttpContext.Current.Request.Cookies.Get("NopCommerce.RecentlyViewedProducts");
            if ((recentlyViewedCookie == null) || (recentlyViewedCookie.Values == null))
                return productIds;
            string[] values = recentlyViewedCookie.Values.GetValues("RecentlyViewedProductIds");
            if (values == null)
                return productIds;
            foreach (string productId in values)
            {
                int prodId = int.Parse(productId);
                if (!productIds.Contains(prodId))
                {
                    productIds.Add(prodId);
                    if (productIds.Count >= number)
                        break;
                }

            }

            return productIds;
        }

        /// <summary>
        /// Adds a product to a recently viewed products list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public void AddProductToRecentlyViewedList(int productId)
        {
            if (!this.RecentlyViewedProductsEnabled)
                return;

            var oldProductIds = GetRecentlyViewedProductsIds();
            var newProductIds = new List<int>();
            newProductIds.Add(productId);
            foreach (int oldProductId in oldProductIds)
                if (oldProductId != productId)
                    newProductIds.Add(oldProductId);

            HttpCookie recentlyViewedCookie = HttpContext.Current.Request.Cookies.Get("NopCommerce.RecentlyViewedProducts");
            if (recentlyViewedCookie == null)
                recentlyViewedCookie = new HttpCookie("NopCommerce.RecentlyViewedProducts");
            recentlyViewedCookie.Values.Clear();
            int maxProducts = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Display.RecentlyViewedProductCount");
            if (maxProducts <= 0)
                maxProducts = 10;
            int i = 1;
            foreach (int newProductId in newProductIds)
            {
                recentlyViewedCookie.Values.Add("RecentlyViewedProductIds", newProductId.ToString());
                if (i == maxProducts)
                    break;
                i++;
            }
            recentlyViewedCookie.Expires = DateTime.Now.AddDays(10.0);
            HttpContext.Current.Response.Cookies.Set(recentlyViewedCookie);
        }

        /// <summary>
        /// Gets a recently added products list
        /// </summary>
        /// <param name="number">Number of products to load</param>
        /// <returns>Recently added products</returns>
        public List<Product> GetRecentlyAddedProducts(int number)
        {
            int totalRecords = 0;
            var products = this.GetAllProducts(0,
                0, 0, null, null, null, 0, string.Empty, false, number,
                0, null, NopContext.Current.WorkingLanguage.LanguageId,
                ProductSortingEnum.CreatedOn, out totalRecords);
            return products;
        }

        /// <summary>
        /// Direct add to cart allowed
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="productVariantId">Default product variant identifier for adding to cart</param>
        /// <returns>A value indicating whether direct add to cart is allowed</returns>
        public bool DirectAddToCartAllowed(int productId, out int productVariantId)
        {
            bool result = false;
            productVariantId = 0;
            var product = GetProductById(productId);
            if (product != null)
            {
                var productVariants = product.ProductVariants;
                if (productVariants.Count == 1)
                {
                    var defaultProductVariant = productVariants[0];
                    if (!defaultProductVariant.CustomerEntersPrice)
                    {
                        var addToCartWarnings = IoC.Resolve<IShoppingCartService>().GetShoppingCartItemWarnings(ShoppingCartTypeEnum.ShoppingCart,
                            defaultProductVariant.ProductVariantId, string.Empty, decimal.Zero, 1);

                        if (addToCartWarnings.Count == 0)
                        {
                            productVariantId = defaultProductVariant.ProductVariantId;
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Creates a copy of product with all depended data
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <param name="name">The name of product duplicate</param>
        /// <param name="isPublished">A value indicating whether the product duplicate should be published</param>
        /// <param name="copyImages">A value indicating whether the product images should be copied</param>
        /// <returns>Product entity</returns>
        public Product DuplicateProduct(int productId, string name,
            bool isPublished, bool copyImages)
        {
            var product = GetProductById(productId);
            if (product == null)
                return null;

            Product productCopy = null;
            //uncomment this line to support transactions
            //using (var scope = new System.Transactions.TransactionScope())
            {
                // product
                productCopy = new Product()
                {
                    Name = name,
                    ShortDescription = product.ShortDescription,
                    FullDescription = product.FullDescription,
                    AdminComment = product.AdminComment,
                    TemplateId = product.TemplateId,
                    ShowOnHomePage = product.ShowOnHomePage,
                    MetaKeywords = product.MetaKeywords,
                    MetaDescription = product.MetaDescription,
                    MetaTitle = product.MetaTitle,
                    SEName = product.SEName,
                    AllowCustomerReviews = product.AllowCustomerReviews,
                    AllowCustomerRatings = product.AllowCustomerRatings,
                    Published = isPublished,
                    Deleted = product.Deleted,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                };
                InsertProduct(productCopy);

                if (productCopy == null)
                    return null;

                var languages = IoC.Resolve<ILanguageService>().GetAllLanguages(true);

                //localization
                foreach (var lang in languages)
                {
                    var productLocalized = GetProductLocalizedByProductIdAndLanguageId(product.ProductId, lang.LanguageId);
                    if (productLocalized != null)
                    {
                        var productLocalizedCopy = new ProductLocalized()
                        {
                            ProductId = productCopy.ProductId,
                            LanguageId = productLocalized.LanguageId,
                            Name = productLocalized.Name,
                            ShortDescription = productLocalized.ShortDescription,
                            FullDescription = productLocalized.FullDescription,
                            MetaKeywords = productLocalized.MetaKeywords,
                            MetaDescription = productLocalized.MetaDescription,
                            MetaTitle = productLocalized.MetaTitle,
                            SEName = productLocalized.SEName
                        };
                        InsertProductLocalized(productLocalizedCopy);
                    }
                }

                // product pictures
                if (copyImages)
                {
                    foreach (var productPicture in product.ProductPictures)
                    {
                        var picture = productPicture.Picture;

                        var pictureCopy = IoC.Resolve<IPictureService>().InsertPicture(picture.PictureBinary,
                            picture.MimeType,
                            picture.IsNew);

                        InsertProductPicture(new ProductPicture()
                        {
                            ProductId = productCopy.ProductId,
                            PictureId = pictureCopy.PictureId,
                            DisplayOrder = productPicture.DisplayOrder
                        });
                    }
                }

                // product <-> categories mappings
                foreach (var productCategory in product.ProductCategories)
                {
                    var productCategoryCopy = new ProductCategory()
                    {
                        ProductId = productCopy.ProductId,
                        CategoryId = productCategory.CategoryId,
                        IsFeaturedProduct = productCategory.IsFeaturedProduct,
                        DisplayOrder = productCategory.DisplayOrder
                    };

                    IoC.Resolve<ICategoryService>().InsertProductCategory(productCategoryCopy);
                }

                // product <-> manufacturers mappings
                foreach (var productManufacturers in product.ProductManufacturers)
                {
                    var productManufacturerCopy = new ProductManufacturer()
                    {
                        ProductId = productCopy.ProductId,
                        ManufacturerId = productManufacturers.ManufacturerId,
                        IsFeaturedProduct = productManufacturers.IsFeaturedProduct,
                        DisplayOrder = productManufacturers.DisplayOrder
                    };

                    IoC.Resolve<IManufacturerService>().InsertProductManufacturer(productManufacturerCopy);
                }

                // product <-> releated products mappings
                foreach (var relatedProduct in product.RelatedProducts)
                {
                    InsertRelatedProduct(
                        new RelatedProduct()
                        {
                            ProductId1 = productCopy.ProductId,
                            ProductId2 = relatedProduct.ProductId2,
                            DisplayOrder = relatedProduct.DisplayOrder
                        });
                }

                // product specifications
                foreach (var productSpecificationAttribute in IoC.Resolve<ISpecificationAttributeService>().GetProductSpecificationAttributesByProductId(product.ProductId))
                {
                    var psaCopy = new ProductSpecificationAttribute()
                    {
                        ProductId = productCopy.ProductId,
                        SpecificationAttributeOptionId = productSpecificationAttribute.SpecificationAttributeOptionId,
                        AllowFiltering = productSpecificationAttribute.AllowFiltering,
                        ShowOnProductPage = productSpecificationAttribute.ShowOnProductPage,
                        DisplayOrder = productSpecificationAttribute.DisplayOrder
                    };
                    IoC.Resolve<ISpecificationAttributeService>().InsertProductSpecificationAttribute(psaCopy);
                }

                // product variants
                var productVariants = GetProductVariantsByProductId(product.ProductId, true);
                foreach (var productVariant in productVariants)
                {
                    // product variant picture
                    int pictureId = 0;
                    if (copyImages)
                    {
                        var picture = productVariant.Picture;
                        if (picture != null)
                        {
                            var pictureCopy = IoC.Resolve<IPictureService>().InsertPicture(picture.PictureBinary, picture.MimeType, picture.IsNew);
                            pictureId = pictureCopy.PictureId;
                        }
                    }

                    // product variant download & sample download
                    int downloadId = productVariant.DownloadId;
                    int sampleDownloadId = productVariant.SampleDownloadId;
                    if (productVariant.IsDownload)
                    {
                        var download = productVariant.Download;
                        if (download != null)
                        {
                            var downloadCopy = new Download()
                                {
                                    UseDownloadUrl = download.UseDownloadUrl,
                                    DownloadUrl = download.DownloadUrl,
                                    DownloadBinary = download.DownloadBinary,
                                    ContentType = download.ContentType,
                                    Filename = download.Filename,
                                    Extension = download.Extension,
                                    IsNew = download.IsNew
                                };
                            IoC.Resolve<IDownloadService>().InsertDownload(downloadCopy);
                            downloadId = downloadCopy.DownloadId;
                        }

                        if (productVariant.HasSampleDownload)
                        {
                            var sampleDownload = productVariant.SampleDownload;
                            if (sampleDownload != null)
                            {
                                var sampleDownloadCopy = new Download()
                                {
                                    UseDownloadUrl = sampleDownload.UseDownloadUrl,
                                    DownloadUrl = sampleDownload.DownloadUrl,
                                    DownloadBinary = sampleDownload.DownloadBinary,
                                    ContentType = sampleDownload.ContentType,
                                    Filename = sampleDownload.Filename,
                                    Extension = sampleDownload.Extension,
                                    IsNew = sampleDownload.IsNew
                                };
                                IoC.Resolve<IDownloadService>().InsertDownload(sampleDownloadCopy);
                                sampleDownloadId = sampleDownloadCopy.DownloadId;
                            }
                        }
                    }

                    // product variant
                    var productVariantCopy = new ProductVariant()
                    {
                        ProductId = productCopy.ProductId,
                        Name = productVariant.Name,
                        SKU = productVariant.SKU,
                        Description = productVariant.Description,
                        AdminComment = productVariant.AdminComment,
                        ManufacturerPartNumber = productVariant.ManufacturerPartNumber,
                        IsGiftCard = productVariant.IsGiftCard,
                        GiftCardType = productVariant.GiftCardType,
                        IsDownload = productVariant.IsDownload,
                        DownloadId = downloadId,
                        UnlimitedDownloads = productVariant.UnlimitedDownloads,
                        MaxNumberOfDownloads = productVariant.MaxNumberOfDownloads,
                        DownloadExpirationDays = productVariant.DownloadExpirationDays,
                        DownloadActivationType = productVariant.DownloadActivationType,
                        HasSampleDownload = productVariant.HasSampleDownload,
                        SampleDownloadId = sampleDownloadId,
                        HasUserAgreement = productVariant.HasUserAgreement,
                        UserAgreementText = productVariant.UserAgreementText,
                        IsRecurring = productVariant.IsRecurring,
                        CycleLength = productVariant.CycleLength,
                        CyclePeriod = productVariant.CyclePeriod,
                        TotalCycles = productVariant.TotalCycles,
                        IsShipEnabled = productVariant.IsShipEnabled,
                        IsFreeShipping = productVariant.IsFreeShipping,
                        AdditionalShippingCharge = productVariant.AdditionalShippingCharge,
                        IsTaxExempt = productVariant.IsTaxExempt,
                        TaxCategoryId = productVariant.TaxCategoryId,
                        ManageInventory = productVariant.ManageInventory,
                        StockQuantity = productVariant.StockQuantity,
                        DisplayStockAvailability = productVariant.DisplayStockAvailability,
                        DisplayStockQuantity = productVariant.DisplayStockQuantity,
                        MinStockQuantity = productVariant.MinStockQuantity,
                        LowStockActivityId = productVariant.LowStockActivityId,
                        NotifyAdminForQuantityBelow = productVariant.NotifyAdminForQuantityBelow,
                        Backorders = productVariant.Backorders,
                        OrderMinimumQuantity = productVariant.OrderMinimumQuantity,
                        OrderMaximumQuantity = productVariant.OrderMaximumQuantity,
                        WarehouseId = productVariant.WarehouseId,
                        DisableBuyButton = productVariant.DisableBuyButton,
                        CallForPrice = productVariant.CallForPrice,
                        Price = productVariant.Price,
                        OldPrice = productVariant.OldPrice,
                        ProductCost = productVariant.ProductCost,
                        CustomerEntersPrice = productVariant.CustomerEntersPrice,
                        MinimumCustomerEnteredPrice = productVariant.MinimumCustomerEnteredPrice,
                        MaximumCustomerEnteredPrice = productVariant.MaximumCustomerEnteredPrice,
                        Weight = productVariant.Weight,
                        Length = productVariant.Length,
                        Width = productVariant.Width,
                        Height = productVariant.Height,
                        PictureId = pictureId,
                        AvailableStartDateTime = productVariant.AvailableStartDateTime,
                        AvailableEndDateTime = productVariant.AvailableEndDateTime,
                        Published = productVariant.Published,
                        Deleted = productVariant.Deleted,
                        DisplayOrder = productVariant.DisplayOrder,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow
                    };

                    InsertProductVariant(productVariantCopy);

                    //localization
                    foreach (var lang in languages)
                    {
                        var productVariantLocalized = GetProductVariantLocalizedByProductVariantIdAndLanguageId(productVariant.ProductVariantId, lang.LanguageId);
                        if (productVariantLocalized != null)
                        {
                            var productVariantLocalizedCopy = new ProductVariantLocalized()
                            {
                                ProductVariantId = productVariantCopy.ProductVariantId,
                                LanguageId = productVariantLocalized.LanguageId,
                                Name = productVariantLocalized.Name,
                                Description = productVariantLocalized.Description
                            };
                            InsertProductVariantLocalized(productVariantLocalizedCopy);
                        }
                    }

                    // product variant <-> attributes mappings
                    foreach (var productVariantAttribute in IoC.Resolve<IProductAttributeService>().GetProductVariantAttributesByProductVariantId(productVariant.ProductVariantId))
                    {
                        var productVariantAttributeCopy = new ProductVariantAttribute()
                        {
                            ProductVariantId = productVariantCopy.ProductVariantId,
                            ProductAttributeId = productVariantAttribute.ProductAttributeId,
                            TextPrompt = productVariantAttribute.TextPrompt,
                            IsRequired = productVariantAttribute.IsRequired,
                            AttributeControlTypeId = productVariantAttribute.AttributeControlTypeId,
                            DisplayOrder = productVariantAttribute.DisplayOrder
                        };
                        IoC.Resolve<IProductAttributeService>().InsertProductVariantAttribute(productVariantAttributeCopy);

                        // product variant attribute values
                        var productVariantAttributeValues = IoC.Resolve<IProductAttributeService>().GetProductVariantAttributeValues(productVariantAttribute.ProductVariantAttributeId);
                        foreach (var productVariantAttributeValue in productVariantAttributeValues)
                        {
                            var pvavCopy = new ProductVariantAttributeValue()
                            {
                                ProductVariantAttributeId = productVariantAttributeCopy.ProductVariantAttributeId,
                                Name = productVariantAttributeValue.Name,
                                PriceAdjustment = productVariantAttributeValue.PriceAdjustment,
                                WeightAdjustment = productVariantAttributeValue.WeightAdjustment,
                                IsPreSelected = productVariantAttributeValue.IsPreSelected,
                                DisplayOrder = productVariantAttributeValue.DisplayOrder
                            };
                            IoC.Resolve<IProductAttributeService>().InsertProductVariantAttributeValue(pvavCopy);

                            //localization
                            foreach (var lang in languages)
                            {
                                var pvavLocalized = IoC.Resolve<IProductAttributeService>().GetProductVariantAttributeValueLocalizedByProductVariantAttributeValueIdAndLanguageId(productVariantAttributeValue.ProductVariantAttributeValueId, lang.LanguageId);
                                if (pvavLocalized != null)
                                {
                                    var pvavLocalizedCopy = new ProductVariantAttributeValueLocalized()
                                    {
                                        ProductVariantAttributeValueId = pvavCopy.ProductVariantAttributeValueId,
                                        LanguageId = pvavLocalized.LanguageId,
                                        Name = pvavLocalized.Name
                                    };
                                    IoC.Resolve<IProductAttributeService>().InsertProductVariantAttributeValueLocalized(pvavLocalizedCopy);
                                }
                            }
                        }
                    }
                    foreach (var combination in IoC.Resolve<IProductAttributeService>().GetAllProductVariantAttributeCombinations(productVariant.ProductVariantId))
                    {
                        var combinationCopy = new ProductVariantAttributeCombination()
                        {
                            ProductVariantId = productVariantCopy.ProductVariantId,
                            AttributesXml = combination.AttributesXml,
                            StockQuantity = combination.StockQuantity,
                            AllowOutOfStockOrders = combination.AllowOutOfStockOrders
                        };
                        IoC.Resolve<IProductAttributeService>().InsertProductVariantAttributeCombination(combinationCopy);
                    }

                    // product variant tier prices
                    foreach (var tierPrice in productVariant.TierPrices)
                    {
                        InsertTierPrice(
                            new TierPrice()
                            {
                                ProductVariantId = productVariantCopy.ProductVariantId,
                                Quantity = tierPrice.Quantity,
                                Price = tierPrice.Price
                            });
                    }

                    // product variant <-> discounts mapping
                    foreach (var discount in productVariant.AllDiscounts)
                    {
                        IoC.Resolve<IDiscountService>().AddDiscountToProductVariant(productVariantCopy.ProductVariantId, discount.DiscountId);
                    }

                    // prices by customer role
                    foreach (var crpp in productVariant.CustomerRoleProductPrices)
                    {
                        this.InsertCustomerRoleProductPrice(
                            new CustomerRoleProductPrice()
                            {
                                CustomerRoleId = crpp.CustomerRoleId,
                                ProductVariantId = productVariantCopy.ProductVariantId,
                                Price = crpp.Price
                            }
                            );
                    }
                }

                //uncomment this line to support transactions
                //scope.Complete();
            }

            return productCopy;
        }

        /// <summary>
        /// Gets a cross-sells
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Cross-sells</returns>
        public List<Product> GetCrosssellProductsByShoppingCart(ShoppingCart cart)
        {
            List<Product> result = new List<Product>();

            if (this.CrossSellsNumber == 0)
                return result;

            if (cart == null || cart.Count == 0)
                return result;

            List<int> cartProductIds = new List<int>();
            foreach (var sci in cart)
            {
                int prodId = sci.ProductVariant.ProductId;
                if (!cartProductIds.Contains(prodId))
                {
                    cartProductIds.Add(prodId);
                }
            }

            for (int i = 0; i < cart.Count; i++)
            {
                var sci = cart[i];
                var crossSells = sci.ProductVariant.Product.CrossSellProducts;
                foreach (var crossSell in crossSells)
                {
                    //TODO create a helper method to validate product availability (dates, quantity) etc
                    

                    //validate that this product is not added to result yet
                    //validate that this product is not in the cart
                    if (result.Find(p => p.ProductId == crossSell.ProductId2) == null &&
                        !cartProductIds.Contains(crossSell.ProductId2))
                    {
                        result.Add(crossSell.Product2);
                        if (result.Count >= this.CrossSellsNumber)
                            return result;
                    }
                }
            }
            return result;
        }

        #endregion

        #region Product variants
        
        /// <summary>
        /// Get low stock product variants
        /// </summary>
        /// <returns>Result</returns>
        public List<ProductVariant> GetLowStockProductVariants()
        {
            
            var query = from pv in _context.ProductVariants
                        orderby pv.MinStockQuantity
                        where !pv.Deleted &&
                        pv.MinStockQuantity >= pv.StockQuantity
                        select pv;
            var productVariants = query.ToList();
            return productVariants;
        }
        
        /// <summary>
        /// Gets a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Product variant</returns>
        public ProductVariant GetProductVariantById(int productVariantId)
        {
            if (productVariantId == 0)
                return null;

            string key = string.Format(PRODUCTVARIANTS_BY_ID_KEY, productVariantId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (ProductVariant)obj2;
            }

            
            var query = from pv in _context.ProductVariants
                        where pv.ProductVariantId == productVariantId
                        select pv;
            var productVariant = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, productVariant);
            }
            return productVariant;
        }

        /// <summary>
        /// Gets a product variant by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Product variant</returns>
        public ProductVariant GetProductVariantBySKU(string sku)
        {
            if (String.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            
            var query = from pv in _context.ProductVariants
                        orderby pv.DisplayOrder, pv.ProductVariantId
                        where !pv.Deleted &&
                        pv.SKU == sku
                        select pv;
            var productVariant = query.FirstOrDefault();
            return productVariant;
        }
        
        /// <summary>
        /// Gets all product variants
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Product variants</returns>
        public List<ProductVariant> GetAllProductVariants(int categoryId,
            int manufacturerId, string keywords, 
            int pageSize, int pageIndex, out int totalRecords)
        {
            if (pageSize <= 0)
                pageSize = 10;
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            if (pageIndex < 0)
                pageIndex = 0;
            if (pageIndex == int.MaxValue)
                pageIndex = int.MaxValue - 1;

            bool showHidden = NopContext.Current.IsAdmin;

            
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));
            var productVariants = _context.Sp_ProductVariantLoadAll(categoryId,
                manufacturerId, keywords, showHidden,
                pageIndex, pageSize, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);

            return productVariants;
        }
        
        /// <summary>
        /// Inserts a product variant
        /// </summary>
        /// <param name="productVariant">The product variant</param>
        public void InsertProductVariant(ProductVariant productVariant)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            productVariant.Name = CommonHelper.EnsureNotNull(productVariant.Name);
            productVariant.Name = CommonHelper.EnsureMaximumLength(productVariant.Name, 400);
            productVariant.SKU = CommonHelper.EnsureNotNull(productVariant.SKU);
            productVariant.SKU = productVariant.SKU.Trim();
            productVariant.SKU = CommonHelper.EnsureMaximumLength(productVariant.SKU, 100);
            productVariant.Description = CommonHelper.EnsureNotNull(productVariant.Description);
            productVariant.Description = CommonHelper.EnsureMaximumLength(productVariant.Description, 4000);
            productVariant.AdminComment = CommonHelper.EnsureNotNull(productVariant.AdminComment);
            productVariant.AdminComment = CommonHelper.EnsureMaximumLength(productVariant.AdminComment, 4000);
            productVariant.ManufacturerPartNumber = CommonHelper.EnsureNotNull(productVariant.ManufacturerPartNumber);
            productVariant.ManufacturerPartNumber = CommonHelper.EnsureMaximumLength(productVariant.ManufacturerPartNumber, 100);
            productVariant.UserAgreementText = CommonHelper.EnsureNotNull(productVariant.UserAgreementText);

            

            _context.ProductVariants.AddObject(productVariant);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                _cacheManager.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }

            //raise event             
            EventContext.Current.OnProductVariantCreated(null,
                new ProductVariantEventArgs() { ProductVariant = productVariant });
        }

        /// <summary>
        /// Updates the product variant
        /// </summary>
        /// <param name="productVariant">The product variant</param>
        public void UpdateProductVariant(ProductVariant productVariant)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");
            
            productVariant.Name = CommonHelper.EnsureNotNull(productVariant.Name);
            productVariant.Name = CommonHelper.EnsureMaximumLength(productVariant.Name, 400);
            productVariant.SKU = CommonHelper.EnsureNotNull(productVariant.SKU);
            productVariant.SKU = productVariant.SKU.Trim();
            productVariant.SKU = CommonHelper.EnsureMaximumLength(productVariant.SKU, 100);
            productVariant.Description = CommonHelper.EnsureNotNull(productVariant.Description);
            productVariant.Description = CommonHelper.EnsureMaximumLength(productVariant.Description, 4000);
            productVariant.AdminComment = CommonHelper.EnsureNotNull(productVariant.AdminComment);
            productVariant.AdminComment = CommonHelper.EnsureMaximumLength(productVariant.AdminComment, 4000);
            productVariant.ManufacturerPartNumber = CommonHelper.EnsureNotNull(productVariant.ManufacturerPartNumber);
            productVariant.ManufacturerPartNumber = CommonHelper.EnsureMaximumLength(productVariant.ManufacturerPartNumber, 100);
            productVariant.UserAgreementText = CommonHelper.EnsureNotNull(productVariant.UserAgreementText);

            
            if (!_context.IsAttached(productVariant))
                _context.ProductVariants.Attach(productVariant);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                _cacheManager.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }

            //raise event             
            EventContext.Current.OnProductVariantUpdated(null,
                new ProductVariantEventArgs() { ProductVariant = productVariant });
        }

        /// <summary>
        /// Gets product variants by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>Product variant collection</returns>
        public List<ProductVariant> GetProductVariantsByProductId(int productId)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetProductVariantsByProductId(productId, showHidden);
        }
        
        /// <summary>
        /// Gets product variants by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product variant collection</returns>
        public List<ProductVariant> GetProductVariantsByProductId(int productId, bool showHidden)
        {
            string key = string.Format(PRODUCTVARIANTS_ALL_KEY, showHidden, productId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<ProductVariant>)obj2;
            }

            
            var query = (IQueryable<ProductVariant>)_context.ProductVariants;
            if (!showHidden)
            {
                query = query.Where(pv => pv.Published);
            }
            if (!showHidden)
            {
                query = query.Where(pv => !pv.AvailableStartDateTime.HasValue || pv.AvailableStartDateTime <= DateTime.UtcNow);
                query = query.Where(pv => !pv.AvailableEndDateTime.HasValue || pv.AvailableEndDateTime >= DateTime.UtcNow);
            }
            query = query.Where(pv => !pv.Deleted);
            query = query.Where(pv => pv.ProductId == productId);
            query = query.OrderBy(pv => pv.DisplayOrder);
            
            var productVariants = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, productVariants);
            }
            return productVariants;
        }

        /// <summary>
        /// Gets restricted product variants by discount identifier
        /// </summary>
        /// <param name="discountId">The discount identifier</param>
        /// <returns>Product variant collection</returns>
        public List<ProductVariant> GetProductVariantsRestrictedByDiscountId(int discountId)
        {
            if (discountId == 0)
                return new List<ProductVariant>();

            
            var query = from pv in _context.ProductVariants
                        from d in pv.NpRestrictedDiscounts
                        where d.DiscountId == discountId
                        select pv;
            var productVariants = query.ToList();
            return productVariants;
        }

        /// <summary>
        /// Gets localized product variant by id
        /// </summary>
        /// <param name="productVariantLocalizedId">Localized product variant identifier</param>
        /// <returns>Product variant content</returns>
        public ProductVariantLocalized GetProductVariantLocalizedById(int productVariantLocalizedId)
        {
            if (productVariantLocalizedId == 0)
                return null;

            
            var query = from pvl in _context.ProductVariantLocalized
                        where pvl.ProductVariantLocalizedId == productVariantLocalizedId
                        select pvl;
            var productVariantLocalized = query.SingleOrDefault();
            return productVariantLocalized;
        }

        /// <summary>
        /// Gets localized product variant by product variant id
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Product variant content</returns>
        public List<ProductVariantLocalized> GetProductVariantLocalizedByProductVariantId(int productVariantId)
        {
            if (productVariantId == 0)
                return new List<ProductVariantLocalized>();

            
            var query = from pvl in _context.ProductVariantLocalized
                        where pvl.ProductVariantId == productVariantId
                        select pvl;
            var content = query.ToList();
            return content;
        }
        
        /// <summary>
        /// Gets localized product variant by product variant id and language id
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Product variant content</returns>
        public ProductVariantLocalized GetProductVariantLocalizedByProductVariantIdAndLanguageId(int productVariantId, int languageId)
        {
            if (productVariantId == 0 || languageId == 0)
                return null;

            
            var query = from pvl in _context.ProductVariantLocalized
                        orderby pvl.ProductVariantLocalizedId
                        where pvl.ProductVariantId == productVariantId &&
                        pvl.LanguageId == languageId
                        select pvl;
            var productVariantLocalized = query.FirstOrDefault();
            return productVariantLocalized;
        }

        /// <summary>
        /// Inserts a localized product variant
        /// </summary>
        /// <param name="productVariantLocalized">Localized product variant</param>
        public void InsertProductVariantLocalized(ProductVariantLocalized productVariantLocalized)
        {
            if (productVariantLocalized == null)
                throw new ArgumentNullException("productVariantLocalized");

            productVariantLocalized.Name = CommonHelper.EnsureNotNull(productVariantLocalized.Name);
            productVariantLocalized.Name = CommonHelper.EnsureMaximumLength(productVariantLocalized.Name, 400);
            productVariantLocalized.Description = CommonHelper.EnsureNotNull(productVariantLocalized.Description);

            

            _context.ProductVariantLocalized.AddObject(productVariantLocalized);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                _cacheManager.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Update a localized product variant
        /// </summary>
        /// <param name="productVariantLocalized">Localized product variant</param>
        public void UpdateProductVariantLocalized(ProductVariantLocalized productVariantLocalized)
        {
            if (productVariantLocalized == null)
                throw new ArgumentNullException("productVariantLocalized");

            productVariantLocalized.Name = CommonHelper.EnsureNotNull(productVariantLocalized.Name);
            productVariantLocalized.Name = CommonHelper.EnsureMaximumLength(productVariantLocalized.Name, 400);
            productVariantLocalized.Description = CommonHelper.EnsureNotNull(productVariantLocalized.Description);

            bool allFieldsAreEmpty = string.IsNullOrEmpty(productVariantLocalized.Name) &&
               string.IsNullOrEmpty(productVariantLocalized.Description);

            
            if (!_context.IsAttached(productVariantLocalized))
                _context.ProductVariantLocalized.Attach(productVariantLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                _context.DeleteObject(productVariantLocalized);
                _context.SaveChanges();
            }
            else
            {
                _context.SaveChanges();
            }

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                _cacheManager.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Marks a product variant as deleted
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        public void MarkProductVariantAsDeleted(int productVariantId)
        {
            var productVariant = GetProductVariantById(productVariantId);
            if (productVariant != null)
            {
                productVariant.Deleted = true;
                UpdateProductVariant(productVariant);
            }
        }

        /// <summary>
        /// Adjusts inventory
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="decrease">A value indicating whether to increase or descrease product variant stock quantity</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        public void AdjustInventory(int productVariantId, bool decrease,
            int quantity, string attributesXml)
        {
            var productVariant = GetProductVariantById(productVariantId);
            if (productVariant == null)
                return;

            switch ((ManageInventoryMethodEnum)productVariant.ManageInventory)
            {
                case ManageInventoryMethodEnum.DontManageStock:
                    {
                        //do nothing
                        return;
                    }
                case ManageInventoryMethodEnum.ManageStock:
                    {
                        int newStockQuantity = 0;
                        if (decrease)
                            newStockQuantity = productVariant.StockQuantity - quantity;
                        else
                            newStockQuantity = productVariant.StockQuantity + quantity;

                        bool newPublished = productVariant.Published;
                        bool newDisableBuyButton = productVariant.DisableBuyButton;

                        //check if minimum quantity is reached
                        if (decrease)
                        {
                            if (productVariant.MinStockQuantity >= newStockQuantity)
                            {
                                switch (productVariant.LowStockActivity)
                                {
                                    case LowStockActivityEnum.DisableBuyButton:
                                        newDisableBuyButton = true;
                                        break;
                                    case LowStockActivityEnum.Unpublish:
                                        newPublished = false;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        if (decrease && productVariant.NotifyAdminForQuantityBelow > newStockQuantity)
                        {
                            IoC.Resolve<IMessageService>().SendQuantityBelowStoreOwnerNotification(productVariant, IoC.Resolve<ILocalizationManager>().DefaultAdminLanguage.LanguageId);
                        }

                        productVariant.StockQuantity = newStockQuantity;
                        productVariant.DisableBuyButton = newDisableBuyButton;
                        productVariant.Published = newPublished;
                        UpdateProductVariant(productVariant);

                        if (decrease)
                        {
                            var product = productVariant.Product;
                            bool allProductVariantsUnpublished = true;
                            foreach (var pv2 in product.ProductVariants)
                            {
                                if (pv2.Published)
                                {
                                    allProductVariantsUnpublished = false;
                                    break;
                                }
                            }

                            if (allProductVariantsUnpublished)
                            {
                                product.Published = false;
                                UpdateProduct(product);
                            }
                        }
                    }
                    break;
                case ManageInventoryMethodEnum.ManageStockByAttributes:
                    {
                        var combination = IoC.Resolve<IProductAttributeService>().FindProductVariantAttributeCombination(productVariant.ProductVariantId, attributesXml);
                        if (combination != null)
                        {
                            int newStockQuantity = 0;
                            if (decrease)
                                newStockQuantity = combination.StockQuantity - quantity;
                            else
                                newStockQuantity = combination.StockQuantity + quantity;

                            combination.StockQuantity = newStockQuantity;
                            IoC.Resolve<IProductAttributeService>().UpdateProductVariantAttributeCombination(combination);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion
        
        #region Product pictures

        /// <summary>
        /// Deletes a product picture mapping
        /// </summary>
        /// <param name="productPictureId">Product picture mapping identifier</param>
        public void DeleteProductPicture(int productPictureId)
        {
            var productPicture = GetProductPictureById(productPictureId);
            if (productPicture == null)
                return;

            
            if (!_context.IsAttached(productPicture))
                _context.ProductPictures.Attach(productPicture);
            _context.DeleteObject(productPicture);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a product picture mapping
        /// </summary>
        /// <param name="productPictureId">Product picture mapping identifier</param>
        /// <returns>Product picture mapping</returns>
        public ProductPicture GetProductPictureById(int productPictureId)
        {
            if (productPictureId == 0)
                return null;

            
            var query = from pp in _context.ProductPictures
                        where pp.ProductPictureId == productPictureId
                        select pp;
            var productPicture = query.SingleOrDefault();
            return productPicture;
        }

        /// <summary>
        /// Inserts a product picture mapping
        /// </summary>
        /// <param name="productPicture">Product picture mapping</param>
        public void InsertProductPicture(ProductPicture productPicture)
        {
            if (productPicture == null)
                throw new ArgumentNullException("productPicture");

            

            _context.ProductPictures.AddObject(productPicture);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the product picture mapping
        /// </summary>
        /// <param name="productPicture">Product picture mapping</param>
        public void UpdateProductPicture(ProductPicture productPicture)
        {
            if (productPicture == null)
                throw new ArgumentNullException("productPicture");

            
            if (!_context.IsAttached(productPicture))
                _context.ProductPictures.Attach(productPicture);

            _context.SaveChanges();
        }

        /// <summary>
        /// Gets all product picture mappings by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product picture mapping collection</returns>
        public List<ProductPicture> GetProductPicturesByProductId(int productId)
        {
            
            var query = (IQueryable<ProductPicture>)_context.ProductPictures;
            query = query.Where(pp => pp.ProductId == productId);
            query = query.OrderBy(pp => pp.DisplayOrder);

            var productPictures = query.ToList();
            return productPictures;
        }
        #endregion

        #region Product reviews

        /// <summary>
        /// Gets a product review
        /// </summary>
        /// <param name="productReviewId">Product review identifier</param>
        /// <returns>Product review</returns>
        public ProductReview GetProductReviewById(int productReviewId)
        {
            if (productReviewId == 0)
                return null;

            
            var query = from pr in _context.ProductReviews
                        where pr.ProductReviewId == productReviewId
                        select pr;
            var productReview = query.SingleOrDefault();
            return productReview;
        }

        /// <summary>
        /// Gets a product review collection by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product review collection</returns>
        public List<ProductReview> GetProductReviewByProductId(int productId)
        {
            bool showHidden = NopContext.Current.IsAdmin;

            
            var query = from pr in _context.ProductReviews
                        orderby pr.CreatedOn descending
                        where (showHidden || pr.IsApproved) &&
                        pr.ProductId == productId
                        select pr;
            var productReviews = query.ToList();
            return productReviews;
        }

        /// <summary>
        /// Deletes a product review
        /// </summary>
        /// <param name="productReviewId">Product review identifier</param>
        public void DeleteProductReview(int productReviewId)
        {
            var productReview = GetProductReviewById(productReviewId);
            if (productReview == null)
                return;

            
            if (!_context.IsAttached(productReview))
                _context.ProductReviews.Attach(productReview);
            _context.DeleteObject(productReview);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets all product reviews
        /// </summary>
        /// <returns>Product review collection</returns>
        public List<ProductReview> GetAllProductReviews()
        {
            bool showHidden = NopContext.Current.IsAdmin;

            
            var query = from pr in _context.ProductReviews
                        orderby pr.CreatedOn descending
                        where (showHidden || pr.IsApproved)
                        select pr;
            var productReviews = query.ToList();
            return productReviews;
        }

        /// <summary>
        /// Inserts a product review
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="title">The review title</param>
        /// <param name="reviewText">The review text</param>
        /// <param name="rating">The review rating</param>
        /// <param name="helpfulYesTotal">Review helpful votes total</param>
        /// <param name="helpfulNoTotal">Review not helpful votes total</param>
        /// <param name="isApproved">A value indicating whether the product review is approved</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <returns>Product review</returns>
        public ProductReview InsertProductReview(int productId, 
            int customerId, string title,
            string reviewText, int rating, int helpfulYesTotal,
            int helpfulNoTotal, bool isApproved, DateTime createdOn)
        {
            return InsertProductReview(productId, customerId,
             title, reviewText, rating, helpfulYesTotal,
             helpfulNoTotal, isApproved, createdOn, 
             this.NotifyAboutNewProductReviews);
        }

        /// <summary>
        /// Inserts a product review
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="title">The review title</param>
        /// <param name="reviewText">The review text</param>
        /// <param name="rating">The review rating</param>
        /// <param name="helpfulYesTotal">Review helpful votes total</param>
        /// <param name="helpfulNoTotal">Review not helpful votes total</param>
        /// <param name="isApproved">A value indicating whether the product review is approved</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="notify">A value indicating whether to notify the store owner</param>
        /// <returns>Product review</returns>
        public ProductReview InsertProductReview(int productId,
            int customerId, string title,
            string reviewText, int rating, int helpfulYesTotal,
            int helpfulNoTotal, bool isApproved, DateTime createdOn, bool notify)
        {
            string IPAddress = NopContext.Current.UserHostAddress;
            return InsertProductReview(productId, customerId, IPAddress, title, reviewText, rating, helpfulYesTotal, helpfulNoTotal, isApproved, createdOn, notify);
        }

        /// <summary>
        /// Inserts a product review
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="ipAddress">The IP address</param>
        /// <param name="title">The review title</param>
        /// <param name="reviewText">The review text</param>
        /// <param name="rating">The review rating</param>
        /// <param name="helpfulYesTotal">Review helpful votes total</param>
        /// <param name="helpfulNoTotal">Review not helpful votes total</param>
        /// <param name="isApproved">A value indicating whether the product review is approved</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="notify">A value indicating whether to notify the store owner</param>
        /// <returns>Product review</returns>
        public ProductReview InsertProductReview(int productId,
            int customerId, string ipAddress, string title,
            string reviewText, int rating, int helpfulYesTotal,
            int helpfulNoTotal, bool isApproved, DateTime createdOn, bool notify)
        {
            if (rating < 1)
                rating = 1;
            if (rating > 5)
                rating = 5;

            ipAddress = CommonHelper.EnsureNotNull(ipAddress);
            ipAddress = CommonHelper.EnsureMaximumLength(ipAddress, 100);
            title = CommonHelper.EnsureNotNull(title);
            title = CommonHelper.EnsureMaximumLength(title, 1000);
            reviewText = CommonHelper.EnsureNotNull(reviewText);

            

            var productReview = _context.ProductReviews.CreateObject();
            productReview.ProductId = productId;
            productReview.CustomerId = customerId;
            productReview.IPAddress = ipAddress;
            productReview.Title = title;
            productReview.ReviewText = reviewText;
            productReview.Rating = rating;
            productReview.HelpfulYesTotal = helpfulYesTotal;
            productReview.HelpfulNoTotal = helpfulNoTotal;
            productReview.IsApproved = isApproved;
            productReview.CreatedOn = createdOn;

            _context.ProductReviews.AddObject(productReview);
            _context.SaveChanges();

            //activity log
            IoC.Resolve<ICustomerActivityService>().InsertActivity(
                "WriteProductReview",
                IoC.Resolve<ILocalizationManager>().GetLocaleResourceString("ActivityLog.WriteProductReview"),
                productId);

            //notify store owner
            if (notify)
            {
                IoC.Resolve<IMessageService>().SendProductReviewNotificationMessage(productReview, IoC.Resolve<ILocalizationManager>().DefaultAdminLanguage.LanguageId);
            }

            return productReview;
        }

        /// <summary>
        /// Updates the product review
        /// </summary>
        /// <param name="ProductReview">Product review</param>
        public void UpdateProductReview(ProductReview productReview)
        {
            if (productReview == null)
                throw new ArgumentNullException("productReview");

            productReview.IPAddress = CommonHelper.EnsureNotNull(productReview.IPAddress);
            productReview.IPAddress = CommonHelper.EnsureMaximumLength(productReview.IPAddress, 100);
            productReview.Title = CommonHelper.EnsureNotNull(productReview.Title);
            productReview.Title = CommonHelper.EnsureMaximumLength(productReview.Title, 1000);
            productReview.ReviewText = CommonHelper.EnsureNotNull(productReview.ReviewText);

            
            if (!_context.IsAttached(productReview))
                _context.ProductReviews.Attach(productReview);

            _context.SaveChanges();
        }

        /// <summary>
        /// Sets a product rating helpfulness
        /// </summary>
        /// <param name="productReviewId">Product review identifer</param>
        /// <param name="wasHelpful">A value indicating whether the product review was helpful or not </param>
        public void SetProductRatingHelpfulness(int productReviewId, bool wasHelpful)
        {
            if (NopContext.Current.User == null)
            {
                return;
            }
            if (NopContext.Current.User.IsGuest && !IoC.Resolve<ICustomerService>().AllowAnonymousUsersToReviewProduct)
            {
                return;
            }

            ProductReview productReview = GetProductReviewById(productReviewId);
            if (productReview == null)
                return;

            //delete previous helpfulness            
            var oldPrh = (from prh in _context.ProductReviewHelpfulness
                         where prh.ProductReviewId == productReviewId &&
                         prh.CustomerId == NopContext.Current.User.CustomerId
                         select prh).FirstOrDefault();
            if (oldPrh != null)
            {
                _context.DeleteObject(oldPrh);
            }
            _context.SaveChanges();

            //insert new helpfulness
            var newPrh = _context.ProductReviewHelpfulness.CreateObject();
            newPrh.ProductReviewId = productReviewId;
            newPrh.CustomerId = NopContext.Current.User.CustomerId;
            newPrh.WasHelpful = wasHelpful;

            _context.ProductReviewHelpfulness.AddObject(newPrh);
            _context.SaveChanges();

            //new totals
            int helpfulYesTotal = (from prh in _context.ProductReviewHelpfulness
                                   where prh.ProductReviewId == productReviewId && 
                                   prh.WasHelpful == true
                                   select prh).Count();
            int helpfulNoTotal = (from prh in _context.ProductReviewHelpfulness
                                   where prh.ProductReviewId == productReviewId &&
                                   prh.WasHelpful == false
                                   select prh).Count();

            productReview.HelpfulYesTotal = helpfulYesTotal;
            productReview.HelpfulNoTotal = helpfulNoTotal;
            UpdateProductReview(productReview);
        }
        
        #endregion

        #region Related products

        /// <summary>
        /// Deletes a related product
        /// </summary>
        /// <param name="relatedProductId">Related product identifer</param>
        public void DeleteRelatedProduct(int relatedProductId)
        {
            var relatedProduct = GetRelatedProductById(relatedProductId);
            if (relatedProduct == null)
                return;

            
            if (!_context.IsAttached(relatedProduct))
                _context.RelatedProducts.Attach(relatedProduct);
            _context.DeleteObject(relatedProduct);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a related product collection by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <returns>Related product collection</returns>
        public List<RelatedProduct> GetRelatedProductsByProductId1(int productId1)
        {
            bool showHidden = NopContext.Current.IsAdmin;

            
            var query = from rp in _context.RelatedProducts
                        join p in _context.Products on rp.ProductId2 equals p.ProductId
                        where rp.ProductId1 == productId1 &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby rp.DisplayOrder
                        select rp;
            var relatedProducts = query.ToList();

            return relatedProducts;
        }

        /// <summary>
        /// Gets a related product
        /// </summary>
        /// <param name="relatedProductId">Related product identifer</param>
        /// <returns>Related product</returns>
        public RelatedProduct GetRelatedProductById(int relatedProductId)
        {
            if (relatedProductId == 0)
                return null;

            
            var query = from rp in _context.RelatedProducts
                        where rp.RelatedProductId == relatedProductId
                        select rp;
            var relatedProduct = query.SingleOrDefault();
            return relatedProduct;
        }

        /// <summary>
        /// Inserts a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        public void InsertRelatedProduct(RelatedProduct relatedProduct)
        {
            if (relatedProduct == null)
                throw new ArgumentNullException("relatedProduct");

            

            _context.RelatedProducts.AddObject(relatedProduct);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        public void UpdateRelatedProduct(RelatedProduct relatedProduct)
        {
            if (relatedProduct == null)
                throw new ArgumentNullException("relatedProduct");

            
            if (!_context.IsAttached(relatedProduct))
                _context.RelatedProducts.Attach(relatedProduct);

            _context.SaveChanges();
        }

        #endregion

        #region Cross-sell products

        /// <summary>
        /// Deletes a cross-sell product
        /// </summary>
        /// <param name="crossSellProductId">Cross-sell identifer</param>
        public void DeleteCrossSellProduct(int crossSellProductId)
        {
            var crossSellProduct = GetCrossSellProductById(crossSellProductId);
            if (crossSellProduct == null)
                return;

            
            if (!_context.IsAttached(crossSellProduct))
                _context.CrossSellProducts.Attach(crossSellProduct);
            _context.DeleteObject(crossSellProduct);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a cross-sell product collection by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <returns>Cross-sell product collection</returns>
        public List<CrossSellProduct> GetCrossSellProductsByProductId1(int productId1)
        {
            bool showHidden = NopContext.Current.IsAdmin;

            
            var query = from csp in _context.CrossSellProducts
                        join p in _context.Products on csp.ProductId2 equals p.ProductId
                        where csp.ProductId1 == productId1 &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby csp.CrossSellProductId
                        select csp;
            var crossSellProducts = query.ToList();

            return crossSellProducts;
        }

        /// <summary>
        /// Gets a cross-sell product
        /// </summary>
        /// <param name="crossSellProductId">Cross-sell product identifer</param>
        /// <returns>Cross-sell product</returns>
        public CrossSellProduct GetCrossSellProductById(int crossSellProductId)
        {
            if (crossSellProductId == 0)
                return null;

            
            var query = from csp in _context.CrossSellProducts
                        where csp.CrossSellProductId == crossSellProductId
                        select csp;
            var crossSellProduct = query.SingleOrDefault();
            return crossSellProduct;
        }

        /// <summary>
        /// Inserts a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell product</param>
        public void InsertCrossSellProduct(CrossSellProduct crossSellProduct)
        {
            if (crossSellProduct == null)
                throw new ArgumentNullException("crossSellProduct");

            

            _context.CrossSellProducts.AddObject(crossSellProduct);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell product</param>
        public void UpdateCrossSellProduct(CrossSellProduct crossSellProduct)
        {
            if (crossSellProduct == null)
                throw new ArgumentNullException("crossSellProduct");

            
            if (!_context.IsAttached(crossSellProduct))
                _context.CrossSellProducts.Attach(crossSellProduct);

            _context.SaveChanges();
        }

        #endregion

        #region Pricelists

        /// <summary>
        /// Gets all product variants directly assigned to a pricelist
        /// </summary>
        /// <param name="pricelistId">Pricelist identifier</param>
        /// <returns>Product variants</returns>
        public List<ProductVariant> GetProductVariantsByPricelistId(int pricelistId)
        {
            
            var query = from pv in _context.ProductVariants
                        join pvpl in _context.ProductVariantPricelists on pv.ProductVariantId equals pvpl.ProductVariantId
                        where pvpl.PricelistId == pricelistId
                        select pv;
            var productVariants = query.ToList();

            return productVariants;
        }

        /// <summary>
        /// Deletes a pricelist
        /// </summary>
        /// <param name="pricelistId">The PricelistId of the item to be deleted</param>
        public void DeletePricelist(int pricelistId)
        {
            var pricelist = GetPricelistById(pricelistId);
            if (pricelist == null)
                return;

            
            if (!_context.IsAttached(pricelist))
                _context.Pricelists.Attach(pricelist);
            _context.DeleteObject(pricelist);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a collection of all available pricelists
        /// </summary>
        /// <returns>Collection of pricelists</returns>
        public List<Pricelist> GetAllPricelists()
        {
            
            var query = from pl in _context.Pricelists
                        orderby pl.PricelistId
                        select pl;
            var pricelists = query.ToList();

            return pricelists;
        }

        /// <summary>
        /// Gets a pricelist
        /// </summary>
        /// <param name="pricelistId">Pricelist identifier</param>
        /// <returns>Pricelist</returns>
        public Pricelist GetPricelistById(int pricelistId)
        {
            if (pricelistId == 0)
                return null;

            
            var query = from pl in _context.Pricelists
                        where pl.PricelistId == pricelistId
                        select pl;
            var pricelist = query.SingleOrDefault();

            return pricelist;
        }

        /// <summary>
        /// Gets a pricelist
        /// </summary>
        /// <param name="pricelistGuid">Pricelist GUId</param>
        /// <returns>Pricelist</returns>
        public Pricelist GetPricelistByGuid(string pricelistGuid)
        {
            
            var query = from pl in _context.Pricelists
                        where pl.PricelistGuid == pricelistGuid
                        select pl;
            var pricelist = query.FirstOrDefault();

            return pricelist;
        }

        /// <summary>
        /// Inserts a Pricelist
        /// </summary>
        /// <param name="pricelist">Pricelist</param>
        public void InsertPricelist(Pricelist pricelist)
        {
            if (pricelist == null)
                throw new ArgumentNullException("pricelist");
            
            pricelist.DisplayName = CommonHelper.EnsureNotNull(pricelist.DisplayName);
            pricelist.DisplayName = CommonHelper.EnsureMaximumLength(pricelist.DisplayName, 100);
            pricelist.ShortName = CommonHelper.EnsureNotNull(pricelist.ShortName);
            pricelist.ShortName = CommonHelper.EnsureMaximumLength(pricelist.ShortName, 20);
            pricelist.PricelistGuid = CommonHelper.EnsureNotNull(pricelist.PricelistGuid);
            pricelist.PricelistGuid = CommonHelper.EnsureMaximumLength(pricelist.PricelistGuid, 40);
            pricelist.FormatLocalization = CommonHelper.EnsureNotNull(pricelist.FormatLocalization);
            pricelist.FormatLocalization = CommonHelper.EnsureMaximumLength(pricelist.FormatLocalization, 5);
            pricelist.Description = CommonHelper.EnsureNotNull(pricelist.Description);
            pricelist.Description = CommonHelper.EnsureMaximumLength(pricelist.Description, 500);
            pricelist.AdminNotes = CommonHelper.EnsureNotNull(pricelist.AdminNotes);
            pricelist.AdminNotes = CommonHelper.EnsureMaximumLength(pricelist.AdminNotes, 500);
            pricelist.Header = CommonHelper.EnsureNotNull(pricelist.Header);
            pricelist.Header = CommonHelper.EnsureMaximumLength(pricelist.Header, 500);
            pricelist.Body = CommonHelper.EnsureNotNull(pricelist.Body);
            pricelist.Body = CommonHelper.EnsureMaximumLength(pricelist.Body, 500);
            pricelist.Footer = CommonHelper.EnsureNotNull(pricelist.Footer);
            pricelist.Footer = CommonHelper.EnsureMaximumLength(pricelist.Footer, 500);

            
            
            _context.Pricelists.AddObject(pricelist);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the Pricelist
        /// </summary>
        /// <param name="pricelist">Pricelist</param>
        public void UpdatePricelist(Pricelist pricelist)
        {
            if (pricelist == null)
                throw new ArgumentNullException("pricelist");

            pricelist.DisplayName = CommonHelper.EnsureNotNull(pricelist.DisplayName);
            pricelist.DisplayName = CommonHelper.EnsureMaximumLength(pricelist.DisplayName, 100);
            pricelist.ShortName = CommonHelper.EnsureNotNull(pricelist.ShortName);
            pricelist.ShortName = CommonHelper.EnsureMaximumLength(pricelist.ShortName, 20);
            pricelist.PricelistGuid = CommonHelper.EnsureNotNull(pricelist.PricelistGuid);
            pricelist.PricelistGuid = CommonHelper.EnsureMaximumLength(pricelist.PricelistGuid, 40);
            pricelist.FormatLocalization = CommonHelper.EnsureNotNull(pricelist.FormatLocalization);
            pricelist.FormatLocalization = CommonHelper.EnsureMaximumLength(pricelist.FormatLocalization, 5);
            pricelist.Description = CommonHelper.EnsureNotNull(pricelist.Description);
            pricelist.Description = CommonHelper.EnsureMaximumLength(pricelist.Description, 500);
            pricelist.AdminNotes = CommonHelper.EnsureNotNull(pricelist.AdminNotes);
            pricelist.AdminNotes = CommonHelper.EnsureMaximumLength(pricelist.AdminNotes, 500);
            pricelist.Header = CommonHelper.EnsureNotNull(pricelist.Header);
            pricelist.Header = CommonHelper.EnsureMaximumLength(pricelist.Header, 500);
            pricelist.Body = CommonHelper.EnsureNotNull(pricelist.Body);
            pricelist.Body = CommonHelper.EnsureMaximumLength(pricelist.Body, 500);
            pricelist.Footer = CommonHelper.EnsureNotNull(pricelist.Footer);
            pricelist.Footer = CommonHelper.EnsureMaximumLength(pricelist.Footer, 500);

            
            if (!_context.IsAttached(pricelist))
                _context.Pricelists.Attach(pricelist);

            _context.SaveChanges();
        }

        /// <summary>
        /// Deletes a ProductVariantPricelist
        /// </summary>
        /// <param name="productVariantPricelistId">ProductVariantPricelist identifier</param>
        public void DeleteProductVariantPricelist(int productVariantPricelistId)
        {
            if (productVariantPricelistId == 0)
                return;

            var productVariantPricelist = GetProductVariantPricelistById(productVariantPricelistId);
            if (productVariantPricelist == null)
                return;

            
            if (!_context.IsAttached(productVariantPricelist))
                _context.ProductVariantPricelists.Attach(productVariantPricelist);
            _context.DeleteObject(productVariantPricelist);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a ProductVariantPricelist
        /// </summary>
        /// <param name="productVariantPricelistId">ProductVariantPricelist identifier</param>
        /// <returns>ProductVariantPricelist</returns>
        public ProductVariantPricelist GetProductVariantPricelistById(int productVariantPricelistId)
        {
            if (productVariantPricelistId == 0)
                return null;

            
            var query = from pvpl in _context.ProductVariantPricelists
                        where pvpl.ProductVariantPricelistId == productVariantPricelistId
                        select pvpl;
            var productVariantPricelist = query.SingleOrDefault();
            return productVariantPricelist;
        }

        /// <summary>
        /// Gets ProductVariantPricelist
        /// </summary>
        /// <param name="productVariantId">ProductVariant identifier</param>
        /// <param name="pricelistId">Pricelist identifier</param>
        /// <returns>ProductVariantPricelist</returns>
        public ProductVariantPricelist GetProductVariantPricelist(int productVariantId, int pricelistId)
        {
            
            var query = from pvpl in _context.ProductVariantPricelists
                        where pvpl.ProductVariantId == productVariantId &&
                        pvpl.PricelistId == pricelistId 
                        select pvpl;
            var productVariantPricelist = query.FirstOrDefault();
            return productVariantPricelist;
        }

        /// <summary>
        /// Inserts a ProductVariantPricelist
        /// </summary>
        /// <param name="productVariantPricelist">The product variant pricelist</param>
        public void InsertProductVariantPricelist(ProductVariantPricelist productVariantPricelist)
        {
            if (productVariantPricelist == null)
                throw new ArgumentNullException("productVariantPricelist");

            
            
            _context.ProductVariantPricelists.AddObject(productVariantPricelist);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the ProductVariantPricelist
        /// </summary>
        /// <param name="productVariantPricelist">The product variant pricelist</param>
        public void UpdateProductVariantPricelist(ProductVariantPricelist productVariantPricelist)
        {
            if (productVariantPricelist == null)
                throw new ArgumentNullException("productVariantPricelist");

            
            if (!_context.IsAttached(productVariantPricelist))
                _context.ProductVariantPricelists.Attach(productVariantPricelist);

            _context.SaveChanges();
        }

        #endregion

        #region Tier prices

        /// <summary>
        /// Gets a tier price
        /// </summary>
        /// <param name="tierPriceId">Tier price identifier</param>
        /// <returns>Tier price</returns>
        public TierPrice GetTierPriceById(int tierPriceId)
        {
            if (tierPriceId == 0)
                return null;

            
            var query = from tp in _context.TierPrices
                        where tp.TierPriceId == tierPriceId
                        select tp;
            var tierPrice = query.SingleOrDefault();
            return tierPrice;
        }

        /// <summary>
        /// Gets tier prices by product variant identifier
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Tier price collection</returns>
        public List<TierPrice> GetTierPricesByProductVariantId(int productVariantId)
        {
            if (productVariantId == 0)
                return new List<TierPrice>();

            string key = string.Format(TIERPRICES_ALLBYPRODUCTVARIANTID_KEY, productVariantId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<TierPrice>)obj2;
            }

            
            var query = from tp in _context.TierPrices
                        orderby tp.Quantity
                        where tp.ProductVariantId == productVariantId
                        select tp;
            var tierPrices = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, tierPrices);
            }
            return tierPrices;
        }

        /// <summary>
        /// Deletes a tier price
        /// </summary>
        /// <param name="tierPriceId">Tier price identifier</param>
        public void DeleteTierPrice(int tierPriceId)
        {
            var tierPrice = GetTierPriceById(tierPriceId);
            if (tierPrice == null)
                return;

            
            if (!_context.IsAttached(tierPrice))
                _context.TierPrices.Attach(tierPrice);
            _context.DeleteObject(tierPrice);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                _cacheManager.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Inserts a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        public void InsertTierPrice(TierPrice tierPrice)
        {
            if (tierPrice == null)
                throw new ArgumentNullException("tierPrice");

            
            
            _context.TierPrices.AddObject(tierPrice);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                _cacheManager.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        public void UpdateTierPrice(TierPrice tierPrice)
        {
            if (tierPrice == null)
                throw new ArgumentNullException("tierPrice");

            
            if (!_context.IsAttached(tierPrice))
                _context.TierPrices.Attach(tierPrice);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                _cacheManager.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
        }

        #endregion

        #region Product prices by customer role

        /// <summary>
        /// Deletes a product price by customer role by identifier 
        /// </summary>
        /// <param name="customerRoleProductPriceId">The identifier</param>
        public void DeleteCustomerRoleProductPrice(int customerRoleProductPriceId)
        {
            var customerRoleProductPrice = GetCustomerRoleProductPriceById(customerRoleProductPriceId);
            if (customerRoleProductPrice == null)
                return;

            
            if (!_context.IsAttached(customerRoleProductPrice))
                _context.CustomerRoleProductPrices.Attach(customerRoleProductPrice);
            _context.DeleteObject(customerRoleProductPrice);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                _cacheManager.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a product price by customer role by identifier 
        /// </summary>
        /// <param name="customerRoleProductPriceId">The identifier</param>
        /// <returns>Product price by customer role by identifier </returns>
        public CustomerRoleProductPrice GetCustomerRoleProductPriceById(int customerRoleProductPriceId)
        {
            if (customerRoleProductPriceId == 0)
                return null;

            
            var query = from crpp in _context.CustomerRoleProductPrices
                        where crpp.CustomerRoleProductPriceId == customerRoleProductPriceId
                        select crpp;
            var customerRoleProductPrice = query.SingleOrDefault();
            return customerRoleProductPrice;
        }

        /// <summary>
        /// Gets a collection of product prices by customer role
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>A collection of product prices by customer role</returns>
        public List<CustomerRoleProductPrice> GetAllCustomerRoleProductPrices(int productVariantId)
        {
            string key = string.Format(CUSTOMERROLEPRICES_ALL_KEY, productVariantId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<CustomerRoleProductPrice>)obj2;
            }
            
            var query = from crpp in _context.CustomerRoleProductPrices
                        where crpp.ProductVariantId == productVariantId
                        select crpp;
            var collection = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, collection);
            }
            return collection;
        }

        /// <summary>
        /// Inserts a product price by customer role
        /// </summary>
        /// <param name="customerRoleProductPrice">A product price by customer role</param>
        public void InsertCustomerRoleProductPrice(CustomerRoleProductPrice customerRoleProductPrice)
        {
            if (customerRoleProductPrice == null)
                throw new ArgumentNullException("customerRoleProductPrice");

            
            
            _context.CustomerRoleProductPrices.AddObject(customerRoleProductPrice);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                _cacheManager.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates a product price by customer role
        /// </summary>
        /// <param name="customerRoleProductPrice">A product price by customer role</param>
        public void UpdateCustomerRoleProductPrice(CustomerRoleProductPrice customerRoleProductPrice)
        {
            if (customerRoleProductPrice == null)
                throw new ArgumentNullException("customerRoleProductPrice");

            
            if (!_context.IsAttached(customerRoleProductPrice))
                _context.CustomerRoleProductPrices.Attach(customerRoleProductPrice);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                _cacheManager.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
        }

        #endregion

        #region Product tags

        /// <summary>
        /// Deletes a product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        public void DeleteProductTag(int productTagId)
        {
            var productTag = GetProductTagById(productTagId);
            if (productTag == null)
                return;

            
            if (!_context.IsAttached(productTag))
                _context.ProductTags.Attach(productTag);
            _context.DeleteObject(productTag);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>Product tag</returns>
        public ProductTag GetProductTagById(int productTagId)
        {
            if (productTagId == 0)
                return null;

            
            var query = from pt in _context.ProductTags
                        where pt.ProductTagId == productTagId
                        select pt;
            var productTag = query.SingleOrDefault();
            return productTag;
        }

        /// <summary>
        /// Gets product tags by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product tag collection</returns>
        public List<ProductTag> GetProductTagsByProductId(int productId)
        {
            if (productId == 0)
                return new List<ProductTag>();

            var query = from pt in _context.ProductTags
                        from p in pt.NpProducts
                        where p.ProductId == productId
                        orderby pt.ProductCount descending
                        select pt;

            var productTags = query.ToList();
            return productTags;
        }

        /// <summary>
        /// Gets product tag by name
        /// </summary>
        /// <param name="name">Product tag name</param>
        /// <returns>Product tag</returns>
        public ProductTag GetProductTagByName(string name)
        {
            if (name == null)
                name = string.Empty;
            name = name.Trim();

            var query = from pt in _context.ProductTags
                        where pt.Name == name
                        select pt;

            var productTag = query.FirstOrDefault();
            return productTag;
        }

        /// <summary>
        /// Gets all product tags
        /// </summary>
        /// <returns>Product tag collection</returns>
        public List<ProductTag> GetAllProductTags()
        {
            var query = from pt in _context.ProductTags
                        orderby pt.ProductCount descending
                        select pt;

            var productTags = query.ToList();
            return productTags;
        }

        /// <summary>
        /// Inserts a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public void InsertProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException("productTag");

            productTag.Name = CommonHelper.EnsureNotNull(productTag.Name);
            productTag.Name = productTag.Name.Trim();
            productTag.Name = CommonHelper.EnsureMaximumLength(productTag.Name, 100);

            
            
            _context.ProductTags.AddObject(productTag);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public void UpdateProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException("productTag");

            productTag.Name = CommonHelper.EnsureNotNull(productTag.Name);
            productTag.Name = productTag.Name.Trim();
            productTag.Name = CommonHelper.EnsureMaximumLength(productTag.Name, 100);


            
            if (!_context.IsAttached(productTag))
                _context.ProductTags.Attach(productTag);

            _context.SaveChanges();
        }

        /// <summary>
        /// Adds a discount tag mapping
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="productTagId">Product tag identifier</param>
        public void AddProductTagMapping(int productId, int productTagId)
        {
            Product product = GetProductById(productId);
            if (product == null)
                return;

            ProductTag productTag = GetProductTagById(productTagId);
            if (productTag == null)
                return;

            if (!_context.IsAttached(product))
                _context.Products.Attach(product);
            if (!_context.IsAttached(productTag))
                _context.ProductTags.Attach(productTag);

            //ensure that navigation property is loaded
            if (product.NpProductTags == null)
                _context.LoadProperty(product, p => p.NpProductTags);

            product.NpProductTags.Add(productTag);
            _context.SaveChanges();
            
            //new totals                        
            if (productTag.NpProducts == null) //ensure that navigation property is loaded
                _context.LoadProperty(productTag, pt => pt.NpProducts);
            int newTotal = productTag.NpProducts.Count();
            if (newTotal > 0)
            {
                productTag.ProductCount = newTotal;
                UpdateProductTag(productTag);
            }
            else
            {
                DeleteProductTag(productTagId);
            }
        }

        /// <summary>
        /// Checking whether the product tag mapping exists
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <param name="productTagId">The product tag identifier</param>
        /// <returns>True if mapping exist, otherwise false</returns>
        public bool DoesProductTagMappingExist(int productId, int productTagId)
        {
            ProductTag productTag = GetProductTagById(productTagId);
            if (productTag == null)
                return false;

            //ensure that navigation property is loaded
            if (productTag.NpProducts == null)
                _context.LoadProperty(productTag, pt => pt.NpProducts);

            bool result = productTag.NpProducts.ToList().Find(p => p.ProductId == productId) != null;
            return result;
        }

        /// <summary>
        /// Removes a discount tag mapping
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="productTagId">Product tag identifier</param>
        public void RemoveProductTagMapping(int productId, int productTagId)
        {
            Product product = GetProductById(productId);
            if (product == null)
                return;

            ProductTag productTag = GetProductTagById(productTagId);
            if (productTag == null)
                return;

            if (!_context.IsAttached(product))
                _context.Products.Attach(product);
            if (!_context.IsAttached(productTag))
                _context.ProductTags.Attach(productTag);

            //ensure that navigation property is loaded
            if (product.NpProductTags == null)
                _context.LoadProperty(product, p => p.NpProductTags);

            product.NpProductTags.Remove(productTag);
            _context.SaveChanges();

            //new totals                        
            if (productTag.NpProducts == null) //ensure that navigation property is loaded
                _context.LoadProperty(productTag, pt => pt.NpProducts);
            int newTotal = productTag.NpProducts.Count();
            if (newTotal > 0)
            {
                productTag.ProductCount = newTotal;
                UpdateProductTag(productTag);
            }
            else
            {
                DeleteProductTag(productTagId);
            }
        }

        #endregion
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.ProductManager.CacheEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether "Recently viewed products" feature is enabled
        /// </summary>
        public bool RecentlyViewedProductsEnabled
        {
            get
            {
                bool recentlyViewedProductsEnabled = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.RecentlyViewedProductsEnabled");
                return recentlyViewedProductsEnabled;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Display.RecentlyViewedProductsEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a number of "Recently viewed products"
        /// </summary>
        public int RecentlyViewedProductsNumber
        {
            get
            {
                int recentlyViewedProductsNumber = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Display.RecentlyViewedProductsNumber");
                return recentlyViewedProductsNumber;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Display.RecentlyViewedProductsNumber", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether "Recently added products" feature is enabled
        /// </summary>
        public bool RecentlyAddedProductsEnabled
        {
            get
            {
                bool recentlyAddedProductsEnabled = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.RecentlyAddedProductsEnabled");
                return recentlyAddedProductsEnabled;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Display.RecentlyAddedProductsEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a number of "Recently added products"
        /// </summary>
        public int RecentlyAddedProductsNumber
        {
            get
            {
                int recentlyAddedProductsNumber = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Display.RecentlyAddedProductsNumber");
                return recentlyAddedProductsNumber;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Display.RecentlyAddedProductsNumber", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a number of "Cross-sells"
        /// </summary>
        public int CrossSellsNumber
        {
            get
            {
                int result = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Display.CrossSellsNumber", 2);
                return result;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Display.CrossSellsNumber", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a number of products per page on search products page
        /// </summary>
        public int SearchPageProductsPerPage
        {
            get
            {
                int result = IoC.Resolve<ISettingManager>().GetSettingValueInteger("SearchPage.ProductsPerPage", 10);
                return result;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("SearchPage.ProductsPerPage", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to displays a button from AddThis.com on your product pages
        /// </summary>
        public bool ShowShareButton
        {
            get
            {
                bool showShareButton = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Products.AddThisSharing.Enabled");
                return showShareButton;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Products.AddThisSharing.Enabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether "Compare products" feature is enabled
        /// </summary>
        public bool CompareProductsEnabled
        {
            get
            {
                bool compareProductsEnabled = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.EnableCompareProducts");
                return compareProductsEnabled;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Common.EnableCompareProducts", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets "List of products purchased by other customers who purchased the above" option is enable
        /// </summary>
        public bool ProductsAlsoPurchasedEnabled
        {
            get
            {
                bool productsAlsoPurchased = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ListOfProductsAlsoPurchasedEnabled");
                return productsAlsoPurchased;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Display.ListOfProductsAlsoPurchasedEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a number of products also purchased by other customers to display
        /// </summary>
        public int ProductsAlsoPurchasedNumber
        {
            get
            {
                int productsAlsoPurchasedNumber = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Display.ListOfProductsAlsoPurchasedNumberToDisplay");
                return productsAlsoPurchasedNumber;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Display.ListOfProductsAlsoPurchasedNumberToDisplay", value.ToString());
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to notify about new product reviews
        /// </summary>
        public bool NotifyAboutNewProductReviews
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Product.NotifyAboutNewProductReviews");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Product.NotifyAboutNewProductReviews", value.ToString());
            }
        }
        #endregion
    }
}
