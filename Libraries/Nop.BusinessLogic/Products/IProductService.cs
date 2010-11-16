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

namespace NopSolutions.NopCommerce.BusinessLogic.Products
{
    /// <summary>
    /// Product service
    /// </summary>
    public partial interface IProductService
    {
        #region Methods

        #region Products

        /// <summary>
        /// Marks a product as deleted
        /// </summary>
        /// <param name="productId">Product identifier</param>
        void MarkProductAsDeleted(int productId);

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>Product collection</returns>
        List<Product> GetAllProducts();

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product collection</returns>
        List<Product> GetAllProducts(bool showHidden);

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Product collection</returns>
        List<Product> GetAllProducts(int pageSize, int pageIndex,
            out int totalRecords);

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
        List<Product> GetAllProducts(int categoryId,
            int manufacturerId, int productTagId, bool? featuredProducts,
            int pageSize, int pageIndex, out int totalRecords);

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search in descriptions</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Product collection</returns>
        List<Product> GetAllProducts(string keywords,
            bool searchDescriptions, int pageSize, int pageIndex, out int totalRecords);

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
        List<Product> GetAllProducts(int categoryId,
            int manufacturerId, int productTagId, bool? featuredProducts,
            string keywords, bool searchDescriptions, int pageSize,
            int pageIndex, List<int> filteredSpecs, out int totalRecords);

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
        List<Product> GetAllProducts(int categoryId,
            int manufacturerId, int productTagId, bool? featuredProducts,
            decimal? priceMin, decimal? priceMax, int pageSize,
            int pageIndex, List<int> filteredSpecs, out int totalRecords);

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
        List<Product> GetAllProducts(int categoryId,
            int manufacturerId, int productTagId, bool? featuredProducts,
            decimal? priceMin, decimal? priceMax, string keywords,
            bool searchDescriptions, int pageSize, int pageIndex,
            List<int> filteredSpecs, out int totalRecords);
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
        List<Product> GetAllProducts(int categoryId,
            int manufacturerId, int productTagId, bool? featuredProducts,
            decimal? priceMin, decimal? priceMax, string keywords,
            bool searchDescriptions, int pageSize, int pageIndex,
            List<int> filteredSpecs, ProductSortingEnum orderBy, out int totalRecords);

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
        List<Product> GetAllProducts(int categoryId,
            int manufacturerId, int productTagId, bool? featuredProducts,
            decimal? priceMin, decimal? priceMax,
            string keywords, bool searchDescriptions, int pageSize,
            int pageIndex, List<int> filteredSpecs, int languageId,
            ProductSortingEnum orderBy, out int totalRecords);

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
        List<Product> GetAllProducts(int categoryId,
            int manufacturerId, int productTagId, bool? featuredProducts,
            decimal? priceMin, decimal? priceMax,
            int relatedToProductId, string keywords, bool searchDescriptions, int pageSize,
            int pageIndex, List<int> filteredSpecs, int languageId,
            ProductSortingEnum orderBy, out int totalRecords);

        /// <summary>
        /// Gets all products displayed on the home page
        /// </summary>
        /// <returns>Product collection</returns>
        List<Product> GetAllProductsDisplayedOnHomePage();
        
        /// <summary>
        /// Gets product
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product</returns>
        Product GetProductById(int productId);
        
        /// <summary>
        /// Inserts a product
        /// </summary>
        /// <param name="product">Product</param>
        void InsertProduct(Product product);

        /// <summary>
        /// Updates the product
        /// </summary>
        /// <param name="product">Product</param>
        void UpdateProduct(Product product);

        /// <summary>
        /// Gets localized product by id
        /// </summary>
        /// <param name="productLocalizedId">Localized product identifier</param>
        /// <returns>Product content</returns>
        ProductLocalized GetProductLocalizedById(int productLocalizedId);

        /// <summary>
        /// Gets localized product by product id
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product content</returns>
        List<ProductLocalized> GetProductLocalizedByProductId(int productId);

