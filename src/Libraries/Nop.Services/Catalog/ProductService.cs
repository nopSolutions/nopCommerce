using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product service
    /// </summary>
    public partial class ProductService : IProductService
    {
        #region Fields

        protected readonly CatalogSettings _catalogSettings;
        protected readonly CommonSettings _commonSettings;
        protected readonly IAclService _aclService;
        protected readonly ICacheKeyService _cacheKeyService;
        protected readonly ICustomerService _customerService;
        protected readonly INopDataProvider _dataProvider;
        protected readonly IDateRangeService _dateRangeService;
        protected readonly IEventPublisher _eventPublisher;
        protected readonly ILanguageService _languageService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IProductAttributeParser _productAttributeParser;
        protected readonly IProductAttributeService _productAttributeService;
        protected readonly IRepository<AclRecord> _aclRepository;
        protected readonly IRepository<CrossSellProduct> _crossSellProductRepository;
        protected readonly IRepository<DiscountProductMapping> _discountProductMappingRepository;
        protected readonly IRepository<Product> _productRepository;
        protected readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        protected readonly IRepository<ProductAttributeMapping> _productAttributeMappingRepository;
        protected readonly IRepository<ProductCategory> _productCategoryRepository;
        protected readonly IRepository<ProductPicture> _productPictureRepository;
        protected readonly IRepository<ProductReview> _productReviewRepository;
        protected readonly IRepository<ProductReviewHelpfulness> _productReviewHelpfulnessRepository;
        protected readonly IRepository<ProductWarehouseInventory> _productWarehouseInventoryRepository;
        protected readonly IRepository<RelatedProduct> _relatedProductRepository;
        protected readonly IRepository<Shipment> _shipmentRepository;
        protected readonly IRepository<StockQuantityHistory> _stockQuantityHistoryRepository;
        protected readonly IRepository<StoreMapping> _storeMappingRepository;
        protected readonly IRepository<TierPrice> _tierPriceRepository;
        protected readonly IRepository<Warehouse> _warehouseRepository;
        protected readonly IStaticCacheManager _staticCacheManager;
        protected readonly IStoreMappingService _storeMappingService;
        protected readonly IStoreService _storeService;
        protected readonly IWorkContext _workContext;
        protected readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public ProductService(CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            IAclService aclService,
            ICacheKeyService cacheKeyService,
            ICustomerService customerService,
            INopDataProvider dataProvider,
            IDateRangeService dateRangeService,
            IEventPublisher eventPublisher,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IRepository<AclRecord> aclRepository,
            IRepository<CrossSellProduct> crossSellProductRepository,
            IRepository<DiscountProductMapping> discountProductMappingRepository,
            IRepository<Product> productRepository,
            IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
            IRepository<ProductAttributeMapping> productAttributeMappingRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<ProductPicture> productPictureRepository,
            IRepository<ProductReview> productReviewRepository,
            IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository,
            IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
            IRepository<RelatedProduct> relatedProductRepository,
            IRepository<Shipment> shipmentRepository,
            IRepository<StockQuantityHistory> stockQuantityHistoryRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IRepository<TierPrice> tierPriceRepository,
            IRepository<Warehouse> warehouseRepositor,
            IStaticCacheManager staticCacheManager,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            LocalizationSettings localizationSettings)
        {
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _aclService = aclService;
            _cacheKeyService = cacheKeyService;
            _customerService = customerService;
            _dataProvider = dataProvider;
            _dateRangeService = dateRangeService;
            _eventPublisher = eventPublisher;
            _languageService = languageService;
            _localizationService = localizationService;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _aclRepository = aclRepository;
            _crossSellProductRepository = crossSellProductRepository;
            _discountProductMappingRepository = discountProductMappingRepository;
            _productRepository = productRepository;
            _productAttributeCombinationRepository = productAttributeCombinationRepository;
            _productAttributeMappingRepository = productAttributeMappingRepository;
            _productCategoryRepository = productCategoryRepository;
            _productPictureRepository = productPictureRepository;
            _productReviewRepository = productReviewRepository;
            _productReviewHelpfulnessRepository = productReviewHelpfulnessRepository;
            _productWarehouseInventoryRepository = productWarehouseInventoryRepository;
            _relatedProductRepository = relatedProductRepository;
            _shipmentRepository = shipmentRepository;
            _stockQuantityHistoryRepository = stockQuantityHistoryRepository;
            _storeMappingRepository = storeMappingRepository;
            _tierPriceRepository = tierPriceRepository;
            _warehouseRepository = warehouseRepositor;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _workContext = workContext;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets SKU, Manufacturer part number and GTIN
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="sku">SKU</param>
        /// <param name="manufacturerPartNumber">Manufacturer part number</param>
        /// <param name="gtin">GTIN</param>
        protected virtual void GetSkuMpnGtin(Product product, string attributesXml,
            out string sku, out string manufacturerPartNumber, out string gtin)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            sku = null;
            manufacturerPartNumber = null;
            gtin = null;

            if (!string.IsNullOrEmpty(attributesXml) &&
                product.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
            {
                //manage stock by attribute combinations
                //let's find appropriate record
                var combination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);
                if (combination != null)
                {
                    sku = combination.Sku;
                    manufacturerPartNumber = combination.ManufacturerPartNumber;
                    gtin = combination.Gtin;
                }
            }

            if (string.IsNullOrEmpty(sku))
                sku = product.Sku;
            if (string.IsNullOrEmpty(manufacturerPartNumber))
                manufacturerPartNumber = product.ManufacturerPartNumber;
            if (string.IsNullOrEmpty(gtin))
                gtin = product.Gtin;
        }

        /// <summary>
        /// Get stock message
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Message</returns>
        protected virtual string GeStockMessage(Product product, string attributesXml)
        {
            if (!product.DisplayStockAvailability)
                return string.Empty;

            string stockMessage;

            var combination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);
            if (combination != null)
            {
                //combination exists
                var stockQuantity = combination.StockQuantity;
                if (stockQuantity > 0)
                {
                    stockMessage = product.DisplayStockQuantity
                        ?
                        //display "in stock" with stock quantity
                        string.Format(_localizationService.GetResource("Products.Availability.InStockWithQuantity"), stockQuantity)
                        :
                        //display "in stock" without stock quantity
                        _localizationService.GetResource("Products.Availability.InStock");
                }
                else if (combination.AllowOutOfStockOrders)
                {
                    stockMessage = _localizationService.GetResource("Products.Availability.InStock");
                }
                else
                {
                    var productAvailabilityRange =
                        _dateRangeService.GetProductAvailabilityRangeById(product.ProductAvailabilityRangeId);
                    stockMessage = productAvailabilityRange == null
                        ? _localizationService.GetResource("Products.Availability.OutOfStock")
                        : string.Format(_localizationService.GetResource("Products.Availability.AvailabilityRange"),
                            _localizationService.GetLocalized(productAvailabilityRange, range => range.Name));
                }
            }
            else
            {
                //no combination configured
                if (product.AllowAddingOnlyExistingAttributeCombinations)
                {
                    var productAvailabilityRange =
                        _dateRangeService.GetProductAvailabilityRangeById(product.ProductAvailabilityRangeId);
                    stockMessage = productAvailabilityRange == null
                        ? _localizationService.GetResource("Products.Availability.OutOfStock")
                        : string.Format(_localizationService.GetResource("Products.Availability.AvailabilityRange"),
                            _localizationService.GetLocalized(productAvailabilityRange, range => range.Name));
                }
                else
                {
                    stockMessage = !_productAttributeService.GetProductAttributeMappingsByProductId(product.Id)
                        .Any(pam => pam.IsRequired) ? _localizationService.GetResource("Products.Availability.InStock") : _localizationService.GetResource("Products.Availability.SelectRequiredAttributes");
                }
            }

            return stockMessage;
        }

        /// <summary>
        /// Get stock message
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="stockMessage">Message</param>
        /// <returns>Message</returns>
        protected virtual string GetStockMessage(Product product, string stockMessage)
        {
            if (!product.DisplayStockAvailability)
                return string.Empty;

            var stockQuantity = GetTotalStockQuantity(product);
            if (stockQuantity > 0)
            {
                stockMessage = product.DisplayStockQuantity
                    ?
                    //display "in stock" with stock quantity
                    string.Format(_localizationService.GetResource("Products.Availability.InStockWithQuantity"), stockQuantity)
                    :
                    //display "in stock" without stock quantity
                    _localizationService.GetResource("Products.Availability.InStock");
            }
            else
            {
                //out of stock
                var productAvailabilityRange = _dateRangeService.GetProductAvailabilityRangeById(product.ProductAvailabilityRangeId);
                switch (product.BackorderMode)
                {
                    case BackorderMode.NoBackorders:
                        stockMessage = productAvailabilityRange == null
                            ? _localizationService.GetResource("Products.Availability.OutOfStock")
                            : string.Format(_localizationService.GetResource("Products.Availability.AvailabilityRange"),
                                _localizationService.GetLocalized(productAvailabilityRange, range => range.Name));
                        break;
                    case BackorderMode.AllowQtyBelow0:
                        stockMessage = _localizationService.GetResource("Products.Availability.InStock");
                        break;
                    case BackorderMode.AllowQtyBelow0AndNotifyCustomer:
                        stockMessage = productAvailabilityRange == null
                            ? _localizationService.GetResource("Products.Availability.Backordering")
                            : string.Format(_localizationService.GetResource("Products.Availability.BackorderingWithDate"),
                                _localizationService.GetLocalized(productAvailabilityRange, range => range.Name));
                        break;
                }
            }

            return stockMessage;
        }

        #endregion

        #region Methods

        #region Products

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="product">Product</param>
        public virtual void DeleteProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            product.Deleted = true;
            //delete product
            UpdateProduct(product);

            //event notification
            _eventPublisher.EntityDeleted(product);
        }

        /// <summary>
        /// Delete products
        /// </summary>
        /// <param name="products">Products</param>
        public virtual void DeleteProducts(IList<Product> products)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products));

            foreach (var product in products)
            {
                product.Deleted = true;
            }

            //delete product
            UpdateProducts(products);

            foreach (var product in products)
            {
                //event notification
                _eventPublisher.EntityDeleted(product);
            }
        }

        /// <summary>
        /// Gets all products displayed on the home page
        /// </summary>
        /// <returns>Products</returns>
        public virtual IList<Product> GetAllProductsDisplayedOnHomepage()
        {
            var query = from p in _productRepository.Table
                        orderby p.DisplayOrder, p.Id
                        where p.Published &&
                        !p.Deleted &&
                        p.ShowOnHomepage
                        select p;

            var products = query.ToCachedList(_cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductsAllDisplayedOnHomepageCacheKey));

            return products;
        }

        /// <summary>
        /// Gets product
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product</returns>
        public virtual Product GetProductById(int productId)
        {
            if (productId == 0)
                return null;

            return _productRepository.ToCachedGetById(productId);
        }

        /// <summary>
        /// Get products by identifiers
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>Products</returns>
        public virtual IList<Product> GetProductsByIds(int[] productIds)
        {
            if (productIds == null || productIds.Length == 0)
                return new List<Product>();

            var key = _cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductsByIdsCacheKey, productIds);

            var query = from p in _productRepository.Table
                        where productIds.Contains(p.Id) && !p.Deleted
                        select p;

            var products = query.ToCachedList(key);

            //sort by passed identifiers
            var sortedProducts = new List<Product>();
            foreach (var id in productIds)
            {
                var product = products.FirstOrDefault(x => x.Id == id);
                if (product != null)
                    sortedProducts.Add(product);
            }

            return sortedProducts;
        }

        /// <summary>
        /// Inserts a product
        /// </summary>
        /// <param name="product">Product</param>
        public virtual void InsertProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //insert
            _productRepository.Insert(product);
            
            //event notification
            _eventPublisher.EntityInserted(product);
        }

        /// <summary>
        /// Updates the product
        /// </summary>
        /// <param name="product">Product</param>
        public virtual void UpdateProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //update
            _productRepository.Update(product);
            
            //event notification
            _eventPublisher.EntityUpdated(product);
        }

        /// <summary>
        /// Update products
        /// </summary>
        /// <param name="products">Products</param>
        public virtual void UpdateProducts(IList<Product> products)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products));

            //update
            _productRepository.Update(products);
            
            //event notification
            foreach (var product in products)
            {
                _eventPublisher.EntityUpdated(product);
            }
        }

        /// <summary>
        /// Get number of product (published and visible) in certain category
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>Number of products</returns>
        public virtual int GetNumberOfProductsInCategory(IList<int> categoryIds = null, int storeId = 0)
        {
            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            var query = _productRepository.Table;
            query = query.Where(p => !p.Deleted && p.Published && p.VisibleIndividually);

            //category filtering
            if (categoryIds != null && categoryIds.Any())
            {
                query = from p in query
                        join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId
                        where categoryIds.Contains(pc.CategoryId)
                        select p;
            }

            var allowedCustomerRolesIds = new List<int>();

            if (!_catalogSettings.IgnoreAcl)
            {
                //Access control list. Allowed customer roles
                allowedCustomerRolesIds.AddRange(_customerService.GetCustomerRoleIds(_workContext.CurrentCustomer));

                query = from p in query
                        join acl in _aclRepository.Table
                        on new { c1 = p.Id, c2 = nameof(Product) } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into p_acl
                        from acl in p_acl.DefaultIfEmpty()
                        where !p.SubjectToAcl || allowedCustomerRolesIds.ToArray().Contains(acl.CustomerRoleId)
                        select p;
            }
            else
            {
                allowedCustomerRolesIds.Add(0);
            }

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                query = from p in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = p.Id, c2 = nameof(Product) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into p_sm
                        from sm in p_sm.DefaultIfEmpty()
                        where !p.LimitedToStores || storeId == sm.StoreId
                        select p;
            }

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoryNumberOfProductsCacheKey,
                allowedCustomerRolesIds, storeId, categoryIds);

            //only distinct products
            var result = _staticCacheManager.Get(cacheKey, () => query.Select(p => p.Id).Distinct().Count());

            return result;
        }

        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
        /// <param name="productType">Product type; 0 to load all records</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only products marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="markedAsNewOnly">A values indicating whether to load only products marked as "new"; "false" to load all records; "true" to load "marked as new" only</param>
        /// <param name="featuredProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
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
        public virtual IPagedList<Product> SearchProducts(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            int manufacturerId = 0,
            int storeId = 0,
            int vendorId = 0,
            int warehouseId = 0,
            ProductType? productType = null,
            bool visibleIndividuallyOnly = false,
            bool markedAsNewOnly = false,
            bool? featuredProducts = null,
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
            bool? overridePublished = null)
        {
            return SearchProducts(out var _, false,
                pageIndex, pageSize, categoryIds, manufacturerId,
                storeId, vendorId, warehouseId,
                productType, visibleIndividuallyOnly, markedAsNewOnly, featuredProducts,
                priceMin, priceMax, productTagId, keywords, searchDescriptions, searchManufacturerPartNumber, searchSku,
                searchProductTags, languageId, filteredSpecs,
                orderBy, showHidden, overridePublished);
        }

        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="filterableSpecificationAttributeOptionIds">The specification attribute option identifiers applied to loaded products (all pages)</param>
        /// <param name="loadFilterableSpecificationAttributeOptionIds">A value indicating whether we should load the specification attribute option identifiers applied to loaded products (all pages)</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
        /// <param name="productType">Product type; 0 to load all records</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only products marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="markedAsNewOnly">A values indicating whether to load only products marked as "new"; "false" to load all records; "true" to load "marked as new" only</param>
        /// <param name="featuredProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
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
        public virtual IPagedList<Product> SearchProducts(
            out IList<int> filterableSpecificationAttributeOptionIds,
            bool loadFilterableSpecificationAttributeOptionIds = false,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            int manufacturerId = 0,
            int storeId = 0,
            int vendorId = 0,
            int warehouseId = 0,
            ProductType? productType = null,
            bool visibleIndividuallyOnly = false,
            bool markedAsNewOnly = false,
            bool? featuredProducts = null,
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
            bool? overridePublished = null)
        {
            filterableSpecificationAttributeOptionIds = new List<int>();

            //search by keyword
            var searchLocalizedValue = false;
            if (languageId > 0)
            {
                if (showHidden)
                {
                    searchLocalizedValue = true;
                }
                else
                {
                    //ensure that we have at least two published languages
                    var totalPublishedLanguages = _languageService.GetAllLanguages().Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
                }
            }

            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            //Access control list. Allowed customer roles
            var allowedCustomerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);

            //pass category identifiers as comma-delimited string
            var commaSeparatedCategoryIds = categoryIds == null ? string.Empty : string.Join(",", categoryIds);

            //pass customer role identifiers as comma-delimited string
            var commaSeparatedAllowedCustomerRoleIds = string.Join(",", allowedCustomerRolesIds);

            //pass specification identifiers as comma-delimited string
            var commaSeparatedSpecIds = string.Empty;
            if (filteredSpecs != null)
            {
                ((List<int>)filteredSpecs).Sort();
                commaSeparatedSpecIds = string.Join(",", filteredSpecs);
            }

            //some databases don't support int.MaxValue
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            //prepare input parameters
            var pCategoryIds = SqlParameterHelper.GetStringParameter("CategoryIds", commaSeparatedCategoryIds);
            var pManufacturerId = SqlParameterHelper.GetInt32Parameter("ManufacturerId", manufacturerId);
            var pStoreId = SqlParameterHelper.GetInt32Parameter("StoreId", !_catalogSettings.IgnoreStoreLimitations ? storeId : 0);
            var pVendorId = SqlParameterHelper.GetInt32Parameter("VendorId", vendorId);
            var pWarehouseId = SqlParameterHelper.GetInt32Parameter("WarehouseId", warehouseId);
            var pProductTypeId = SqlParameterHelper.GetInt32Parameter("ProductTypeId", (int?)productType);
            var pVisibleIndividuallyOnly = SqlParameterHelper.GetBooleanParameter("VisibleIndividuallyOnly", visibleIndividuallyOnly);
            var pMarkedAsNewOnly = SqlParameterHelper.GetBooleanParameter("MarkedAsNewOnly", markedAsNewOnly);
            var pProductTagId = SqlParameterHelper.GetInt32Parameter("ProductTagId", productTagId);
            var pFeaturedProducts = SqlParameterHelper.GetBooleanParameter("FeaturedProducts", featuredProducts);
            var pPriceMin = SqlParameterHelper.GetDecimalParameter("PriceMin", priceMin);
            var pPriceMax = SqlParameterHelper.GetDecimalParameter("PriceMax", priceMax);
            var pKeywords = SqlParameterHelper.GetStringParameter("Keywords", keywords);
            var pSearchDescriptions = SqlParameterHelper.GetBooleanParameter("SearchDescriptions", searchDescriptions);
            var pSearchManufacturerPartNumber = SqlParameterHelper.GetBooleanParameter("SearchManufacturerPartNumber", searchManufacturerPartNumber);
            var pSearchSku = SqlParameterHelper.GetBooleanParameter("SearchSku", searchSku);
            var pSearchProductTags = SqlParameterHelper.GetBooleanParameter("SearchProductTags", searchProductTags);
            var pUseFullTextSearch = SqlParameterHelper.GetBooleanParameter("UseFullTextSearch", _commonSettings.UseFullTextSearch);
            var pFullTextMode = SqlParameterHelper.GetInt32Parameter("FullTextMode", (int)_commonSettings.FullTextMode);
            var pFilteredSpecs = SqlParameterHelper.GetStringParameter("FilteredSpecs", commaSeparatedSpecIds);
            var pLanguageId = SqlParameterHelper.GetInt32Parameter("LanguageId", searchLocalizedValue ? languageId : 0);
            var pOrderBy = SqlParameterHelper.GetInt32Parameter("OrderBy", (int)orderBy);
            var pAllowedCustomerRoleIds = SqlParameterHelper.GetStringParameter("AllowedCustomerRoleIds", !_catalogSettings.IgnoreAcl ? commaSeparatedAllowedCustomerRoleIds : string.Empty);
            var pPageIndex = SqlParameterHelper.GetInt32Parameter("PageIndex", pageIndex);
            var pPageSize = SqlParameterHelper.GetInt32Parameter("PageSize", pageSize);
            var pShowHidden = SqlParameterHelper.GetBooleanParameter("ShowHidden", showHidden);
            var pOverridePublished = SqlParameterHelper.GetBooleanParameter("OverridePublished", overridePublished);
            var pLoadFilterableSpecificationAttributeOptionIds = SqlParameterHelper.GetBooleanParameter("LoadFilterableSpecificationAttributeOptionIds", loadFilterableSpecificationAttributeOptionIds);

            //prepare output parameters
            var pFilterableSpecificationAttributeOptionIds = SqlParameterHelper.GetOutputStringParameter("FilterableSpecificationAttributeOptionIds");
            pFilterableSpecificationAttributeOptionIds.Size = int.MaxValue - 1;
            var pTotalRecords = SqlParameterHelper.GetOutputInt32Parameter("TotalRecords");

            //invoke stored procedure
            var products = _productRepository.EntityFromSql("ProductLoadAllPaged",
                pCategoryIds,
                pManufacturerId,
                pStoreId,
                pVendorId,
                pWarehouseId,
                pProductTypeId,
                pVisibleIndividuallyOnly,
                pMarkedAsNewOnly,
                pProductTagId,
                pFeaturedProducts,
                pPriceMin,
                pPriceMax,
                pKeywords,
                pSearchDescriptions,
                pSearchManufacturerPartNumber,
                pSearchSku,
                pSearchProductTags,
                pUseFullTextSearch,
                pFullTextMode,
                pFilteredSpecs,
                pLanguageId,
                pOrderBy,
                pAllowedCustomerRoleIds,
                pPageIndex,
                pPageSize,
                pShowHidden,
                pOverridePublished,
                pLoadFilterableSpecificationAttributeOptionIds,
                pFilterableSpecificationAttributeOptionIds,
                pTotalRecords).ToList();

            //get filterable specification attribute option identifier
            var filterableSpecificationAttributeOptionIdsStr =
                pFilterableSpecificationAttributeOptionIds.Value != DBNull.Value
                    ? (string)pFilterableSpecificationAttributeOptionIds.Value
                    : string.Empty;

            if (loadFilterableSpecificationAttributeOptionIds &&
                !string.IsNullOrWhiteSpace(filterableSpecificationAttributeOptionIdsStr))
            {
                filterableSpecificationAttributeOptionIds = filterableSpecificationAttributeOptionIdsStr
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x.Trim()))
                    .ToList();
            }
            //return products
            var totalRecords = pTotalRecords.Value != DBNull.Value ? Convert.ToInt32(pTotalRecords.Value) : 0;
            
            return new PagedList<Product>(products, pageIndex, pageSize, totalRecords);
        }

        /// <summary>
        /// Gets products by product attribute
        /// </summary>
        /// <param name="productAttributeId">Product attribute identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Products</returns>
        public virtual IPagedList<Product> GetProductsByProductAtributeId(int productAttributeId,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from p in _productRepository.Table
                join pam in _productAttributeMappingRepository.Table on p.Id equals pam.ProductId
                where
                    pam.ProductAttributeId == productAttributeId &&
                    !p.Deleted
                orderby p.Name
                select p;

            return new PagedList<Product>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets associated products
        /// </summary>
        /// <param name="parentGroupedProductId">Parent product identifier (used with grouped products)</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Products</returns>
        public virtual IList<Product> GetAssociatedProducts(int parentGroupedProductId,
            int storeId = 0, int vendorId = 0, bool showHidden = false)
        {
            var query = _productRepository.Table;
            query = query.Where(x => x.ParentGroupedProductId == parentGroupedProductId);
            if (!showHidden)
            {
                query = query.Where(x => x.Published);

                //available dates
                query = query.Where(p =>
                    (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < DateTime.UtcNow) &&
                    (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > DateTime.UtcNow));
            }
            //vendor filtering
            if (vendorId > 0)
            {
                query = query.Where(p => p.VendorId == vendorId);
            }

            query = query.Where(x => !x.Deleted);
            query = query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id);

            var products = query.ToList();

            //ACL mapping
            if (!showHidden)
            {
                products = products.Where(x => _aclService.Authorize(x)).ToList();
            }

            //Store mapping
            if (!showHidden && storeId > 0)
            {
                products = products.Where(x => _storeMappingService.Authorize(x, storeId)).ToList();
            }

            return products;
        }

        /// <summary>
        /// Update product review totals
        /// </summary>
        /// <param name="product">Product</param>
        public virtual void UpdateProductReviewTotals(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var approvedRatingSum = 0;
            var notApprovedRatingSum = 0;
            var approvedTotalReviews = 0;
            var notApprovedTotalReviews = 0;

            var reviews = _productReviewRepository.Table.Where(r => r.ProductId == product.Id);
            foreach (var pr in reviews)
            {
                if (pr.IsApproved)
                {
                    approvedRatingSum += pr.Rating;
                    approvedTotalReviews++;
                }
                else
                {
                    notApprovedRatingSum += pr.Rating;
                    notApprovedTotalReviews++;
                }
            }

            product.ApprovedRatingSum = approvedRatingSum;
            product.NotApprovedRatingSum = notApprovedRatingSum;
            product.ApprovedTotalReviews = approvedTotalReviews;
            product.NotApprovedTotalReviews = notApprovedTotalReviews;
            UpdateProduct(product);
        }

        /// <summary>
        /// Get low stock products
        /// </summary>
        /// <param name="vendorId">Vendor identifier; pass null to load all records</param>
        /// <param name="loadPublishedOnly">Whether to load published products only; pass null to load all products, pass true to load only published products, pass false to load only unpublished products</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>Products</returns>
        public virtual IPagedList<Product> GetLowStockProducts(int? vendorId = null, bool? loadPublishedOnly = true,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _productRepository.Table;

            //filter by products with tracking inventory
            query = query.Where(product => product.ManageInventoryMethodId == (int)ManageInventoryMethod.ManageStock);

            //filter by products with stock quantity less than the minimum
            query = query.Where(product =>
                (product.UseMultipleWarehouses ? _productWarehouseInventoryRepository.Table.Where(pwi => pwi.ProductId == product.Id).Sum(pwi => pwi.StockQuantity - pwi.ReservedQuantity)
                    : product.StockQuantity) <= product.MinStockQuantity);

            //ignore deleted products
            query = query.Where(product => !product.Deleted);

            //ignore grouped products
            query = query.Where(product => product.ProductTypeId != (int)ProductType.GroupedProduct);

            //filter by vendor
            if (vendorId.HasValue && vendorId.Value > 0)
                query = query.Where(product => product.VendorId == vendorId.Value);

            //whether to load published products only
            if (loadPublishedOnly.HasValue)
                query = loadPublishedOnly.Value ? query.Where(product => product.Published) : query.Where(product => !product.Published);

            query = query.OrderBy(product => product.MinStockQuantity).ThenBy(product => product.DisplayOrder).ThenBy(product => product.Id);

            return new PagedList<Product>(query, pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Get low stock product combinations
        /// </summary>
        /// <param name="vendorId">Vendor identifier; pass null to load all records</param>
        /// <param name="loadPublishedOnly">Whether to load combinations of published products only; pass null to load all products, pass true to load only published products, pass false to load only unpublished products</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>Product combinations</returns>
        public virtual IPagedList<ProductAttributeCombination> GetLowStockProductCombinations(int? vendorId = null, bool? loadPublishedOnly = true,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var combinations = from pac in _productAttributeCombinationRepository.Table
                join p in _productRepository.Table on pac.ProductId equals p.Id
                where
                    //filter by combinations with stock quantity less than the minimum
                    pac.StockQuantity < pac.NotifyAdminForQuantityBelow  &&
                    //filter by products with tracking inventory by attributes
                    p.ManageInventoryMethodId == (int)ManageInventoryMethod.ManageStockByAttributes &&
                    //ignore deleted products
                    !p.Deleted &&
                    //ignore grouped products
                    p.ProductTypeId != (int)ProductType.GroupedProduct &&
                    //filter by vendor
                    (vendorId ?? 0) == 0 || p.VendorId == vendorId &&
                    //whether to load published products only
                    loadPublishedOnly == null || p.Published == loadPublishedOnly
                orderby pac.ProductId, pac.Id
                select pac;

            return new PagedList<ProductAttributeCombination>(combinations, pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Gets a product by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Product</returns>
        public virtual Product GetProductBySku(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from p in _productRepository.Table
                        orderby p.Id
                        where !p.Deleted &&
                        p.Sku == sku
                        select p;
            var product = query.FirstOrDefault();

            return product;
        }

        /// <summary>
        /// Gets a products by SKU array
        /// </summary>
        /// <param name="skuArray">SKU array</param>
        /// <param name="vendorId">Vendor ID; 0 to load all records</param>
        /// <returns>Products</returns>
        public IList<Product> GetProductsBySku(string[] skuArray, int vendorId = 0)
        {
            if (skuArray == null)
                throw new ArgumentNullException(nameof(skuArray));

            var query = _productRepository.Table;
            query = query.Where(p => !p.Deleted && skuArray.Contains(p.Sku));

            if (vendorId != 0)
                query = query.Where(p => p.VendorId == vendorId);

            return query.ToList();
        }

        /// <summary>
        /// Update HasTierPrices property (used for performance optimization)
        /// </summary>
        /// <param name="product">Product</param>
        public virtual void UpdateHasTierPricesProperty(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            product.HasTierPrices = GetTierPricesByProduct(product.Id).Any();
            UpdateProduct(product);
        }

        /// <summary>
        /// Update HasDiscountsApplied property (used for performance optimization)
        /// </summary>
        /// <param name="product">Product</param>
        public virtual void UpdateHasDiscountsApplied(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            product.HasDiscountsApplied = _discountProductMappingRepository.Table.Any(dpm => dpm.EntityId == product.Id);
            UpdateProduct(product);
        }

        /// <summary>
        /// Gets number of products by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Number of products</returns>
        public int GetNumberOfProductsByVendorId(int vendorId)
        {
            if (vendorId == 0)
                return 0;

            return _productRepository.Table.Count(p => p.VendorId == vendorId && !p.Deleted);
        }

        /// <summary>
        /// Parse "required product Ids" property
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>A list of required product IDs</returns>
        public virtual int[] ParseRequiredProductIds(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (string.IsNullOrEmpty(product.RequiredProductIds))
                return Array.Empty<int>();

            var ids = new List<int>();

            foreach (var idStr in product.RequiredProductIds
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()))
            {
                if (int.TryParse(idStr, out var id))
                    ids.Add(id);
            }

            return ids.ToArray();
        }

        /// <summary>
        /// Get a value indicating whether a product is available now (availability dates)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="dateTime">Datetime to check; pass null to use current date</param>
        /// <returns>Result</returns>
        public virtual bool ProductIsAvailable(Product product, DateTime? dateTime = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (product.AvailableStartDateTimeUtc.HasValue && product.AvailableStartDateTimeUtc.Value > dateTime)
                return false;

            if (product.AvailableEndDateTimeUtc.HasValue && product.AvailableEndDateTimeUtc.Value < dateTime)
                return false;

            return true;
        }

        /// <summary>
        /// Get a list of allowed quantities (parse 'AllowedQuantities' property)
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Result</returns>
        public virtual int[] ParseAllowedQuantities(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var result = new List<int>();
            if (!string.IsNullOrWhiteSpace(product.AllowedQuantities))
            {
                product.AllowedQuantities
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList()
                    .ForEach(qtyStr =>
                    {
                        if (int.TryParse(qtyStr.Trim(), out var qty))
                        {
                            result.Add(qty);
                        }
                    });
            }

            return result.ToArray();
        }

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
        public virtual int GetTotalStockQuantity(Product product, bool useReservedQuantity = true, int warehouseId = 0)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock)
            {
                //We can calculate total stock quantity when 'Manage inventory' property is set to 'Track inventory'
                return 0;
            }

            if (!product.UseMultipleWarehouses)
                return product.StockQuantity;

            var pwi = _productWarehouseInventoryRepository.Table.Where(wi => wi.ProductId == product.Id);

            if (warehouseId > 0)
            {
                pwi = pwi.Where(x => x.WarehouseId == warehouseId);
            }

            var result = pwi.Sum(x => x.StockQuantity);
            if (useReservedQuantity)
            {
                result -= pwi.Sum(x => x.ReservedQuantity);
            }

            return result;
        }

        /// <summary>
        /// Get number of rental periods (price ratio)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Number of rental periods</returns>
        public virtual int GetRentalPeriods(Product product, DateTime startDate, DateTime endDate)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (!product.IsRental)
                return 1;

            if (startDate.CompareTo(endDate) >= 0)
                return 1;

            int totalPeriods;
            switch (product.RentalPricePeriod)
            {
                case RentalPricePeriod.Days:
                    {
                        var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                        var configuredPeriodDays = product.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent / configuredPeriodDays));
                    }

                    break;
                case RentalPricePeriod.Weeks:
                    {
                        var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                        var configuredPeriodDays = 7 * product.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent / configuredPeriodDays));
                    }

                    break;
                case RentalPricePeriod.Months:
                    {
                        //Source: http://stackoverflow.com/questions/4638993/difference-in-months-between-two-dates
                        var totalMonthsToRent = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;
                        if (startDate.AddMonths(totalMonthsToRent) < endDate)
                            //several days added (not full month)
                            totalMonthsToRent++;

                        var configuredPeriodMonths = product.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling((double)totalMonthsToRent / configuredPeriodMonths));
                    }

                    break;
                case RentalPricePeriod.Years:
                    {
                        var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                        var configuredPeriodDays = 365 * product.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent / configuredPeriodDays));
                    }

                    break;
                default:
                    throw new Exception("Not supported rental period");
            }

            return totalPeriods;
        }

        /// <summary>
        /// Formats the stock availability/quantity message
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Selected product attributes in XML format (if specified)</param>
        /// <returns>The stock message</returns>
        public virtual string FormatStockMessage(Product product, string attributesXml)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var stockMessage = string.Empty;

            switch (product.ManageInventoryMethod)
            {
                case ManageInventoryMethod.ManageStock:
                    stockMessage = GetStockMessage(product, stockMessage);
                    break;
                case ManageInventoryMethod.ManageStockByAttributes:
                    stockMessage = GeStockMessage(product, attributesXml);
                    break;
            }

            return stockMessage;
        }

        /// <summary>
        /// Formats SKU
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>SKU</returns>
        public virtual string FormatSku(Product product, string attributesXml = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            GetSkuMpnGtin(product, attributesXml, out var sku, out var _, out var _);

            return sku;
        }

        /// <summary>
        /// Formats manufacturer part number
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Manufacturer part number</returns>
        public virtual string FormatMpn(Product product, string attributesXml = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            GetSkuMpnGtin(product, attributesXml, out var _, out var manufacturerPartNumber, out var _);

            return manufacturerPartNumber;
        }

        /// <summary>
        /// Formats GTIN
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>GTIN</returns>
        public virtual string FormatGtin(Product product, string attributesXml = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            GetSkuMpnGtin(product, attributesXml, out var _, out var _, out var gtin);

            return gtin;
        }

        /// <summary>
        /// Formats start/end date for rental product
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="date">Date</param>
        /// <returns>Formatted date</returns>
        public virtual string FormatRentalDate(Product product, DateTime date)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (!product.IsRental)
                return null;

            return date.ToShortDateString();
        }

        /// <summary>
        /// Update product store mappings
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="limitedToStoresIds">A list of store ids for mapping</param>
        public virtual void UpdateProductStoreMappings(Product product, IList<int> limitedToStoresIds)
        {
            product.LimitedToStores = limitedToStoresIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(product);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (limitedToStoresIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(product, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        /// <summary>
        /// Gets the value whether the sequence contains downloadable products
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>Result</returns>
        public virtual bool HasAnyDownloadableProduct(int[] productIds)
        {
            return _productRepository.Table.Any(p => productIds.Contains(p.Id) && p.IsDownload);
        }

        /// <summary>
        /// Gets the value whether the sequence contains gift card products
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>Result</returns>
        public virtual bool HasAnyGiftCardProduct(int[] productIds)
        {
            return _productRepository.Table.Any(p => productIds.Contains(p.Id) && p.IsGiftCard);
        }

        /// <summary>
        /// Gets the value whether the sequence contains recurring products
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>Result</returns>
        public virtual bool HasAnyRecurringProduct(int[] productIds)
        {
            return _productRepository.Table.Any(p => productIds.Contains(p.Id) && p.IsRecurring);
        }

        #endregion

        #region Inventory management methods

        /// <summary>
        /// Adjust inventory
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="quantityToChange">Quantity to increase or decrease</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="message">Message for the stock quantity history</param>
        public virtual void AdjustInventory(Product product, int quantityToChange, string attributesXml = "", string message = "")
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (quantityToChange == 0)
                return;

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
            {
                //previous stock
                var prevStockQuantity = GetTotalStockQuantity(product);

                //update stock quantity
                if (product.UseMultipleWarehouses)
                {
                    //use multiple warehouses
                    if (quantityToChange < 0)
                        ReserveInventory(product, quantityToChange);
                    else
                        UnblockReservedInventory(product, quantityToChange);
                }
                else
                {
                    //do not use multiple warehouses
                    //simple inventory management
                    product.StockQuantity += quantityToChange;
                    UpdateProduct(product);

                    //quantity change history
                    AddStockQuantityHistoryEntry(product, quantityToChange, product.StockQuantity, product.WarehouseId, message);
                }

                //qty is reduced. check if minimum stock quantity is reached
                if (quantityToChange < 0 && product.MinStockQuantity >= GetTotalStockQuantity(product))
                {
                    //what should we do now? disable buy button, unpublish the product, or do nothing? check "Low stock activity" property
                    switch (product.LowStockActivity)
                    {
                        case LowStockActivity.DisableBuyButton:
                            product.DisableBuyButton = true;
                            product.DisableWishlistButton = true;
                            UpdateProduct(product);
                            break;
                        case LowStockActivity.Unpublish:
                            product.Published = false;
                            UpdateProduct(product);
                            break;
                        default:
                            break;
                    }
                }
                //qty is increased. product is back in stock (minimum stock quantity is reached again)?
                if (_catalogSettings.PublishBackProductWhenCancellingOrders)
                {
                    if (quantityToChange > 0 && prevStockQuantity <= product.MinStockQuantity && product.MinStockQuantity < GetTotalStockQuantity(product))
                    {
                        switch (product.LowStockActivity)
                        {
                            case LowStockActivity.DisableBuyButton:
                                product.DisableBuyButton = false;
                                product.DisableWishlistButton = false;
                                UpdateProduct(product);
                                break;
                            case LowStockActivity.Unpublish:
                                product.Published = true;
                                UpdateProduct(product);
                                break;
                            default:
                                break;
                        }
                    }
                }

                //send email notification
                if (quantityToChange < 0 && GetTotalStockQuantity(product) < product.NotifyAdminForQuantityBelow)
                {
                    var workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
                    workflowMessageService.SendQuantityBelowStoreOwnerNotification(product, _localizationSettings.DefaultAdminLanguageId);
                }
            }

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
            {
                var combination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);
                if (combination != null)
                {
                    combination.StockQuantity += quantityToChange;
                    _productAttributeService.UpdateProductAttributeCombination(combination);

                    //quantity change history
                    AddStockQuantityHistoryEntry(product, quantityToChange, combination.StockQuantity, message: message, combinationId: combination.Id);

                    //send email notification
                    if (quantityToChange < 0 && combination.StockQuantity < combination.NotifyAdminForQuantityBelow)
                    {
                        var workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
                        workflowMessageService.SendQuantityBelowStoreOwnerNotification(combination, _localizationSettings.DefaultAdminLanguageId);
                    }
                }
            }

            //bundled products
            var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                if (attributeValue.AttributeValueType != AttributeValueType.AssociatedToProduct)
                    continue;

                //associated product (bundle)
                var associatedProduct = GetProductById(attributeValue.AssociatedProductId);
                if (associatedProduct != null)
                {
                    AdjustInventory(associatedProduct, quantityToChange * attributeValue.Quantity, message);
                }
            }
        }

        /// <summary>
        /// Reserve the given quantity in the warehouses.
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="quantity">Quantity, must be negative</param>
        public virtual void ReserveInventory(Product product, int quantity)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (quantity >= 0)
                throw new ArgumentException("Value must be negative.", nameof(quantity));

            var qty = -quantity;

            var productInventory = _productWarehouseInventoryRepository.Table.Where(pwi => pwi.ProductId == product.Id)
                .OrderByDescending(pwi => pwi.StockQuantity - pwi.ReservedQuantity)
                .ToList();

            if (productInventory.Count <= 0)
                return;

            // 1st pass: Applying reserved
            foreach (var item in productInventory)
            {
                var selectQty = Math.Min(item.StockQuantity - item.ReservedQuantity, qty);
                item.ReservedQuantity += selectQty;
                qty -= selectQty;

                if (qty <= 0)
                    break;
            }

            if (qty > 0)
            {
                // 2rd pass: Booking negative stock!
                var pwi = productInventory[0];
                pwi.ReservedQuantity += qty;
            }

            UpdateProduct(product);
        }

        /// <summary>
        /// Balance the given quantity in the warehouses.
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="quantity">Quantity</param>
        public virtual void BalanceInventory(Product product, int warehouseId, int quantity)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //Warehouse to which reserve is being transferred
            var productInventory = _productWarehouseInventoryRepository.Table
                .Where(pwi => pwi.ProductId == product.Id && pwi.WarehouseId == warehouseId)
                .ToList()
                .FirstOrDefault();

            if (productInventory == null)
                return;

            var selectQty = Math.Min(productInventory.StockQuantity - productInventory.ReservedQuantity, quantity);
            productInventory.ReservedQuantity += selectQty;

            //remove from reserve in other warehouses what has just been reserved in the current warehouse to equalize the total
            var productAnotherInventories = _productWarehouseInventoryRepository.Table
                .Where(pwi => pwi.ProductId == product.Id && pwi.WarehouseId != warehouseId)
                .OrderByDescending(ob => ob.ReservedQuantity)
                .ToList();

            var qty = selectQty;
            //We need to make a balance in all warehouses, as resources of one warehouse may not be enough
            foreach (var productAnotherInventory in productAnotherInventories)
            {
                if (qty > 0)
                {
                    if (productAnotherInventory.ReservedQuantity >= qty)
                    {
                        productAnotherInventory.ReservedQuantity -= qty;
                    }
                    else
                    {
                        //Here we can transfer only a part of the reserve, the rest will be sought in other warehouses.
                        qty = selectQty - productAnotherInventory.ReservedQuantity;
                        productAnotherInventory.ReservedQuantity = 0;
                    }
                }
            }

            UpdateProduct(product);
        }

        /// <summary>
        /// Unblocks the given quantity reserved items in the warehouses
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="quantity">Quantity, must be positive</param>
        public virtual void UnblockReservedInventory(Product product, int quantity)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (quantity < 0)
                throw new ArgumentException("Value must be positive.", nameof(quantity));

            var productInventory = _productWarehouseInventoryRepository.Table.Where(pwi => pwi.ProductId == product.Id)
                .OrderByDescending(pwi => pwi.ReservedQuantity)
                .ThenByDescending(pwi => pwi.StockQuantity)
                .ToList();

            if (!productInventory.Any())
                return;

            var qty = quantity;

            foreach (var item in productInventory)
            {
                var selectQty = Math.Min(item.ReservedQuantity, qty);
                item.ReservedQuantity -= selectQty;
                qty -= selectQty;

                if (qty <= 0)
                    break;
            }

            if (qty > 0)
            {
                var pwi = productInventory[0];
                pwi.StockQuantity += qty;
            }

            UpdateProductWarehouseInventory(productInventory);
        }

        /// <summary>
        /// Book the reserved quantity
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="quantity">Quantity, must be negative</param>
        /// <param name="message">Message for the stock quantity history</param>
        public virtual void BookReservedInventory(Product product, int warehouseId, int quantity, string message = "")
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (quantity >= 0)
                throw new ArgumentException("Value must be negative.", nameof(quantity));

            //only products with "use multiple warehouses" are handled this way
            if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock || !product.UseMultipleWarehouses)
                return;

            var pwi = _productWarehouseInventoryRepository.Table.FirstOrDefault(wi => wi.ProductId == product.Id && wi.WarehouseId == warehouseId);
            if (pwi == null)
                return;

            pwi.ReservedQuantity = Math.Max(pwi.ReservedQuantity + quantity, 0);
            pwi.StockQuantity += quantity;

            UpdateProductWarehouseInventory(pwi);

            //quantity change history
            AddStockQuantityHistoryEntry(product, quantity, pwi.StockQuantity, warehouseId, message);
        }

        /// <summary>
        /// Reverse booked inventory (if acceptable)
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="shipmentItem">Shipment item</param>
        /// <param name="message">Message for the stock quantity history</param>
        /// <returns>Quantity reversed</returns>
        public virtual int ReverseBookedInventory(Product product, ShipmentItem shipmentItem, string message = "")
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (shipmentItem == null)
                throw new ArgumentNullException(nameof(shipmentItem));

            //only products with "use multiple warehouses" are handled this way
            if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock || !product.UseMultipleWarehouses)
                return 0;

            var pwi = _productWarehouseInventoryRepository.Table.FirstOrDefault(wi => wi.ProductId == product.Id && wi.WarehouseId == shipmentItem.WarehouseId);
            if (pwi == null)
                return 0;
            
            var shipment = _shipmentRepository.ToCachedGetById(shipmentItem.ShipmentId);

            //not shipped yet? hence "BookReservedInventory" method was not invoked
            if (!shipment.ShippedDateUtc.HasValue)
                return 0;

            var qty = shipmentItem.Quantity;

            pwi.StockQuantity += qty;
            pwi.ReservedQuantity += qty;

            UpdateProductWarehouseInventory(pwi);

            //quantity change history
            AddStockQuantityHistoryEntry(product, qty, pwi.StockQuantity, shipmentItem.WarehouseId, message);

            return qty;
        }

        #endregion

        #region Related products

        /// <summary>
        /// Deletes a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        public virtual void DeleteRelatedProduct(RelatedProduct relatedProduct)
        {
            if (relatedProduct == null)
                throw new ArgumentNullException(nameof(relatedProduct));

            _relatedProductRepository.Delete(relatedProduct);

            //event notification
            _eventPublisher.EntityDeleted(relatedProduct);
        }

        /// <summary>
        /// Gets related products by product identifier
        /// </summary>
        /// <param name="productId">The first product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Related products</returns>
        public virtual IList<RelatedProduct> GetRelatedProductsByProductId1(int productId, bool showHidden = false)
        {
            var query = from rp in _relatedProductRepository.Table
                        join p in _productRepository.Table on rp.ProductId2 equals p.Id
                        where rp.ProductId1 == productId &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby rp.DisplayOrder, rp.Id
                        select rp;

            var relatedProducts = query.ToCachedList(_cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductsRelatedCacheKey, productId, showHidden));

            return relatedProducts;
        }

        /// <summary>
        /// Gets a related product
        /// </summary>
        /// <param name="relatedProductId">Related product identifier</param>
        /// <returns>Related product</returns>
        public virtual RelatedProduct GetRelatedProductById(int relatedProductId)
        {
            if (relatedProductId == 0)
                return null;

            return _relatedProductRepository.ToCachedGetById(relatedProductId);
        }

        /// <summary>
        /// Inserts a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        public virtual void InsertRelatedProduct(RelatedProduct relatedProduct)
        {
            if (relatedProduct == null)
                throw new ArgumentNullException(nameof(relatedProduct));

            _relatedProductRepository.Insert(relatedProduct);

            //event notification
            _eventPublisher.EntityInserted(relatedProduct);
        }

        /// <summary>
        /// Updates a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        public virtual void UpdateRelatedProduct(RelatedProduct relatedProduct)
        {
            if (relatedProduct == null)
                throw new ArgumentNullException(nameof(relatedProduct));

            _relatedProductRepository.Update(relatedProduct);

            //event notification
            _eventPublisher.EntityUpdated(relatedProduct);
        }

        /// <summary>
        /// Finds a related product item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="productId2">The second product identifier</param>
        /// <returns>Related product</returns>
        public virtual RelatedProduct FindRelatedProduct(IList<RelatedProduct> source, int productId1, int productId2)
        {
            foreach (var relatedProduct in source)
                if (relatedProduct.ProductId1 == productId1 && relatedProduct.ProductId2 == productId2)
                    return relatedProduct;
            return null;
        }

        #endregion

        #region Cross-sell products

        /// <summary>
        /// Deletes a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell identifier</param>
        public virtual void DeleteCrossSellProduct(CrossSellProduct crossSellProduct)
        {
            if (crossSellProduct == null)
                throw new ArgumentNullException(nameof(crossSellProduct));

            _crossSellProductRepository.Delete(crossSellProduct);

            //event notification
            _eventPublisher.EntityDeleted(crossSellProduct);
        }

        /// <summary>
        /// Gets cross-sell products by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Cross-sell products</returns>
        public virtual IList<CrossSellProduct> GetCrossSellProductsByProductId1(int productId1, bool showHidden = false)
        {
            return GetCrossSellProductsByProductIds(new[] { productId1 }, showHidden);
        }

        /// <summary>
        /// Gets cross-sell products by product identifier
        /// </summary>
        /// <param name="productIds">The first product identifiers</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Cross-sell products</returns>
        public virtual IList<CrossSellProduct> GetCrossSellProductsByProductIds(int[] productIds, bool showHidden = false)
        {
            if (productIds == null || productIds.Length == 0)
                return new List<CrossSellProduct>();

            var query = from csp in _crossSellProductRepository.Table
                        join p in _productRepository.Table on csp.ProductId2 equals p.Id
                        where productIds.Contains(csp.ProductId1) &&
                              !p.Deleted &&
                              (showHidden || p.Published)
                        orderby csp.Id
                        select csp;
            var crossSellProducts = query.ToList();
            return crossSellProducts;
        }

        /// <summary>
        /// Gets a cross-sell product
        /// </summary>
        /// <param name="crossSellProductId">Cross-sell product identifier</param>
        /// <returns>Cross-sell product</returns>
        public virtual CrossSellProduct GetCrossSellProductById(int crossSellProductId)
        {
            if (crossSellProductId == 0)
                return null;

            return _crossSellProductRepository.ToCachedGetById(crossSellProductId);
        }

        /// <summary>
        /// Inserts a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell product</param>
        public virtual void InsertCrossSellProduct(CrossSellProduct crossSellProduct)
        {
            if (crossSellProduct == null)
                throw new ArgumentNullException(nameof(crossSellProduct));

            _crossSellProductRepository.Insert(crossSellProduct);

            //event notification
            _eventPublisher.EntityInserted(crossSellProduct);
        }

        /// <summary>
        /// Updates a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell product</param>
        public virtual void UpdateCrossSellProduct(CrossSellProduct crossSellProduct)
        {
            if (crossSellProduct == null)
                throw new ArgumentNullException(nameof(crossSellProduct));

            _crossSellProductRepository.Update(crossSellProduct);

            //event notification
            _eventPublisher.EntityUpdated(crossSellProduct);
        }

        /// <summary>
        /// Gets a cross-sells
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="numberOfProducts">Number of products to return</param>
        /// <returns>Cross-sells</returns>
        public virtual IList<Product> GetCrosssellProductsByShoppingCart(IList<ShoppingCartItem> cart, int numberOfProducts)
        {
            var result = new List<Product>();

            if (numberOfProducts == 0)
                return result;

            if (cart == null || !cart.Any())
                return result;

            var cartProductIds = new List<int>();
            foreach (var sci in cart)
            {
                var prodId = sci.ProductId;
                if (!cartProductIds.Contains(prodId))
                    cartProductIds.Add(prodId);
            }

            var productIds = cart.Select(sci => sci.ProductId).ToArray();
            var crossSells = GetCrossSellProductsByProductIds(productIds);
            foreach (var crossSell in crossSells)
            {
                //validate that this product is not added to result yet
                //validate that this product is not in the cart
                if (result.Find(p => p.Id == crossSell.ProductId2) != null || cartProductIds.Contains(crossSell.ProductId2))
                    continue;

                var productToAdd = GetProductById(crossSell.ProductId2);
                //validate product
                if (productToAdd == null || productToAdd.Deleted || !productToAdd.Published)
                    continue;

                //add a product to result
                result.Add(productToAdd);
                if (result.Count >= numberOfProducts)
                    return result;
            }

            return result;
        }

        /// <summary>
        /// Finds a cross-sell product item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="productId2">The second product identifier</param>
        /// <returns>Cross-sell product</returns>
        public virtual CrossSellProduct FindCrossSellProduct(IList<CrossSellProduct> source, int productId1, int productId2)
        {
            foreach (var crossSellProduct in source)
                if (crossSellProduct.ProductId1 == productId1 && crossSellProduct.ProductId2 == productId2)
                    return crossSellProduct;
            return null;
        }

        #endregion

        #region Tier prices

        /// <summary>
        /// Gets a product tier prices for customer
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Store identifier</param>
        public virtual IList<TierPrice> GetTierPrices(Product product, Customer customer, int storeId)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            if (customer is null)
                throw new ArgumentNullException(nameof(customer));

            if (!product.HasTierPrices)
                return null;

            //get actual tier prices
            var actualTierPrices = GetTierPricesByProduct(product.Id).OrderBy(price => price.Quantity)
                .FilterByStore(storeId)
                .FilterByDate()
                .RemoveDuplicatedQuantities();

            if (!_catalogSettings.IgnoreAcl)
            {
                var customerRoleIds = _customerService.GetCustomerRoleIds(customer);

                actualTierPrices = actualTierPrices.Where(tierPrice =>
                   !tierPrice.CustomerRoleId.HasValue ||
                   tierPrice.CustomerRoleId.Value == 0 ||
                   customerRoleIds.Contains(tierPrice.CustomerRoleId.Value));
            }

            return actualTierPrices.ToList();
        }

        /// <summary>
        /// Gets a tier prices by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public virtual IList<TierPrice> GetTierPricesByProduct(int productId)
        {
            return _tierPriceRepository.Table.Where(tp => tp.ProductId == productId)
                .ToCachedList(_cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductTierPricesCacheKey, productId));
        }

        /// <summary>
        /// Deletes a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        public virtual void DeleteTierPrice(TierPrice tierPrice)
        {
            if (tierPrice == null)
                throw new ArgumentNullException(nameof(tierPrice));

            _tierPriceRepository.Delete(tierPrice);
            
            //event notification
            _eventPublisher.EntityDeleted(tierPrice);
        }

        /// <summary>
        /// Gets a tier price
        /// </summary>
        /// <param name="tierPriceId">Tier price identifier</param>
        /// <returns>Tier price</returns>
        public virtual TierPrice GetTierPriceById(int tierPriceId)
        {
            if (tierPriceId == 0)
                return null;

            return _tierPriceRepository.ToCachedGetById(tierPriceId);
        }

        /// <summary>
        /// Inserts a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        public virtual void InsertTierPrice(TierPrice tierPrice)
        {
            if (tierPrice == null)
                throw new ArgumentNullException(nameof(tierPrice));

            _tierPriceRepository.Insert(tierPrice);
            
            //event notification
            _eventPublisher.EntityInserted(tierPrice);
        }

        /// <summary>
        /// Updates the tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        public virtual void UpdateTierPrice(TierPrice tierPrice)
        {
            if (tierPrice == null)
                throw new ArgumentNullException(nameof(tierPrice));

            _tierPriceRepository.Update(tierPrice);
            
            //event notification
            _eventPublisher.EntityUpdated(tierPrice);
        }

        /// <summary>
        /// Gets a preferred tier price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Tier price</returns>
        public virtual TierPrice GetPreferredTierPrice(Product product, Customer customer, int storeId, int quantity)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            if (customer is null)
                throw new ArgumentNullException(nameof(customer));

            if (!product.HasTierPrices)
                return null;

            //get the most suitable tier price based on the passed quantity
            return GetTierPrices(product, customer, storeId)?.LastOrDefault(price => quantity >= price.Quantity);
        }

        #endregion

        #region Product pictures

        /// <summary>
        /// Deletes a product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        public virtual void DeleteProductPicture(ProductPicture productPicture)
        {
            if (productPicture == null)
                throw new ArgumentNullException(nameof(productPicture));

            _productPictureRepository.Delete(productPicture);

            //event notification
            _eventPublisher.EntityDeleted(productPicture);
        }

        /// <summary>
        /// Gets a product pictures by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>Product pictures</returns>
        public virtual IList<ProductPicture> GetProductPicturesByProductId(int productId)
        {
            var query = from pp in _productPictureRepository.Table
                        where pp.ProductId == productId
                        orderby pp.DisplayOrder, pp.Id
                        select pp;

            var productPictures = query.ToList();

            return productPictures;
        }

        /// <summary>
        /// Gets a product picture
        /// </summary>
        /// <param name="productPictureId">Product picture identifier</param>
        /// <returns>Product picture</returns>
        public virtual ProductPicture GetProductPictureById(int productPictureId)
        {
            if (productPictureId == 0)
                return null;

            return _productPictureRepository.ToCachedGetById(productPictureId);
        }

        /// <summary>
        /// Inserts a product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        public virtual void InsertProductPicture(ProductPicture productPicture)
        {
            if (productPicture == null)
                throw new ArgumentNullException(nameof(productPicture));

            _productPictureRepository.Insert(productPicture);

            //event notification
            _eventPublisher.EntityInserted(productPicture);
        }

        /// <summary>
        /// Updates a product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        public virtual void UpdateProductPicture(ProductPicture productPicture)
        {
            if (productPicture == null)
                throw new ArgumentNullException(nameof(productPicture));

            _productPictureRepository.Update(productPicture);

            //event notification
            _eventPublisher.EntityUpdated(productPicture);
        }

        /// <summary>
        /// Get the IDs of all product images 
        /// </summary>
        /// <param name="productsIds">Products IDs</param>
        /// <returns>All picture identifiers grouped by product ID</returns>
        public IDictionary<int, int[]> GetProductsImagesIds(int[] productsIds)
        {
            var productPictures = _productPictureRepository.Table.Where(p => productsIds.Contains(p.ProductId)).ToList();

            return productPictures.GroupBy(p => p.ProductId).ToDictionary(p => p.Key, p => p.Select(p1 => p1.PictureId).ToArray());
        }

        /// <summary>
        /// Get products for which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted products</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of products</returns>
        public virtual IPagedList<Product> GetProductsWithAppliedDiscount(int? discountId = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var products = _productRepository.Table.Where(product => product.HasDiscountsApplied);

            if (discountId.HasValue)
                products = from product in products
                    join dpm in _discountProductMappingRepository.Table on product.Id equals dpm.EntityId
                           where dpm.DiscountId == discountId.Value
                    select product;

            if (!showHidden)
                products = products.Where(product => !product.Deleted);

            products = products.OrderBy(product => product.DisplayOrder).ThenBy(product => product.Id);

            return new PagedList<Product>(products, pageIndex, pageSize);
        }

        #endregion

        #region Product reviews

        /// <summary>
        /// Gets all product reviews
        /// </summary>
        /// <param name="customerId">Customer identifier (who wrote a review); 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item item creation to; null to load all records</param>
        /// <param name="message">Search title or review text; null to load all records</param>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="productId">The product identifier; pass 0 to load all records</param>
        /// <param name="vendorId">The vendor identifier (limit to products of this vendor); pass 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Reviews</returns>
        public virtual IPagedList<ProductReview> GetAllProductReviews(int customerId = 0, bool? approved = null,
            DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = null, int storeId = 0, int productId = 0, int vendorId = 0, bool showHidden = false,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _productReviewRepository.Table;

            if (approved.HasValue)
                query = query.Where(pr => pr.IsApproved == approved);
            if (customerId > 0)
                query = query.Where(pr => pr.CustomerId == customerId);
            if (fromUtc.HasValue)
                query = query.Where(pr => fromUtc.Value <= pr.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(pr => toUtc.Value >= pr.CreatedOnUtc);
            if (!string.IsNullOrEmpty(message))
                query = query.Where(pr => pr.Title.Contains(message) || pr.ReviewText.Contains(message));
            if (storeId > 0 && (showHidden || _catalogSettings.ShowProductReviewsPerStore))
                query = query.Where(pr => pr.StoreId == storeId);
            if (productId > 0)
                query = query.Where(pr => pr.ProductId == productId);
            
            query = from productReview in query
                join product in _productRepository.Table on productReview.ProductId equals product.Id
                where
                    (vendorId == 0 || product.VendorId == vendorId) &&
                    //ignore deleted products
                    !product.Deleted
                select productReview;

            //filter by limited to store products
            if (storeId > 0 && !showHidden && !_catalogSettings.IgnoreStoreLimitations)
            {
                query = from productReview in query
                        join product in _productRepository.Table on productReview.ProductId equals product.Id
                        join storeMapping in _storeMappingRepository.Table
                            on new { Id = productReview.ProductId, Name = nameof(Product) }
                            equals new { Id = storeMapping.EntityId, Name = storeMapping.EntityName } into storeMappingsWithNulls
                        from storeMapping in storeMappingsWithNulls.DefaultIfEmpty()
                        where !product.LimitedToStores || storeMapping.StoreId == storeId
                        select productReview;
            }

            query = _catalogSettings.ProductReviewsSortByCreatedDateAscending
                ? query.OrderBy(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id)
                : query.OrderByDescending(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id);

            var productReviews = new PagedList<ProductReview>(query, pageIndex, pageSize);

            return productReviews;
        }

        /// <summary>
        /// Gets product review
        /// </summary>
        /// <param name="productReviewId">Product review identifier</param>
        /// <returns>Product review</returns>
        public virtual ProductReview GetProductReviewById(int productReviewId)
        {
            if (productReviewId == 0)
                return null;

            return _productReviewRepository.ToCachedGetById(productReviewId);
        }

        /// <summary>
        /// Get product reviews by identifiers
        /// </summary>
        /// <param name="productReviewIds">Product review identifiers</param>
        /// <returns>Product reviews</returns>
        public virtual IList<ProductReview> GetProducReviewsByIds(int[] productReviewIds)
        {
            if (productReviewIds == null || productReviewIds.Length == 0)
                return new List<ProductReview>();

            var query = from pr in _productReviewRepository.Table
                        where productReviewIds.Contains(pr.Id)
                        select pr;
            var productReviews = query.ToList();
            //sort by passed identifiers
            var sortedProductReviews = new List<ProductReview>();
            foreach (var id in productReviewIds)
            {
                var productReview = productReviews.Find(x => x.Id == id);
                if (productReview != null)
                    sortedProductReviews.Add(productReview);
            }

            return sortedProductReviews;
        }

        /// <summary>
        /// Inserts a product review
        /// </summary>
        /// <param name="productReview">Product review</param>
        public virtual void InsertProductReview(ProductReview productReview)
        {
            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            _productReviewRepository.Insert(productReview);

            //event notification
            _eventPublisher.EntityInserted(productReview);
        }

        /// <summary>
        /// Deletes a product review
        /// </summary>
        /// <param name="productReview">Product review</param>
        public virtual void DeleteProductReview(ProductReview productReview)
        {
            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            _productReviewRepository.Delete(productReview);

            //event notification
            _eventPublisher.EntityDeleted(productReview);
        }

        /// <summary>
        /// Deletes product reviews
        /// </summary>
        /// <param name="productReviews">Product reviews</param>
        public virtual void DeleteProductReviews(IList<ProductReview> productReviews)
        {
            if (productReviews == null)
                throw new ArgumentNullException(nameof(productReviews));

            _productReviewRepository.Delete(productReviews);

            //event notification
            foreach (var productReview in productReviews)
            {
                _eventPublisher.EntityDeleted(productReview);
            }
        }

        /// <summary>
        /// Inserts a product review helpfulness record
        /// </summary>
        /// <param name="productReviewHelpfulness">Product review helpfulness record</param>
        public virtual void InsertProductReviewHelpfulness(ProductReviewHelpfulness productReviewHelpfulness)
        {
            if (productReviewHelpfulness == null)
                throw new ArgumentNullException(nameof(productReviewHelpfulness));

            _productReviewHelpfulnessRepository.Insert(productReviewHelpfulness);

            //event notification
            _eventPublisher.EntityInserted(productReviewHelpfulness);
        }

        /// <summary>
        /// Sets or create a product review helpfulness record
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="helpfulness">Value indicating whether a review a helpful</param>
        public virtual void SetProductReviewHelpfulness(ProductReview productReview, bool helpfulness)
        {
            if (productReview is null)
                throw new ArgumentNullException(nameof(productReview));

            var prh = _productReviewHelpfulnessRepository.Table.SingleOrDefault(h => h.ProductReviewId == productReview.Id && h.CustomerId == _workContext.CurrentCustomer.Id);

            if (prh is null)
            {
                //insert new helpfulness
                prh = new ProductReviewHelpfulness
                {
                    ProductReviewId = productReview.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    WasHelpful = helpfulness,
                };

                InsertProductReviewHelpfulness(prh);

                //event notification
                _eventPublisher.EntityInserted(prh);
            }
            else
            {
                //existing one
                prh.WasHelpful = helpfulness;
                _productReviewHelpfulnessRepository.Update(prh);
                //event notification
                _eventPublisher.EntityUpdated(prh);
            }
        }

        /// <summary>
        /// Gets ratio of useful and not useful product reviews 
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <returns>Result</returns>
        public virtual (int usefulCount, int notUsefulCount) GetHelpfulnessCounts(ProductReview productReview)
        {
            if (productReview is null)
                throw new ArgumentNullException(nameof(productReview));

            var productReviewHelpfulness = _productReviewHelpfulnessRepository.Table.Where(prh => prh.ProductReviewId == productReview.Id);

            return (productReviewHelpfulness.Count(prh => prh.WasHelpful), productReviewHelpfulness.Count(prh => !prh.WasHelpful));
        }

        /// <summary>
        /// Updates a product review
        /// </summary>
        /// <param name="productReview">Product review</param>
        public virtual void UpdateProductReview(ProductReview productReview)
        {
            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            //update
            _productReviewRepository.Update(productReview);
            
            //event notification
            _eventPublisher.EntityUpdated(productReview);
        }

        /// <summary>
        /// Updates a totals helpfulness count for product review
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <returns>Result</returns>
        public virtual void UpdateProductReviewHelpfulnessTotals(ProductReview productReview)
        {
            if (productReview is null)
                throw new ArgumentNullException(nameof(productReview));

            (productReview.HelpfulYesTotal, productReview.HelpfulNoTotal) = GetHelpfulnessCounts(productReview);

            _productReviewRepository.Update(productReview);

            //event notification
            _eventPublisher.EntityUpdated(productReview);
        }

        #endregion

        #region Product warehouses

        /// <summary>
        /// Get a product warehouse-inventory records by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public virtual IList<ProductWarehouseInventory> GetAllProductWarehouseInventoryRecords(int productId)
        {
            return _productWarehouseInventoryRepository.Table.Where(pwi => pwi.ProductId == productId).ToList();
        }

        /// <summary>
        /// Gets a warehouse by identifier
        /// </summary>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <returns>Result</returns>
        public virtual Warehouse GetWarehousesById(int warehouseId)
        {
            if (warehouseId == 0)
                return null;

            return _warehouseRepository.ToCachedGetById(warehouseId);
        }

        /// <summary>
        /// Gets a warehouses by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>List of warehouses</returns>
        public virtual IList<Warehouse> GetWarehousesByProductId(int productId)
        {
            return (from w in _warehouseRepository.Table
                    join pwi in _productWarehouseInventoryRepository.Table on w.Id equals pwi.WarehouseId
                    where pwi.ProductId == productId
                    select w).ToList();
        }

        /// <summary>
        /// Deletes a record to manage product inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage product inventory per warehouse</param>
        public virtual void DeleteProductWarehouseInventory(ProductWarehouseInventory pwi)
        {
            if (pwi == null)
                throw new ArgumentNullException(nameof(pwi));

            _productWarehouseInventoryRepository.Delete(pwi);
            
            _eventPublisher.EntityDeleted(pwi);
        }

        /// <summary>
        /// Inserts a record to manage product inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage product inventory per warehouse</param>
        public virtual void InsertProductWarehouseInventory(ProductWarehouseInventory pwi)
        {
            if (pwi == null)
                throw new ArgumentNullException(nameof(pwi));

            _productWarehouseInventoryRepository.Insert(pwi);

            //event notification
            _eventPublisher.EntityInserted(pwi);
        }

        /// <summary>
        /// Updates a record to manage product inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage product inventory per warehouse</param>
        public virtual void UpdateProductWarehouseInventory(ProductWarehouseInventory pwi)
        {
            if (pwi == null)
                throw new ArgumentNullException(nameof(pwi));

            _productWarehouseInventoryRepository.Update(pwi);

            //event notification
            _eventPublisher.EntityUpdated(pwi);
        }

        /// <summary>
        /// Updates a records to manage product inventory per warehouse
        /// </summary>
        /// <param name="pwis">Records to manage product inventory per warehouse</param>
        public virtual void UpdateProductWarehouseInventory(IList<ProductWarehouseInventory> pwis)
        {
            if (pwis == null)
                throw new ArgumentNullException(nameof(pwis));

            if (!pwis.Any())
                return;

            _productWarehouseInventoryRepository.Update(pwis);

            //event notification
            foreach (var pwi in pwis)
            {
                _eventPublisher.EntityUpdated(pwi);
            }
        }

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
        public virtual void AddStockQuantityHistoryEntry(Product product, int quantityAdjustment, int stockQuantity,
            int warehouseId = 0, string message = "", int? combinationId = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (quantityAdjustment == 0)
                return;

            var historyEntry = new StockQuantityHistory
            {
                ProductId = product.Id,
                CombinationId = combinationId,
                WarehouseId = warehouseId > 0 ? (int?)warehouseId : null,
                QuantityAdjustment = quantityAdjustment,
                StockQuantity = stockQuantity,
                Message = message,
                CreatedOnUtc = DateTime.UtcNow
            };

            _stockQuantityHistoryRepository.Insert(historyEntry);

            //event notification
            _eventPublisher.EntityInserted(historyEntry);
        }

        /// <summary>
        /// Get the history of the product stock quantity changes
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="warehouseId">Warehouse identifier; pass 0 to load all entries</param>
        /// <param name="combinationId">Product attribute combination identifier; pass 0 to load all entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of stock quantity change entries</returns>
        public virtual IPagedList<StockQuantityHistory> GetStockQuantityHistory(Product product, int warehouseId = 0, int combinationId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var query = _stockQuantityHistoryRepository.Table.Where(historyEntry => historyEntry.ProductId == product.Id);

            if (warehouseId > 0)
                query = query.Where(historyEntry => historyEntry.WarehouseId == warehouseId);

            if (combinationId > 0)
                query = query.Where(historyEntry => historyEntry.CombinationId == combinationId);

            query = query.OrderByDescending(historyEntry => historyEntry.CreatedOnUtc).ThenByDescending(historyEntry => historyEntry.Id);

            return new PagedList<StockQuantityHistory>(query, pageIndex, pageSize);
        }

        #endregion

        #region Product discounts

        /// <summary>
        /// Clean up product references for a specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
        public virtual void ClearDiscountProductMapping(Discount discount)
        {
            if (discount is null)
                throw new ArgumentNullException(nameof(discount));

            var mappingsWithProducts =
                from dcm in _discountProductMappingRepository.Table
                join p in _productRepository.Table on dcm.EntityId equals p.Id
                where dcm.DiscountId == discount.Id
                select new { product = p, dcm };

            foreach (var pdcm in mappingsWithProducts.ToList())
            {
                _discountProductMappingRepository.Delete(pdcm.dcm);
                //event notification
                _eventPublisher.EntityDeleted(pdcm.dcm);

                //update "HasDiscountsApplied" property
                UpdateHasDiscountsApplied(pdcm.product);                    
            }   
        }        

        /// <summary>
        /// Get a discount-product mapping records by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public virtual IList<DiscountProductMapping> GetAllDiscountsAppliedToProduct(int productId)
        {
            return _discountProductMappingRepository.Table.Where(dcm => dcm.EntityId == productId).ToList();
        }

        /// <summary>
        /// Get a discount-product mapping record
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Result</returns>
        public virtual DiscountProductMapping GetDiscountAppliedToProduct(int productId, int discountId)
        {
            return _discountProductMappingRepository.Table.FirstOrDefault(dcm => dcm.EntityId == productId && dcm.DiscountId == discountId);
        }

        /// <summary>
        /// Inserts a discount-product mapping record
        /// </summary>
        /// <param name="discountProductMapping">Discount-product mapping</param>
        public virtual void InsertDiscountProductMapping(DiscountProductMapping discountProductMapping)
        {
            if (discountProductMapping is null)
                throw new ArgumentNullException(nameof(discountProductMapping));

            _discountProductMappingRepository.Insert(discountProductMapping);

            //event notification
            _eventPublisher.EntityInserted(discountProductMapping);
        }

        /// <summary>
        /// Deletes a discount-product mapping record
        /// </summary>
        /// <param name="discountProductMapping">Discount-product mapping</param>
        public virtual void DeleteDiscountProductMapping(DiscountProductMapping discountProductMapping)
        {
            if (discountProductMapping is null)
                throw new ArgumentNullException(nameof(discountProductMapping));

            _discountProductMappingRepository.Delete(discountProductMapping);

            //event notification
            _eventPublisher.EntityDeleted(discountProductMapping);
        }

        #endregion

        #endregion
    }
}
