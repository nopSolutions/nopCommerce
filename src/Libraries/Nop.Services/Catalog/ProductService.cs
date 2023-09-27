using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Customers;
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
        protected readonly ICustomerService _customerService;
        protected readonly IDateRangeService _dateRangeService;
        protected readonly ILanguageService _languageService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IProductAttributeParser _productAttributeParser;
        protected readonly IProductAttributeService _productAttributeService;
        protected readonly IRepository<Category> _categoryRepository;
        protected readonly IRepository<CrossSellProduct> _crossSellProductRepository;
        protected readonly IRepository<DiscountProductMapping> _discountProductMappingRepository;
        protected readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        protected readonly IRepository<Manufacturer> _manufacturerRepository;
        protected readonly IRepository<Product> _productRepository;
        protected readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        protected readonly IRepository<ProductAttributeMapping> _productAttributeMappingRepository;
        protected readonly IRepository<ProductCategory> _productCategoryRepository;
        protected readonly IRepository<ProductManufacturer> _productManufacturerRepository;
        protected readonly IRepository<ProductPicture> _productPictureRepository;
        protected readonly IRepository<ProductProductTagMapping> _productTagMappingRepository;
        protected readonly IRepository<ProductReview> _productReviewRepository;
        protected readonly IRepository<ProductReviewHelpfulness> _productReviewHelpfulnessRepository;
        protected readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
        protected readonly IRepository<ProductTag> _productTagRepository;
        protected readonly IRepository<ProductVideo> _productVideoRepository;
        protected readonly IRepository<ProductWarehouseInventory> _productWarehouseInventoryRepository;
        protected readonly IRepository<RelatedProduct> _relatedProductRepository;
        protected readonly IRepository<Shipment> _shipmentRepository;
        protected readonly IRepository<StockQuantityHistory> _stockQuantityHistoryRepository;
        protected readonly IRepository<TierPrice> _tierPriceRepository;
        protected readonly ISearchPluginManager _searchPluginManager;
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
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IRepository<Category> categoryRepository,
            IRepository<CrossSellProduct> crossSellProductRepository,
            IRepository<DiscountProductMapping> discountProductMappingRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Product> productRepository,
            IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
            IRepository<ProductAttributeMapping> productAttributeMappingRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<ProductManufacturer> productManufacturerRepository,
            IRepository<ProductPicture> productPictureRepository,
            IRepository<ProductProductTagMapping> productTagMappingRepository,
            IRepository<ProductReview> productReviewRepository,
            IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository,
            IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
            IRepository<ProductTag> productTagRepository,
            IRepository<ProductVideo> productVideoRepository,
            IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
            IRepository<RelatedProduct> relatedProductRepository,
            IRepository<Shipment> shipmentRepository,
            IRepository<StockQuantityHistory> stockQuantityHistoryRepository,
            IRepository<TierPrice> tierPriceRepository,
            ISearchPluginManager searchPluginManager,
            IStaticCacheManager staticCacheManager,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            LocalizationSettings localizationSettings)
        {
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _aclService = aclService;
            _customerService = customerService;
            _dateRangeService = dateRangeService;
            _languageService = languageService;
            _localizationService = localizationService;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _categoryRepository = categoryRepository;
            _crossSellProductRepository = crossSellProductRepository;
            _discountProductMappingRepository = discountProductMappingRepository;
            _localizedPropertyRepository = localizedPropertyRepository;
            _manufacturerRepository = manufacturerRepository;
            _productRepository = productRepository;
            _productAttributeCombinationRepository = productAttributeCombinationRepository;
            _productAttributeMappingRepository = productAttributeMappingRepository;
            _productCategoryRepository = productCategoryRepository;
            _productManufacturerRepository = productManufacturerRepository;
            _productPictureRepository = productPictureRepository;
            _productTagMappingRepository = productTagMappingRepository;
            _productReviewRepository = productReviewRepository;
            _productReviewHelpfulnessRepository = productReviewHelpfulnessRepository;
            _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
            _productTagRepository = productTagRepository;
            _productVideoRepository = productVideoRepository;
            _productWarehouseInventoryRepository = productWarehouseInventoryRepository;
            _relatedProductRepository = relatedProductRepository;
            _shipmentRepository = shipmentRepository;
            _stockQuantityHistoryRepository = stockQuantityHistoryRepository;
            _tierPriceRepository = tierPriceRepository;
            _searchPluginManager = searchPluginManager;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _workContext = workContext;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Applies the low stock activity to specified product by the total stock quantity
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="totalStock">Total stock</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task ApplyLowStockActivityAsync(Product product, int totalStock)
        {
            var isMinimumStockReached = totalStock <= product.MinStockQuantity;

            if (!isMinimumStockReached && !_catalogSettings.PublishBackProductWhenCancellingOrders)
                return;

            switch (product.LowStockActivity)
            {
                case LowStockActivity.DisableBuyButton:
                    product.DisableBuyButton = isMinimumStockReached;
                    product.DisableWishlistButton = isMinimumStockReached;
                    await UpdateProductAsync(product);
                    break;

                case LowStockActivity.Unpublish:
                    product.Published = !isMinimumStockReached;
                    await UpdateProductAsync(product);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Gets SKU, Manufacturer part number and GTIN
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sKU, Manufacturer part number, GTIN
        /// </returns>
        protected virtual async Task<(string sku, string manufacturerPartNumber, string gtin)> GetSkuMpnGtinAsync(Product product, string attributesXml)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            string sku = null;
            string manufacturerPartNumber = null;
            string gtin = null;

            if (!string.IsNullOrEmpty(attributesXml) &&
                product.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
            {
                //manage stock by attribute combinations
                //let's find appropriate record
                var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
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

            return (sku, manufacturerPartNumber, gtin);
        }

        /// <summary>
        /// Get stock message for a product with attributes
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the message
        /// </returns>
        protected virtual async Task<string> GetStockMessageForAttributesAsync(Product product, string attributesXml)
        {
            if (!product.DisplayStockAvailability)
                return string.Empty;

            string stockMessage;

            var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
            if (combination != null)
            {
                //combination exists
                var stockQuantity = combination.StockQuantity;
                if (stockQuantity > 0)
                {
                    if (product.MinStockQuantity >= stockQuantity && product.LowStockActivity == LowStockActivity.Nothing)
                    {
                        stockMessage = product.DisplayStockQuantity
                        ?
                        //display "low stock" with stock quantity
                        string.Format(await _localizationService.GetResourceAsync("Products.Availability.LowStockWithQuantity"), stockQuantity)
                        :
                        //display "low stock" without stock quantity
                        await _localizationService.GetResourceAsync("Products.Availability.LowStock");
                    }
                    else
                    {
                        stockMessage = product.DisplayStockQuantity
                        ?
                        //display "in stock" with stock quantity
                        string.Format(await _localizationService.GetResourceAsync("Products.Availability.InStockWithQuantity"), stockQuantity)
                        :
                        //display "in stock" without stock quantity
                        await _localizationService.GetResourceAsync("Products.Availability.InStock");
                    }
                }
                else
                {
                    if (combination.AllowOutOfStockOrders)
                    {
                        stockMessage = await _localizationService.GetResourceAsync("Products.Availability.InStock");
                    }
                    else
                    {
                        var productAvailabilityRange = await
                            _dateRangeService.GetProductAvailabilityRangeByIdAsync(product.ProductAvailabilityRangeId);
                        stockMessage = productAvailabilityRange == null
                            ? await _localizationService.GetResourceAsync("Products.Availability.OutOfStock")
                            : string.Format(await _localizationService.GetResourceAsync("Products.Availability.AvailabilityRange"),
                                await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
                    }
                }
            }
            else
            {
                //no combination configured
                if (product.AllowAddingOnlyExistingAttributeCombinations)
                {
                    var allIds = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id)).Where(pa => pa.IsRequired).Select(pa => pa.Id).ToList();
                    var exIds = (await _productAttributeParser.ParseProductAttributeMappingsAsync(attributesXml)).Select(pa => pa.Id).ToList();

                    var selectedIds = allIds.Intersect(exIds).ToList();

                    if (selectedIds.Count() != allIds.Count)
                        if (_catalogSettings.AttributeValueOutOfStockDisplayType == AttributeValueOutOfStockDisplayType.AlwaysDisplay)
                            return await _localizationService.GetResourceAsync("Products.Availability.SelectRequiredAttributes");
                        else
                        {
                            var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);

                            combinations = combinations.Where(p => p.StockQuantity >= 0 || p.AllowOutOfStockOrders).ToList();

                            var attributes = await combinations.SelectAwait(async c => await _productAttributeParser.ParseProductAttributeMappingsAsync(c.AttributesXml)).ToListAsync();

                            var flag = attributes.SelectMany(a => a).Any(a => selectedIds.Contains(a.Id));

                            if (flag)
                                return await _localizationService.GetResourceAsync("Products.Availability.SelectRequiredAttributes");
                        }

                    var productAvailabilityRange = await
                        _dateRangeService.GetProductAvailabilityRangeByIdAsync(product.ProductAvailabilityRangeId);
                    stockMessage = productAvailabilityRange == null
                        ? await _localizationService.GetResourceAsync("Products.Availability.OutOfStock")
                        : string.Format(await _localizationService.GetResourceAsync("Products.Availability.AvailabilityRange"),
                            await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
                }
                else
                {
                    stockMessage = await _localizationService.GetResourceAsync("Products.Availability.InStock");
                }
            }

            return stockMessage;
        }

        /// <summary>
        /// Get stock message
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the message
        /// </returns>
        protected virtual async Task<string> GetStockMessageAsync(Product product)
        {
            if (!product.DisplayStockAvailability)
                return string.Empty;

            var stockMessage = string.Empty;
            var stockQuantity = await GetTotalStockQuantityAsync(product);

            if (stockQuantity > 0)
            {
                if (product.MinStockQuantity >= stockQuantity && product.LowStockActivity == LowStockActivity.Nothing)
                {
                    stockMessage = product.DisplayStockQuantity
                        ?
                        //display "low stock" with stock quantity
                        string.Format(await _localizationService.GetResourceAsync("Products.Availability.LowStockWithQuantity"), stockQuantity)
                        :
                        //display "low stock" without stock quantity
                        await _localizationService.GetResourceAsync("Products.Availability.LowStock");
                }
                else
                {
                    stockMessage = product.DisplayStockQuantity
                        ?
                        //display "in stock" with stock quantity
                        string.Format(await _localizationService.GetResourceAsync("Products.Availability.InStockWithQuantity"), stockQuantity)
                        :
                        //display "in stock" without stock quantity
                        await _localizationService.GetResourceAsync("Products.Availability.InStock");
                }
            }
            else
            {
                //out of stock
                var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(product.ProductAvailabilityRangeId);
                switch (product.BackorderMode)
                {
                    case BackorderMode.NoBackorders:
                        stockMessage = productAvailabilityRange == null
                            ? await _localizationService.GetResourceAsync("Products.Availability.OutOfStock")
                            : string.Format(await _localizationService.GetResourceAsync("Products.Availability.AvailabilityRange"),
                                await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
                        break;
                    case BackorderMode.AllowQtyBelow0:
                        stockMessage = await _localizationService.GetResourceAsync("Products.Availability.InStock");
                        break;
                    case BackorderMode.AllowQtyBelow0AndNotifyCustomer:
                        stockMessage = productAvailabilityRange == null
                            ? await _localizationService.GetResourceAsync("Products.Availability.Backordering")
                            : string.Format(await _localizationService.GetResourceAsync("Products.Availability.BackorderingWithDate"),
                                await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
                        break;
                }
            }

            return stockMessage;
        }

        /// <summary>
        /// Reserve the given quantity in the warehouses.
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="quantity">Quantity, must be negative</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task ReserveInventoryAsync(Product product, int quantity)
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
                var selectQty = Math.Min(Math.Max(0, item.StockQuantity - item.ReservedQuantity), qty);
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

            await UpdateProductWarehouseInventoryAsync(productInventory);
        }

        /// <summary>
        /// Unblocks the given quantity reserved items in the warehouses
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="quantity">Quantity, must be positive</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task UnblockReservedInventoryAsync(Product product, int quantity)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (quantity < 0)
                throw new ArgumentException("Value must be positive.", nameof(quantity));

            var productInventory = await _productWarehouseInventoryRepository.Table.Where(pwi => pwi.ProductId == product.Id)
                .OrderByDescending(pwi => pwi.ReservedQuantity)
                .ThenByDescending(pwi => pwi.StockQuantity)
                .ToListAsync();

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

            await UpdateProductWarehouseInventoryAsync(productInventory);
        }

        /// <summary>
        /// Gets cross-sell products by product identifier
        /// </summary>
        /// <param name="productIds">The first product identifiers</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the cross-sell products
        /// </returns>
        protected virtual async Task<IList<CrossSellProduct>> GetCrossSellProductsByProductIdsAsync(int[] productIds, bool showHidden = false)
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
            var crossSellProducts = await query.ToListAsync();

            return crossSellProducts;
        }

        /// <summary>
        /// Gets ratio of useful and not useful product reviews 
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<(int usefulCount, int notUsefulCount)> GetHelpfulnessCountsAsync(ProductReview productReview)
        {
            if (productReview is null)
                throw new ArgumentNullException(nameof(productReview));

            var productReviewHelpfulness = _productReviewHelpfulnessRepository.Table.Where(prh => prh.ProductReviewId == productReview.Id);

            return (await productReviewHelpfulness.CountAsync(prh => prh.WasHelpful),
                await productReviewHelpfulness.CountAsync(prh => !prh.WasHelpful));
        }

        /// <summary>
        /// Inserts a product review helpfulness record
        /// </summary>
        /// <param name="productReviewHelpfulness">Product review helpfulness record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertProductReviewHelpfulnessAsync(ProductReviewHelpfulness productReviewHelpfulness)
        {
            await _productReviewHelpfulnessRepository.InsertAsync(productReviewHelpfulness);
        }

        #endregion

        #region Methods

        #region Products

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductAsync(Product product)
        {
            await _productRepository.DeleteAsync(product);
        }

        /// <summary>
        /// Delete products
        /// </summary>
        /// <param name="products">Products</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductsAsync(IList<Product> products)
        {
            await _productRepository.DeleteAsync(products);
        }

        /// <summary>
        /// Gets all products displayed on the home page
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products
        /// </returns>
        public virtual async Task<IList<Product>> GetAllProductsDisplayedOnHomepageAsync()
        {
            var products = await _productRepository.GetAllAsync(query =>
            {
                return from p in query
                       orderby p.DisplayOrder, p.Id
                       where p.Published &&
                             !p.Deleted &&
                             p.ShowOnHomepage
                       select p;
            }, cache => cache.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductsHomepageCacheKey));

            return products;
        }

        /// <summary>
        /// Gets product
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product
        /// </returns>
        public virtual async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _productRepository.GetByIdAsync(productId, cache => default);
        }

        /// <summary>
        /// Get products by identifiers
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products
        /// </returns>
        public virtual async Task<IList<Product>> GetProductsByIdsAsync(int[] productIds)
        {
            return await _productRepository.GetByIdsAsync(productIds, cache => default, false);
        }

        /// <summary>
        /// Inserts a product
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertProductAsync(Product product)
        {
            await _productRepository.InsertAsync(product);
        }

        /// <summary>
        /// Updates the product
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }

        /// <summary>
        /// Gets featured products by a category identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of featured products
        /// </returns>
        public virtual async Task<IList<Product>> GetCategoryFeaturedProductsAsync(int categoryId, int storeId = 0)
        {
            IList<Product> featuredProducts = new List<Product>();

            if (categoryId == 0)
                return featuredProducts;

            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoryFeaturedProductsIdsKey, categoryId, customerRoleIds, storeId);

            var featuredProductIds = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var query = from p in _productRepository.Table
                            join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId
                            where p.Published && !p.Deleted && p.VisibleIndividually &&
                                (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < DateTime.UtcNow) &&
                                (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > DateTime.UtcNow) &&
                                pc.IsFeaturedProduct && categoryId == pc.CategoryId
                            select p;

                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                //apply ACL constraints
                query = await _aclService.ApplyAcl(query, customerRoleIds);

                featuredProducts = query.ToList();

                return featuredProducts.Select(p => p.Id).ToList();
            });

            if (featuredProducts.Count == 0 && featuredProductIds.Count > 0)
                featuredProducts = await _productRepository.GetByIdsAsync(featuredProductIds, cache => default, false);

            return featuredProducts;
        }

        /// <summary>
        /// Gets featured products by manufacturer identifier
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of featured products
        /// </returns>
        public virtual async Task<IList<Product>> GetManufacturerFeaturedProductsAsync(int manufacturerId, int storeId = 0)
        {
            IList<Product> featuredProducts = new List<Product>();

            if (manufacturerId == 0)
                return featuredProducts;

            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ManufacturerFeaturedProductIdsKey, manufacturerId, customerRoleIds, storeId);

            var featuredProductIds = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var query = from p in _productRepository.Table
                            join pm in _productManufacturerRepository.Table on p.Id equals pm.ProductId
                            where p.Published && !p.Deleted && p.VisibleIndividually &&
                                (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < DateTime.UtcNow) &&
                                (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > DateTime.UtcNow) &&
                                pm.IsFeaturedProduct && manufacturerId == pm.ManufacturerId
                            select p;

                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                //apply ACL constraints
                query = await _aclService.ApplyAcl(query, customerRoleIds);

                return query.Select(p => p.Id).ToList();
            });

            if (featuredProducts.Count == 0 && featuredProductIds.Count > 0)
                featuredProducts = await _productRepository.GetByIdsAsync(featuredProductIds, cache => default, false);

            return featuredProducts;
        }

        /// <summary>
        /// Gets products which marked as new
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of new products
        /// </returns>
        public virtual async Task<IPagedList<Product>> GetProductsMarkedAsNewAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from p in _productRepository.Table
                        where p.Published && p.VisibleIndividually && p.MarkAsNew && !p.Deleted &&
                            DateTime.UtcNow >= (p.MarkAsNewStartDateTimeUtc ?? DateTime.MinValue) &&
                            DateTime.UtcNow <= (p.MarkAsNewEndDateTimeUtc ?? DateTime.MaxValue)
                        select p;

            //apply store mapping constraints
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);

            //apply ACL constraints
            var customer = await _workContext.GetCurrentCustomerAsync();
            query = await _aclService.ApplyAcl(query, customer);

            query = query.OrderByDescending(p => p.CreatedOnUtc);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Get number of product (published and visible) in certain category
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of products
        /// </returns>
        public virtual async Task<int> GetNumberOfProductsInCategoryAsync(IList<int> categoryIds = null, int storeId = 0)
        {
            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            var query = _productRepository.Table.Where(p => p.Published && !p.Deleted && p.VisibleIndividually);

            //apply store mapping constraints
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);

            //apply ACL constraints
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            query = await _aclService.ApplyAcl(query, customerRoleIds);

            //category filtering
            if (categoryIds != null && categoryIds.Any())
            {
                query = from p in query
                        join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId
                        where categoryIds.Contains(pc.CategoryId)
                        select p;
            }

            var cacheKey = _staticCacheManager
                .PrepareKeyForDefaultCache(NopCatalogDefaults.CategoryProductsNumberCacheKey, customerRoleIds, storeId, categoryIds);

            //only distinct products
            return await _staticCacheManager.GetAsync(cacheKey, () => query.Select(p => p.Id).Count());
        }

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
        /// <param name="filteredSpecOptions">Specification options list to filter products; null to load all records</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products
        /// </returns>
        public virtual async Task<IPagedList<Product>> SearchProductsAsync(
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
            IList<SpecificationAttributeOption> filteredSpecOptions = null,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null)
        {
            //some databases don't support int.MaxValue
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            var productsQuery = _productRepository.Table;

            if (!showHidden)
                productsQuery = productsQuery.Where(p => p.Published);
            else if (overridePublished.HasValue)
                productsQuery = productsQuery.Where(p => p.Published == overridePublished.Value);

            var customer = await _workContext.GetCurrentCustomerAsync();

            if (!showHidden || storeId > 0)
            {
                //apply store mapping constraints
                productsQuery = await _storeMappingService.ApplyStoreMapping(productsQuery, storeId);
            }

            if (!showHidden)
            {
                //apply ACL constraints
                productsQuery = await _aclService.ApplyAcl(productsQuery, customer);
            }

            productsQuery =
                from p in productsQuery
                where !p.Deleted &&
                    (!visibleIndividuallyOnly || p.VisibleIndividually) &&
                    (vendorId == 0 || p.VendorId == vendorId) &&
                    (
                        warehouseId == 0 ||
                        (
                            !p.UseMultipleWarehouses ? p.WarehouseId == warehouseId :
                                _productWarehouseInventoryRepository.Table.Any(pwi => pwi.WarehouseId == warehouseId && pwi.ProductId == p.Id)
                        )
                    ) &&
                    (productType == null || p.ProductTypeId == (int)productType) &&
                    (showHidden ||
                            DateTime.UtcNow >= (p.AvailableStartDateTimeUtc ?? DateTime.MinValue) &&
                            DateTime.UtcNow <= (p.AvailableEndDateTimeUtc ?? DateTime.MaxValue)
                    ) &&
                    (priceMin == null || p.Price >= priceMin) &&
                    (priceMax == null || p.Price <= priceMax)
                select p;

            if (!string.IsNullOrEmpty(keywords))
            {
                var langs = await _languageService.GetAllLanguagesAsync(showHidden: true);

                //Set a flag which will to points need to search in localized properties. If showHidden doesn't set to true should be at least two published languages.
                var searchLocalizedValue = languageId > 0 && langs.Count >= 2 && (showHidden || langs.Count(l => l.Published) >= 2);
                IQueryable<int> productsByKeywords;

                var activeSearchProvider = await _searchPluginManager.LoadPrimaryPluginAsync(customer, storeId);

                if (activeSearchProvider is not null)
                {
                    productsByKeywords = (await activeSearchProvider.SearchProductsAsync(keywords, searchLocalizedValue)).AsQueryable();
                }
                else
                {
                    productsByKeywords =
                        from p in _productRepository.Table
                        where p.Name.Contains(keywords) ||
                            (searchDescriptions &&
                                (p.ShortDescription.Contains(keywords) || p.FullDescription.Contains(keywords))) ||
                            (searchManufacturerPartNumber && p.ManufacturerPartNumber == keywords) ||
                            (searchSku && p.Sku == keywords)
                        select p.Id;

                    if (searchLocalizedValue)
                    {
                        productsByKeywords = productsByKeywords.Union(
                            from lp in _localizedPropertyRepository.Table
                            let checkName = lp.LocaleKey == nameof(Product.Name) &&
                                            lp.LocaleValue.Contains(keywords)
                            let checkShortDesc = searchDescriptions &&
                                            lp.LocaleKey == nameof(Product.ShortDescription) &&
                                            lp.LocaleValue.Contains(keywords)
                            where
                                lp.LocaleKeyGroup == nameof(Product) && lp.LanguageId == languageId && (checkName || checkShortDesc)

                            select lp.EntityId);
                    }
                }

                //search by SKU for ProductAttributeCombination
                if (searchSku)
                {
                    productsByKeywords = productsByKeywords.Union(
                        from pac in _productAttributeCombinationRepository.Table
                        where pac.Sku == keywords
                        select pac.ProductId);
                }

                //search by category name if admin allows
                if (_catalogSettings.AllowCustomersToSearchWithCategoryName)
                {
                    var categoryQuery = _categoryRepository.Table;

                    if (!showHidden)
                        categoryQuery = categoryQuery.Where(p => p.Published);
                    else if (overridePublished.HasValue)
                        categoryQuery = categoryQuery.Where(p => p.Published == overridePublished.Value);

                    if (!showHidden || storeId > 0)
                        categoryQuery = await _storeMappingService.ApplyStoreMapping(categoryQuery, storeId);

                    if (!showHidden)
                        categoryQuery = await _aclService.ApplyAcl(categoryQuery, customer);

                    productsByKeywords = productsByKeywords.Union(
                        from pc in _productCategoryRepository.Table
                        join c in categoryQuery on pc.CategoryId equals c.Id
                        where c.Name.Contains(keywords) && !c.Deleted
                        select pc.ProductId
                    );

                    if (searchLocalizedValue)
                    {
                        productsByKeywords = productsByKeywords.Union(
                        from pc in _productCategoryRepository.Table
                        join lp in _localizedPropertyRepository.Table on pc.CategoryId equals lp.EntityId
                        where lp.LocaleKeyGroup == nameof(Category) &&
                              lp.LocaleKey == nameof(Category.Name) &&
                              lp.LocaleValue.Contains(keywords) &&
                              lp.LanguageId == languageId
                        select pc.ProductId);
                    }
                }

                //search by manufacturer name if admin allows
                if (_catalogSettings.AllowCustomersToSearchWithManufacturerName)
                {
                    var manufacturerQuery = _manufacturerRepository.Table;

                    if (!showHidden)
                        manufacturerQuery = manufacturerQuery.Where(p => p.Published);
                    else if (overridePublished.HasValue)
                        manufacturerQuery = manufacturerQuery.Where(p => p.Published == overridePublished.Value);

                    if (!showHidden || storeId > 0)
                        manufacturerQuery = await _storeMappingService.ApplyStoreMapping(manufacturerQuery, storeId);

                    if (!showHidden)
                        manufacturerQuery = await _aclService.ApplyAcl(manufacturerQuery, customer);

                    productsByKeywords = productsByKeywords.Union(
                        from pm in _productManufacturerRepository.Table
                        join m in manufacturerQuery on pm.ManufacturerId equals m.Id
                        where m.Name.Contains(keywords) && !m.Deleted
                        select pm.ProductId
                    );

                    if (searchLocalizedValue)
                    {
                        productsByKeywords = productsByKeywords.Union(
                        from pm in _productManufacturerRepository.Table
                        join lp in _localizedPropertyRepository.Table on pm.ManufacturerId equals lp.EntityId
                        where lp.LocaleKeyGroup == nameof(Manufacturer) &&
                              lp.LocaleKey == nameof(Manufacturer.Name) &&
                              lp.LocaleValue.Contains(keywords) &&
                              lp.LanguageId == languageId
                        select pm.ProductId);
                    }
                }

                if (searchProductTags)
                {
                    productsByKeywords = productsByKeywords.Union(
                        from pptm in _productTagMappingRepository.Table
                        join pt in _productTagRepository.Table on pptm.ProductTagId equals pt.Id
                        where pt.Name.Contains(keywords)
                        select pptm.ProductId
                    );

                    if (searchLocalizedValue)
                    {
                        productsByKeywords = productsByKeywords.Union(
                        from pptm in _productTagMappingRepository.Table
                        join lp in _localizedPropertyRepository.Table on pptm.ProductTagId equals lp.EntityId
                        where lp.LocaleKeyGroup == nameof(ProductTag) &&
                              lp.LocaleKey == nameof(ProductTag.Name) &&
                              lp.LocaleValue.Contains(keywords) &&
                              lp.LanguageId == languageId
                        select pptm.ProductId);
                    }
                }

                productsQuery =
                    from p in productsQuery
                    join pbk in productsByKeywords on p.Id equals pbk
                    select p;
            }

            if (categoryIds is not null)
            {
                if (categoryIds.Contains(0))
                    categoryIds.Remove(0);

                if (categoryIds.Any())
                {
                    var productCategoryQuery =
                        from pc in _productCategoryRepository.Table
                        where (!excludeFeaturedProducts || !pc.IsFeaturedProduct) &&
                            categoryIds.Contains(pc.CategoryId)
                        group pc by pc.ProductId into pc
                        select new
                        {
                            ProductId = pc.Key,
                            DisplayOrder = pc.First().DisplayOrder
                        };

                    productsQuery =
                        from p in productsQuery
                        join pc in productCategoryQuery on p.Id equals pc.ProductId
                        orderby pc.DisplayOrder, p.Name
                        select p;
                }
            }

            if (manufacturerIds is not null)
            {
                if (manufacturerIds.Contains(0))
                    manufacturerIds.Remove(0);

                if (manufacturerIds.Any())
                {
                    var productManufacturerQuery =
                        from pm in _productManufacturerRepository.Table
                        where (!excludeFeaturedProducts || !pm.IsFeaturedProduct) &&
                            manufacturerIds.Contains(pm.ManufacturerId)
                        group pm by pm.ProductId into pm
                        select new
                        {
                            ProductId = pm.Key,
                            DisplayOrder = pm.First().DisplayOrder
                        };

                    productsQuery =
                        from p in productsQuery
                        join pm in productManufacturerQuery on p.Id equals pm.ProductId
                        orderby pm.DisplayOrder, p.Name
                        select p;
                }
            }

            if (productTagId > 0)
            {
                productsQuery =
                    from p in productsQuery
                    join ptm in _productTagMappingRepository.Table on p.Id equals ptm.ProductId
                    where ptm.ProductTagId == productTagId
                    select p;
            }

            if (filteredSpecOptions?.Count > 0)
            {
                var specificationAttributeIds = filteredSpecOptions
                    .Select(sao => sao.SpecificationAttributeId)
                    .Distinct();

                foreach (var specificationAttributeId in specificationAttributeIds)
                {
                    var optionIdsBySpecificationAttribute = filteredSpecOptions
                        .Where(o => o.SpecificationAttributeId == specificationAttributeId)
                        .Select(o => o.Id);

                    var productSpecificationQuery =
                        from psa in _productSpecificationAttributeRepository.Table
                        where psa.AllowFiltering && optionIdsBySpecificationAttribute.Contains(psa.SpecificationAttributeOptionId)
                        select psa;

                    productsQuery =
                        from p in productsQuery
                        where productSpecificationQuery.Any(pc => pc.ProductId == p.Id)
                        select p;
                }
            }

            return await productsQuery.OrderBy(_localizedPropertyRepository, await _workContext.GetWorkingLanguageAsync(), orderBy).ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets products by product attribute
        /// </summary>
        /// <param name="productAttributeId">Product attribute identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products
        /// </returns>
        public virtual async Task<IPagedList<Product>> GetProductsByProductAttributeIdAsync(int productAttributeId,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from p in _productRepository.Table
                        join pam in _productAttributeMappingRepository.Table on p.Id equals pam.ProductId
                        where
                            pam.ProductAttributeId == productAttributeId &&
                            !p.Deleted
                        orderby p.Name
                        select p;

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets associated products
        /// </summary>
        /// <param name="parentGroupedProductId">Parent product identifier (used with grouped products)</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products
        /// </returns>
        public virtual async Task<IList<Product>> GetAssociatedProductsAsync(int parentGroupedProductId,
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

            var products = await query.ToListAsync();

            //ACL mapping
            if (!showHidden)
                products = await products.WhereAwait(async x => await _aclService.AuthorizeAsync(x)).ToListAsync();

            //Store mapping
            if (!showHidden && storeId > 0)
                products = await products.WhereAwait(async x => await _storeMappingService.AuthorizeAsync(x, storeId)).ToListAsync();

            return products;
        }

        /// <summary>
        /// Update product review totals
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductReviewTotalsAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var approvedRatingSum = 0;
            var notApprovedRatingSum = 0;
            var approvedTotalReviews = 0;
            var notApprovedTotalReviews = 0;

            var reviews = _productReviewRepository.Table
                .Where(r => r.ProductId == product.Id)
                .ToAsyncEnumerable();
            await foreach (var pr in reviews)
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

            product.ApprovedRatingSum = approvedRatingSum;
            product.NotApprovedRatingSum = notApprovedRatingSum;
            product.ApprovedTotalReviews = approvedTotalReviews;
            product.NotApprovedTotalReviews = notApprovedTotalReviews;
            await UpdateProductAsync(product);
        }

        /// <summary>
        /// Get low stock products
        /// </summary>
        /// <param name="vendorId">Vendor identifier; pass null to load all records</param>
        /// <param name="loadPublishedOnly">Whether to load published products only; pass null to load all products, pass true to load only published products, pass false to load only unpublished products</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products
        /// </returns>
        public virtual async Task<IPagedList<Product>> GetLowStockProductsAsync(int? vendorId = null, bool? loadPublishedOnly = true,
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
                query = query.Where(product => product.Published == loadPublishedOnly.Value);

            query = query.OrderBy(product => product.MinStockQuantity).ThenBy(product => product.DisplayOrder).ThenBy(product => product.Id);

            return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Get low stock product combinations
        /// </summary>
        /// <param name="vendorId">Vendor identifier; pass null to load all records</param>
        /// <param name="loadPublishedOnly">Whether to load combinations of published products only; pass null to load all products, pass true to load only published products, pass false to load only unpublished products</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product combinations
        /// </returns>
        public virtual async Task<IPagedList<ProductAttributeCombination>> GetLowStockProductCombinationsAsync(int? vendorId = null, bool? loadPublishedOnly = true,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var combinations = from pac in _productAttributeCombinationRepository.Table
                               join p in _productRepository.Table on pac.ProductId equals p.Id
                               where
                                   //filter by combinations with stock quantity less than the minimum
                                   pac.StockQuantity <= pac.MinStockQuantity &&
                                   //filter by products with tracking inventory by attributes
                                   p.ManageInventoryMethodId == (int)ManageInventoryMethod.ManageStockByAttributes &&
                                   //ignore deleted products
                                   !p.Deleted &&
                                   //ignore grouped products
                                   p.ProductTypeId != (int)ProductType.GroupedProduct &&
                                   //filter by vendor
                                   ((vendorId ?? 0) == 0 || p.VendorId == vendorId) &&
                                   //whether to load published products only
                                   (loadPublishedOnly == null || p.Published == loadPublishedOnly)
                               orderby pac.ProductId, pac.Id
                               select pac;

            return await combinations.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Gets a product by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product
        /// </returns>
        public virtual async Task<Product> GetProductBySkuAsync(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from p in _productRepository.Table
                        orderby p.Id
                        where !p.Deleted &&
                        p.Sku == sku
                        select p;
            var product = await query.FirstOrDefaultAsync();

            return product;
        }

        /// <summary>
        /// Gets a products by SKU array
        /// </summary>
        /// <param name="skuArray">SKU array</param>
        /// <param name="vendorId">Vendor ID; 0 to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the products
        /// </returns>
        public async Task<IList<Product>> GetProductsBySkuAsync(string[] skuArray, int vendorId = 0)
        {
            if (skuArray == null)
                throw new ArgumentNullException(nameof(skuArray));

            var query = _productRepository.Table;
            query = query.Where(p => !p.Deleted && skuArray.Contains(p.Sku));

            if (vendorId != 0)
                query = query.Where(p => p.VendorId == vendorId);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Update HasTierPrices property (used for performance optimization)
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateHasTierPricesPropertyAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            product.HasTierPrices = (await GetTierPricesByProductAsync(product.Id)).Any();
            await UpdateProductAsync(product);
        }

        /// <summary>
        /// Update HasDiscountsApplied property (used for performance optimization)
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateHasDiscountsAppliedAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            product.HasDiscountsApplied = _discountProductMappingRepository.Table.Any(dpm => dpm.EntityId == product.Id);
            await UpdateProductAsync(product);
        }

        /// <summary>
        /// Gets number of products by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of products
        /// </returns>
        public async Task<int> GetNumberOfProductsByVendorIdAsync(int vendorId)
        {
            if (vendorId == 0)
                return 0;

            return await _productRepository.Table.CountAsync(p => p.VendorId == vendorId && !p.Deleted);
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
                if (int.TryParse(idStr, out var id))
                    ids.Add(id);

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

            dateTime ??= DateTime.UtcNow;

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
                var quantities = product.AllowedQuantities
                   .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToList();
                foreach (var qtyStr in quantities)
                {
                    if (int.TryParse(qtyStr.Trim(), out var qty))
                        result.Add(qty);
                }
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<int> GetTotalStockQuantityAsync(Product product, bool useReservedQuantity = true, int warehouseId = 0)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock)
                //We can calculate total stock quantity when 'Manage inventory' property is set to 'Track inventory'
                return 0;

            if (!product.UseMultipleWarehouses)
                return product.StockQuantity;

            var pwi = _productWarehouseInventoryRepository.Table.Where(wi => wi.ProductId == product.Id);

            if (warehouseId > 0)
                pwi = pwi.Where(x => x.WarehouseId == warehouseId);

            var result = await pwi.SumAsync(x => x.StockQuantity);
            if (useReservedQuantity)
                result -= await pwi.SumAsync(x => x.ReservedQuantity);

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the stock message
        /// </returns>
        public virtual async Task<string> FormatStockMessageAsync(Product product, string attributesXml)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var stockMessage = string.Empty;

            switch (product.ManageInventoryMethod)
            {
                case ManageInventoryMethod.ManageStock:
                    stockMessage = await GetStockMessageAsync(product);
                    break;
                case ManageInventoryMethod.ManageStockByAttributes:
                    stockMessage = await GetStockMessageForAttributesAsync(product, attributesXml);
                    break;
            }

            return stockMessage;
        }

        /// <summary>
        /// Formats SKU
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sKU
        /// </returns>
        public virtual async Task<string> FormatSkuAsync(Product product, string attributesXml = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var (sku, _, _) = await GetSkuMpnGtinAsync(product, attributesXml);

            return sku;
        }

        /// <summary>
        /// Formats manufacturer part number
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer part number
        /// </returns>
        public virtual async Task<string> FormatMpnAsync(Product product, string attributesXml = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var (_, manufacturerPartNumber, _) = await GetSkuMpnGtinAsync(product, attributesXml);

            return manufacturerPartNumber;
        }

        /// <summary>
        /// Formats GTIN
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gTIN
        /// </returns>
        public virtual async Task<string> FormatGtinAsync(Product product, string attributesXml = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var (_, _, gtin) = await GetSkuMpnGtinAsync(product, attributesXml);

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
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductStoreMappingsAsync(Product product, IList<int> limitedToStoresIds)
        {
            product.LimitedToStores = limitedToStoresIds.Any();

            var limitedToStoresIdsSet = limitedToStoresIds.ToHashSet();
            var existingStoreMappingsByStoreId = (await _storeMappingService.GetStoreMappingsAsync(product))
                .ToDictionary(sm => sm.StoreId);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (limitedToStoresIdsSet.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappingsByStoreId.ContainsKey(store.Id))
                        await _storeMappingService.InsertStoreMappingAsync(product, store.Id);
                }
                else
                {
                    //remove store
                    if (existingStoreMappingsByStoreId.TryGetValue(store.Id, out var storeMappingToDelete))
                        await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        /// <summary>
        /// Gets the value whether the sequence contains downloadable products
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> HasAnyDownloadableProductAsync(int[] productIds)
        {
            return await _productRepository.Table
                .AnyAsync(p => productIds.Contains(p.Id) && p.IsDownload);
        }

        /// <summary>
        /// Gets the value whether the sequence contains gift card products
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> HasAnyGiftCardProductAsync(int[] productIds)
        {
            return await _productRepository.Table
                .AnyAsync(p => productIds.Contains(p.Id) && p.IsGiftCard);
        }

        /// <summary>
        /// Gets the value whether the sequence contains recurring products
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> HasAnyRecurringProductAsync(int[] productIds)
        {
            return await _productRepository.Table
                .AnyAsync(p => productIds.Contains(p.Id) && p.IsRecurring);
        }

        /// <summary>
        /// Returns a list of sku of not existing products
        /// </summary>
        /// <param name="productSku">The sku of the products to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of sku not existing products
        /// </returns>
        public virtual async Task<string[]> GetNotExistingProductsAsync(string[] productSku)
        {
            if (productSku == null)
                throw new ArgumentNullException(nameof(productSku));

            var query = _productRepository.Table;
            var queryFilter = productSku.Distinct().ToArray();
            //filtering by SKU
            var filter = await query.Select(p => p.Sku)
                .Where(p => queryFilter.Contains(p))
                .ToListAsync();

            return queryFilter.Except(filter).ToArray();
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
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AdjustInventoryAsync(Product product, int quantityToChange, string attributesXml = "", string message = "")
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (quantityToChange == 0)
                return;

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
            {
                //update stock quantity
                if (product.UseMultipleWarehouses)
                {
                    //use multiple warehouses
                    if (quantityToChange < 0)
                        await ReserveInventoryAsync(product, quantityToChange);
                    else
                        await UnblockReservedInventoryAsync(product, quantityToChange);
                }
                else
                {
                    //do not use multiple warehouses
                    //simple inventory management
                    product.StockQuantity += quantityToChange;
                    await UpdateProductAsync(product);

                    //quantity change history
                    await AddStockQuantityHistoryEntryAsync(product, quantityToChange, product.StockQuantity, product.WarehouseId, message);
                }

                var totalStock = await GetTotalStockQuantityAsync(product);

                await ApplyLowStockActivityAsync(product, totalStock);

                //send email notification
                if (quantityToChange < 0 && totalStock < product.NotifyAdminForQuantityBelow)
                {
                    //do not inject IWorkflowMessageService via constructor because it'll cause circular references
                    var workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
                    await workflowMessageService.SendQuantityBelowStoreOwnerNotificationAsync(product, _localizationSettings.DefaultAdminLanguageId);
                }
            }

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
            {
                var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
                if (combination != null)
                {
                    combination.StockQuantity += quantityToChange;
                    await _productAttributeService.UpdateProductAttributeCombinationAsync(combination);

                    //quantity change history
                    await AddStockQuantityHistoryEntryAsync(product, quantityToChange, combination.StockQuantity, message: message, combinationId: combination.Id);

                    if (product.AllowAddingOnlyExistingAttributeCombinations)
                    {
                        var totalStockByAllCombinations = await (await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id))
                            .ToAsyncEnumerable()
                            .SumAsync(c => c.StockQuantity);

                        await ApplyLowStockActivityAsync(product, totalStockByAllCombinations);
                    }

                    //send email notification
                    if (quantityToChange < 0 && combination.StockQuantity < combination.NotifyAdminForQuantityBelow)
                    {
                        //do not inject IWorkflowMessageService via constructor because it'll cause circular references
                        var workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
                        await workflowMessageService.SendQuantityBelowStoreOwnerNotificationAsync(combination, _localizationSettings.DefaultAdminLanguageId);
                    }
                }
            }

            //bundled products
            var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                if (attributeValue.AttributeValueType != AttributeValueType.AssociatedToProduct)
                    continue;

                //associated product (bundle)
                var associatedProduct = await GetProductByIdAsync(attributeValue.AssociatedProductId);
                if (associatedProduct != null)
                {
                    await AdjustInventoryAsync(associatedProduct, quantityToChange * attributeValue.Quantity, message);
                }
            }
        }

        /// <summary>
        /// Book the reserved quantity
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="quantity">Quantity, must be negative</param>
        /// <param name="message">Message for the stock quantity history</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task BookReservedInventoryAsync(Product product, int warehouseId, int quantity, string message = "")
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (quantity >= 0)
                throw new ArgumentException("Value must be negative.", nameof(quantity));

            //only products with "use multiple warehouses" are handled this way
            if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock || !product.UseMultipleWarehouses)
                return;

            var pwi = await _productWarehouseInventoryRepository.Table

                .FirstOrDefaultAsync(wi => wi.ProductId == product.Id && wi.WarehouseId == warehouseId);
            if (pwi == null)
                return;

            pwi.ReservedQuantity = Math.Max(pwi.ReservedQuantity + quantity, 0);
            pwi.StockQuantity += quantity;

            await UpdateProductWarehouseInventoryAsync(pwi);

            //quantity change history
            await AddStockQuantityHistoryEntryAsync(product, quantity, pwi.StockQuantity, warehouseId, message);
        }

        /// <summary>
        /// Reverse booked inventory (if acceptable)
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="shipmentItem">Shipment item</param>
        /// <param name="message">Message for the stock quantity history</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the quantity reversed
        /// </returns>
        public virtual async Task<int> ReverseBookedInventoryAsync(Product product, ShipmentItem shipmentItem, string message = "")
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (shipmentItem == null)
                throw new ArgumentNullException(nameof(shipmentItem));

            //only products with "use multiple warehouses" are handled this way
            if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock || !product.UseMultipleWarehouses)
                return 0;

            var pwi = await _productWarehouseInventoryRepository.Table

                .FirstOrDefaultAsync(wi => wi.ProductId == product.Id && wi.WarehouseId == shipmentItem.WarehouseId);
            if (pwi == null)
                return 0;

            var shipment = await _shipmentRepository.GetByIdAsync(shipmentItem.ShipmentId, cache => default);

            //not shipped yet? hence "BookReservedInventory" method was not invoked
            if (!shipment.ShippedDateUtc.HasValue)
                return 0;

            var qty = shipmentItem.Quantity;

            pwi.StockQuantity += qty;
            pwi.ReservedQuantity += qty;

            await UpdateProductWarehouseInventoryAsync(pwi);

            //quantity change history
            await AddStockQuantityHistoryEntryAsync(product, qty, pwi.StockQuantity, shipmentItem.WarehouseId, message);

            return qty;
        }

        #endregion

        #region Related products

        /// <summary>
        /// Deletes a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteRelatedProductAsync(RelatedProduct relatedProduct)
        {
            await _relatedProductRepository.DeleteAsync(relatedProduct);
        }

        /// <summary>
        /// Gets related products by product identifier
        /// </summary>
        /// <param name="productId">The first product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the related products
        /// </returns>
        public virtual async Task<IList<RelatedProduct>> GetRelatedProductsByProductId1Async(int productId, bool showHidden = false)
        {
            var query = from rp in _relatedProductRepository.Table
                        join p in _productRepository.Table on rp.ProductId2 equals p.Id
                        where rp.ProductId1 == productId &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby rp.DisplayOrder, rp.Id
                        select rp;

            var relatedProducts = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.RelatedProductsCacheKey, productId, showHidden), async () => await query.ToListAsync());

            return relatedProducts;
        }

        /// <summary>
        /// Gets a related product
        /// </summary>
        /// <param name="relatedProductId">Related product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the related product
        /// </returns>
        public virtual async Task<RelatedProduct> GetRelatedProductByIdAsync(int relatedProductId)
        {
            return await _relatedProductRepository.GetByIdAsync(relatedProductId, cache => default);
        }

        /// <summary>
        /// Inserts a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertRelatedProductAsync(RelatedProduct relatedProduct)
        {
            await _relatedProductRepository.InsertAsync(relatedProduct);
        }

        /// <summary>
        /// Updates a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateRelatedProductAsync(RelatedProduct relatedProduct)
        {
            await _relatedProductRepository.UpdateAsync(relatedProduct);
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
            return source.FirstOrDefault(rp => rp.ProductId1 == productId1 && rp.ProductId2 == productId2);
        }

        #endregion

        #region Cross-sell products

        /// <summary>
        /// Deletes a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCrossSellProductAsync(CrossSellProduct crossSellProduct)
        {
            await _crossSellProductRepository.DeleteAsync(crossSellProduct);
        }

        /// <summary>
        /// Gets cross-sell products by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the cross-sell products
        /// </returns>
        public virtual async Task<IList<CrossSellProduct>> GetCrossSellProductsByProductId1Async(int productId1, bool showHidden = false)
        {
            return await GetCrossSellProductsByProductIdsAsync(new[] { productId1 }, showHidden);
        }

        /// <summary>
        /// Gets a cross-sell product
        /// </summary>
        /// <param name="crossSellProductId">Cross-sell product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the cross-sell product
        /// </returns>
        public virtual async Task<CrossSellProduct> GetCrossSellProductByIdAsync(int crossSellProductId)
        {
            return await _crossSellProductRepository.GetByIdAsync(crossSellProductId, cache => default);
        }

        /// <summary>
        /// Inserts a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCrossSellProductAsync(CrossSellProduct crossSellProduct)
        {
            await _crossSellProductRepository.InsertAsync(crossSellProduct);
        }

        /// <summary>
        /// Gets a cross-sells
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="numberOfProducts">Number of products to return</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the cross-sells
        /// </returns>
        public virtual async Task<IList<Product>> GetCrossSellProductsByShoppingCartAsync(IList<ShoppingCartItem> cart, int numberOfProducts)
        {
            var result = new List<Product>();

            if (numberOfProducts == 0 || cart?.Any() != true)
                return result;

            var cartProductIds = cart.Select(sci => sci.ProductId).ToHashSet();
            return await (await GetCrossSellProductsByProductIdsAsync(cartProductIds.ToArray()))
                .Select(cs => cs.ProductId2)
                .Except(cartProductIds)
                .SelectAwait(async cs => await GetProductByIdAsync(cs))
                .Where(p => p != null && !p.Deleted && p.Published)
                .Take(numberOfProducts)
                .ToListAsync();
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
            return source.FirstOrDefault(csp => csp.ProductId1 == productId1 && csp.ProductId2 == productId2);
        }

        #endregion

        #region Tier prices

        /// <summary>
        /// Gets a product tier prices for customer
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <param name="store">Store</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IList<TierPrice>> GetTierPricesAsync(Product product, Customer customer, Store store)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            if (customer is null)
                throw new ArgumentNullException(nameof(customer));

            if (!product.HasTierPrices)
                return null;

            //get actual tier prices
            return (await GetTierPricesByProductAsync(product.Id))
                .OrderBy(price => price.Quantity)
                .FilterByStore(store)
                .FilterByCustomerRole(await _customerService.GetCustomerRoleIdsAsync(customer))
                .FilterByDate()
                .RemoveDuplicatedQuantities()
                .ToList();
        }

        /// <summary>
        /// Gets a tier prices by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IList<TierPrice>> GetTierPricesByProductAsync(int productId)
        {
            var query = _tierPriceRepository.Table.Where(tp => tp.ProductId == productId);

            return await _staticCacheManager.GetAsync(
                _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.TierPricesByProductCacheKey, productId),
                async () => await query.ToListAsync());
        }

        /// <summary>
        /// Deletes a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTierPriceAsync(TierPrice tierPrice)
        {
            await _tierPriceRepository.DeleteAsync(tierPrice);
        }

        /// <summary>
        /// Gets a tier price
        /// </summary>
        /// <param name="tierPriceId">Tier price identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ier price
        /// </returns>
        public virtual async Task<TierPrice> GetTierPriceByIdAsync(int tierPriceId)
        {
            return await _tierPriceRepository.GetByIdAsync(tierPriceId, cache => default);
        }

        /// <summary>
        /// Inserts a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTierPriceAsync(TierPrice tierPrice)
        {
            await _tierPriceRepository.InsertAsync(tierPrice);
        }

        /// <summary>
        /// Updates the tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTierPriceAsync(TierPrice tierPrice)
        {
            await _tierPriceRepository.UpdateAsync(tierPrice);
        }

        /// <summary>
        /// Gets a preferred tier price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <param name="store">Store</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tier price
        /// </returns>
        public virtual async Task<TierPrice> GetPreferredTierPriceAsync(Product product, Customer customer, Store store, int quantity)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            if (customer is null)
                throw new ArgumentNullException(nameof(customer));

            if (!product.HasTierPrices)
                return null;

            //get the most suitable tier price based on the passed quantity
            return (await GetTierPricesAsync(product, customer, store))?.LastOrDefault(price => quantity >= price.Quantity);
        }

        #endregion

        #region Product pictures

        /// <summary>
        /// Deletes a product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductPictureAsync(ProductPicture productPicture)
        {
            await _productPictureRepository.DeleteAsync(productPicture);
        }

        /// <summary>
        /// Gets a product pictures by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product pictures
        /// </returns>
        public virtual async Task<IList<ProductPicture>> GetProductPicturesByProductIdAsync(int productId)
        {
            var query = from pp in _productPictureRepository.Table
                        where pp.ProductId == productId
                        orderby pp.DisplayOrder, pp.Id
                        select pp;

            var productPictures = await query.ToListAsync();

            return productPictures;
        }

        /// <summary>
        /// Gets a product picture
        /// </summary>
        /// <param name="productPictureId">Product picture identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product picture
        /// </returns>
        public virtual async Task<ProductPicture> GetProductPictureByIdAsync(int productPictureId)
        {
            return await _productPictureRepository.GetByIdAsync(productPictureId, cache => default);
        }

        /// <summary>
        /// Inserts a product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertProductPictureAsync(ProductPicture productPicture)
        {
            await _productPictureRepository.InsertAsync(productPicture);
        }

        /// <summary>
        /// Updates a product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductPictureAsync(ProductPicture productPicture)
        {
            await _productPictureRepository.UpdateAsync(productPicture);
        }

        /// <summary>
        /// Get the IDs of all product images 
        /// </summary>
        /// <param name="productsIds">Products IDs</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the all picture identifiers grouped by product ID
        /// </returns>
        public async Task<IDictionary<int, int[]>> GetProductsImagesIdsAsync(int[] productsIds)
        {
            var productPictures = await _productPictureRepository.Table
                .Where(p => productsIds.Contains(p.ProductId))
                .ToListAsync();

            return productPictures.GroupBy(p => p.ProductId).ToDictionary(p => p.Key, p => p.Select(p1 => p1.PictureId).ToArray());
        }

        /// <summary>
        /// Get products for which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted products</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of products
        /// </returns>
        public virtual async Task<IPagedList<Product>> GetProductsWithAppliedDiscountAsync(int? discountId = null,
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

            return await products.ToPagedListAsync(pageIndex, pageSize);
        }

        #endregion

        #region Product videos

        /// <summary>
        /// Deletes a product video
        /// </summary>
        /// <param name="productVideo">Product video</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductVideoAsync(ProductVideo productVideo)
        {
            await _productVideoRepository.DeleteAsync(productVideo);
        }

        /// <summary>
        /// Gets a product videos by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product videos
        /// </returns>
        public virtual async Task<IList<ProductVideo>> GetProductVideosByProductIdAsync(int productId)
        {
            var query = from pvm in _productVideoRepository.Table
                        where pvm.ProductId == productId
                        orderby pvm.DisplayOrder, pvm.Id
                        select pvm;

            var productVideos = await query.ToListAsync();

            return productVideos;
        }

        /// <summary>
        /// Gets a product video
        /// </summary>
        /// <param name="productPictureId">Product video identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product video
        /// </returns>
        public virtual async Task<ProductVideo> GetProductVideoByIdAsync(int productVideoId)
        {
            return await _productVideoRepository.GetByIdAsync(productVideoId, cache => default);
        }

        /// <summary>
        /// Inserts a product video
        /// </summary>
        /// <param name="productVideo">Product picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertProductVideoAsync(ProductVideo productVideo)
        {
            await _productVideoRepository.InsertAsync(productVideo);
        }

        /// <summary>
        /// Updates a product video
        /// </summary>
        /// <param name="productVideo">Product video</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductVideoAsync(ProductVideo productVideo)
        {
            await _productVideoRepository.UpdateAsync(productVideo);
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
        /// <param name="storeId">The store identifier, where a review has been created; pass 0 to load all records</param>
        /// <param name="productId">The product identifier; pass 0 to load all records</param>
        /// <param name="vendorId">The vendor identifier (limit to products of this vendor); pass 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the reviews
        /// </returns>
        public virtual async Task<IPagedList<ProductReview>> GetAllProductReviewsAsync(int customerId = 0, bool? approved = null,
            DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = null, int storeId = 0, int productId = 0, int vendorId = 0, bool showHidden = false,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var productReviews = await _productReviewRepository.GetAllPagedAsync(async query =>
            {
                if (!showHidden)
                {
                    var productsQuery = _productRepository.Table.Where(p => p.Published);

                    //apply store mapping constraints
                    productsQuery = await _storeMappingService.ApplyStoreMapping(productsQuery, storeId);

                    //apply ACL constraints
                    var customer = await _workContext.GetCurrentCustomerAsync();
                    productsQuery = await _aclService.ApplyAcl(productsQuery, customer);

                    query = query.Where(review => productsQuery.Any(product => product.Id == review.ProductId));
                }

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
                if (storeId > 0)
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

                query = _catalogSettings.ProductReviewsSortByCreatedDateAscending
                    ? query.OrderBy(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id)
                    : query.OrderByDescending(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id);

                return query;
            }, pageIndex, pageSize);

            return productReviews;
        }

        /// <summary>
        /// Gets product review
        /// </summary>
        /// <param name="productReviewId">Product review identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product review
        /// </returns>
        public virtual async Task<ProductReview> GetProductReviewByIdAsync(int productReviewId)
        {
            return await _productReviewRepository.GetByIdAsync(productReviewId, cache => default);
        }

        /// <summary>
        /// Get product reviews by identifiers
        /// </summary>
        /// <param name="productReviewIds">Product review identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product reviews
        /// </returns>
        public virtual async Task<IList<ProductReview>> GetProductReviewsByIdsAsync(int[] productReviewIds)
        {
            return await _productReviewRepository.GetByIdsAsync(productReviewIds);
        }

        /// <summary>
        /// Inserts a product review
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertProductReviewAsync(ProductReview productReview)
        {
            await _productReviewRepository.InsertAsync(productReview);
        }

        /// <summary>
        /// Deletes a product review
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductReviewAsync(ProductReview productReview)
        {
            await _productReviewRepository.DeleteAsync(productReview);
        }

        /// <summary>
        /// Deletes product reviews
        /// </summary>
        /// <param name="productReviews">Product reviews</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductReviewsAsync(IList<ProductReview> productReviews)
        {
            await _productReviewRepository.DeleteAsync(productReviews);
        }

        /// <summary>
        /// Sets or create a product review helpfulness record
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="helpfulness">Value indicating whether a review a helpful</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetProductReviewHelpfulnessAsync(ProductReview productReview, bool helpfulness)
        {
            if (productReview is null)
                throw new ArgumentNullException(nameof(productReview));

            var customer = await _workContext.GetCurrentCustomerAsync();
            var prh = _productReviewHelpfulnessRepository.Table
                .SingleOrDefault(h => h.ProductReviewId == productReview.Id && h.CustomerId == customer.Id);

            if (prh is null)
            {
                //insert new helpfulness
                prh = new ProductReviewHelpfulness
                {
                    ProductReviewId = productReview.Id,
                    CustomerId = customer.Id,
                    WasHelpful = helpfulness,
                };

                await InsertProductReviewHelpfulnessAsync(prh);
            }
            else
            {
                //existing one
                prh.WasHelpful = helpfulness;

                await _productReviewHelpfulnessRepository.UpdateAsync(prh);
            }
        }

        /// <summary>
        /// Updates a product review
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductReviewAsync(ProductReview productReview)
        {
            await _productReviewRepository.UpdateAsync(productReview);
        }

        /// <summary>
        /// Updates a totals helpfulness count for product review
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task UpdateProductReviewHelpfulnessTotalsAsync(ProductReview productReview)
        {
            if (productReview is null)
                throw new ArgumentNullException(nameof(productReview));

            (productReview.HelpfulYesTotal, productReview.HelpfulNoTotal) = await GetHelpfulnessCountsAsync(productReview);

            await _productReviewRepository.UpdateAsync(productReview);
        }

        /// <summary>
        /// Check possibility added review for current customer
        /// </summary>
        /// <param name="productId">Current product</param>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the 
        /// </returns>
        public virtual async Task<bool> CanAddReviewAsync(int productId, int storeId = 0)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (_catalogSettings.OneReviewPerProductFromCustomer)
                return (await GetAllProductReviewsAsync(customerId: customer.Id, productId: productId, storeId: storeId)).TotalCount == 0;

            return true;
        }

        #endregion

        #region Product warehouses

        /// <summary>
        /// Get a product warehouse-inventory records by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IList<ProductWarehouseInventory>> GetAllProductWarehouseInventoryRecordsAsync(int productId)
        {
            return await _productWarehouseInventoryRepository.GetAllAsync(query => query.Where(pwi => pwi.ProductId == productId));
        }

        /// <summary>
        /// Deletes a record to manage product inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage product inventory per warehouse</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductWarehouseInventoryAsync(ProductWarehouseInventory pwi)
        {
            await _productWarehouseInventoryRepository.DeleteAsync(pwi);
        }

        /// <summary>
        /// Inserts a record to manage product inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage product inventory per warehouse</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertProductWarehouseInventoryAsync(ProductWarehouseInventory pwi)
        {
            await _productWarehouseInventoryRepository.InsertAsync(pwi);
        }

        /// <summary>
        /// Updates a record to manage product inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage product inventory per warehouse</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductWarehouseInventoryAsync(ProductWarehouseInventory pwi)
        {
            await _productWarehouseInventoryRepository.UpdateAsync(pwi);
        }

        /// <summary>
        /// Updates a records to manage product inventory per warehouse
        /// </summary>
        /// <param name="pwis">Records to manage product inventory per warehouse</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductWarehouseInventoryAsync(IList<ProductWarehouseInventory> pwis)
        {
            await _productWarehouseInventoryRepository.UpdateAsync(pwis);
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
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddStockQuantityHistoryEntryAsync(Product product, int quantityAdjustment, int stockQuantity,
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

            await _stockQuantityHistoryRepository.InsertAsync(historyEntry);
        }

        /// <summary>
        /// Get the history of the product stock quantity changes
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="warehouseId">Warehouse identifier; pass 0 to load all entries</param>
        /// <param name="combinationId">Product attribute combination identifier; pass 0 to load all entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of stock quantity change entries
        /// </returns>
        public virtual async Task<IPagedList<StockQuantityHistory>> GetStockQuantityHistoryAsync(Product product, int warehouseId = 0, int combinationId = 0,
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

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        #endregion

        #region Product discounts

        /// <summary>
        /// Clean up product references for a specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ClearDiscountProductMappingAsync(Discount discount)
        {
            if (discount is null)
                throw new ArgumentNullException(nameof(discount));

            var mappingsWithProducts =
                from dcm in _discountProductMappingRepository.Table
                join p in _productRepository.Table on dcm.EntityId equals p.Id
                where dcm.DiscountId == discount.Id
                select new { product = p, dcm };

            var mappingsToDelete = new List<DiscountProductMapping>();
            await foreach (var pdcm in mappingsWithProducts.ToAsyncEnumerable())
            {
                mappingsToDelete.Add(pdcm.dcm);

                //update "HasDiscountsApplied" property
                await UpdateHasDiscountsAppliedAsync(pdcm.product);
            }
            await _discountProductMappingRepository.DeleteAsync(mappingsToDelete);
        }

        /// <summary>
        /// Get a discount-product mapping records by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IList<DiscountProductMapping>> GetAllDiscountsAppliedToProductAsync(int productId)
        {
            return await _discountProductMappingRepository.GetAllAsync(query => query.Where(dcm => dcm.EntityId == productId));
        }

        /// <summary>
        /// Get a discount-product mapping record
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<DiscountProductMapping> GetDiscountAppliedToProductAsync(int productId, int discountId)
        {
            return await _discountProductMappingRepository.Table
                .FirstOrDefaultAsync(dcm => dcm.EntityId == productId && dcm.DiscountId == discountId);
        }

        /// <summary>
        /// Inserts a discount-product mapping record
        /// </summary>
        /// <param name="discountProductMapping">Discount-product mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertDiscountProductMappingAsync(DiscountProductMapping discountProductMapping)
        {
            await _discountProductMappingRepository.InsertAsync(discountProductMapping);
        }

        /// <summary>
        /// Deletes a discount-product mapping record
        /// </summary>
        /// <param name="discountProductMapping">Discount-product mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteDiscountProductMappingAsync(DiscountProductMapping discountProductMapping)
        {
            await _discountProductMappingRepository.DeleteAsync(discountProductMapping);
        }

        #endregion

        #endregion
    }
}