        /// <summary>
        /// Gets localized product by product id and language id
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Product content</returns>
        ProductLocalized GetProductLocalizedByProductIdAndLanguageId(int productId, int languageId);

        /// <summary>
        /// Inserts a localized product
        /// </summary>
        /// <param name="productLocalized">Product content</param>
        void InsertProductLocalized(ProductLocalized productLocalized);

        /// <summary>
        /// Update a localized product
        /// </summary>
        /// <param name="productLocalized">Product content</param>
        void UpdateProductLocalized(ProductLocalized productLocalized);

        /// <summary>
        /// Gets a list of products purchased by other customers who purchased the above
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product collection</returns>
        List<Product> GetProductsAlsoPurchasedById(int productId);

        /// <summary>
        /// Gets a list of products purchased by other customers who purchased the above
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>Product collection</returns>
        List<Product> GetProductsAlsoPurchasedById(int productId,
            int pageSize, int pageIndex, out int totalRecords);

        /// <summary>
        /// Sets a product rating
        /// </summary>
        /// <param name="productId">Product identifer</param>
        /// <param name="rating">Rating</param>
        void SetProductRating(int productId, int rating);

        /// <summary>
        /// Clears a "compare products" list
        /// </summary>
        void ClearCompareProducts();

        /// <summary>
        /// Gets a "compare products" list
        /// </summary>
        /// <returns>"Compare products" list</returns>
        List<Product> GetCompareProducts();

        /// <summary>
        /// Gets a "compare products" identifier list
        /// </summary>
        /// <returns>"compare products" identifier list</returns>
        List<int> GetCompareProductsIds();

        /// <summary>
        /// Removes a product from a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifer</param>
        void RemoveProductFromCompareList(int productId);

        /// <summary>
        /// Adds a product to a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifer</param>
        void AddProductToCompareList(int productId);

        /// <summary>
        /// Gets a "recently viewed products" list
        /// </summary>
        /// <param name="number">Number of products to load</param>
        /// <returns>"recently viewed products" list</returns>
        List<Product> GetRecentlyViewedProducts(int number);

        /// <summary>
        /// Gets a "recently viewed products" identifier list
        /// </summary>
        /// <returns>"recently viewed products" list</returns>
        List<int> GetRecentlyViewedProductsIds();

        /// <summary>
        /// Gets a "recently viewed products" identifier list
        /// </summary>
        /// <param name="number">Number of products to load</param>
        /// <returns>"recently viewed products" list</returns>
        List<int> GetRecentlyViewedProductsIds(int number);

        /// <summary>
        /// Adds a product to a recently viewed products list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        void AddProductToRecentlyViewedList(int productId);

        /// <summary>
        /// Gets a recently added products list
        /// </summary>
        /// <param name="number">Number of products to load</param>
        /// <returns>Recently added products</returns>
        List<Product> GetRecentlyAddedProducts(int number);

        /// <summary>
        /// Direct add to cart allowed
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="productVariantId">Default product variant identifier for adding to cart</param>
        /// <returns>A value indicating whether direct add to cart is allowed</returns>
        bool DirectAddToCartAllowed(int productId, out int productVariantId);

        /// <summary>
        /// Creates a copy of product with all depended data
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <param name="name">The name of product duplicate</param>
        /// <param name="isPublished">A value indicating whether the product duplicate should be published</param>
        /// <param name="copyImages">A value indicating whether the product images should be copied</param>
        /// <returns>Product entity</returns>
        Product DuplicateProduct(int productId, string name,
            bool isPublished, bool copyImages);

        /// <summary>
        /// Gets a cross-sells
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Cross-sells</returns>
        List<Product> GetCrosssellProductsByShoppingCart(ShoppingCart cart);

        #endregion

        #region Product variants

        /// <summary>
        /// Get low stock product variants
        /// </summary>
        /// <returns>Result</returns>
        List<ProductVariant> GetLowStockProductVariants();

