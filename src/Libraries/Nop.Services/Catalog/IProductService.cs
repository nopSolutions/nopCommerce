using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product service
    /// </summary>
    public partial interface IProductService
    {
        #region Products

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="product">Product</param>
        Task DeleteProductAsync(Product product);

        /// <summary>
        /// Delete products
        /// </summary>
        /// <param name="products">Products</param>
        Task DeleteProductsAsync(IList<Product> products);

        /// <summary>
        /// Gets all products displayed on the home page
        /// </summary>
        /// <returns>Products</returns>
        Task<IList<Product>> GetAllProductsDisplayedOnHomepageAsync();

        /// <summary>
        /// Gets featured products by a category identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>List of featured products</returns>
        Task<IList<Product>> GetCategoryFeaturedProductsAsync(int categoryId, int storeId = 0);

        /// <summary>
        /// Gets featured products by a manufacturer identifier
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>List of featured products</returns>
        Task<IList<Product>> GetManufacturerFeaturedProductsAsync(int manufacturerId, int storeId = 0);

        /// <summary>
        /// Gets products which marked as new
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>List of new products</returns>
        Task<IList<Product>> GetProductsMarkedAsNewAsync(int storeId = 0);

        /// <summary>
        /// Gets product
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product</returns>
        Task<Product> GetProductByIdAsync(int productId);

        /// <summary>
        /// Gets products by identifier
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>Products</returns>
        Task<IList<Product>> GetProductsByIdsAsync(int[] productIds);

        /// <summary>
        /// Inserts a product
        /// </summary>
        /// <param name="product">Product</param>
        Task InsertProductAsync(Product product);

        /// <summary>
        /// Updates the product
        /// </summary>
        /// <param name="product">Product</param>
        Task UpdateProductAsync(Product product);

        /// <summary>
        /// Get number of product (published and visible) in certain category
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>Number of products</returns>
        Task<int> GetNumberOfProductsInCategoryAsync(IList<int> categoryIds = null, int storeId = 0);

        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="manufacturerIds">Manufacturer identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
        /// <param name="productType">Product type; 0 to load all records</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only products marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="excludeFeaturedProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers); "false" (by default) to load all records; "true" to exclude featured products from results</param>
        /// <param name="priceMin">Minimum price; null to load all records</param>
        /// <param name="priceMax">Maximum price; null to load all records</param>
        /// <param name="productTagId">Product tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in product descriptions</param>
        /// <param name="searchManufacturerPartNumber">A value indicating whether to search by a specified "keyword" in manufacturer part number</param>
        /// <param name="searchSku">A value indicating whether to search by a specified "keyword" in product SKU</param>
        /// <param name="searchProductTags">A value indicating whether to search by a specified "keyword" in product tags</param>
        /// <param name="languageId">Language identifier (search for text searching)</param>
        /// <param name="filteredSpecs">Filtered product specification identifiers</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>Products</returns>
        Task<IPagedList<Product>> SearchProductsAsync(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            IList<int> manufacturerIds = null,
            int storeId = 0,
            int vendorId = 0,
            int warehouseId = 0,
            ProductType? productType = null,
            bool visibleIndividuallyOnly = false,
            bool excludeFeaturedProducts = false,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int productTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchManufacturerPartNumber = true,
            bool searchSku = true,
            bool searchProductTags = false,
            int languageId = 0,
            IList<int> filteredSpecs = null,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null);

        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="loadFilterableSpecificationAttributeOptionIds">A value indicating whether we should load the specification attribute option identifiers applied to loaded products (all pages)</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="manufacturerIds">Manufacturer identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
        /// <param name="productType">Product type; 0 to load all records</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only products marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="excludeFeaturedProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers); "false" (by default) to load all records; "true" to exclude featured products from results</param>
        /// <param name="priceMin">Minimum price; null to load all records</param>
        /// <param name="priceMax">Maximum price; null to load all records</param>
        /// <param name="productTagId">Product tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in product descriptions</param>
        /// <param name="searchManufacturerPartNumber">A value indicating whether to search by a specified "keyword" in manufacturer part number</param>
        /// <param name="searchSku">A value indicating whether to search by a specified "keyword" in product SKU</param>
        /// <param name="searchProductTags">A value indicating whether to search by a specified "keyword" in product tags</param>
        /// <param name="languageId">Language identifier (search for text searching)</param>
        /// <param name="filteredSpecs">Filtered product specification identifiers</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>Products; specification attribute option ids</returns>
        Task<(IPagedList<Product> Products, IList<int> SpecificationAttributeOptionIds)> SearchProductsAsync(
            bool loadFilterableSpecificationAttributeOptionIds = false,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            IList<int> manufacturerIds = null,
            int storeId = 0,
            int vendorId = 0,
            int warehouseId = 0,
            ProductType? productType = null,
            bool visibleIndividuallyOnly = false,
            bool excludeFeaturedProducts = false,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int productTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchManufacturerPartNumber = true,
            bool searchSku = true,
            bool searchProductTags = false,
            int languageId = 0,
            IList<int> filteredSpecs = null,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null);

        /// <summary>
        /// Gets products by product attribute
        /// </summary>
        /// <param name="productAttributeId">Product attribute identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Products</returns>
        Task<IPagedList<Product>> GetProductsByProductAtributeIdAsync(int productAttributeId,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets associated products
        /// </summary>
        /// <param name="parentGroupedProductId">Parent product identifier (used with grouped products)</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Products</returns>
        Task<IList<Product>> GetAssociatedProductsAsync(int parentGroupedProductId,
            int storeId = 0, int vendorId = 0, bool showHidden = false);

        /// <summary>
        /// Update product review totals
        /// </summary>
        /// <param name="product">Product</param>
        Task UpdateProductReviewTotalsAsync(Product product);

        /// <summary>
        /// Get low stock products
        /// </summary>
        /// <param name="vendorId">Vendor identifier; pass null to load all records</param>
        /// <param name="loadPublishedOnly">Whether to load published products only; pass null to load all products, pass true to load only published products, pass false to load only unpublished products</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>Products</returns>
        Task<IPagedList<Product>> GetLowStockProductsAsync(int? vendorId = null, bool? loadPublishedOnly = true,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Get low stock product combinations
        /// </summary>
        /// <param name="vendorId">Vendor identifier; pass null to load all records</param>
        /// <param name="loadPublishedOnly">Whether to load combinations of published products only; pass null to load all products, pass true to load only published products, pass false to load only unpublished products</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>Product combinations</returns>
        Task<IPagedList<ProductAttributeCombination>> GetLowStockProductCombinationsAsync(int? vendorId = null, bool? loadPublishedOnly = true,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Gets a product by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Product</returns>
        Task<Product> GetProductBySkuAsync(string sku);

        /// <summary>
        /// Gets a products by SKU array
        /// </summary>
        /// <param name="skuArray">SKU array</param>
        /// <param name="vendorId">Vendor ID; 0 to load all records</param>
        /// <returns>Products</returns>
        Task<IList<Product>> GetProductsBySkuAsync(string[] skuArray, int vendorId = 0);

        /// <summary>
        /// Update HasTierPrices property (used for performance optimization)
        /// </summary>
        /// <param name="product">Product</param>
        Task UpdateHasTierPricesPropertyAsync(Product product);

        /// <summary>
        /// Update HasDiscountsApplied property (used for performance optimization)
        /// </summary>
        /// <param name="product">Product</param>
        Task UpdateHasDiscountsAppliedAsync(Product product);

        /// <summary>
        /// Gets number of products by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Number of products</returns>
        Task<int> GetNumberOfProductsByVendorIdAsync(int vendorId);

        //TODO: migrate to an extension method
        /// <summary>
        /// Parse "required product Ids" property
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>A list of required product IDs</returns>
        int[] ParseRequiredProductIds(Product product);

        //TODO: migrate to an extension method
        /// <summary>
        /// Get a value indicating whether a product is available now (availability dates)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="dateTime">Datetime to check; pass null to use current date</param>
        /// <returns>Result</returns>
        bool ProductIsAvailable(Product product, DateTime? dateTime = null);

        //TODO: migrate to an extension method
        /// <summary>
        /// Get a list of allowed quantities (parse 'AllowedQuantities' property)
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Result</returns>
        int[] ParseAllowedQuantities(Product product);

        /// <summary>
        /// Get total quantity
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="useReservedQuantity">
        /// A value indicating whether we should consider "Reserved Quantity" property 
        /// when "multiple warehouses" are used
        /// </param>
        /// <param name="warehouseId">
        /// Warehouse identifier. Used to limit result to certain warehouse.
        /// Used only with "multiple warehouses" enabled.
        /// </param>
        /// <returns>Result</returns>
        Task<int> GetTotalStockQuantityAsync(Product product, bool useReservedQuantity = true, int warehouseId = 0);

        //TODO: migrate to an extension method
        /// <summary>
        /// Get number of rental periods (price ratio)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Number of rental periods</returns>
        int GetRentalPeriods(Product product, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Formats the stock availability/quantity message
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Selected product attributes in XML format (if specified)</param>
        /// <returns>The stock message</returns>
        Task<string> FormatStockMessageAsync(Product product, string attributesXml);

        //TODO: migrate to an extension method
        /// <summary>
        /// Formats SKU
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>SKU</returns>
        Task<string> FormatSkuAsync(Product product, string attributesXml = null);

        //TODO: migrate to an extension method
        /// <summary>
        /// Formats manufacturer part number
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Manufacturer part number</returns>
        Task<string> FormatMpnAsync(Product product, string attributesXml = null);

        //TODO: migrate to an extension method
        /// <summary>
        /// Formats GTIN
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>GTIN</returns>
        Task<string> FormatGtinAsync(Product product, string attributesXml = null);

        //TODO: migrate to an extension method
        /// <summary>
        /// Formats start/end date for rental product
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="date">Date</param>
        /// <returns>Formatted date</returns>
        string FormatRentalDate(Product product, DateTime date);

        /// <summary>
        /// Update product store mappings
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="limitedToStoresIds">A list of store ids for mapping</param>
        Task UpdateProductStoreMappingsAsync(Product product, IList<int> limitedToStoresIds);

        /// <summary>
        /// Gets the value whether the sequence contains downloadable products
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>Result</returns>
        Task<bool> HasAnyDownloadableProductAsync(int[] productIds);

        /// <summary>
        /// Gets the value whether the sequence contains gift card products
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>Result</returns>
        Task<bool> HasAnyGiftCardProductAsync(int[] productIds);

        /// <summary>
        /// Gets the value whether the sequence contains recurring products
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>Result</returns>
        Task<bool> HasAnyRecurringProductAsync(int[] productIds);

        #endregion

        #region Inventory management methods

        /// <summary>
        /// Adjust inventory
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="quantityToChange">Quantity to increase or decrease</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="message">Message for the stock quantity history</param>
        Task AdjustInventoryAsync(Product product, int quantityToChange, string attributesXml = "", string message = "");

        /// <summary>
        /// Book the reserved quantity
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="quantity">Quantity, must be negative</param>
        /// <param name="message">Message for the stock quantity history</param>
        Task BookReservedInventoryAsync(Product product, int warehouseId, int quantity, string message = "");

        /// <summary>
        /// Reverse booked inventory (if acceptable)
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="shipmentItem">Shipment item</param>
        /// <returns>Quantity reversed</returns>
        /// <param name="message">Message for the stock quantity history</param>
        Task<int> ReverseBookedInventoryAsync(Product product, ShipmentItem shipmentItem, string message = "");

        #endregion

        #region Related products

        /// <summary>
        /// Deletes a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        Task DeleteRelatedProductAsync(RelatedProduct relatedProduct);

        /// <summary>
        /// Gets related products by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Related products</returns>
        Task<IList<RelatedProduct>> GetRelatedProductsByProductId1Async(int productId1, bool showHidden = false);

        /// <summary>
        /// Gets a related product
        /// </summary>
        /// <param name="relatedProductId">Related product identifier</param>
        /// <returns>Related product</returns>
        Task<RelatedProduct> GetRelatedProductByIdAsync(int relatedProductId);

        /// <summary>
        /// Inserts a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        Task InsertRelatedProductAsync(RelatedProduct relatedProduct);

        /// <summary>
        /// Updates a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        Task UpdateRelatedProductAsync(RelatedProduct relatedProduct);

        //TODO: migrate to an extension method
        /// <summary>
        /// Finds a related product item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="productId2">The second product identifier</param>
        /// <returns>Related product</returns>
        RelatedProduct FindRelatedProduct(IList<RelatedProduct> source, int productId1, int productId2);

        #endregion

        #region Cross-sell products

        /// <summary>
        /// Deletes a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell</param>
        Task DeleteCrossSellProductAsync(CrossSellProduct crossSellProduct);

        /// <summary>
        /// Gets cross-sell products by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Cross-sell products</returns>
        Task<IList<CrossSellProduct>> GetCrossSellProductsByProductId1Async(int productId1, bool showHidden = false);

        /// <summary>
        /// Gets a cross-sell product
        /// </summary>
        /// <param name="crossSellProductId">Cross-sell product identifier</param>
        /// <returns>Cross-sell product</returns>
        Task<CrossSellProduct> GetCrossSellProductByIdAsync(int crossSellProductId);

        /// <summary>
        /// Inserts a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell product</param>
        Task InsertCrossSellProductAsync(CrossSellProduct crossSellProduct);

        /// <summary>
        /// Gets a cross-sells
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="numberOfProducts">Number of products to return</param>
        /// <returns>Cross-sells</returns>
        Task<IList<Product>> GetCrosssellProductsByShoppingCartAsync(IList<ShoppingCartItem> cart, int numberOfProducts);

        //TODO: migrate to an extension method
        /// <summary>
        /// Finds a cross-sell product item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="productId2">The second product identifier</param>
        /// <returns>Cross-sell product</returns>
        CrossSellProduct FindCrossSellProduct(IList<CrossSellProduct> source, int productId1, int productId2);

        #endregion

        #region Tier prices

        /// <summary>
        /// Gets a product tier prices for customer
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Store identifier</param>
        Task<IList<TierPrice>> GetTierPricesAsync(Product product, Customer customer, int storeId);

        /// <summary>
        /// Gets a tier prices by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        Task<IList<TierPrice>> GetTierPricesByProductAsync(int productId);

        /// <summary>
        /// Deletes a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        Task DeleteTierPriceAsync(TierPrice tierPrice);

        /// <summary>
        /// Gets a tier price
        /// </summary>
        /// <param name="tierPriceId">Tier price identifier</param>
        /// <returns>Tier price</returns>
        Task<TierPrice> GetTierPriceByIdAsync(int tierPriceId);

        /// <summary>
        /// Inserts a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        Task InsertTierPriceAsync(TierPrice tierPrice);

        /// <summary>
        /// Updates the tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        Task UpdateTierPriceAsync(TierPrice tierPrice);

        /// <summary>
        /// Gets a preferred tier price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Tier price</returns>
        Task<TierPrice> GetPreferredTierPriceAsync(Product product, Customer customer, int storeId, int quantity);

        #endregion

        #region Product pictures

        /// <summary>
        /// Deletes a product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        Task DeleteProductPictureAsync(ProductPicture productPicture);

        /// <summary>
        /// Gets a product pictures by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>Product pictures</returns>
        Task<IList<ProductPicture>> GetProductPicturesByProductIdAsync(int productId);

        /// <summary>
        /// Gets a product picture
        /// </summary>
        /// <param name="productPictureId">Product picture identifier</param>
        /// <returns>Product picture</returns>
        Task<ProductPicture> GetProductPictureByIdAsync(int productPictureId);

        /// <summary>
        /// Inserts a product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        Task InsertProductPictureAsync(ProductPicture productPicture);

        /// <summary>
        /// Updates a product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        Task UpdateProductPictureAsync(ProductPicture productPicture);

        /// <summary>
        /// Get the IDs of all product images 
        /// </summary>
        /// <param name="productsIds">Products IDs</param>
        /// <returns>All picture identifiers grouped by product ID</returns>
        Task<IDictionary<int, int[]>> GetProductsImagesIdsAsync(int[] productsIds);

        /// <summary>
        /// Get products to which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted products</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of products</returns>
        Task<IPagedList<Product>> GetProductsWithAppliedDiscountAsync(int? discountId = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);

        #endregion

        #region Product reviews

        /// <summary>
        /// Gets all product reviews
        /// </summary>
        /// <param name="customerId">Customer identifier (who wrote a review); 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item creation to; null to load all records</param>
        /// <param name="message">Search title or review text; null to load all records</param>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="productId">The product identifier; pass 0 to load all records</param>
        /// <param name="vendorId">The vendor identifier (limit to products of this vendor); pass 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Reviews</returns>
        Task<IPagedList<ProductReview>> GetAllProductReviewsAsync(int customerId = 0, bool? approved = null,
            DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = null, int storeId = 0, int productId = 0, int vendorId = 0, bool showHidden = false,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets product review
        /// </summary>
        /// <param name="productReviewId">Product review identifier</param>
        /// <returns>Product review</returns>
        Task<ProductReview> GetProductReviewByIdAsync(int productReviewId);

        /// <summary>
        /// Get product reviews by identifiers
        /// </summary>
        /// <param name="productReviewIds">Product review identifiers</param>
        /// <returns>Product reviews</returns>
        Task<IList<ProductReview>> GetProductReviewsByIdsAsync(int[] productReviewIds);

        /// <summary>
        /// Inserts a product review
        /// </summary>
        /// <param name="productReview">Product review</param>
        Task InsertProductReviewAsync(ProductReview productReview);

        /// <summary>
        /// Deletes a product review
        /// </summary>
        /// <param name="productReview">Product review</param>
        Task DeleteProductReviewAsync(ProductReview productReview);

        /// <summary>
        /// Deletes product reviews
        /// </summary>
        /// <param name="productReviews">Product reviews</param>
        Task DeleteProductReviewsAsync(IList<ProductReview> productReviews);

        /// <summary>
        /// Sets or create a product review helpfulness record
        /// </summary>
        /// <param name="productReview">Product reviews</param>
        /// <param name="helpfulness">Value indicating whether a review a helpful</param>
        Task SetProductReviewHelpfulnessAsync(ProductReview productReview, bool helpfulness);

        /// <summary>
        /// Updates a totals helpfulness count for product review
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <returns>Result</returns>
        Task UpdateProductReviewHelpfulnessTotalsAsync(ProductReview productReview);

        /// <summary>
        /// Updates a product review
        /// </summary>
        /// <param name="productReview">Product review</param>
        Task UpdateProductReviewAsync(ProductReview productReview);

        /// <summary>
        /// Check possibility added review for current customer
        /// </summary>
        /// <param name="productId">Current product</param>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <returns></returns>
        Task<bool> CanAddReviewAsync(int productId, int storeId = 0);

        #endregion

        #region Product warehouses

        /// <summary>
        /// Get a product warehouse-inventory records by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        Task<IList<ProductWarehouseInventory>> GetAllProductWarehouseInventoryRecordsAsync(int productId);

        /// <summary>
        /// Gets a warehouse by identifier
        /// </summary>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <returns>Result</returns>
        Task<Warehouse> GetWarehousesByIdAsync(int warehouseId);

        /// <summary>
        /// Deletes a ProductWarehouseInventory
        /// </summary>
        /// <param name="pwi">ProductWarehouseInventory</param>
        Task DeleteProductWarehouseInventoryAsync(ProductWarehouseInventory pwi);

        /// <summary>
        /// Inserts a ProductWarehouseInventory
        /// </summary>
        /// <param name="pwi">ProductWarehouseInventory</param>
        Task InsertProductWarehouseInventoryAsync(ProductWarehouseInventory pwi);

        /// <summary>
        /// Updates a record to manage product inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage product inventory per warehouse</param>
        Task UpdateProductWarehouseInventoryAsync(ProductWarehouseInventory pwi);

        #endregion

        #region Stock quantity history

        /// <summary>
        /// Add stock quantity change entry
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="quantityAdjustment">Quantity adjustment</param>
        /// <param name="stockQuantity">Current stock quantity</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="message">Message</param>
        /// <param name="combinationId">Product attribute combination identifier</param>
        Task AddStockQuantityHistoryEntryAsync(Product product, int quantityAdjustment, int stockQuantity,
            int warehouseId = 0, string message = "", int? combinationId = null);

        /// <summary>
        /// Get the history of the product stock quantity changes
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="warehouseId">Warehouse identifier; pass 0 to load all entries</param>
        /// <param name="combinationId">Product attribute combination identifier; pass 0 to load all entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of stock quantity change entries</returns>
        Task<IPagedList<StockQuantityHistory>> GetStockQuantityHistoryAsync(Product product, int warehouseId = 0, int combinationId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue);

        #endregion

        #region Product discounts

        /// <summary>
        /// Clean up product references for a specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
        Task ClearDiscountProductMappingAsync(Discount discount);

        /// <summary>
        /// Get a discount-product mapping records by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        Task<IList<DiscountProductMapping>> GetAllDiscountsAppliedToProductAsync(int productId);

        /// <summary>
        /// Get a discount-product mapping record
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Result</returns>
        Task<DiscountProductMapping> GetDiscountAppliedToProductAsync(int productId, int discountId);

        /// <summary>
        /// Inserts a discount-product mapping record
        /// </summary>
        /// <param name="discountProductMapping">Discount-product mapping</param>
        Task InsertDiscountProductMappingAsync(DiscountProductMapping discountProductMapping);

        /// <summary>
        /// Deletes a discount-product mapping record
        /// </summary>
        /// <param name="discountProductMapping">Discount-product mapping</param>
        Task DeleteDiscountProductMappingAsync(DiscountProductMapping discountProductMapping);

        #endregion
    }
}