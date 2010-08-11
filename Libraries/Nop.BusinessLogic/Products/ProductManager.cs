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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Utils.Html;

namespace NopSolutions.NopCommerce.BusinessLogic.Products
{
    /// <summary>
    /// Product manager
    /// </summary>
    public partial class ProductManager
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
        
        #region Methods

        #region Products

        /// <summary>
        /// Marks a product as deleted
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public static void MarkProductAsDeleted(int productId)
        {
            if (productId == 0)
                return;

            var product = GetProductById(productId);
            if (product != null)
            {
                product = UpdateProduct(product.ProductId, product.Name, product.ShortDescription,
                    product.FullDescription, product.AdminComment,
                    product.TemplateId, product.ShowOnHomePage, product.MetaKeywords, product.MetaDescription,
                    product.MetaTitle, product.SEName, product.AllowCustomerReviews, product.AllowCustomerRatings, product.RatingSum,
                    product.TotalRatingVotes, product.Published, true, product.CreatedOn, product.UpdatedOn);

                foreach (var productVariant in product.ProductVariants)
                    MarkProductVariantAsDeleted(productVariant.ProductVariantId);
            }
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>Product collection</returns>
        public static List<Product> GetAllProducts()
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetAllProducts(showHidden);
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product collection</returns>
        public static List<Product> GetAllProducts(bool showHidden)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from p in context.Products
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
        public static List<Product> GetAllProducts(int pageSize, int pageIndex, 
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
        public static List<Product> GetAllProducts(int categoryId, 
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
        public static List<Product> GetAllProducts(string keywords, 
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
        public static List<Product> GetAllProducts(int categoryId,
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
        public static List<Product> GetAllProducts(int categoryId,
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
        public static List<Product> GetAllProducts(int categoryId,
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
        public static List<Product> GetAllProducts(int categoryId,
            int manufacturerId, int productTagId, bool? featuredProducts,
            decimal? priceMin, decimal? priceMax, string keywords,
            bool searchDescriptions, int pageSize, int pageIndex,
            List<int> filteredSpecs, ProductSortingEnum orderBy, out int totalRecords)
        {
            int languageId = 0;
            if (NopContext.Current != null)
                languageId = NopContext.Current.WorkingLanguage.LanguageId;

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
        public static List<Product> GetAllProducts(int categoryId,
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
        public static List<Product> GetAllProducts(int categoryId,
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

            var context = ObjectContextHelper.CurrentObjectContext;
            var products = context.Sp_ProductLoadAllPaged(categoryId,
               manufacturerId, productTagId, featuredProducts,
               priceMin, priceMax, relatedToProductId, 
               keywords, searchDescriptions, pageSize, pageIndex, filteredSpecs,
               languageId, (int)orderBy, showHidden, out totalRecords);

            return products;
        }

        /// <summary>
        /// Gets all products displayed on the home page
        /// </summary>
        /// <returns>Product collection</returns>
        public static List<Product> GetAllProductsDisplayedOnHomePage()
        {
            bool showHidden = NopContext.Current.IsAdmin;

            int languageId = 0;
            if (NopContext.Current != null)
                languageId = NopContext.Current.WorkingLanguage.LanguageId;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from p in context.Products
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
        public static Product GetProductById(int productId)
        {
            if (productId == 0)
                return null;

            string key = string.Format(PRODUCTS_BY_ID_KEY, productId);
            object obj2 = NopRequestCache.Get(key);
            if (ProductManager.CacheEnabled && (obj2 != null))
            {
                return (Product)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from p in context.Products
                        where p.ProductId == productId
                        select p;
            var product = query.SingleOrDefault();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.Add(key, product);
            }
            return product;
        }
        
        /// <summary>
        /// Inserts a product
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="shortDescription">The short description</param>
        /// <param name="fullDescription">The full description</param>
        /// <param name="adminComment">The admin comment</param>
        /// <param name="templateId">The template identifier</param>
        /// <param name="showOnHomePage">A value indicating whether to show the product on the home page</param>
        /// <param name="metaKeywords">The meta keywords</param>
        /// <param name="metaDescription">The meta description</param>
        /// <param name="metaTitle">The meta title</param>
        /// <param name="seName">The search-engine name</param>
        /// <param name="allowCustomerReviews">A value indicating whether the product allows customer reviews</param>
        /// <param name="allowCustomerRatings">A value indicating whether the product allows customer ratings</param>
        /// <param name="ratingSum">The rating sum</param>
        /// <param name="totalRatingVotes">The total rating votes</param>
        /// <param name="published">A value indicating whether the entity is published</param>
        /// <param name="deleted">A value indicating whether the entity has been deleted</param>
        /// <param name="createdOn">The date and time of product creation</param>
        /// <param name="updatedOn">The date and time of product update</param>
        /// <returns>Product</returns>
        public static Product InsertProduct(string name, string shortDescription,
            string fullDescription, string adminComment,
            int templateId, bool showOnHomePage,
            string metaKeywords, string metaDescription, string metaTitle,
            string seName, bool allowCustomerReviews, bool allowCustomerRatings,
            int ratingSum, int totalRatingVotes, bool published,
            bool deleted, DateTime createdOn, DateTime updatedOn)
        {
            name = CommonHelper.EnsureMaximumLength(name, 400);
            metaKeywords = CommonHelper.EnsureMaximumLength(metaKeywords, 400);
            metaDescription = CommonHelper.EnsureMaximumLength(metaDescription, 4000);
            metaTitle = CommonHelper.EnsureMaximumLength(metaTitle, 400);
            seName = CommonHelper.EnsureMaximumLength(seName, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            var product = context.Products.CreateObject();
            product.Name = name;
            product.ShortDescription = shortDescription;
            product.FullDescription = fullDescription;
            product.AdminComment = adminComment;
            product.TemplateId = templateId;
            product.ShowOnHomePage = showOnHomePage;
            product.MetaKeywords = metaKeywords;
            product.MetaDescription = metaDescription;
            product.MetaTitle = metaTitle;
            product.SEName = seName;
            product.AllowCustomerReviews = allowCustomerReviews;
            product.AllowCustomerRatings = allowCustomerRatings;
            product.RatingSum = ratingSum;
            product.TotalRatingVotes = totalRatingVotes;
            product.Published = published;
            product.Deleted = deleted;
            product.CreatedOn = createdOn;
            product.UpdatedOn = updatedOn;

            context.Products.AddObject(product);
            context.SaveChanges();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
            
            //raise event             
            EventContext.Current.OnProductCreated(null,
                new ProductEventArgs() { Product = product });
            
            return product;
        }

        /// <summary>
        /// Updates the product
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="name">The name</param>
        /// <param name="shortDescription">The short description</param>
        /// <param name="fullDescription">The full description</param>
        /// <param name="adminComment">The admin comment</param>
        /// <param name="templateId">The template identifier</param>
        /// <param name="showOnHomePage">A value indicating whether to show the product on the home page</param>
        /// <param name="metaKeywords">The meta keywords</param>
        /// <param name="metaDescription">The meta description</param>
        /// <param name="metaTitle">The meta title</param>
        /// <param name="seName">The search-engine name</param>
        /// <param name="allowCustomerReviews">A value indicating whether the product allows customer reviews</param>
        /// <param name="allowCustomerRatings">A value indicating whether the product allows customer ratings</param>
        /// <param name="ratingSum">The rating sum</param>
        /// <param name="totalRatingVotes">The total rating votes</param>
        /// <param name="published">A value indicating whether the entity is published</param>
        /// <param name="deleted">A value indicating whether the entity has been deleted</param>
        /// <param name="createdOn">The date and time of product creation</param>
        /// <param name="updatedOn">The date and time of product update</param>
        /// <returns>Product</returns>
        public static Product UpdateProduct(int productId,
            string name, string shortDescription,
            string fullDescription, string adminComment,
            int templateId, bool showOnHomePage,
            string metaKeywords, string metaDescription, string metaTitle,
            string seName, bool allowCustomerReviews, bool allowCustomerRatings,
            int ratingSum, int totalRatingVotes, bool published,
            bool deleted, DateTime createdOn, DateTime updatedOn)
        {
            name = CommonHelper.EnsureMaximumLength(name, 400);
            metaKeywords = CommonHelper.EnsureMaximumLength(metaKeywords, 400);
            metaDescription = CommonHelper.EnsureMaximumLength(metaDescription, 4000);
            metaTitle = CommonHelper.EnsureMaximumLength(metaTitle, 400);
            seName = CommonHelper.EnsureMaximumLength(seName, 100);

            var product = GetProductById(productId);
            if (product == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(product))
                context.Products.Attach(product);

            product.Name = name;
            product.ShortDescription = shortDescription;
            product.FullDescription = fullDescription;
            product.AdminComment = adminComment;
            product.TemplateId = templateId;
            product.ShowOnHomePage = showOnHomePage;
            product.MetaKeywords = metaKeywords;
            product.MetaDescription = metaDescription;
            product.MetaTitle = metaTitle;
            product.SEName = seName;
            product.AllowCustomerReviews = allowCustomerReviews;
            product.AllowCustomerRatings = allowCustomerRatings;
            product.RatingSum = ratingSum;
            product.TotalRatingVotes = totalRatingVotes;
            product.Published = published;
            product.Deleted = deleted;
            product.CreatedOn = createdOn;
            product.UpdatedOn = updatedOn;
            context.SaveChanges();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }

            //raise event             
            EventContext.Current.OnProductUpdated(null,
                new ProductEventArgs() { Product = product });
            
            return product;
        }

        /// <summary>
        /// Gets localized product by id
        /// </summary>
        /// <param name="productLocalizedId">Localized product identifier</param>
        /// <returns>Product content</returns>
        public static ProductLocalized GetProductLocalizedById(int productLocalizedId)
        {
            if (productLocalizedId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pl in context.ProductLocalized
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
        public static List<ProductLocalized> GetProductLocalizedByProductId(int productId)
        {
            if (productId == 0)
                return new List<ProductLocalized>();

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pl in context.ProductLocalized
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
        public static ProductLocalized GetProductLocalizedByProductIdAndLanguageId(int productId, int languageId)
        {
            if (productId == 0 || languageId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pl in context.ProductLocalized
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
        /// <param name="productId">Product identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">Name text</param>
        /// <param name="shortDescription">The short description</param>
        /// <param name="fullDescription">The full description</param>
        /// <param name="metaKeywords">Meta keywords text</param>
        /// <param name="metaDescription">Meta descriptions text</param>
        /// <param name="metaTitle">Metat title text</param>
        /// <param name="seName">Se Name text</param>
        /// <returns>Product content</returns>
        public static ProductLocalized InsertProductLocalized(int productId,
            int languageId, string name, string shortDescription, string fullDescription,
            string metaKeywords, string metaDescription, string metaTitle,
            string seName)
        {
            name = CommonHelper.EnsureMaximumLength(name, 400);
            metaKeywords = CommonHelper.EnsureMaximumLength(metaKeywords, 400);
            metaDescription = CommonHelper.EnsureMaximumLength(metaDescription, 4000);
            metaTitle = CommonHelper.EnsureMaximumLength(metaTitle, 400);
            seName = CommonHelper.EnsureMaximumLength(seName, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            var productLocalized = context.ProductLocalized.CreateObject();
            productLocalized.ProductId = productId;
            productLocalized.LanguageId = languageId;
            productLocalized.Name = name;
            productLocalized.ShortDescription = shortDescription;
            productLocalized.FullDescription = fullDescription;
            productLocalized.MetaKeywords = metaKeywords;
            productLocalized.MetaDescription = metaDescription;
            productLocalized.MetaTitle = metaTitle;
            productLocalized.SEName = seName;

            context.ProductLocalized.AddObject(productLocalized);
            context.SaveChanges();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }

            return productLocalized;
        }

        /// <summary>
        /// Update a localized product
        /// </summary>
        /// <param name="productLocalizedId">Localized product identifier</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">Name text</param>
        /// <param name="shortDescription">The short description</param>
        /// <param name="fullDescription">The full description</param>
        /// <param name="metaKeywords">Meta keywords text</param>
        /// <param name="metaDescription">Meta descriptions text</param>
        /// <param name="metaTitle">Metat title text</param>
        /// <param name="seName">Se Name text</param>
        /// <returns>Product content</returns>
        public static ProductLocalized UpdateProductLocalized(int productLocalizedId,
            int productId, int languageId, string name, string shortDescription,
            string fullDescription, string metaKeywords, string metaDescription,
            string metaTitle, string seName)
        {
            name = CommonHelper.EnsureMaximumLength(name, 400);
            metaKeywords = CommonHelper.EnsureMaximumLength(metaKeywords, 400);
            metaDescription = CommonHelper.EnsureMaximumLength(metaDescription, 4000);
            metaTitle = CommonHelper.EnsureMaximumLength(metaTitle, 400);
            seName = CommonHelper.EnsureMaximumLength(seName, 100);

            var productLocalized = GetProductLocalizedById(productLocalizedId);
            if (productLocalized == null)
                return null;

            bool allFieldsAreEmpty = string.IsNullOrEmpty(name) &&
               string.IsNullOrEmpty(shortDescription) &&
               string.IsNullOrEmpty(fullDescription) &&
               string.IsNullOrEmpty(metaKeywords) &&
               string.IsNullOrEmpty(metaDescription) &&
               string.IsNullOrEmpty(metaTitle) &&
               string.IsNullOrEmpty(seName);

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productLocalized))
                context.ProductLocalized.Attach(productLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                context.DeleteObject(productLocalized);
                context.SaveChanges();
            }
            else
            {
                productLocalized.ProductId = productId;
                productLocalized.LanguageId = languageId;
                productLocalized.Name = name;
                productLocalized.ShortDescription = shortDescription;
                productLocalized.FullDescription = fullDescription;
                productLocalized.MetaKeywords = metaKeywords;
                productLocalized.MetaDescription = metaDescription;
                productLocalized.MetaTitle = metaTitle;
                productLocalized.SEName = seName;
                context.SaveChanges();
            }

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }

            return productLocalized;
        }

        /// <summary>
        /// Gets a list of products purchased by other customers who purchased the above
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product collection</returns>
        public static List<Product> GetProductsAlsoPurchasedById(int productId)
        {
            int totalRecords = 0;
            var products = GetProductsAlsoPurchasedById(productId, 
                ProductManager.ProductsAlsoPurchasedNumber, 0, out totalRecords);
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
        public static List<Product> GetProductsAlsoPurchasedById(int productId,
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

            var context = ObjectContextHelper.CurrentObjectContext;
            var products = context.Sp_ProductAlsoPurchasedLoadByProductID(productId,
               showHidden, pageSize, pageIndex, out totalRecords);
            return products;
        }

        /// <summary>
        /// Sets a product rating
        /// </summary>
        /// <param name="productId">Product identifer</param>
        /// <param name="rating">Rating</param>
        public static void SetProductRating(int productId, int rating)
        {
            if (NopContext.Current.User == null)
            {
                return;
            }
            if (NopContext.Current.User.IsGuest && !CustomerManager.AllowAnonymousUsersToSetProductRatings)
            {
                return;
            }

            if (rating < 1 || rating > 5)
                rating = 1;
            var ratedOn = DateTime.UtcNow;


            var context = ObjectContextHelper.CurrentObjectContext;
            context.Sp_ProductRatingCreate(productId, NopContext.Current.User.CustomerId,
                rating, ratedOn);

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Clears a "compare products" list
        /// </summary>
        public static void ClearCompareProducts()
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
        public static List<Product> GetCompareProducts()
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
        public static List<int> GetCompareProductsIds()
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
        public static void RemoveProductFromCompareList(int productId)
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
        public static void AddProductToCompareList(int productId)
        {
            if (!ProductManager.CompareProductsEnabled)
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
        public static List<Product> GetRecentlyViewedProducts(int number)
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
        public static List<int> GetRecentlyViewedProductsIds()
        {
            return GetRecentlyViewedProductsIds(int.MaxValue);
        }

        /// <summary>
        /// Gets a "recently viewed products" identifier list
        /// </summary>
        /// <param name="number">Number of products to load</param>
        /// <returns>"recently viewed products" list</returns>
        public static List<int> GetRecentlyViewedProductsIds(int number)
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
        public static void AddProductToRecentlyViewedList(int productId)
        {
            if (!ProductManager.RecentlyViewedProductsEnabled)
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
            int maxProducts = SettingManager.GetSettingValueInteger("Display.RecentlyViewedProductCount");
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
        public static List<Product> GetRecentlyAddedProducts(int number)
        {
            int totalRecords = 0;
            var products = ProductManager.GetAllProducts(0,
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
        public static bool DirectAddToCartAllowed(int productId, out int productVariantId)
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
                        var addToCartWarnings = ShoppingCartManager.GetShoppingCartItemWarnings(ShoppingCartTypeEnum.ShoppingCart,
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
        public static Product DuplicateProduct(int productId, string name,
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
                productCopy = InsertProduct(name, product.ShortDescription,
                    product.FullDescription, product.AdminComment,
                    product.TemplateId, product.ShowOnHomePage, product.MetaKeywords,
                    product.MetaDescription, product.MetaTitle, product.SEName,
                    product.AllowCustomerReviews, product.AllowCustomerRatings, 0, 0,
                    isPublished, product.Deleted, DateTime.UtcNow, DateTime.UtcNow);

                if (productCopy == null)
                    return null;

                var languages = LanguageManager.GetAllLanguages(true);

                //localization
                foreach (var lang in languages)
                {
                    var productLocalized = GetProductLocalizedByProductIdAndLanguageId(product.ProductId, lang.LanguageId);
                    if (productLocalized != null)
                    {
                        var productLocalizedCopy = InsertProductLocalized(productCopy.ProductId,
                            productLocalized.LanguageId,
                            productLocalized.Name,
                            productLocalized.ShortDescription,
                            productLocalized.FullDescription,
                            productLocalized.MetaKeywords,
                            productLocalized.MetaDescription,
                            productLocalized.MetaTitle,
                            productLocalized.SEName);
                    }
                }

                // product pictures
                if (copyImages)
                {
                    foreach (var productPicture in product.ProductPictures)
                    {
                        var picture = productPicture.Picture;
                        var pictureCopy = PictureManager.InsertPicture(picture.PictureBinary,
                            picture.Extension,
                            picture.IsNew);
                        InsertProductPicture(productCopy.ProductId,
                            pictureCopy.PictureId,
                            productPicture.DisplayOrder);
                    }
                }

                // product <-> categories mappings
                foreach (var productCategory in product.ProductCategories)
                {
                    CategoryManager.InsertProductCategory(productCopy.ProductId,
                        productCategory.CategoryId,
                        productCategory.IsFeaturedProduct,
                        productCategory.DisplayOrder);
                }

                // product <-> manufacturers mappings
                foreach (var productManufacturers in product.ProductManufacturers)
                {
                    ManufacturerManager.InsertProductManufacturer(productCopy.ProductId,
                        productManufacturers.ManufacturerId,
                        productManufacturers.IsFeaturedProduct,
                        productManufacturers.DisplayOrder);
                }

                // product <-> releated products mappings
                foreach (var relatedProduct in product.RelatedProducts)
                {
                    InsertRelatedProduct(productCopy.ProductId,
                        relatedProduct.ProductId2,
                        relatedProduct.DisplayOrder);
                }

                // product specifications
                foreach (var productSpecificationAttribute in SpecificationAttributeManager.GetProductSpecificationAttributesByProductId(product.ProductId))
                {
                    SpecificationAttributeManager.InsertProductSpecificationAttribute(productCopy.ProductId,
                        productSpecificationAttribute.SpecificationAttributeOptionId,
                        productSpecificationAttribute.AllowFiltering,
                        productSpecificationAttribute.ShowOnProductPage,
                        productSpecificationAttribute.DisplayOrder);
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
                            var pictureCopy = PictureManager.InsertPicture(picture.PictureBinary, picture.Extension, picture.IsNew);
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
                            var downloadCopy = DownloadManager.InsertDownload(download.UseDownloadUrl, download.DownloadUrl, download.DownloadBinary, download.ContentType, download.Filename, download.Extension, download.IsNew);
                            downloadId = downloadCopy.DownloadId;
                        }

                        if (productVariant.HasSampleDownload)
                        {
                            var sampleDownload = productVariant.SampleDownload;
                            if (sampleDownload != null)
                            {
                                var sampleDownloadCopy = DownloadManager.InsertDownload(sampleDownload.UseDownloadUrl, sampleDownload.DownloadUrl, sampleDownload.DownloadBinary, sampleDownload.ContentType, sampleDownload.Filename, sampleDownload.Extension, sampleDownload.IsNew);
                                sampleDownloadId = sampleDownloadCopy.DownloadId;
                            }
                        }
                    }

                    // product variant
                    var productVariantCopy = InsertProductVariant(productCopy.ProductId, productVariant.Name,
                        productVariant.SKU, productVariant.Description, productVariant.AdminComment, productVariant.ManufacturerPartNumber,
                        productVariant.IsGiftCard, productVariant.GiftCardType, 
                        productVariant.IsDownload, downloadId,
                        productVariant.UnlimitedDownloads, productVariant.MaxNumberOfDownloads,
                        productVariant.DownloadExpirationDays, (DownloadActivationTypeEnum)productVariant.DownloadActivationType,
                        productVariant.HasSampleDownload, sampleDownloadId,
                        productVariant.HasUserAgreement, productVariant.UserAgreementText,
                        productVariant.IsRecurring, productVariant.CycleLength,
                        productVariant.CyclePeriod, productVariant.TotalCycles,
                        productVariant.IsShipEnabled, productVariant.IsFreeShipping, productVariant.AdditionalShippingCharge,
                        productVariant.IsTaxExempt, productVariant.TaxCategoryId,
                        productVariant.ManageInventory, productVariant.StockQuantity,
                        productVariant.DisplayStockAvailability, productVariant.DisplayStockQuantity, 
                        productVariant.MinStockQuantity, productVariant.LowStockActivity,
                        productVariant.NotifyAdminForQuantityBelow, productVariant.Backorders,
                        productVariant.OrderMinimumQuantity, productVariant.OrderMaximumQuantity,
                        productVariant.WarehouseId, productVariant.DisableBuyButton,
                        productVariant.CallForPrice, productVariant.Price, productVariant.OldPrice, 
                        productVariant.ProductCost, productVariant.CustomerEntersPrice,
                        productVariant.MinimumCustomerEnteredPrice, productVariant.MaximumCustomerEnteredPrice,
                        productVariant.Weight, productVariant.Length, productVariant.Width, productVariant.Height, pictureId,
                        productVariant.AvailableStartDateTime, productVariant.AvailableEndDateTime,
                        productVariant.Published, productVariant.Deleted, productVariant.DisplayOrder, DateTime.UtcNow, DateTime.UtcNow);

                    //localization
                    foreach (var lang in languages)
                    {
                        var productVariantLocalized = GetProductVariantLocalizedByProductVariantIdAndLanguageId(productVariant.ProductVariantId, lang.LanguageId);
                        if (productVariantLocalized != null)
                        {
                            var productVariantLocalizedCopy = InsertProductVariantLocalized(productVariantCopy.ProductVariantId,
                                productVariantLocalized.LanguageId,
                                productVariantLocalized.Name,
                                productVariantLocalized.Description);
                        }
                    }

                    // product variant <-> attributes mappings
                    foreach (var productVariantAttribute in ProductAttributeManager.GetProductVariantAttributesByProductVariantId(productVariant.ProductVariantId))
                    {
                        var productVariantAttributeCopy = ProductAttributeManager.InsertProductVariantAttribute(productVariantCopy.ProductVariantId, productVariantAttribute.ProductAttributeId, productVariantAttribute.TextPrompt, productVariantAttribute.IsRequired, productVariantAttribute.AttributeControlType, productVariantAttribute.DisplayOrder);

                        // product variant attribute values
                        var productVariantAttributeValues = ProductAttributeManager.GetProductVariantAttributeValues(productVariantAttribute.ProductVariantAttributeId);
                        foreach (var productVariantAttributeValue in productVariantAttributeValues)
                        {
                            var pvavCopy = ProductAttributeManager.InsertProductVariantAttributeValue(productVariantAttributeCopy.ProductVariantAttributeId, productVariantAttributeValue.Name, productVariantAttributeValue.PriceAdjustment, productVariantAttributeValue.WeightAdjustment, productVariantAttributeValue.IsPreSelected, productVariantAttributeValue.DisplayOrder);

                            //localization
                            foreach (var lang in languages)
                            {
                                var pvavLocalized = ProductAttributeManager.GetProductVariantAttributeValueLocalizedByProductVariantAttributeValueIdAndLanguageId(productVariantAttributeValue.ProductVariantAttributeValueId, lang.LanguageId);
                                if (pvavLocalized != null)
                                {
                                    var pvavLocalizedCopy = ProductAttributeManager.InsertProductVariantAttributeValueLocalized(pvavCopy.ProductVariantAttributeValueId,
                                        pvavLocalized.LanguageId,
                                        pvavLocalized.Name);
                                }
                            }
                        }
                    }
                    foreach (var combination in ProductAttributeManager.GetAllProductVariantAttributeCombinations(productVariant.ProductVariantId))
                    {
                        ProductAttributeManager.InsertProductVariantAttributeCombination(productVariantCopy.ProductVariantId,
                              combination.AttributesXml,
                              combination.StockQuantity,
                              combination.AllowOutOfStockOrders);
                    }

                    // product variant tier prices
                    foreach (var tierPrice in productVariant.TierPrices)
                    {
                        InsertTierPrice(productVariantCopy.ProductVariantId, tierPrice.Quantity, tierPrice.Price);
                    }

                    // product variant <-> discounts mapping
                    foreach (var discount in productVariant.AllDiscounts)
                    {
                        DiscountManager.AddDiscountToProductVariant(productVariantCopy.ProductVariantId, discount.DiscountId);
                    }

                    // prices by customer role
                    foreach (var crpp in productVariant.CustomerRoleProductPrices)
                    {
                        ProductManager.InsertCustomerRoleProductPrice(crpp.CustomerRoleId,
                            productVariantCopy.ProductVariantId, crpp.Price);
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
        public static List<Product> GetCrosssellProductsByShoppingCart(ShoppingCart cart)
        {
            List<Product> result = new List<Product>();

            if (ProductManager.CrossSellsNumber == 0)
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
                        if (result.Count >= ProductManager.CrossSellsNumber)
                            return result;
                    }
                }
            }
            return result;
        }

        #endregion

        #region Product variants

        /// <summary>
        /// Remove a product variant picture
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        public static void RemoveProductVariantPicture(int productVariantId)
        {
            var productVariant = GetProductVariantById(productVariantId);
            if (productVariant != null)
            {
                UpdateProductVariant(productVariant.ProductVariantId, productVariant.ProductId, productVariant.Name,
                    productVariant.SKU, productVariant.Description, productVariant.AdminComment, productVariant.ManufacturerPartNumber,
                    productVariant.IsGiftCard, productVariant.GiftCardType, 
                    productVariant.IsDownload, productVariant.DownloadId,
                    productVariant.UnlimitedDownloads, productVariant.MaxNumberOfDownloads,
                    productVariant.DownloadExpirationDays, (DownloadActivationTypeEnum)productVariant.DownloadActivationType,
                    productVariant.HasSampleDownload, productVariant.SampleDownloadId,
                    productVariant.HasUserAgreement, productVariant.UserAgreementText,
                    productVariant.IsRecurring, productVariant.CycleLength,
                    productVariant.CyclePeriod, productVariant.TotalCycles,
                    productVariant.IsShipEnabled, productVariant.IsFreeShipping, productVariant.AdditionalShippingCharge,
                    productVariant.IsTaxExempt, productVariant.TaxCategoryId, productVariant.ManageInventory,
                    productVariant.StockQuantity, productVariant.DisplayStockAvailability,
                    productVariant.DisplayStockQuantity, productVariant.MinStockQuantity,
                    productVariant.LowStockActivity, productVariant.NotifyAdminForQuantityBelow,
                    productVariant.Backorders, productVariant.OrderMinimumQuantity,
                    productVariant.OrderMaximumQuantity, productVariant.WarehouseId,
                    productVariant.DisableBuyButton, productVariant.CallForPrice,
                    productVariant.Price, productVariant.OldPrice, productVariant.ProductCost,
                    productVariant.CustomerEntersPrice,
                    productVariant.MinimumCustomerEnteredPrice, 
                    productVariant.MaximumCustomerEnteredPrice,
                    productVariant.Weight, productVariant.Length, productVariant.Width, productVariant.Height, 0,
                    productVariant.AvailableStartDateTime, productVariant.AvailableEndDateTime,
                    productVariant.Published, productVariant.Deleted,
                    productVariant.DisplayOrder, productVariant.CreatedOn, productVariant.UpdatedOn);
            }
        }

        /// <summary>
        /// Get low stock product variants
        /// </summary>
        /// <returns>Result</returns>
        public static List<ProductVariant> GetLowStockProductVariants()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pv in context.ProductVariants
                        orderby pv.MinStockQuantity
                        where !pv.Deleted &&
                        pv.MinStockQuantity >= pv.StockQuantity
                        select pv;
            var productVariants = query.ToList();
            return productVariants;
        }

        /// <summary>
        /// Remove a product variant download
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        public static void RemoveProductVariantDownload(int productVariantId)
        {
            var productVariant = GetProductVariantById(productVariantId);
            if (productVariant != null)
            {
                UpdateProductVariant(productVariant.ProductVariantId, productVariant.ProductId, productVariant.Name,
                    productVariant.SKU, productVariant.Description, productVariant.AdminComment,
                    productVariant.ManufacturerPartNumber, productVariant.IsGiftCard,
                    productVariant.GiftCardType, productVariant.IsDownload, 0,
                    productVariant.UnlimitedDownloads, productVariant.MaxNumberOfDownloads,
                    productVariant.DownloadExpirationDays, (DownloadActivationTypeEnum)productVariant.DownloadActivationType,
                    productVariant.HasSampleDownload, productVariant.SampleDownloadId,
                    productVariant.HasUserAgreement, productVariant.UserAgreementText,
                    productVariant.IsRecurring, productVariant.CycleLength,
                    productVariant.CyclePeriod, productVariant.TotalCycles,
                    productVariant.IsShipEnabled, productVariant.IsFreeShipping, productVariant.AdditionalShippingCharge,
                    productVariant.IsTaxExempt, productVariant.TaxCategoryId, productVariant.ManageInventory,
                    productVariant.StockQuantity, productVariant.DisplayStockAvailability,
                    productVariant.DisplayStockQuantity, productVariant.MinStockQuantity,
                    productVariant.LowStockActivity, productVariant.NotifyAdminForQuantityBelow,
                    productVariant.Backorders, productVariant.OrderMinimumQuantity,
                    productVariant.OrderMaximumQuantity, productVariant.WarehouseId,
                    productVariant.DisableBuyButton, productVariant.CallForPrice, 
                    productVariant.Price, productVariant.OldPrice, productVariant.ProductCost,
                    productVariant.CustomerEntersPrice,
                    productVariant.MinimumCustomerEnteredPrice, 
                    productVariant.MaximumCustomerEnteredPrice,
                    productVariant.Weight, productVariant.Length, productVariant.Width, productVariant.Height,
                    productVariant.PictureId, productVariant.AvailableStartDateTime, productVariant.AvailableEndDateTime,
                    productVariant.Published, productVariant.Deleted,
                    productVariant.DisplayOrder, productVariant.CreatedOn, productVariant.UpdatedOn);
            }
        }

        /// <summary>
        /// Remove a product variant sample download
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        public static void RemoveProductVariantSampleDownload(int productVariantId)
        {
            var productVariant = GetProductVariantById(productVariantId);
            if (productVariant != null)
            {
                UpdateProductVariant(productVariant.ProductVariantId, productVariant.ProductId, productVariant.Name,
                    productVariant.SKU, productVariant.Description, productVariant.AdminComment,
                    productVariant.ManufacturerPartNumber, productVariant.IsGiftCard,
                    productVariant.GiftCardType, productVariant.IsDownload, productVariant.DownloadId,
                    productVariant.UnlimitedDownloads, productVariant.MaxNumberOfDownloads,
                    productVariant.DownloadExpirationDays, (DownloadActivationTypeEnum)productVariant.DownloadActivationType,
                    productVariant.HasSampleDownload, 0,
                    productVariant.HasUserAgreement, productVariant.UserAgreementText,
                    productVariant.IsRecurring, productVariant.CycleLength,
                    productVariant.CyclePeriod, productVariant.TotalCycles,
                    productVariant.IsShipEnabled, productVariant.IsFreeShipping,
                    productVariant.AdditionalShippingCharge, productVariant.IsTaxExempt,
                    productVariant.TaxCategoryId, productVariant.ManageInventory,
                    productVariant.StockQuantity, productVariant.DisplayStockAvailability,
                    productVariant.DisplayStockQuantity, productVariant.MinStockQuantity,
                    productVariant.LowStockActivity, productVariant.NotifyAdminForQuantityBelow,
                    productVariant.Backorders, productVariant.OrderMinimumQuantity,
                    productVariant.OrderMaximumQuantity, productVariant.WarehouseId,
                    productVariant.DisableBuyButton, productVariant.CallForPrice, 
                    productVariant.Price, productVariant.OldPrice,
                    productVariant.ProductCost, productVariant.CustomerEntersPrice,
                    productVariant.MinimumCustomerEnteredPrice, productVariant.MaximumCustomerEnteredPrice,
                    productVariant.Weight, productVariant.Length, productVariant.Width, productVariant.Height,
                    productVariant.PictureId, productVariant.AvailableStartDateTime, productVariant.AvailableEndDateTime,
                    productVariant.Published, productVariant.Deleted,
                    productVariant.DisplayOrder, productVariant.CreatedOn, productVariant.UpdatedOn);
            }
        }

        /// <summary>
        /// Gets a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Product variant</returns>
        public static ProductVariant GetProductVariantById(int productVariantId)
        {
            if (productVariantId == 0)
                return null;

            string key = string.Format(PRODUCTVARIANTS_BY_ID_KEY, productVariantId);
            object obj2 = NopRequestCache.Get(key);
            if (ProductManager.CacheEnabled && (obj2 != null))
            {
                return (ProductVariant)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pv in context.ProductVariants
                        where pv.ProductVariantId == productVariantId
                        select pv;
            var productVariant = query.SingleOrDefault();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.Add(key, productVariant);
            }
            return productVariant;
        }

        /// <summary>
        /// Gets a product variant by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Product variant</returns>
        public static ProductVariant GetProductVariantBySKU(string sku)
        {
            if (String.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pv in context.ProductVariants
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
        public static List<ProductVariant> GetAllProductVariants(int categoryId,
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

            var context = ObjectContextHelper.CurrentObjectContext;
            var productVariants = context.Sp_ProductVariantLoadAll(categoryId,
                manufacturerId, keywords, showHidden, pageSize,
                pageIndex, out totalRecords);
            return productVariants;
        }

        /// <summary>
        /// Inserts a product variant
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <param name="name">The name</param>
        /// <param name="sku">The SKU</param>
        /// <param name="description">The description</param>
        /// <param name="adminComment">The admin comment</param>
        /// <param name="manufacturerPartNumber">The manufacturer part number</param>
        /// <param name="isGiftCard">A value indicating whether the product variant is gift card</param>
        /// <param name="giftCardType">Gift card type</param>
        /// <param name="isDownload">A value indicating whether the product variant is download</param>
        /// <param name="downloadId">The download identifier</param>
        /// <param name="unlimitedDownloads">The value indicating whether this downloadable product can be downloaded unlimited number of times</param>
        /// <param name="maxNumberOfDownloads">The maximum number of downloads</param>
        /// <param name="downloadExpirationDays">The number of days during customers keeps access to the file</param>
        /// <param name="downloadActivationType">The download activation type</param>
        /// <param name="hasSampleDownload">The value indicating whether the product variant has a sample download file</param>
        /// <param name="sampleDownloadId">The sample download identifier</param>
        /// <param name="hasUserAgreement">A value indicating whether the product variant has a user agreement</param>
        /// <param name="userAgreementText">The text of user agreement</param>
        /// <param name="isRecurring">A value indicating whether the product variant is recurring</param>
        /// <param name="cycleLength">The cycle length</param>
        /// <param name="cyclePeriod">The cycle period</param>
        /// <param name="totalCycles">The total cycles</param>
        /// <param name="isShipEnabled">A value indicating whether the entity is ship enabled</param>
        /// <param name="isFreeShipping">A value indicating whether the entity is free shipping</param>
        /// <param name="additionalShippingCharge">The additional shipping charge</param>
        /// <param name="isTaxExempt">A value indicating whether the product variant is marked as tax exempt</param>
        /// <param name="taxCategoryId">The tax category identifier</param>
        /// <param name="manageInventory">The value indicating how to manage inventory</param>
        /// <param name="stockQuantity">The stock quantity</param>
        /// <param name="displayStockAvailability">The value indicating whether to display stock availability</param>
        /// <param name="displayStockQuantity">The value indicating whether to display stock quantity</param>
        /// <param name="minStockQuantity">The minimum stock quantity</param>
        /// <param name="lowStockActivity">The low stock activity</param>
        /// <param name="notifyAdminForQuantityBelow">The quantity when admin should be notified</param>
        /// <param name="backorders">The backorders mode</param>
        /// <param name="orderMinimumQuantity">The order minimum quantity</param>
        /// <param name="orderMaximumQuantity">The order maximum quantity</param>
        /// <param name="warehouseId">The warehouse identifier</param>
        /// <param name="disableBuyButton">A value indicating whether to disable buy button</param>
        /// <param name="callForPrice">A value indicating whether to show "Call for Pricing" or "Call for quote" instead of price</param>
        /// <param name="price">The price</param>
        /// <param name="oldPrice">The old price</param>
        /// <param name="productCost">The product cost</param>
        /// <param name="customerEntersPrice">The value indicating whether a customer enters price</param>
        /// <param name="minimumCustomerEnteredPrice">The minimum price entered by a customer</param>
        /// <param name="maximumCustomerEnteredPrice">The maximum price entered by a customer</param>
        /// <param name="weight">The weight</param>
        /// <param name="length">The length</param>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        /// <param name="pictureId">The picture identifier</param>
        /// <param name="availableStartDateTime">The available start date and time</param>
        /// <param name="availableEndDateTime">The available end date and time</param>
        /// <param name="published">A value indicating whether the entity is published</param>
        /// <param name="deleted">A value indicating whether the entity has been deleted</param>
        /// <param name="displayOrder">The display order</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>Product variant</returns>
        public static ProductVariant InsertProductVariant(int productId,
            string name, string sku,
            string description, string adminComment, string manufacturerPartNumber,
            bool isGiftCard, int giftCardType, 
            bool isDownload, int downloadId, bool unlimitedDownloads,
            int maxNumberOfDownloads, int? downloadExpirationDays,
            DownloadActivationTypeEnum downloadActivationType, bool hasSampleDownload,
            int sampleDownloadId, bool hasUserAgreement,
            string userAgreementText, bool isRecurring,
            int cycleLength, int cyclePeriod, int totalCycles,
            bool isShipEnabled, bool isFreeShipping,
            decimal additionalShippingCharge, bool isTaxExempt, int taxCategoryId,
            int manageInventory, int stockQuantity, bool displayStockAvailability,
            bool displayStockQuantity, int minStockQuantity, LowStockActivityEnum lowStockActivity,
            int notifyAdminForQuantityBelow, int backorders,
            int orderMinimumQuantity, int orderMaximumQuantity,
            int warehouseId, bool disableBuyButton, 
            bool callForPrice, decimal price,
            decimal oldPrice, decimal productCost, bool customerEntersPrice,
            decimal minimumCustomerEnteredPrice, decimal maximumCustomerEnteredPrice,
            decimal weight, decimal length, decimal width, decimal height, int pictureId,
            DateTime? availableStartDateTime, DateTime? availableEndDateTime,
            bool published, bool deleted, int displayOrder,
            DateTime createdOn, DateTime updatedOn)
        {
            sku = sku.Trim();

            name = CommonHelper.EnsureMaximumLength(name, 400);
            sku = CommonHelper.EnsureMaximumLength(sku, 100);
            description = CommonHelper.EnsureMaximumLength(description, 4000);
            adminComment = CommonHelper.EnsureMaximumLength(adminComment, 4000);
            manufacturerPartNumber = CommonHelper.EnsureMaximumLength(manufacturerPartNumber, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            var productVariant = context.ProductVariants.CreateObject();
            productVariant.ProductId = productId;
            productVariant.Name = name;
            productVariant.SKU = sku;
            productVariant.Description = description;
            productVariant.AdminComment = adminComment;
            productVariant.ManufacturerPartNumber = manufacturerPartNumber;
            productVariant.IsGiftCard = isGiftCard;
            productVariant.GiftCardType = giftCardType;
            productVariant.IsDownload = isDownload;
            productVariant.DownloadId = downloadId;
            productVariant.UnlimitedDownloads = unlimitedDownloads;
            productVariant.MaxNumberOfDownloads = maxNumberOfDownloads;
            productVariant.DownloadExpirationDays = downloadExpirationDays;
            productVariant.DownloadActivationType = (int)downloadActivationType;
            productVariant.HasSampleDownload = hasSampleDownload;
            productVariant.SampleDownloadId = sampleDownloadId;
            productVariant.HasUserAgreement = hasUserAgreement;
            productVariant.UserAgreementText = userAgreementText;
            productVariant.IsRecurring = isRecurring;
            productVariant.CycleLength = cycleLength;
            productVariant.CyclePeriod = cyclePeriod;
            productVariant.TotalCycles = totalCycles;
            productVariant.IsShipEnabled = isShipEnabled;
            productVariant.IsFreeShipping = isFreeShipping;
            productVariant.AdditionalShippingCharge = additionalShippingCharge;
            productVariant.IsTaxExempt = isTaxExempt;
            productVariant.TaxCategoryId = taxCategoryId;
            productVariant.ManageInventory = manageInventory;
            productVariant.StockQuantity = stockQuantity;
            productVariant.DisplayStockAvailability = displayStockAvailability;
            productVariant.DisplayStockQuantity = displayStockQuantity;            
            productVariant.MinStockQuantity = minStockQuantity;
            productVariant.LowStockActivityId = (int)lowStockActivity;
            productVariant.NotifyAdminForQuantityBelow = notifyAdminForQuantityBelow;
            productVariant.Backorders = backorders;
            productVariant.OrderMinimumQuantity = orderMinimumQuantity;
            productVariant.OrderMaximumQuantity = orderMaximumQuantity;
            productVariant.WarehouseId = warehouseId;
            productVariant.DisableBuyButton = disableBuyButton;
            productVariant.CallForPrice = callForPrice;
            productVariant.Price = price;
            productVariant.OldPrice = oldPrice;
            productVariant.ProductCost = productCost;
            productVariant.CustomerEntersPrice = customerEntersPrice;
            productVariant.MinimumCustomerEnteredPrice = minimumCustomerEnteredPrice;
            productVariant.MaximumCustomerEnteredPrice = maximumCustomerEnteredPrice;
            productVariant.Weight = weight;
            productVariant.Length = length;
            productVariant.Width = width;
            productVariant.Height = height;
            productVariant.PictureId = pictureId;
            productVariant.AvailableStartDateTime = availableStartDateTime;
            productVariant.AvailableEndDateTime = availableEndDateTime;
            productVariant.Published = published;
            productVariant.Deleted = deleted;
            productVariant.DisplayOrder = displayOrder;
            productVariant.CreatedOn = createdOn;
            productVariant.UpdatedOn = updatedOn;

            context.ProductVariants.AddObject(productVariant);
            context.SaveChanges();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }

            //raise event             
            EventContext.Current.OnProductVariantCreated(null,
                new ProductVariantEventArgs() { ProductVariant = productVariant });
            
            return productVariant;
        }

        /// <summary>
        /// Updates the product variant
        /// </summary>
        /// <param name="productVariantId">The product variant identifier</param>
        /// <param name="productId">The product identifier</param>
        /// <param name="name">The name</param>
        /// <param name="sku">The SKU</param>
        /// <param name="description">The description</param>
        /// <param name="adminComment">The admin comment</param>
        /// <param name="manufacturerPartNumber">The manufacturer part number</param>
        /// <param name="isGiftCard">A value indicating whether the product variant is gift card</param>
        /// <param name="giftCardType">Gift card type</param>
        /// <param name="isDownload">A value indicating whether the product variant is download</param>
        /// <param name="downloadId">The download identifier</param>
        /// <param name="unlimitedDownloads">The value indicating whether this downloadable product can be downloaded unlimited number of times</param>
        /// <param name="maxNumberOfDownloads">The maximum number of downloads</param>
        /// <param name="downloadExpirationDays">The number of days during customers keeps access to the file</param>
        /// <param name="downloadActivationType">The download activation type</param>
        /// <param name="hasSampleDownload">The value indicating whether the product variant has a sample download file</param>
        /// <param name="sampleDownloadId">The sample download identifier</param>
        /// <param name="hasUserAgreement">A value indicating whether the product variant has a user agreement</param>
        /// <param name="userAgreementText">The text of user agreement</param>
        /// <param name="isRecurring">A value indicating whether the product variant is recurring</param>
        /// <param name="cycleLength">The cycle length</param>
        /// <param name="cyclePeriod">The cycle period</param>
        /// <param name="totalCycles">The total cycles</param>
        /// <param name="isShipEnabled">A value indicating whether the entity is ship enabled</param>
        /// <param name="isFreeShipping">A value indicating whether the entity is free shipping</param>
        /// <param name="additionalShippingCharge">The additional shipping charge</param>
        /// <param name="isTaxExempt">A value indicating whether the product variant is marked as tax exempt</param>
        /// <param name="taxCategoryId">The tax category identifier</param>
        /// <param name="manageInventory">The value indicating how to manage inventory</param>
        /// <param name="stockQuantity">The stock quantity</param>
        /// <param name="displayStockAvailability">The value indicating whether to display stock availability</param>
        /// <param name="displayStockQuantity">The value indicating whether to display stock quantity</param>
        /// <param name="minStockQuantity">The minimum stock quantity</param>
        /// <param name="lowStockActivity">The low stock activity</param>
        /// <param name="notifyAdminForQuantityBelow">The quantity when admin should be notified</param>
        /// <param name="backorders">The backorders mode</param>
        /// <param name="orderMinimumQuantity">The order minimum quantity</param>
        /// <param name="orderMaximumQuantity">The order maximum quantity</param>
        /// <param name="warehouseId">The warehouse identifier</param>
        /// <param name="disableBuyButton">A value indicating whether to disable buy button</param>
        /// <param name="callForPrice">A value indicating whether to show "Call for Pricing" or "Call for quote" instead of price</param>
        /// <param name="price">The price</param>
        /// <param name="oldPrice">The old price</param>
        /// <param name="productCost">The product cost</param>
        /// <param name="customerEntersPrice">The value indicating whether a customer enters price</param>
        /// <param name="minimumCustomerEnteredPrice">The minimum price entered by a customer</param>
        /// <param name="maximumCustomerEnteredPrice">The maximum price entered by a customer</param>
        /// <param name="weight">The weight</param>
        /// <param name="length">The length</param>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        /// <param name="pictureId">The picture identifier</param>
        /// <param name="availableStartDateTime">The available start date and time</param>
        /// <param name="availableEndDateTime">The available end date and time</param>
        /// <param name="published">A value indicating whether the entity is published</param>
        /// <param name="deleted">A value indicating whether the entity has been deleted</param>
        /// <param name="displayOrder">The display order</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>Product variant</returns>
        public static ProductVariant UpdateProductVariant(int productVariantId,
            int productId, string name, string sku,
            string description, string adminComment, string manufacturerPartNumber,
            bool isGiftCard, int giftCardType, 
            bool isDownload, int downloadId, bool unlimitedDownloads,
            int maxNumberOfDownloads, int? downloadExpirationDays,
            DownloadActivationTypeEnum downloadActivationType, bool hasSampleDownload,
            int sampleDownloadId, bool hasUserAgreement,
            string userAgreementText, bool isRecurring,
            int cycleLength, int cyclePeriod, int totalCycles,
            bool isShipEnabled, bool isFreeShipping,
            decimal additionalShippingCharge, bool isTaxExempt, int taxCategoryId,
            int manageInventory, int stockQuantity, bool displayStockAvailability,
            bool displayStockQuantity, int minStockQuantity, LowStockActivityEnum lowStockActivity,
            int notifyAdminForQuantityBelow, int backorders,
            int orderMinimumQuantity, int orderMaximumQuantity,
            int warehouseId, bool disableBuyButton, 
            bool callForPrice, decimal price,
            decimal oldPrice, decimal productCost, bool customerEntersPrice,
            decimal minimumCustomerEnteredPrice, decimal maximumCustomerEnteredPrice,
            decimal weight, decimal length, decimal width, decimal height, int pictureId,
            DateTime? availableStartDateTime, DateTime? availableEndDateTime,
            bool published, bool deleted, int displayOrder,
            DateTime createdOn, DateTime updatedOn)
        {
            sku = sku.Trim();

            name = CommonHelper.EnsureMaximumLength(name, 400);
            sku = CommonHelper.EnsureMaximumLength(sku, 100);
            description = CommonHelper.EnsureMaximumLength(description, 4000);
            adminComment = CommonHelper.EnsureMaximumLength(adminComment, 4000);
            manufacturerPartNumber = CommonHelper.EnsureMaximumLength(manufacturerPartNumber, 100);

            var productVariant = GetProductVariantById(productVariantId);
            if (productVariant == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productVariant))
                context.ProductVariants.Attach(productVariant);

            productVariant.ProductId = productId;
            productVariant.Name = name;
            productVariant.SKU = sku;
            productVariant.Description = description;
            productVariant.AdminComment = adminComment;
            productVariant.ManufacturerPartNumber = manufacturerPartNumber;
            productVariant.IsGiftCard = isGiftCard;
            productVariant.GiftCardType = giftCardType;
            productVariant.IsDownload = isDownload;
            productVariant.DownloadId = downloadId;
            productVariant.UnlimitedDownloads = unlimitedDownloads;
            productVariant.MaxNumberOfDownloads = maxNumberOfDownloads;
            productVariant.DownloadExpirationDays = downloadExpirationDays;
            productVariant.DownloadActivationType = (int)downloadActivationType;
            productVariant.HasSampleDownload = hasSampleDownload;
            productVariant.SampleDownloadId = sampleDownloadId;
            productVariant.HasUserAgreement = hasUserAgreement;
            productVariant.UserAgreementText = userAgreementText;
            productVariant.IsRecurring = isRecurring;
            productVariant.CycleLength = cycleLength;
            productVariant.CyclePeriod = cyclePeriod;
            productVariant.TotalCycles = totalCycles;
            productVariant.IsShipEnabled = isShipEnabled;
            productVariant.IsFreeShipping = isFreeShipping;
            productVariant.AdditionalShippingCharge = additionalShippingCharge;
            productVariant.IsTaxExempt = isTaxExempt;
            productVariant.TaxCategoryId = taxCategoryId;
            productVariant.ManageInventory = manageInventory;
            productVariant.StockQuantity = stockQuantity;
            productVariant.DisplayStockAvailability = displayStockAvailability;
            productVariant.DisplayStockQuantity = displayStockQuantity;
            productVariant.MinStockQuantity = minStockQuantity;
            productVariant.LowStockActivityId = (int)lowStockActivity;
            productVariant.NotifyAdminForQuantityBelow = notifyAdminForQuantityBelow;
            productVariant.Backorders = backorders;
            productVariant.OrderMinimumQuantity = orderMinimumQuantity;
            productVariant.OrderMaximumQuantity = orderMaximumQuantity;
            productVariant.WarehouseId = warehouseId;
            productVariant.DisableBuyButton = disableBuyButton;
            productVariant.CallForPrice = callForPrice;
            productVariant.Price = price;
            productVariant.OldPrice = oldPrice;
            productVariant.ProductCost = productCost;
            productVariant.CustomerEntersPrice = customerEntersPrice;
            productVariant.MinimumCustomerEnteredPrice = minimumCustomerEnteredPrice;
            productVariant.MaximumCustomerEnteredPrice = maximumCustomerEnteredPrice;
            productVariant.Weight = weight;
            productVariant.Length = length;
            productVariant.Width = width;
            productVariant.Height = height;
            productVariant.PictureId = pictureId;
            productVariant.AvailableStartDateTime = availableStartDateTime;
            productVariant.AvailableEndDateTime = availableEndDateTime;
            productVariant.Published = published;
            productVariant.Deleted = deleted;
            productVariant.DisplayOrder = displayOrder;
            productVariant.CreatedOn = createdOn;
            productVariant.UpdatedOn = updatedOn;
            context.SaveChanges();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }

            //raise event             
            EventContext.Current.OnProductVariantUpdated(null,
                new ProductVariantEventArgs() { ProductVariant = productVariant });
            

            return productVariant;
        }

        /// <summary>
        /// Gets product variants by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>Product variant collection</returns>
        public static List<ProductVariant> GetProductVariantsByProductId(int productId)
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
        public static List<ProductVariant> GetProductVariantsByProductId(int productId, bool showHidden)
        {
            string key = string.Format(PRODUCTVARIANTS_ALL_KEY, showHidden, productId);
            object obj2 = NopRequestCache.Get(key);
            if (ProductManager.CacheEnabled && (obj2 != null))
            {
                return (List<ProductVariant>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = (IQueryable<ProductVariant>)context.ProductVariants;
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

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.Add(key, productVariants);
            }
            return productVariants;
        }

        /// <summary>
        /// Gets restricted product variants by discount identifier
        /// </summary>
        /// <param name="discountId">The discount identifier</param>
        /// <returns>Product variant collection</returns>
        public static List<ProductVariant> GetProductVariantsRestrictedByDiscountId(int discountId)
        {
            if (discountId == 0)
                return new List<ProductVariant>();

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pv in context.ProductVariants
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
        public static ProductVariantLocalized GetProductVariantLocalizedById(int productVariantLocalizedId)
        {
            if (productVariantLocalizedId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pvl in context.ProductVariantLocalized
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
        public static List<ProductVariantLocalized> GetProductVariantLocalizedByProductVariantId(int productVariantId)
        {
            if (productVariantId == 0)
                return new List<ProductVariantLocalized>();

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pvl in context.ProductVariantLocalized
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
        public static ProductVariantLocalized GetProductVariantLocalizedByProductVariantIdAndLanguageId(int productVariantId, int languageId)
        {
            if (productVariantId == 0 || languageId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pvl in context.ProductVariantLocalized
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
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">Name text</param>
        /// <param name="description">Description text</param>
        /// <returns>Product variant content</returns>
        public static ProductVariantLocalized InsertProductVariantLocalized(int productVariantId,
            int languageId, string name, string description)
        {
            name = CommonHelper.EnsureMaximumLength(name, 400);

            var context = ObjectContextHelper.CurrentObjectContext;

            var productVariantLocalized = context.ProductVariantLocalized.CreateObject();
            productVariantLocalized.ProductVariantId = productVariantId;
            productVariantLocalized.LanguageId = languageId;
            productVariantLocalized.Name = name;
            productVariantLocalized.Description = description;

            context.ProductVariantLocalized.AddObject(productVariantLocalized);
            context.SaveChanges();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }

            return productVariantLocalized;
        }

        /// <summary>
        /// Update a localized product variant
        /// </summary>
        /// <param name="productVariantLocalizedId">Localized product variant identifier</param>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">Name text</param>
        /// <param name="description">Description text</param>
        /// <returns>Product variant content</returns>
        public static ProductVariantLocalized UpdateProductVariantLocalized(int productVariantLocalizedId,
            int productVariantId, int languageId, string name, string description)
        {
            name = CommonHelper.EnsureMaximumLength(name, 400);

            var productVariantLocalized = GetProductVariantLocalizedById(productVariantLocalizedId);
            if (productVariantLocalized == null)
                return null;
            
            bool allFieldsAreEmpty = string.IsNullOrEmpty(name) &&
               string.IsNullOrEmpty(description);

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productVariantLocalized))
                context.ProductVariantLocalized.Attach(productVariantLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                context.DeleteObject(productVariantLocalized);
                context.SaveChanges();
            }
            else
            {
                productVariantLocalized.ProductVariantId = productVariantId;
                productVariantLocalized.LanguageId = languageId;
                productVariantLocalized.Name = name;
                productVariantLocalized.Description = description;
                context.SaveChanges();
            }

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }

            return productVariantLocalized;
        }

        /// <summary>
        /// Marks a product variant as deleted
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        public static void MarkProductVariantAsDeleted(int productVariantId)
        {
            var productVariant = GetProductVariantById(productVariantId);
            if (productVariant != null)
            {
                productVariant = UpdateProductVariant(productVariant.ProductVariantId, 
                    productVariant.ProductId, productVariant.Name,
                    productVariant.SKU, productVariant.Description, 
                    productVariant.AdminComment, productVariant.ManufacturerPartNumber,
                    productVariant.IsGiftCard, productVariant.GiftCardType, productVariant.IsDownload, 
                    productVariant.DownloadId, productVariant.UnlimitedDownloads, 
                    productVariant.MaxNumberOfDownloads,
                    productVariant.DownloadExpirationDays, (DownloadActivationTypeEnum)productVariant.DownloadActivationType,
                    productVariant.HasSampleDownload, productVariant.SampleDownloadId,
                    productVariant.HasUserAgreement, productVariant.UserAgreementText,
                    productVariant.IsRecurring, productVariant.CycleLength,
                    productVariant.CyclePeriod, productVariant.TotalCycles,
                    productVariant.IsShipEnabled, productVariant.IsFreeShipping,
                    productVariant.AdditionalShippingCharge,
                    productVariant.IsTaxExempt, productVariant.TaxCategoryId,
                    productVariant.ManageInventory, productVariant.StockQuantity,
                    productVariant.DisplayStockAvailability, productVariant.DisplayStockQuantity,
                    productVariant.MinStockQuantity, productVariant.LowStockActivity,
                    productVariant.NotifyAdminForQuantityBelow, productVariant.Backorders,
                    productVariant.OrderMinimumQuantity, productVariant.OrderMaximumQuantity,
                    productVariant.WarehouseId, productVariant.DisableBuyButton,
                    productVariant.CallForPrice, productVariant.Price, productVariant.OldPrice,
                    productVariant.ProductCost, productVariant.CustomerEntersPrice,
                    productVariant.MinimumCustomerEnteredPrice, productVariant.MaximumCustomerEnteredPrice,
                    productVariant.Weight, productVariant.Length, productVariant.Width, productVariant.Height, productVariant.PictureId,
                    productVariant.AvailableStartDateTime, productVariant.AvailableEndDateTime,
                    productVariant.Published, true, productVariant.DisplayOrder, productVariant.CreatedOn, productVariant.UpdatedOn);
            }
        }

        /// <summary>
        /// Adjusts inventory
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="decrease">A value indicating whether to increase or descrease product variant stock quantity</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        public static void AdjustInventory(int productVariantId, bool decrease,
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
                            MessageManager.SendQuantityBelowStoreOwnerNotification(productVariant, LocalizationManager.DefaultAdminLanguage.LanguageId);
                        }

                        productVariant = UpdateProductVariant(productVariant.ProductVariantId, productVariant.ProductId, productVariant.Name,
                             productVariant.SKU, productVariant.Description, productVariant.AdminComment, productVariant.ManufacturerPartNumber,
                             productVariant.IsGiftCard, productVariant.GiftCardType, 
                             productVariant.IsDownload, productVariant.DownloadId,
                             productVariant.UnlimitedDownloads, productVariant.MaxNumberOfDownloads,
                             productVariant.DownloadExpirationDays, (DownloadActivationTypeEnum)productVariant.DownloadActivationType,
                             productVariant.HasSampleDownload, productVariant.SampleDownloadId,
                             productVariant.HasUserAgreement, productVariant.UserAgreementText,
                             productVariant.IsRecurring, productVariant.CycleLength,
                             productVariant.CyclePeriod, productVariant.TotalCycles,
                             productVariant.IsShipEnabled, productVariant.IsFreeShipping, productVariant.AdditionalShippingCharge,
                             productVariant.IsTaxExempt, productVariant.TaxCategoryId,
                             productVariant.ManageInventory, newStockQuantity,
                             productVariant.DisplayStockAvailability, productVariant.DisplayStockQuantity,
                             productVariant.MinStockQuantity, productVariant.LowStockActivity,
                             productVariant.NotifyAdminForQuantityBelow, productVariant.Backorders,
                             productVariant.OrderMinimumQuantity, productVariant.OrderMaximumQuantity,
                             productVariant.WarehouseId, newDisableBuyButton,
                             productVariant.CallForPrice, productVariant.Price,
                             productVariant.OldPrice, productVariant.ProductCost, 
                             productVariant.CustomerEntersPrice,
                             productVariant.MinimumCustomerEnteredPrice, 
                             productVariant.MaximumCustomerEnteredPrice,
                             productVariant.Weight, productVariant.Length, productVariant.Width,
                             productVariant.Height, productVariant.PictureId,
                             productVariant.AvailableStartDateTime, productVariant.AvailableEndDateTime,
                             newPublished, productVariant.Deleted, productVariant.DisplayOrder, productVariant.CreatedOn, productVariant.UpdatedOn);

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
                                UpdateProduct(product.ProductId, product.Name, product.ShortDescription,
                                    product.FullDescription, product.AdminComment,
                                    product.TemplateId, product.ShowOnHomePage, product.MetaKeywords, product.MetaDescription,
                                    product.MetaTitle, product.SEName, product.AllowCustomerReviews, product.AllowCustomerRatings, product.RatingSum,
                                    product.TotalRatingVotes, false, product.Deleted, product.CreatedOn, product.UpdatedOn);
                            }
                        }
                    }
                    break;
                case ManageInventoryMethodEnum.ManageStockByAttributes:
                    {
                        var combination = ProductAttributeManager.FindProductVariantAttributeCombination(productVariant.ProductVariantId, attributesXml);
                        if (combination != null)
                        {
                            int newStockQuantity = 0;
                            if (decrease)
                                newStockQuantity = combination.StockQuantity - quantity;
                            else
                                newStockQuantity = combination.StockQuantity + quantity;

                            combination = ProductAttributeManager.UpdateProductVariantAttributeCombination(combination.ProductVariantAttributeCombinationId,
                                combination.ProductVariantId, combination.AttributesXml, newStockQuantity, combination.AllowOutOfStockOrders);
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
        public static void DeleteProductPicture(int productPictureId)
        {
            var productPicture = GetProductPictureById(productPictureId);
            if (productPicture == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productPicture))
                context.ProductPictures.Attach(productPicture);
            context.DeleteObject(productPicture);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets a product picture mapping
        /// </summary>
        /// <param name="productPictureId">Product picture mapping identifier</param>
        /// <returns>Product picture mapping</returns>
        public static ProductPicture GetProductPictureById(int productPictureId)
        {
            if (productPictureId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pp in context.ProductPictures
                        where pp.ProductPictureId == productPictureId
                        select pp;
            var productPicture = query.SingleOrDefault();
            return productPicture;
        }

        /// <summary>
        /// Inserts a product picture mapping
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="pictureId">Picture identifier</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Product picture mapping</returns>
        public static ProductPicture InsertProductPicture(int productId,
            int pictureId, int displayOrder)
        {
            var context = ObjectContextHelper.CurrentObjectContext;

            var productPicture = context.ProductPictures.CreateObject();
            productPicture.ProductId = productId;
            productPicture.PictureId = pictureId;
            productPicture.DisplayOrder = displayOrder;

            context.ProductPictures.AddObject(productPicture);
            context.SaveChanges();

            return productPicture;
        }

        /// <summary>
        /// Updates the product picture mapping
        /// </summary>
        /// <param name="productPictureId">Product picture mapping identifier</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="pictureId">Picture identifier</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Product picture mapping</returns>
        public static ProductPicture UpdateProductPicture(int productPictureId, int productId,
            int pictureId, int displayOrder)
        {
            var productPicture = GetProductPictureById(productPictureId);
            if (productPicture == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productPicture))
                context.ProductPictures.Attach(productPicture);

            productPicture.ProductId = productId;
            productPicture.PictureId = pictureId;
            productPicture.DisplayOrder = displayOrder;
            context.SaveChanges();
            return productPicture;
        }

        /// <summary>
        /// Gets all product picture mappings by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product picture mapping collection</returns>
        public static List<ProductPicture> GetProductPicturesByProductId(int productId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = (IQueryable<ProductPicture>)context.ProductPictures;
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
        public static ProductReview GetProductReviewById(int productReviewId)
        {
            if (productReviewId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pr in context.ProductReviews
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
        public static List<ProductReview> GetProductReviewByProductId(int productId)
        {
            bool showHidden = NopContext.Current.IsAdmin;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pr in context.ProductReviews
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
        public static void DeleteProductReview(int productReviewId)
        {
            var productReview = GetProductReviewById(productReviewId);
            if (productReview == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productReview))
                context.ProductReviews.Attach(productReview);
            context.DeleteObject(productReview);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets all product reviews
        /// </summary>
        /// <returns>Product review collection</returns>
        public static List<ProductReview> GetAllProductReviews()
        {
            bool showHidden = NopContext.Current.IsAdmin;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pr in context.ProductReviews
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
        public static ProductReview InsertProductReview(int productId, 
            int customerId, string title,
            string reviewText, int rating, int helpfulYesTotal,
            int helpfulNoTotal, bool isApproved, DateTime createdOn)
        {
            return InsertProductReview(productId, customerId,
             title, reviewText, rating, helpfulYesTotal,
             helpfulNoTotal, isApproved, createdOn, 
             ProductManager.NotifyAboutNewProductReviews);
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
        public static ProductReview InsertProductReview(int productId,
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
        public static ProductReview InsertProductReview(int productId,
            int customerId, string ipAddress, string title,
            string reviewText, int rating, int helpfulYesTotal,
            int helpfulNoTotal, bool isApproved, DateTime createdOn, bool notify)
        {
            if (rating < 1)
                rating = 1;
            if (rating > 5)
                rating = 5;

            ipAddress = CommonHelper.EnsureMaximumLength(ipAddress, 100);
            title = CommonHelper.EnsureMaximumLength(title, 1000);

            var context = ObjectContextHelper.CurrentObjectContext;

            var productReview = context.ProductReviews.CreateObject();
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

            context.ProductReviews.AddObject(productReview);
            context.SaveChanges();
                        
            //activity log
            CustomerActivityManager.InsertActivity(
                "WriteProductReview",
                LocalizationManager.GetLocaleResourceString("ActivityLog.WriteProductReview"),
                productId);

            //notify store owner
            if (notify)
            {
                MessageManager.SendProductReviewNotificationMessage(productReview, LocalizationManager.DefaultAdminLanguage.LanguageId);
            }

            return productReview;
        }

        /// <summary>
        /// Updates the product review
        /// </summary>
        /// <param name="productReviewId">The product review identifier</param>
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
        /// <returns>Product review</returns>
        public static ProductReview UpdateProductReview(int productReviewId, int productId, int customerId, string ipAddress, string title,
            string reviewText, int rating, int helpfulYesTotal,
            int helpfulNoTotal, bool isApproved, DateTime createdOn)
        {
            ipAddress = CommonHelper.EnsureMaximumLength(ipAddress, 100);
            title = CommonHelper.EnsureMaximumLength(title, 1000);

            var productReview = GetProductReviewById(productReviewId);
            if (productReview == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productReview))
                context.ProductReviews.Attach(productReview);

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
            context.SaveChanges();

            return productReview;
        }

        /// <summary>
        /// Sets a product rating helpfulness
        /// </summary>
        /// <param name="productReviewId">Product review identifer</param>
        /// <param name="wasHelpful">A value indicating whether the product review was helpful or not </param>
        public static void SetProductRatingHelpfulness(int productReviewId, bool wasHelpful)
        {
            if (NopContext.Current.User == null)
            {
                return;
            }
            if (NopContext.Current.User.IsGuest && !CustomerManager.AllowAnonymousUsersToReviewProduct)
            {
                return;
            }

            ProductReview productReview = GetProductReviewById(productReviewId);
            if (productReview == null)
                return;

            //delete previous helpfulness
            var context = ObjectContextHelper.CurrentObjectContext;
            var oldPrh = (from prh in context.ProductReviewHelpfulness
                         where prh.ProductReviewId == productReviewId &&
                         prh.CustomerId == NopContext.Current.User.CustomerId
                         select prh).FirstOrDefault();
            if (oldPrh != null)
            {
                context.DeleteObject(oldPrh);
            }
            context.SaveChanges();

            //insert new helpfulness
            var newPrh = context.ProductReviewHelpfulness.CreateObject();
            newPrh.ProductReviewId = productReviewId;
            newPrh.CustomerId = NopContext.Current.User.CustomerId;
            newPrh.WasHelpful = wasHelpful;

            context.ProductReviewHelpfulness.AddObject(newPrh);
            context.SaveChanges();

            //new totals
            int helpfulYesTotal = (from prh in context.ProductReviewHelpfulness
                                   where prh.ProductReviewId == productReviewId && 
                                   prh.WasHelpful == true
                                   select prh).Count();
            int helpfulNoTotal = (from prh in context.ProductReviewHelpfulness
                                   where prh.ProductReviewId == productReviewId &&
                                   prh.WasHelpful == false
                                   select prh).Count();

            productReview = UpdateProductReview(productReview.ProductReviewId,
                productReview.ProductId,
                productReview.CustomerId,
                productReview.IPAddress,
                productReview.Title,
                productReview.ReviewText,
                productReview.Rating,
                helpfulYesTotal,
                helpfulNoTotal,
                productReview.IsApproved,
                productReview.CreatedOn);
        }
        
        #endregion

        #region Related products

        /// <summary>
        /// Deletes a related product
        /// </summary>
        /// <param name="relatedProductId">Related product identifer</param>
        public static void DeleteRelatedProduct(int relatedProductId)
        {
            var relatedProduct = GetRelatedProductById(relatedProductId);
            if (relatedProduct == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(relatedProduct))
                context.RelatedProducts.Attach(relatedProduct);
            context.DeleteObject(relatedProduct);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets a related product collection by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <returns>Related product collection</returns>
        public static List<RelatedProduct> GetRelatedProductsByProductId1(int productId1)
        {
            bool showHidden = NopContext.Current.IsAdmin;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from rp in context.RelatedProducts
                        join p in context.Products on rp.ProductId2 equals p.ProductId
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
        public static RelatedProduct GetRelatedProductById(int relatedProductId)
        {
            if (relatedProductId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from rp in context.RelatedProducts
                        where rp.RelatedProductId == relatedProductId
                        select rp;
            var relatedProduct = query.SingleOrDefault();
            return relatedProduct;
        }

        /// <summary>
        /// Inserts a related product
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="productId2">The second product identifier</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Related product</returns>
        public static RelatedProduct InsertRelatedProduct(int productId1, 
            int productId2, int displayOrder)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var relatedProduct = context.RelatedProducts.CreateObject();
            relatedProduct.ProductId1 = productId1;
            relatedProduct.ProductId2 = productId2;
            relatedProduct.DisplayOrder = displayOrder;

            context.RelatedProducts.AddObject(relatedProduct);
            context.SaveChanges();

            return relatedProduct;
        }

        /// <summary>
        /// Updates a related product
        /// </summary>
        /// <param name="relatedProductId">The related product identifier</param>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="productId2">The second product identifier</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Related product</returns>
        public static RelatedProduct UpdateRelatedProduct(int relatedProductId, 
            int productId1, int productId2, int displayOrder)
        {
            var relatedProduct = GetRelatedProductById(relatedProductId);
            if (relatedProduct == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(relatedProduct))
                context.RelatedProducts.Attach(relatedProduct);

            relatedProduct.ProductId1 = productId1;
            relatedProduct.ProductId2 = productId2;
            relatedProduct.DisplayOrder = displayOrder;
            context.SaveChanges();

            return relatedProduct;
        }

        #endregion

        #region Cross-sell products

        /// <summary>
        /// Deletes a cross-sell product
        /// </summary>
        /// <param name="crossSellProductId">Cross-sell identifer</param>
        public static void DeleteCrossSellProduct(int crossSellProductId)
        {
            var crossSellProduct = GetCrossSellProductById(crossSellProductId);
            if (crossSellProduct == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(crossSellProduct))
                context.CrossSellProducts.Attach(crossSellProduct);
            context.DeleteObject(crossSellProduct);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets a cross-sell product collection by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <returns>Cross-sell product collection</returns>
        public static List<CrossSellProduct> GetCrossSellProductsByProductId1(int productId1)
        {
            bool showHidden = NopContext.Current.IsAdmin;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from csp in context.CrossSellProducts
                        join p in context.Products on csp.ProductId2 equals p.ProductId
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
        public static CrossSellProduct GetCrossSellProductById(int crossSellProductId)
        {
            if (crossSellProductId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from csp in context.CrossSellProducts
                        where csp.CrossSellProductId == crossSellProductId
                        select csp;
            var crossSellProduct = query.SingleOrDefault();
            return crossSellProduct;
        }

        /// <summary>
        /// Inserts a cross-sell product
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="productId2">The second product identifier</param>
        /// <returns>Cross-sell product</returns>
        public static CrossSellProduct InsertCrossSellProduct(int productId1,
            int productId2)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var crossSellProduct = context.CrossSellProducts.CreateObject();
            crossSellProduct.ProductId1 = productId1;
            crossSellProduct.ProductId2 = productId2;

            context.CrossSellProducts.AddObject(crossSellProduct);
            context.SaveChanges();

            return crossSellProduct;
        }

        /// <summary>
        /// Updates a cross-sell product
        /// </summary>
        /// <param name="crossSellProductId">Cross-sell product identifer</param>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="productId2">The second product identifier</param>
        /// <returns>Cross-sell product</returns>
        public static CrossSellProduct UpdateCrossSellProduct(int crossSellProductId,
            int productId1, int productId2)
        {
            var crossSellProduct = GetCrossSellProductById(crossSellProductId);
            if (crossSellProduct == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(crossSellProduct))
                context.CrossSellProducts.Attach(crossSellProduct);

            crossSellProduct.ProductId1 = productId1;
            crossSellProduct.ProductId2 = productId2;
            context.SaveChanges();

            return crossSellProduct;
        }

        #endregion

        #region Pricelists

        /// <summary>
        /// Gets all product variants directly assigned to a pricelist
        /// </summary>
        /// <param name="pricelistId">Pricelist identifier</param>
        /// <returns>Product variants</returns>
        public static List<ProductVariant> GetProductVariantsByPricelistId(int pricelistId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pv in context.ProductVariants
                        join pvpl in context.ProductVariantPricelists on pv.ProductVariantId equals pvpl.ProductVariantId
                        where pvpl.PricelistId == pricelistId
                        select pv;
            var productVariants = query.ToList();

            return productVariants;
        }

        /// <summary>
        /// Deletes a pricelist
        /// </summary>
        /// <param name="pricelistId">The PricelistId of the item to be deleted</param>
        public static void DeletePricelist(int pricelistId)
        {
            var pricelist = GetPricelistById(pricelistId);
            if (pricelist == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(pricelist))
                context.Pricelists.Attach(pricelist);
            context.DeleteObject(pricelist);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets a collection of all available pricelists
        /// </summary>
        /// <returns>Collection of pricelists</returns>
        public static List<Pricelist> GetAllPricelists()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pl in context.Pricelists
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
        public static Pricelist GetPricelistById(int pricelistId)
        {
            if (pricelistId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pl in context.Pricelists
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
        public static Pricelist GetPricelistByGuid(string pricelistGuid)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pl in context.Pricelists
                        where pl.PricelistGuid == pricelistGuid
                        select pl;
            var pricelist = query.FirstOrDefault();

            return pricelist;
        }

        /// <summary>
        /// Inserts a Pricelist
        /// </summary>
        /// <param name="exportMode">Mode of list creation (Export all, assigned only, assigned only with special price)</param>
        /// <param name="exportType">CSV or XML</param>
        /// <param name="affiliateId">Affiliate connected to this pricelist (optional), links will be created with AffiliateId</param>
        /// <param name="displayName">Displayedname</param>
        /// <param name="shortName">shortname to identify the pricelist</param>
        /// <param name="pricelistGuid">unique identifier to get pricelist "anonymous"</param>
        /// <param name="cacheTime">how long will the pricelist be in cached before new creation</param>
        /// <param name="formatLocalization">what localization will be used (numeric formats, etc.) en-US, de-DE etc.</param>
        /// <param name="description">Displayed description</param>
        /// <param name="adminNotes">Admin can put some notes here, not displayed in public</param>
        /// <param name="header">Headerline of the exported file (plain text)</param>
        /// <param name="body">template for an exportet productvariant, uses delimiters and replacement strings</param>
        /// <param name="footer">Footer line of the exportet file (plain text)</param>
        /// <param name="priceAdjustmentType">type of price adjustment (if used) (relative or absolute)</param>
        /// <param name="priceAdjustment">price will be adjusted by this amount (in accordance with PriceAdjustmentType)</param>
        /// <param name="overrideIndivAdjustment">use individual adjustment, if available, or override</param>
        /// <param name="createdOn">when was this record originally created</param>
        /// <param name="updatedOn">last time this recordset was updated</param>
        /// <returns>Pricelist</returns>
        public static Pricelist InsertPricelist(PriceListExportModeEnum exportMode, 
            PriceListExportTypeEnum exportType, int affiliateId,
            string displayName, string shortName, string pricelistGuid, 
            int cacheTime, string formatLocalization, string description,
            string adminNotes, string header, string body, string footer,
            PriceAdjustmentTypeEnum priceAdjustmentType, decimal priceAdjustment, 
            bool overrideIndivAdjustment, DateTime createdOn, DateTime updatedOn)
        {
            displayName = CommonHelper.EnsureMaximumLength(displayName, 100);
            shortName = CommonHelper.EnsureMaximumLength(shortName, 20);
            pricelistGuid = CommonHelper.EnsureMaximumLength(pricelistGuid, 40);
            formatLocalization = CommonHelper.EnsureMaximumLength(formatLocalization, 5);
            description = CommonHelper.EnsureMaximumLength(description, 500);
            adminNotes = CommonHelper.EnsureMaximumLength(adminNotes, 500);
            header = CommonHelper.EnsureMaximumLength(header, 500);
            body = CommonHelper.EnsureMaximumLength(body, 500);
            footer = CommonHelper.EnsureMaximumLength(footer, 500);

            var context = ObjectContextHelper.CurrentObjectContext;

            var pricelist = context.Pricelists.CreateObject();
            pricelist.ExportModeId = (int)exportMode;
            pricelist.ExportTypeId = (int)exportType;
            pricelist.AffiliateId = affiliateId;
            pricelist.DisplayName = displayName;
            pricelist.ShortName = shortName;
            pricelist.PricelistGuid = pricelistGuid;
            pricelist.CacheTime = cacheTime;
            pricelist.FormatLocalization = formatLocalization;
            pricelist.Description = description;
            pricelist.AdminNotes = adminNotes;
            pricelist.Header = header;
            pricelist.Body = body;
            pricelist.Footer = footer;
            pricelist.PriceAdjustmentTypeId = (int)priceAdjustmentType;
            pricelist.PriceAdjustment = priceAdjustment;
            pricelist.OverrideIndivAdjustment = overrideIndivAdjustment;
            pricelist.CreatedOn = createdOn;
            pricelist.UpdatedOn = updatedOn;

            context.Pricelists.AddObject(pricelist);
            context.SaveChanges();

            return pricelist;
        }

        /// <summary>
        /// Updates the Pricelist
        /// </summary>
        /// <param name="pricelistId">Unique Identifier</param>
        /// <param name="exportMode">Mode of list creation (Export all, assigned only, assigned only with special price)</param>
        /// <param name="exportType">CSV or XML</param>
        /// <param name="affiliateId">Affiliate connected to this pricelist (optional), links will be created with AffiliateId</param>
        /// <param name="displayName">Displayedname</param>
        /// <param name="shortName">shortname to identify the pricelist</param>
        /// <param name="pricelistGuid">unique identifier to get pricelist "anonymous"</param>
        /// <param name="cacheTime">how long will the pricelist be in cached before new creation</param>
        /// <param name="formatLocalization">what localization will be used (numeric formats, etc.) en-US, de-DE etc.</param>
        /// <param name="description">Displayed description</param>
        /// <param name="adminNotes">Admin can put some notes here, not displayed in public</param>
        /// <param name="header">Headerline of the exported file (plain text)</param>
        /// <param name="body">template for an exportet productvariant, uses delimiters and replacement strings</param>
        /// <param name="footer">Footer line of the exportet file (plain text)</param>
        /// <param name="priceAdjustmentType">type of price adjustment (if used) (relative or absolute)</param>
        /// <param name="priceAdjustment">price will be adjusted by this amount (in accordance with PriceAdjustmentType)</param>
        /// <param name="overrideIndivAdjustment">use individual adjustment, if available, or override</param>
        /// <param name="createdOn">when was this record originally created</param>
        /// <param name="updatedOn">last time this recordset was updated</param>
        /// <returns>Pricelist</returns>
        public static Pricelist UpdatePricelist(int pricelistId, 
            PriceListExportModeEnum exportMode, PriceListExportTypeEnum exportType, 
            int affiliateId,  string displayName, string shortName, 
            string pricelistGuid, int cacheTime, string formatLocalization,
            string description, string adminNotes,
            string header, string body, string footer,
            PriceAdjustmentTypeEnum priceAdjustmentType, decimal priceAdjustment, 
            bool overrideIndivAdjustment, DateTime createdOn, DateTime updatedOn)
        {
            displayName = CommonHelper.EnsureMaximumLength(displayName, 100);
            shortName = CommonHelper.EnsureMaximumLength(shortName, 20);
            pricelistGuid = CommonHelper.EnsureMaximumLength(pricelistGuid, 40);
            formatLocalization = CommonHelper.EnsureMaximumLength(formatLocalization, 5);
            description = CommonHelper.EnsureMaximumLength(description, 500);
            adminNotes = CommonHelper.EnsureMaximumLength(adminNotes, 500);
            header = CommonHelper.EnsureMaximumLength(header, 500);
            body = CommonHelper.EnsureMaximumLength(body, 500);
            footer = CommonHelper.EnsureMaximumLength(footer, 500);

            var pricelist = GetPricelistById(pricelistId);
            if (pricelist == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(pricelist))
                context.Pricelists.Attach(pricelist);

            pricelist.ExportModeId = (int)exportMode;
            pricelist.ExportTypeId = (int)exportType;
            pricelist.AffiliateId = affiliateId;
            pricelist.DisplayName = displayName;
            pricelist.ShortName = shortName;
            pricelist.PricelistGuid = pricelistGuid;
            pricelist.CacheTime = cacheTime;
            pricelist.FormatLocalization = formatLocalization;
            pricelist.Description = description;
            pricelist.AdminNotes = adminNotes;
            pricelist.Header = header;
            pricelist.Body = body;
            pricelist.Footer = footer;
            pricelist.PriceAdjustmentTypeId = (int)priceAdjustmentType;
            pricelist.PriceAdjustment = priceAdjustment;
            pricelist.OverrideIndivAdjustment = overrideIndivAdjustment;
            pricelist.CreatedOn = createdOn;
            pricelist.UpdatedOn = updatedOn;
            context.SaveChanges();

            return pricelist;
        }

        /// <summary>
        /// Deletes a ProductVariantPricelist
        /// </summary>
        /// <param name="productVariantPricelistId">ProductVariantPricelist identifier</param>
        public static void DeleteProductVariantPricelist(int productVariantPricelistId)
        {
            if (productVariantPricelistId == 0)
                return;

            var productVariantPricelist = GetProductVariantPricelistById(productVariantPricelistId);
            if (productVariantPricelist == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productVariantPricelist))
                context.ProductVariantPricelists.Attach(productVariantPricelist);
            context.DeleteObject(productVariantPricelist);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets a ProductVariantPricelist
        /// </summary>
        /// <param name="productVariantPricelistId">ProductVariantPricelist identifier</param>
        /// <returns>ProductVariantPricelist</returns>
        public static ProductVariantPricelist GetProductVariantPricelistById(int productVariantPricelistId)
        {
            if (productVariantPricelistId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pvpl in context.ProductVariantPricelists
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
        public static ProductVariantPricelist GetProductVariantPricelist(int productVariantId, int pricelistId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pvpl in context.ProductVariantPricelists
                        where pvpl.ProductVariantId == productVariantId &&
                        pvpl.PricelistId == pricelistId 
                        select pvpl;
            var productVariantPricelist = query.FirstOrDefault();
            return productVariantPricelist;
        }

        /// <summary>
        /// Inserts a ProductVariantPricelist
        /// </summary>
        /// <param name="productVariantId">The product variant identifer</param>
        /// <param name="pricelistId">The pricelist identifier</param>
        /// <param name="priceAdjustmentType">The type of price adjustment (if used) (relative or absolute)</param>
        /// <param name="priceAdjustment">The price will be adjusted by this amount</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>ProductVariantPricelist</returns>
        public static ProductVariantPricelist InsertProductVariantPricelist(int productVariantId, 
            int pricelistId, PriceAdjustmentTypeEnum priceAdjustmentType,
            decimal priceAdjustment, DateTime updatedOn)
        {
            var context = ObjectContextHelper.CurrentObjectContext;

            var productVariantPricelist = context.ProductVariantPricelists.CreateObject();
            productVariantPricelist.ProductVariantId = productVariantId;
            productVariantPricelist.PricelistId = pricelistId;
            productVariantPricelist.PriceAdjustmentTypeId = (int)priceAdjustmentType;
            productVariantPricelist.PriceAdjustment = priceAdjustment;
            productVariantPricelist.UpdatedOn = updatedOn;

            context.ProductVariantPricelists.AddObject(productVariantPricelist);
            context.SaveChanges();
            return productVariantPricelist;
        }

        /// <summary>
        /// Updates the ProductVariantPricelist
        /// </summary>
        /// <param name="productVariantPricelistId">The product variant pricelist identifier</param>
        /// <param name="productVariantId">The product variant identifer</param>
        /// <param name="pricelistId">The pricelist identifier</param>
        /// <param name="priceAdjustmentType">The type of price adjustment (if used) (relative or absolute)</param>
        /// <param name="priceAdjustment">The price will be adjusted by this amount</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>ProductVariantPricelist</returns>
        public static ProductVariantPricelist UpdateProductVariantPricelist(int productVariantPricelistId, 
            int productVariantId, int pricelistId,
            PriceAdjustmentTypeEnum priceAdjustmentType, decimal priceAdjustment,
            DateTime updatedOn)
        {
            if (productVariantPricelistId == 0)
                return null;

            var productVariantPricelist = GetProductVariantPricelistById(productVariantPricelistId);
            if (productVariantPricelist == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productVariantPricelist))
                context.ProductVariantPricelists.Attach(productVariantPricelist);

            productVariantPricelist.ProductVariantId = productVariantId;
            productVariantPricelist.PricelistId = pricelistId;
            productVariantPricelist.PriceAdjustmentTypeId = (int)priceAdjustmentType;
            productVariantPricelist.PriceAdjustment = priceAdjustment;
            productVariantPricelist.UpdatedOn = updatedOn;
            context.SaveChanges();

            return productVariantPricelist;
        }

        #endregion

        #region Tier prices

        /// <summary>
        /// Gets a tier price
        /// </summary>
        /// <param name="tierPriceId">Tier price identifier</param>
        /// <returns>Tier price</returns>
        public static TierPrice GetTierPriceById(int tierPriceId)
        {
            if (tierPriceId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from tp in context.TierPrices
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
        public static List<TierPrice> GetTierPricesByProductVariantId(int productVariantId)
        {
            if (productVariantId == 0)
                return new List<TierPrice>();

            string key = string.Format(TIERPRICES_ALLBYPRODUCTVARIANTID_KEY, productVariantId);
            object obj2 = NopRequestCache.Get(key);
            if (ProductManager.CacheEnabled && (obj2 != null))
            {
                return (List<TierPrice>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from tp in context.TierPrices
                        orderby tp.Quantity
                        where tp.ProductVariantId == productVariantId
                        select tp;
            var tierPrices = query.ToList();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.Add(key, tierPrices);
            }
            return tierPrices;
        }

        /// <summary>
        /// Deletes a tier price
        /// </summary>
        /// <param name="tierPriceId">Tier price identifier</param>
        public static void DeleteTierPrice(int tierPriceId)
        {
            var tierPrice = GetTierPriceById(tierPriceId);
            if (tierPrice == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(tierPrice))
                context.TierPrices.Attach(tierPrice);
            context.DeleteObject(tierPrice);
            context.SaveChanges();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Inserts a tier price
        /// </summary>
        /// <param name="productVariantId">The product variant identifier</param>
        /// <param name="quantity">The quantity</param>
        /// <param name="price">The price</param>
        /// <returns>Tier price</returns>
        public static TierPrice InsertTierPrice(int productVariantId, 
            int quantity, decimal price)
        {
            var context = ObjectContextHelper.CurrentObjectContext;

            var tierPrice = context.TierPrices.CreateObject();
            tierPrice.ProductVariantId = productVariantId;
            tierPrice.Quantity = quantity;
            tierPrice.Price = price;

            context.TierPrices.AddObject(tierPrice);
            context.SaveChanges();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
            return tierPrice;
        }

        /// <summary>
        /// Updates the tier price
        /// </summary>
        /// <param name="tierPriceId">The tier price identifier</param>
        /// <param name="productVariantId">The product variant identifier</param>
        /// <param name="quantity">The quantity</param>
        /// <param name="price">The price</param>
        /// <returns>Tier price</returns>
        public static TierPrice UpdateTierPrice(int tierPriceId, int productVariantId, 
            int quantity, decimal price)
        {
            var tierPrice = GetTierPriceById(tierPriceId);
            if (tierPrice == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(tierPrice))
                context.TierPrices.Attach(tierPrice);

            tierPrice.ProductVariantId = productVariantId;
            tierPrice.Quantity = quantity;
            tierPrice.Price = price;
            context.SaveChanges();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }

            return tierPrice;
        }

        #endregion

        #region Product prices by customer role

        /// <summary>
        /// Deletes a product price by customer role by identifier 
        /// </summary>
        /// <param name="customerRoleProductPriceId">The identifier</param>
        public static void DeleteCustomerRoleProductPrice(int customerRoleProductPriceId)
        {
            var customerRoleProductPrice = GetCustomerRoleProductPriceById(customerRoleProductPriceId);
            if (customerRoleProductPrice == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(customerRoleProductPrice))
                context.CustomerRoleProductPrices.Attach(customerRoleProductPrice);
            context.DeleteObject(customerRoleProductPrice);
            context.SaveChanges();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a product price by customer role by identifier 
        /// </summary>
        /// <param name="customerRoleProductPriceId">The identifier</param>
        /// <returns>Product price by customer role by identifier </returns>
        public static CustomerRoleProductPrice GetCustomerRoleProductPriceById(int customerRoleProductPriceId)
        {
            if (customerRoleProductPriceId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from crpp in context.CustomerRoleProductPrices
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
        public static List<CustomerRoleProductPrice> GetAllCustomerRoleProductPrices(int productVariantId)
        {
            string key = string.Format(CUSTOMERROLEPRICES_ALL_KEY, productVariantId);
            object obj2 = NopRequestCache.Get(key);
            if (ProductManager.CacheEnabled && (obj2 != null))
            {
                return (List<CustomerRoleProductPrice>)obj2;
            }
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from crpp in context.CustomerRoleProductPrices
                        where crpp.ProductVariantId == productVariantId
                        select crpp;
            var collection = query.ToList();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.Add(key, collection);
            }
            return collection;
        }

        /// <summary>
        /// Inserts a product price by customer role
        /// </summary>
        /// <param name="customerRoleId">The customer role identifier</param>
        /// <param name="productVariantId">The product variant identifier</param>
        /// <param name="price">The price</param>
        /// <returns>A product price by customer role</returns>
        public static CustomerRoleProductPrice InsertCustomerRoleProductPrice(int customerRoleId,
            int productVariantId, decimal price)
        {
            var context = ObjectContextHelper.CurrentObjectContext;

            var customerRoleProductPrice = context.CustomerRoleProductPrices.CreateObject();
            customerRoleProductPrice.CustomerRoleId = customerRoleId;
            customerRoleProductPrice.ProductVariantId = productVariantId;
            customerRoleProductPrice.Price = price;

            context.CustomerRoleProductPrices.AddObject(customerRoleProductPrice);
            context.SaveChanges();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }

            return customerRoleProductPrice;
        }

        /// <summary>
        /// Updates a product price by customer role
        /// </summary>
        /// <param name="customerRoleProductPriceId">The identifier</param>
        /// <param name="customerRoleId">The customer role identifier</param>
        /// <param name="productVariantId">The product variant identifier</param>
        /// <param name="price">The price</param>
        /// <returns>A product price by customer role</returns>
        public static CustomerRoleProductPrice UpdateCustomerRoleProductPrice(int customerRoleProductPriceId,
            int customerRoleId, int productVariantId, decimal price)
        {
            var customerRoleProductPrice = GetCustomerRoleProductPriceById(customerRoleProductPriceId);
            if (customerRoleProductPrice == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(customerRoleProductPrice))
                context.CustomerRoleProductPrices.Attach(customerRoleProductPrice);

            customerRoleProductPrice.CustomerRoleId = customerRoleId;
            customerRoleProductPrice.ProductVariantId = productVariantId;
            customerRoleProductPrice.Price = price;
            context.SaveChanges();

            if (ProductManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(TIERPRICES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(CUSTOMERROLEPRICES_PATTERN_KEY);
            }

            return customerRoleProductPrice;
        }

        #endregion

        #region Product tags

        /// <summary>
        /// Deletes a product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        public static void DeleteProductTag(int productTagId)
        {
            var productTag = GetProductTagById(productTagId);
            if (productTag == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productTag))
                context.ProductTags.Attach(productTag);
            context.DeleteObject(productTag);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets a product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>Product tag</returns>
        public static ProductTag GetProductTagById(int productTagId)
        {
            if (productTagId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pt in context.ProductTags
                        where pt.ProductTagId == productTagId
                        select pt;
            var productTag = query.SingleOrDefault();
            return productTag;
        }

        /// <summary>
        /// Gets all product tags
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="name">Product tag name or empty string to load all records</param>
        /// <returns>Product tag collection</returns>
        public static List<ProductTag> GetAllProductTags(int productId,
            string name)
        {
            if (name == null)
                name = string.Empty;
            name = name.Trim();

            var context = ObjectContextHelper.CurrentObjectContext;
            var productTags = context.Sp_ProductTagLoadAll(productId, name);
            return productTags;
        }

        /// <summary>
        /// Inserts a product tag
        /// </summary>
        /// <param name="name">Product tag name</param>
        /// <param name="productCount">Product count</param>
        /// <returns>Product tag</returns>
        public static ProductTag InsertProductTag(string name, int productCount)
        {
            if (name == null)
                name = string.Empty;
            name = name.Trim();

            name = CommonHelper.EnsureMaximumLength(name, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            var productTag = context.ProductTags.CreateObject();
            productTag.Name = name;
            productTag.ProductCount = productCount;

            context.ProductTags.AddObject(productTag);
            context.SaveChanges();

            return productTag;
        }

        /// <summary>
        /// Updates a product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <param name="name">Product tag name</param>
        /// <param name="productCount">Product count</param>
        /// <returns>Product tag</returns>
        public static ProductTag UpdateProductTag(int productTagId,
            string name, int productCount)
        {
            if (name == null)
                name = string.Empty;
            name = name.Trim();

            name = CommonHelper.EnsureMaximumLength(name, 100);

            var productTag = GetProductTagById(productTagId);
            if (productTag == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(productTag))
                context.ProductTags.Attach(productTag);

            productTag.Name = name;
            productTag.ProductCount = productCount;
            context.SaveChanges();

            return productTag;
        }

        /// <summary>
        /// Adds a discount tag mapping
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="productTagId">Product tag identifier</param>
        public static void AddProductTagMapping(int productId, int productTagId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            context.Sp_ProductTag_Product_MappingInsert(productTagId, productId);
        }

        /// <summary>
        /// Removes a discount tag mapping
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="productTagId">Product tag identifier</param>
        public static void RemoveProductTagMapping(int productId, int productTagId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            context.Sp_ProductTag_Product_MappingDelete(productTagId, productId);
        }

        #endregion
        
        #region Etc

        /// <summary>
        /// Formats the text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string FormatProductReviewText(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);

            return text;
        }

        /// <summary>
        /// Formats the email a friend text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string FormatEmailAFriendText(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);
            return text;
        }

        #endregion 

        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public static bool CacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.ProductManager.CacheEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether "Recently viewed products" feature is enabled
        /// </summary>
        public static bool RecentlyViewedProductsEnabled
        {
            get
            {
                bool recentlyViewedProductsEnabled = SettingManager.GetSettingValueBoolean("Display.RecentlyViewedProductsEnabled");
                return recentlyViewedProductsEnabled;
            }
            set
            {
                SettingManager.SetParam("Display.RecentlyViewedProductsEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a number of "Recently viewed products"
        /// </summary>
        public static int RecentlyViewedProductsNumber
        {
            get
            {
                int recentlyViewedProductsNumber = SettingManager.GetSettingValueInteger("Display.RecentlyViewedProductsNumber");
                return recentlyViewedProductsNumber;
            }
            set
            {
                SettingManager.SetParam("Display.RecentlyViewedProductsNumber", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether "Recently added products" feature is enabled
        /// </summary>
        public static bool RecentlyAddedProductsEnabled
        {
            get
            {
                bool recentlyAddedProductsEnabled = SettingManager.GetSettingValueBoolean("Display.RecentlyAddedProductsEnabled");
                return recentlyAddedProductsEnabled;
            }
            set
            {
                SettingManager.SetParam("Display.RecentlyAddedProductsEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a number of "Recently added products"
        /// </summary>
        public static int RecentlyAddedProductsNumber
        {
            get
            {
                int recentlyAddedProductsNumber = SettingManager.GetSettingValueInteger("Display.RecentlyAddedProductsNumber");
                return recentlyAddedProductsNumber;
            }
            set
            {
                SettingManager.SetParam("Display.RecentlyAddedProductsNumber", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a number of "Cross-sells"
        /// </summary>
        public static int CrossSellsNumber
        {
            get
            {
                int result = SettingManager.GetSettingValueInteger("Display.CrossSellsNumber", 2);
                return result;
            }
            set
            {
                SettingManager.SetParam("Display.CrossSellsNumber", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to displays a button from AddThis.com on your product pages
        /// </summary>
        public static bool ShowShareButton
        {
            get
            {
                bool showShareButton = SettingManager.GetSettingValueBoolean("Products.AddThisSharing.Enabled");
                return showShareButton;
            }
            set
            {
                SettingManager.SetParam("Products.AddThisSharing.Enabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether "Compare products" feature is enabled
        /// </summary>
        public static bool CompareProductsEnabled
        {
            get
            {
                bool compareProductsEnabled = SettingManager.GetSettingValueBoolean("Common.EnableCompareProducts");
                return compareProductsEnabled;
            }
            set
            {
                SettingManager.SetParam("Common.EnableCompareProducts", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets "List of products purchased by other customers who purchased the above" option is enable
        /// </summary>
        public static bool ProductsAlsoPurchasedEnabled
        {
            get
            {
                bool productsAlsoPurchased = SettingManager.GetSettingValueBoolean("Display.ListOfProductsAlsoPurchasedEnabled");
                return productsAlsoPurchased;
            }
            set
            {
                SettingManager.SetParam("Display.ListOfProductsAlsoPurchasedEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a number of products also purchased by other customers to display
        /// </summary>
        public static int ProductsAlsoPurchasedNumber
        {
            get
            {
                int productsAlsoPurchasedNumber = SettingManager.GetSettingValueInteger("Display.ListOfProductsAlsoPurchasedNumberToDisplay");
                return productsAlsoPurchasedNumber;
            }
            set
            {
                SettingManager.SetParam("Display.ListOfProductsAlsoPurchasedNumberToDisplay", value.ToString());
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to notify about new product reviews
        /// </summary>
        public static bool NotifyAboutNewProductReviews
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Product.NotifyAboutNewProductReviews");
            }
            set
            {
                SettingManager.SetParam("Product.NotifyAboutNewProductReviews", value.ToString());
            }
        }
        #endregion
    }
}