        /// <summary>
        /// Gets a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Product variant</returns>
        ProductVariant GetProductVariantById(int productVariantId);

        /// <summary>
        /// Gets a product variant by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Product variant</returns>
        ProductVariant GetProductVariantBySKU(string sku);
        
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
        List<ProductVariant> GetAllProductVariants(int categoryId,
            int manufacturerId, string keywords,
            int pageSize, int pageIndex, out int totalRecords);
        
        /// <summary>
        /// Inserts a product variant
        /// </summary>
        /// <param name="productVariant">The product variant</param>
        void InsertProductVariant(ProductVariant productVariant);

        /// <summary>
        /// Updates the product variant
        /// </summary>
        /// <param name="productVariant">The product variant</param>
        void UpdateProductVariant(ProductVariant productVariant);

        /// <summary>
        /// Gets product variants by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>Product variant collection</returns>
        List<ProductVariant> GetProductVariantsByProductId(int productId);
        
        /// <summary>
        /// Gets product variants by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product variant collection</returns>
        List<ProductVariant> GetProductVariantsByProductId(int productId, bool showHidden);

        /// <summary>
        /// Gets restricted product variants by discount identifier
        /// </summary>
        /// <param name="discountId">The discount identifier</param>
        /// <returns>Product variant collection</returns>
        List<ProductVariant> GetProductVariantsRestrictedByDiscountId(int discountId);

        /// <summary>
        /// Gets localized product variant by id
        /// </summary>
        /// <param name="productVariantLocalizedId">Localized product variant identifier</param>
        /// <returns>Product variant content</returns>
        ProductVariantLocalized GetProductVariantLocalizedById(int productVariantLocalizedId);

        /// <summary>
        /// Gets localized product variant by product variant id
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Product variant content</returns>
        List<ProductVariantLocalized> GetProductVariantLocalizedByProductVariantId(int productVariantId);
        
        /// <summary>
        /// Gets localized product variant by product variant id and language id
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Product variant content</returns>
        ProductVariantLocalized GetProductVariantLocalizedByProductVariantIdAndLanguageId(int productVariantId, int languageId);

        /// <summary>
        /// Inserts a localized product variant
        /// </summary>
        /// <param name="productVariantLocalized">Localized product variant</param>
        void InsertProductVariantLocalized(ProductVariantLocalized productVariantLocalized);

        /// <summary>
        /// Update a localized product variant
        /// </summary>
        /// <param name="productVariantLocalized">Localized product variant</param>
        void UpdateProductVariantLocalized(ProductVariantLocalized productVariantLocalized);

        /// <summary>
        /// Marks a product variant as deleted
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        void MarkProductVariantAsDeleted(int productVariantId);

        /// <summary>
        /// Adjusts inventory
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="decrease">A value indicating whether to increase or descrease product variant stock quantity</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        void AdjustInventory(int productVariantId, bool decrease,
            int quantity, string attributesXml);

        #endregion
        
        #region Product pictures

        /// <summary>
        /// Deletes a product picture mapping
        /// </summary>
        /// <param name="productPictureId">Product picture mapping identifier</param>
        void DeleteProductPicture(int productPictureId);

        /// <summary>
        /// Gets a product picture mapping
        /// </summary>
        /// <param name="productPictureId">Product picture mapping identifier</param>
        /// <returns>Product picture mapping</returns>
        ProductPicture GetProductPictureById(int productPictureId);

        /// <summary>
        /// Inserts a product picture mapping
        /// </summary>
        /// <param name="productPicture">Product picture mapping</param>
        void InsertProductPicture(ProductPicture productPicture);

        /// <summary>
        /// Updates the product picture mapping
        /// </summary>
        /// <param name="productPicture">Product picture mapping</param>
        void UpdateProductPicture(ProductPicture productPicture);

        /// <summary>
        /// Gets all product picture mappings by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product picture mapping collection</returns>
        List<ProductPicture> GetProductPicturesByProductId(int productId);
        #endregion

        #region Product reviews

        /// <summary>
        /// Gets a product review
        /// </summary>
        /// <param name="productReviewId">Product review identifier</param>
        /// <returns>Product review</returns>
        ProductReview GetProductReviewById(int productReviewId);

        /// <summary>
        /// Gets a product review collection by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product review collection</returns>
        List<ProductReview> GetProductReviewByProductId(int productId);

        /// <summary>
        /// Deletes a product review
        /// </summary>
        /// <param name="productReviewId">Product review identifier</param>
        void DeleteProductReview(int productReviewId);

        /// <summary>
        /// Gets all product reviews
        /// </summary>
        /// <returns>Product review collection</returns>
        List<ProductReview> GetAllProductReviews();

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
        ProductReview InsertProductReview(int productId,
            int customerId, string title,
            string reviewText, int rating, int helpfulYesTotal,
            int helpfulNoTotal, bool isApproved, DateTime createdOn);

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
        ProductReview InsertProductReview(int productId,
            int customerId, string title,
            string reviewText, int rating, int helpfulYesTotal,
            int helpfulNoTotal, bool isApproved, DateTime createdOn, bool notify);

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
        ProductReview InsertProductReview(int productId,
            int customerId, string ipAddress, string title,
            string reviewText, int rating, int helpfulYesTotal,
            int helpfulNoTotal, bool isApproved, DateTime createdOn, bool notify);

        /// <summary>
        /// Updates the product review
        /// </summary>
        /// <param name="ProductReview">Product review</param>
        void UpdateProductReview(ProductReview productReview);

        /// <summary>
        /// Sets a product rating helpfulness
        /// </summary>
        /// <param name="productReviewId">Product review identifer</param>
        /// <param name="wasHelpful">A value indicating whether the product review was helpful or not </param>
        void SetProductRatingHelpfulness(int productReviewId, bool wasHelpful);
        
        #endregion

        #region Related products

        /// <summary>
        /// Deletes a related product
        /// </summary>
        /// <param name="relatedProductId">Related product identifer</param>
        void DeleteRelatedProduct(int relatedProductId);

        /// <summary>
        /// Gets a related product collection by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <returns>Related product collection</returns>
        List<RelatedProduct> GetRelatedProductsByProductId1(int productId1);

        /// <summary>
        /// Gets a related product
        /// </summary>
        /// <param name="relatedProductId">Related product identifer</param>
        /// <returns>Related product</returns>
        RelatedProduct GetRelatedProductById(int relatedProductId);

        /// <summary>
        /// Inserts a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        void InsertRelatedProduct(RelatedProduct relatedProduct);

        /// <summary>
        /// Updates a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        void UpdateRelatedProduct(RelatedProduct relatedProduct);

        #endregion

        #region Cross-sell products

        /// <summary>
        /// Deletes a cross-sell product
        /// </summary>
        /// <param name="crossSellProductId">Cross-sell identifer</param>
        void DeleteCrossSellProduct(int crossSellProductId);

        /// <summary>
        /// Gets a cross-sell product collection by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <returns>Cross-sell product collection</returns>
        List<CrossSellProduct> GetCrossSellProductsByProductId1(int productId1);

        /// <summary>
        /// Gets a cross-sell product
        /// </summary>
        /// <param name="crossSellProductId">Cross-sell product identifer</param>
        /// <returns>Cross-sell product</returns>
        CrossSellProduct GetCrossSellProductById(int crossSellProductId);

        /// <summary>
        /// Inserts a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell product</param>
        void InsertCrossSellProduct(CrossSellProduct crossSellProduct);

        /// <summary>
        /// Updates a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell product</param>
        void UpdateCrossSellProduct(CrossSellProduct crossSellProduct);

        #endregion

        #region Pricelists

        /// <summary>
        /// Gets all product variants directly assigned to a pricelist
        /// </summary>
        /// <param name="pricelistId">Pricelist identifier</param>
        /// <returns>Product variants</returns>
        List<ProductVariant> GetProductVariantsByPricelistId(int pricelistId);

        /// <summary>
        /// Deletes a pricelist
        /// </summary>
        /// <param name="pricelistId">The PricelistId of the item to be deleted</param>
        void DeletePricelist(int pricelistId);

        /// <summary>
        /// Gets a collection of all available pricelists
        /// </summary>
        /// <returns>Collection of pricelists</returns>
        List<Pricelist> GetAllPricelists();

        /// <summary>
        /// Gets a pricelist
        /// </summary>
        /// <param name="pricelistId">Pricelist identifier</param>
        /// <returns>Pricelist</returns>
        Pricelist GetPricelistById(int pricelistId);

        /// <summary>
        /// Gets a pricelist
        /// </summary>
        /// <param name="pricelistGuid">Pricelist GUId</param>
        /// <returns>Pricelist</returns>
        Pricelist GetPricelistByGuid(string pricelistGuid);

        /// <summary>
        /// Inserts a Pricelist
        /// </summary>
        /// <param name="pricelist">Pricelist</param>
        void InsertPricelist(Pricelist pricelist);

        /// <summary>
        /// Updates the Pricelist
        /// </summary>
        /// <param name="pricelist">Pricelist</param>
        void UpdatePricelist(Pricelist pricelist);

        /// <summary>
        /// Deletes a ProductVariantPricelist
        /// </summary>
        /// <param name="productVariantPricelistId">ProductVariantPricelist identifier</param>
        void DeleteProductVariantPricelist(int productVariantPricelistId);

        /// <summary>
        /// Gets a ProductVariantPricelist
        /// </summary>
        /// <param name="productVariantPricelistId">ProductVariantPricelist identifier</param>
        /// <returns>ProductVariantPricelist</returns>
        ProductVariantPricelist GetProductVariantPricelistById(int productVariantPricelistId);

        /// <summary>
        /// Gets ProductVariantPricelist
        /// </summary>
        /// <param name="productVariantId">ProductVariant identifier</param>
        /// <param name="pricelistId">Pricelist identifier</param>
        /// <returns>ProductVariantPricelist</returns>
        ProductVariantPricelist GetProductVariantPricelist(int productVariantId, int pricelistId);

        /// <summary>
        /// Inserts a ProductVariantPricelist
        /// </summary>
        /// <param name="productVariantPricelist">The product variant pricelist</param>
        void InsertProductVariantPricelist(ProductVariantPricelist productVariantPricelist);

        /// <summary>
        /// Updates the ProductVariantPricelist
        /// </summary>
        /// <param name="productVariantPricelist">The product variant pricelist</param>
        void UpdateProductVariantPricelist(ProductVariantPricelist productVariantPricelist);

        #endregion

        #region Tier prices

        /// <summary>
        /// Gets a tier price
        /// </summary>
        /// <param name="tierPriceId">Tier price identifier</param>
        /// <returns>Tier price</returns>
        TierPrice GetTierPriceById(int tierPriceId);

        /// <summary>
        /// Gets tier prices by product variant identifier
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Tier price collection</returns>
        List<TierPrice> GetTierPricesByProductVariantId(int productVariantId);

        /// <summary>
        /// Deletes a tier price
        /// </summary>
        /// <param name="tierPriceId">Tier price identifier</param>
        void DeleteTierPrice(int tierPriceId);

        /// <summary>
        /// Inserts a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        void InsertTierPrice(TierPrice tierPrice);

        /// <summary>
        /// Updates the tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        void UpdateTierPrice(TierPrice tierPrice);

        #endregion

        #region Product prices by customer role

        /// <summary>
        /// Deletes a product price by customer role by identifier 
        /// </summary>
        /// <param name="customerRoleProductPriceId">The identifier</param>
        void DeleteCustomerRoleProductPrice(int customerRoleProductPriceId);

        /// <summary>
        /// Gets a product price by customer role by identifier 
        /// </summary>
        /// <param name="customerRoleProductPriceId">The identifier</param>
        /// <returns>Product price by customer role by identifier </returns>
        CustomerRoleProductPrice GetCustomerRoleProductPriceById(int customerRoleProductPriceId);

        /// <summary>
        /// Gets a collection of product prices by customer role
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>A collection of product prices by customer role</returns>
        List<CustomerRoleProductPrice> GetAllCustomerRoleProductPrices(int productVariantId);

        /// <summary>
        /// Inserts a product price by customer role
        /// </summary>
        /// <param name="customerRoleProductPrice">A product price by customer role</param>
        void InsertCustomerRoleProductPrice(CustomerRoleProductPrice customerRoleProductPrice);

        /// <summary>
        /// Updates a product price by customer role
        /// </summary>
        /// <param name="customerRoleProductPrice">A product price by customer role</param>
        void UpdateCustomerRoleProductPrice(CustomerRoleProductPrice customerRoleProductPrice);

        #endregion

        #region Product tags

        /// <summary>
        /// Deletes a product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        void DeleteProductTag(int productTagId);

        /// <summary>
        /// Gets a product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>Product tag</returns>
        ProductTag GetProductTagById(int productTagId);

        /// <summary>
        /// Gets product tags by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product tag collection</returns>
        List<ProductTag> GetProductTagsByProductId(int productId);

        /// <summary>
        /// Gets product tag by name
        /// </summary>
        /// <param name="name">Product tag name</param>
        /// <returns>Product tag</returns>
        ProductTag GetProductTagByName(string name);

        /// <summary>
        /// Gets all product tags
        /// </summary>
        /// <returns>Product tag collection</returns>
        List<ProductTag> GetAllProductTags();

        /// <summary>
        /// Inserts a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        void InsertProductTag(ProductTag productTag);

        /// <summary>
        /// Updates a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        void UpdateProductTag(ProductTag productTag);

        /// <summary>
        /// Adds a discount tag mapping
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="productTagId">Product tag identifier</param>
        void AddProductTagMapping(int productId, int productTagId);

        /// <summary>
        /// Checking whether the product tag mapping exists
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <param name="productTagId">The product tag identifier</param>
        /// <returns>True if mapping exist, otherwise false</returns>
        bool DoesProductTagMappingExist(int productId, int productTagId);

        /// <summary>
        /// Removes a discount tag mapping
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="productTagId">Product tag identifier</param>
        void RemoveProductTagMapping(int productId, int productTagId);

        #endregion
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether "Recently viewed products" feature is enabled
        /// </summary>
        bool RecentlyViewedProductsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a number of "Recently viewed products"
        /// </summary>
        int RecentlyViewedProductsNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Recently added products" feature is enabled
        /// </summary>
        bool RecentlyAddedProductsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a number of "Recently added products"
        /// </summary>
        int RecentlyAddedProductsNumber { get; set; }

        /// <summary>
        /// Gets or sets a number of "Cross-sells"
        /// </summary>
        int CrossSellsNumber { get; set; }

        /// <summary>
        /// Gets or sets a number of products per page on search products page
        /// </summary>
        int SearchPageProductsPerPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to displays a button from AddThis.com on your product pages
        /// </summary>
        bool ShowShareButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "Compare products" feature is enabled
        /// </summary>
        bool CompareProductsEnabled { get; set; }

        /// <summary>
        /// Gets or sets "List of products purchased by other customers who purchased the above" option is enable
        /// </summary>
        bool ProductsAlsoPurchasedEnabled { get; set; }

        /// <summary>
        /// Gets or sets a number of products also purchased by other customers to display
        /// </summary>
        int ProductsAlsoPurchasedNumber { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to notify about new product reviews
        /// </summary>
        bool NotifyAboutNewProductReviews { get; set; }

        #endregion
    }
}
