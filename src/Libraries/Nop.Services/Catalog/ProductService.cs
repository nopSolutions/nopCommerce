using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Messages;

namespace Nop.Services.Catalog
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
        private const string PRODUCTS_PATTERN_KEY = "Nop.product.";
        private const string PRODUCTVARIANTS_PATTERN_KEY = "Nop.productvariant.";
        private const string TIERPRICES_PATTERN_KEY = "Nop.tierprice.";
        #endregion

        #region Fields

        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductVariant> _productVariantRepository;
        private readonly IRepository<RelatedProduct> _relatedProductRepository;
        private readonly IRepository<CrossSellProduct> _crossSellProductRepository;
        private readonly IRepository<TierPrice> _tierPriceRepository;
        private readonly IRepository<ProductPicture> _productPictureRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ILanguageService _languageService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CommonSettings _commonSettings;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="productVariantRepository">Product variant repository</param>
        /// <param name="relatedProductRepository">Related product repository</param>
        /// <param name="crossSellProductRepository">Cross-sell product repository</param>
        /// <param name="tierPriceRepository">Tier price repository</param>
        /// <param name="localizedPropertyRepository">Localized property repository</param>
        /// <param name="productPictureRepository">Product picture repository</param>
        /// <param name="productAttributeService">Product attribute service</param>
        /// <param name="productAttributeParser">Product attribute parser service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="eventPublisher"></param>
        public ProductService(ICacheManager cacheManager,
            IRepository<Product> productRepository,
            IRepository<ProductVariant> productVariantRepository,
            IRepository<RelatedProduct> relatedProductRepository,
            IRepository<CrossSellProduct> crossSellProductRepository,
            IRepository<TierPrice> tierPriceRepository,
            IRepository<ProductPicture> productPictureRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            ILanguageService languageService,
            IWorkflowMessageService workflowMessageService,
            IDataProvider dataProvider, IDbContext dbContext,
            LocalizationSettings localizationSettings, CommonSettings commonSettings,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _relatedProductRepository = relatedProductRepository;
            _crossSellProductRepository = crossSellProductRepository;
            _tierPriceRepository = tierPriceRepository;
            _productPictureRepository = productPictureRepository;
            _localizedPropertyRepository = localizedPropertyRepository;
            _productAttributeService = productAttributeService;
            _productAttributeParser = productAttributeParser;
            _languageService = languageService;
            _workflowMessageService = workflowMessageService;
            _dataProvider = dataProvider;
            _dbContext = dbContext;
            _localizationSettings = localizationSettings;
            _commonSettings = commonSettings;
            _eventPublisher = eventPublisher;
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
                throw new ArgumentNullException("product");

            product.Deleted = true;
            //delete product
            UpdateProduct(product);

            //delete product variants
            foreach (var productVariant in product.ProductVariants)
                DeleteProductVariant(productVariant);
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product collection</returns>
        public virtual IList<Product> GetAllProducts(bool showHidden = false)
        {
            var query = from p in _productRepository.Table
                        orderby p.Name
                        where (showHidden || p.Published) &&
                        !p.Deleted
                        select p;
            var products = query.ToList();
            return products;
        }

        /// <summary>
        /// Gets all products displayed on the home page
        /// </summary>
        /// <returns>Product collection</returns>
        public virtual IList<Product> GetAllProductsDisplayedOnHomePage()
        {
            var query = from p in _productRepository.Table
                        orderby p.Name
                        where p.Published &&
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
        public virtual Product GetProductById(int productId)
        {
            if (productId == 0)
                return null;

            string key = string.Format(PRODUCTS_BY_ID_KEY, productId);
            return _cacheManager.Get(key, () =>
            {
                var product = _productRepository.GetById(productId);
                return product;
            });
        }
        
        /// <summary>
        /// Inserts a product
        /// </summary>
        /// <param name="product">Product</param>
        public virtual void InsertProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            _productRepository.Insert(product);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);

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
                throw new ArgumentNullException("product");

            _productRepository.Update(product);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(product);
        }
         
        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="categoryId">Category identifier; 0 to load all recordss</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="featuredProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
        /// <param name="priceMin">Minimum price; null to load all records</param>
        /// <param name="priceMax">Maximum price; null to load all records</param>
        /// <param name="productTagId">Product tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search in descriptions</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="filteredSpecs">Filtered product specification identifiers</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product collection</returns>
        public virtual IPagedList<Product> SearchProducts(int categoryId, int manufacturerId, bool? featuredProducts,
            decimal? priceMin, decimal? priceMax, int productTagId,
            string keywords, bool searchDescriptions, int languageId,
            IList<int> filteredSpecs, ProductSortingEnum orderBy,
            int pageIndex, int pageSize, bool showHidden = false)
        {
            bool searchLocalizedValue = false;
            if (languageId > 0)
            {
                if (showHidden)
                {
                    searchLocalizedValue = true;
                }
                else
                {
                    //ensure that we have at least two published languages
                    var totalPublishedLanguages = _languageService.GetAllLanguages(false).Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
                }
            }

            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database. 
                //It's much faster than the LINQ implementation below 

                //TODO We should not reference SqlParameter here. DAL should do it.
                var pTotalRecords = new SqlParameter { ParameterName = "TotalRecords", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int };

                string commaSeparatedSpecIds = "";
                if (filteredSpecs != null)
                {
                    ((List<int>)filteredSpecs).Sort();
                    for (int i = 0; i < filteredSpecs.Count; i++)
                    {
                        commaSeparatedSpecIds += filteredSpecs[i].ToString();
                        if (i != filteredSpecs.Count - 1)
                        {
                            commaSeparatedSpecIds += ",";
                        }
                    }
                }

                //some databases don't support int.MaxValue
                if (pageSize == int.MaxValue)
                    pageSize = int.MaxValue - 1;

                var products = _dbContext.ExecuteStoredProcedureList<Product>(
                    //"EXEC [ProductLoadAllPaged] @CategoryId, @ManufacturerId, @ProductTagId, @FeaturedProducts, @PriceMin, @PriceMax, @Keywords, @SearchDescriptions, @FilteredSpecs, @LanguageId, @OrderBy, @PageIndex, @PageSize, @ShowHidden, @TotalRecords",
                    "ProductLoadAllPaged",
                    new SqlParameter { ParameterName = "CategoryId", Value = categoryId, SqlDbType = SqlDbType.Int },
                    new SqlParameter { ParameterName = "ManufacturerId", Value = manufacturerId, SqlDbType = SqlDbType.Int },
                    new SqlParameter { ParameterName = "ProductTagId", Value = productTagId, SqlDbType = SqlDbType.Int },
                    new SqlParameter { ParameterName = "FeaturedProducts", Value = featuredProducts.HasValue ? (object)featuredProducts.Value : DBNull.Value, SqlDbType = SqlDbType.Bit },
                    new SqlParameter { ParameterName = "PriceMin", Value = priceMin.HasValue ? (object)priceMin.Value : DBNull.Value, SqlDbType = SqlDbType.Decimal },
                    new SqlParameter { ParameterName = "PriceMax", Value = priceMax.HasValue ? (object)priceMax.Value : DBNull.Value, SqlDbType = SqlDbType.Decimal },
                    new SqlParameter { ParameterName = "Keywords", Value = keywords != null ? (object)keywords : DBNull.Value, SqlDbType = SqlDbType.NVarChar },
                    new SqlParameter { ParameterName = "SearchDescriptions", Value = searchDescriptions, SqlDbType = SqlDbType.Bit },
                    new SqlParameter { ParameterName = "FilteredSpecs", Value = commaSeparatedSpecIds != null ? (object)commaSeparatedSpecIds : DBNull.Value, SqlDbType = SqlDbType.NVarChar },
                    new SqlParameter { ParameterName = "LanguageId", Value = searchLocalizedValue ? languageId : 0, SqlDbType = SqlDbType.Int },
                    new SqlParameter { ParameterName = "OrderBy", Value = (int)orderBy, SqlDbType = SqlDbType.Int },
                    new SqlParameter { ParameterName = "PageIndex", Value = pageIndex, SqlDbType = SqlDbType.Int },
                    new SqlParameter { ParameterName = "PageSize", Value = pageSize, SqlDbType = SqlDbType.Int },
                    new SqlParameter { ParameterName = "ShowHidden", Value = showHidden, SqlDbType = SqlDbType.Bit },
                    pTotalRecords);
                int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
                return new PagedList<Product>(products, pageIndex, pageSize, totalRecords);
            }
            else
            {
                //stored procedures aren't supported. Use LINQ

                #region Search products

                //products
                var query = _productRepository.Table;
                query = query.Where(p => !p.Deleted);
                if (!showHidden)
                {
                    query = query.Where(p => p.Published);
                }

                //searching by keyword
                if (!String.IsNullOrWhiteSpace(keywords))
                {
                    query = from p in query
                            join lp in _localizedPropertyRepository.Table on p.Id equals lp.EntityId into p_lp
                            from lp in p_lp.DefaultIfEmpty()
                            from pv in p.ProductVariants.DefaultIfEmpty()
                            where (p.Name.Contains(keywords)) ||
                                  (searchDescriptions && p.ShortDescription.Contains(keywords)) ||
                                  (searchDescriptions && p.FullDescription.Contains(keywords)) ||
                                  (pv.Name.Contains(keywords)) ||
                                  (searchDescriptions && pv.Description.Contains(keywords)) ||
                                  //localized values
                                  (searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "Name" && lp.LocaleValue.Contains(keywords)) ||
                                  (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "ShortDescription" && lp.LocaleValue.Contains(keywords)) ||
                                  (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "FullDescription" && lp.LocaleValue.Contains(keywords))
                            select p;
                }

                //product variants
                //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
                //That's why we pass the date value
                var nowUtc = DateTime.UtcNow;
                query = from p in query
                        from pv in p.ProductVariants.DefaultIfEmpty()
                        where (showHidden || !pv.Deleted) &&
                              (showHidden || pv.Published) &&
                              (!priceMin.HasValue || pv.Price >= priceMin.Value) &&
                              (!priceMax.HasValue || pv.Price <= priceMax.Value) &&
                              (showHidden ||
                               (!pv.AvailableStartDateTimeUtc.HasValue || pv.AvailableStartDateTimeUtc.Value < nowUtc)) &&
                              (showHidden ||
                               (!pv.AvailableEndDateTimeUtc.HasValue || pv.AvailableEndDateTimeUtc.Value > nowUtc))
                        select p;


                //search by specs
                if (filteredSpecs != null && filteredSpecs.Count > 0)
                {
                    query = from p in query
                            where !filteredSpecs
                                       .Except(
                                           p.ProductSpecificationAttributes.Where(psa => psa.AllowFiltering).Select(
                                               psa => psa.SpecificationAttributeOptionId))
                                       .Any()
                            select p;
                }

                //category filtering
                if (categoryId > 0)
                {
                    query = from p in query
                            from pc in p.ProductCategories.Where(pc => pc.CategoryId == categoryId)
                            where (!featuredProducts.HasValue || featuredProducts.Value == pc.IsFeaturedProduct)
                            select p;
                }

                //manufacturer filtering
                if (manufacturerId > 0)
                {
                    query = from p in query
                            from pm in p.ProductManufacturers.Where(pm => pm.ManufacturerId == manufacturerId)
                            where (!featuredProducts.HasValue || featuredProducts.Value == pm.IsFeaturedProduct)
                            select p;
                }

                //related products filtering
                //if (relatedToProductId > 0)
                //{
                //    query = from p in query
                //            join rp in _relatedProductRepository.Table on p.Id equals rp.ProductId2
                //            where (relatedToProductId == rp.ProductId1)
                //            select p;
                //}

                //tag filtering
                if (productTagId > 0)
                {
                    query = from p in query
                            from pt in p.ProductTags.Where(pt => pt.Id == productTagId)
                            select p;
                }

                //only distinct products (group by ID)
                //if we use standard Distinct() method, then all fields will be compared (low performance)
                //it'll not work in SQL Server Compact when searching products by a keyword)
                query = from p in query
                        group p by p.Id
                            into pGroup
                            orderby pGroup.Key
                            select pGroup.FirstOrDefault();

                //sort products
                if (orderBy == ProductSortingEnum.Position && categoryId > 0)
                {
                    //category position
                    //query = query.OrderBy(p => p.ProductCategories.FirstOrDefault().DisplayOrder);

                    //category position
                    query =
                        query.OrderBy(
                            p =>
                            p.ProductCategories.Where(pc => pc.CategoryId == categoryId).FirstOrDefault().DisplayOrder);
                }
                else if (orderBy == ProductSortingEnum.Position && manufacturerId > 0)
                {
                    //manufacturer position
                    //query = query.OrderBy(p => p.ProductManufacturers.FirstOrDefault().DisplayOrder);

                    //manufacturer position
                    query =
                        query.OrderBy(
                            p =>
                            p.ProductManufacturers.Where(pm => pm.ManufacturerId == manufacturerId).FirstOrDefault().
                                DisplayOrder);
                }
                //else if (orderBy == ProductSortingEnum.Position && relatedToProductId > 0)
                //{
                //    //sort by related product display order
                //    query = from p in query
                //            join rp in _relatedProductRepository.Table on p.Id equals rp.ProductId2
                //            where (relatedToProductId == rp.ProductId1)
                //            orderby rp.DisplayOrder
                //            select p;
                //}
                else if (orderBy == ProductSortingEnum.Position)
                {
                    query = query.OrderBy(p => p.Name);
                }
                else if (orderBy == ProductSortingEnum.Name)
                {
                    query = query.OrderBy(p => p.Name);
                }
                else if (orderBy == ProductSortingEnum.Price)
                {
                    query = query.OrderBy(p => p.ProductVariants.FirstOrDefault().Price);
                }
                else if (orderBy == ProductSortingEnum.CreatedOn)
                    query = query.OrderByDescending(p => p.CreatedOnUtc);
                else
                    query = query.OrderBy(p => p.Name);

                var products = new PagedList<Product>(query, pageIndex, pageSize);
                return products;

                #endregion
            }
        }

        /// <summary>
        /// Update product review totals
        /// </summary>
        /// <param name="product">Product</param>
        public virtual void UpdateProductReviewTotals(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            int approvedRatingSum = 0;
            int notApprovedRatingSum = 0; 
            int approvedTotalReviews = 0;
            int notApprovedTotalReviews = 0;
            var reviews = product.ProductReviews;
            foreach (var pr in reviews)
            {
                if (pr.IsApproved)
                {
                    approvedRatingSum += pr.Rating;
                    approvedTotalReviews ++;
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

        #endregion

        #region Product variants

        /// <summary>
        /// Search product variants
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product variants</returns>
        public virtual IPagedList<ProductVariant> SearchProductVariants(int pageIndex, int pageSize, bool showHidden = false)
        {
            //product variants
            var query = _productVariantRepository.Table;
            query = query.Where(pv => !pv.Deleted);
            if (!showHidden)
            {
                query = query.Where(pv => pv.Published);
            }

            //products
            query = from pv in query
                    where (showHidden || !pv.Product.Deleted) &&
                          (showHidden || pv.Product.Published)
                    select pv;

            //only distinct products (group by ID)
            //if we use standard Distinct() method, then all fields will be compared (low performance)
            //query = from pv in query
            //        group pv by pv.Id
            //        into pGroup
            //        orderby pGroup.Key
            //        select pGroup.FirstOrDefault();

            query = query.OrderBy(pv => pv.Product.Name);
            var productVariants = new PagedList<ProductVariant>(query, pageIndex, pageSize);
            return productVariants;
        }

        /// <summary>
        /// Get low stock product variants
        /// </summary>
        /// <returns>Result</returns>
        public virtual IList<ProductVariant> GetLowStockProductVariants()
        {
            //Track inventory for product variant
            var query1 = from pv in _productVariantRepository.Table
                         orderby pv.MinStockQuantity
                         where !pv.Deleted &&
                         pv.ManageInventoryMethodId == (int)ManageInventoryMethod.ManageStock &&
                         pv.MinStockQuantity >= pv.StockQuantity
                         select pv;
            var productVariants1 = query1.ToList();
            //Track inventory for product variant by product attributes
            var query2 = from pv in _productVariantRepository.Table
                         from pvac in pv.ProductVariantAttributeCombinations
                         where !pv.Deleted &&
                         pv.ManageInventoryMethodId == (int)ManageInventoryMethod.ManageStockByAttributes &&
                         pvac.StockQuantity <= 0
                         select pv;
            //only distinct products (group by ID)
            //if we use standard Distinct() method, then all fields will be compared (low performance)
            query2 = from pv in query2
                    group pv by pv.Id into pGroup
                    orderby pGroup.Key
                    select pGroup.FirstOrDefault();
            var productVariants2 = query2.ToList();

            var result = new List<ProductVariant>();
            result.AddRange(productVariants1);
            result.AddRange(productVariants2);
            return result;
        }
        
        /// <summary>
        /// Gets a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Product variant</returns>
        public virtual ProductVariant GetProductVariantById(int productVariantId)
        {
            if (productVariantId == 0)
                return null;

            string key = string.Format(PRODUCTVARIANTS_BY_ID_KEY, productVariantId);
            return _cacheManager.Get(key, () =>
            {
                var pv = _productVariantRepository.GetById(productVariantId);
                return pv;
            });
        }

        /// <summary>
        /// Gets a product variant by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Product variant</returns>
        public virtual ProductVariant GetProductVariantBySku(string sku)
        {
            if (String.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from pv in _productVariantRepository.Table
                        orderby pv.DisplayOrder, pv.Id
                        where !pv.Deleted &&
                        pv.Sku == sku
                        select pv;
            var productVariant = query.FirstOrDefault();
            return productVariant;
        }
        
        /// <summary>
        /// Inserts a product variant
        /// </summary>
        /// <param name="productVariant">The product variant</param>
        public virtual void InsertProductVariant(ProductVariant productVariant)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            _productVariantRepository.Insert(productVariant);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productVariant);
        }

        /// <summary>
        /// Updates the product variant
        /// </summary>
        /// <param name="productVariant">The product variant</param>
        public virtual void UpdateProductVariant(ProductVariant productVariant)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            _productVariantRepository.Update(productVariant);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productVariant);
        }
        
        /// <summary>
        /// Gets product variants by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product variant collection</returns>
        public virtual IList<ProductVariant> GetProductVariantsByProductId(int productId, bool showHidden = false)
        {
            string key = string.Format(PRODUCTVARIANTS_ALL_KEY, showHidden, productId);
            return _cacheManager.Get(key, () =>
            {
                var query = (IQueryable<ProductVariant>)_productVariantRepository.Table;
                if (!showHidden)
                {
                    query = query.Where(pv => pv.Published);
                }
                if (!showHidden)
                {
                    //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
                    //That's why we pass the date value
                    var nowUtc = DateTime.UtcNow;
                    query = query.Where(pv =>
                            !pv.AvailableStartDateTimeUtc.HasValue ||
                            pv.AvailableStartDateTimeUtc <= nowUtc);
                    query = query.Where(pv =>
                            !pv.AvailableEndDateTimeUtc.HasValue ||
                            pv.AvailableEndDateTimeUtc >= nowUtc);
                }
                query = query.Where(pv => !pv.Deleted);
                query = query.Where(pv => pv.ProductId == productId);
                query = query.OrderBy(pv => pv.DisplayOrder);

                var productVariants = query.ToList();
                return productVariants;
            });
        }

        /// <summary>
        /// Delete a product variant
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        public virtual void DeleteProductVariant(ProductVariant productVariant)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            productVariant.Deleted = true;
            UpdateProductVariant(productVariant);
        }


        /// <summary>
        /// Adjusts inventory
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="decrease">A value indicating whether to increase or descrease product variant stock quantity</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        public virtual void AdjustInventory(ProductVariant productVariant, bool decrease,
            int quantity, string attributesXml)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            switch (productVariant.ManageInventoryMethod)
            {
                case ManageInventoryMethod.DontManageStock:
                    {
                        //do nothing
                        return;
                    }
                case ManageInventoryMethod.ManageStock:
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
                                    case LowStockActivity.DisableBuyButton:
                                        newDisableBuyButton = true;
                                        break;
                                    case LowStockActivity.Unpublish:
                                        newPublished = false;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        productVariant.StockQuantity = newStockQuantity;
                        productVariant.DisableBuyButton = newDisableBuyButton;
                        productVariant.Published = newPublished;
                        UpdateProductVariant(productVariant);

                        //send email notification
                        if (decrease && productVariant.NotifyAdminForQuantityBelow > newStockQuantity)
                            _workflowMessageService.SendQuantityBelowStoreOwnerNotification(productVariant, _localizationSettings.DefaultAdminLanguageId);
                        
                        if (decrease)
                        {
                            var product = productVariant.Product;
                            bool allProductVariantsUnpublished = true;
                            foreach (var pv2 in GetProductVariantsByProductId(product.Id))
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
                case ManageInventoryMethod.ManageStockByAttributes:
                    {
                        var combination = _productAttributeParser.FindProductVariantAttributeCombination(productVariant, attributesXml);
                        if (combination != null)
                        {
                            int newStockQuantity = 0;
                            if (decrease)
                                newStockQuantity = combination.StockQuantity - quantity;
                            else
                                newStockQuantity = combination.StockQuantity + quantity;

                            combination.StockQuantity = newStockQuantity;
                            _productAttributeService.UpdateProductVariantAttributeCombination(combination);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Search product variants
        /// </summary>
        /// <param name="categoryId">Category identifier; 0 to load all recordss</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search in descriptions</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product variants</returns>
        public virtual IPagedList<ProductVariant> SearchProductVariants(int categoryId, int manufacturerId,
             string keywords, bool searchDescriptions, int pageIndex, int pageSize, bool showHidden = false)
        {
            //products
            var query = _productVariantRepository.Table;
            query = query.Where(pv => !pv.Deleted);
            if (!showHidden)
            {
                query = query.Where(pv => pv.Published);
            }
            query = query.Where(pv => !pv.Product.Deleted);
            if (!showHidden)
            {
                query = query.Where(pv => pv.Product.Published);
            }

            //searching by keyword
            if (!String.IsNullOrWhiteSpace(keywords))
            {
                query = from pv in query
                        where (pv.Product.Name.Contains(keywords)) ||
                        (searchDescriptions && pv.Product.ShortDescription.Contains(keywords)) ||
                        (searchDescriptions && pv.Product.FullDescription.Contains(keywords)) ||
                        (pv.Name.Contains(keywords)) ||
                        (searchDescriptions && pv.Description.Contains(keywords))
                        select pv;
            }

            //category filtering
            if (categoryId > 0)
            {
                query = from pv in query
                        from pc in pv.Product.ProductCategories.Where(pc => pc.CategoryId == categoryId)
                        select pv;
            }

            //manufacturer filtering
            if (manufacturerId > 0)
            {
                query = from pv in query
                        from pm in pv.Product.ProductManufacturers.Where(pm => pm.ManufacturerId == manufacturerId)
                        select pv;
            }

            //only distinct products (group by ID)
            //if we use standard Distinct() method, then all fields will be compared (low performance)
            //it'll not work in SQl Server Compact when searching products by a keyword)
            query = from pv in query
                    group pv by pv.Id into pvGroup
                    orderby pvGroup.Key
                    select pvGroup.FirstOrDefault();

            query = query.OrderBy(pv => pv.Product.Name);
            var productVariants = new PagedList<ProductVariant>(query, pageIndex, pageSize);
            return productVariants;
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
                throw new ArgumentNullException("relatedProduct");

            _relatedProductRepository.Delete(relatedProduct);

            //event notification
            _eventPublisher.EntityDeleted(relatedProduct);
        }

        /// <summary>
        /// Gets a related product collection by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Related product collection</returns>
        public virtual IList<RelatedProduct> GetRelatedProductsByProductId1(int productId1, bool showHidden = false)
        {
            var query = from rp in _relatedProductRepository.Table
                        join p in _productRepository.Table on rp.ProductId2 equals p.Id
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
        public virtual RelatedProduct GetRelatedProductById(int relatedProductId)
        {
            if (relatedProductId == 0)
                return null;
            
            var relatedProduct = _relatedProductRepository.GetById(relatedProductId);
            return relatedProduct;
        }

        /// <summary>
        /// Inserts a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        public virtual void InsertRelatedProduct(RelatedProduct relatedProduct)
        {
            if (relatedProduct == null)
                throw new ArgumentNullException("relatedProduct");

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
                throw new ArgumentNullException("relatedProduct");

            _relatedProductRepository.Update(relatedProduct);

            //event notification
            _eventPublisher.EntityUpdated(relatedProduct);
        }

        #endregion

        #region Cross-sell products

        /// <summary>
        /// Deletes a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell identifer</param>
        public virtual void DeleteCrossSellProduct(CrossSellProduct crossSellProduct)
        {
            if (crossSellProduct == null)
                throw new ArgumentNullException("crossSellProduct");

            _crossSellProductRepository.Delete(crossSellProduct);

            //event notification
            _eventPublisher.EntityDeleted(crossSellProduct);
        }

        /// <summary>
        /// Gets a cross-sell product collection by product identifier
        /// </summary>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Cross-sell product collection</returns>
        public virtual IList<CrossSellProduct> GetCrossSellProductsByProductId1(int productId1, bool showHidden = false)
        {
            var query = from csp in _crossSellProductRepository.Table
                        join p in _productRepository.Table on csp.ProductId2 equals p.Id
                        where csp.ProductId1 == productId1 &&
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
        /// <param name="crossSellProductId">Cross-sell product identifer</param>
        /// <returns>Cross-sell product</returns>
        public virtual CrossSellProduct GetCrossSellProductById(int crossSellProductId)
        {
            if (crossSellProductId == 0)
                return null;

            var crossSellProduct = _crossSellProductRepository.GetById(crossSellProductId);
            return crossSellProduct;
        }

        /// <summary>
        /// Inserts a cross-sell product
        /// </summary>
        /// <param name="crossSellProduct">Cross-sell product</param>
        public virtual void InsertCrossSellProduct(CrossSellProduct crossSellProduct)
        {
            if (crossSellProduct == null)
                throw new ArgumentNullException("crossSellProduct");

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
                throw new ArgumentNullException("crossSellProduct");

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

            if (cart == null || cart.Count == 0)
                return result;

            var cartProductIds = new List<int>();
            foreach (var sci in cart)
            {
                int prodId = sci.ProductVariant.ProductId;
                if (!cartProductIds.Contains(prodId))
                    cartProductIds.Add(prodId);
            }

            for (int i = 0; i < cart.Count; i++)
            {
                var sci = cart[i];
                var crossSells = GetCrossSellProductsByProductId1(sci.ProductVariant.ProductId);
                foreach (var crossSell in crossSells)
                {
                    //TODO create a helper method to validate product availability (dates, quantity) etc


                    //validate that this product is not added to result yet
                    //validate that this product is not in the cart
                    if (result.Find(p => p.Id == crossSell.ProductId2) == null &&
                        !cartProductIds.Contains(crossSell.ProductId2))
                    {
                        var productToAdd = GetProductById(crossSell.ProductId2);
                        result.Add(productToAdd);
                        if (result.Count >= numberOfProducts)
                            return result;
                    }
                }
            }
            return result;
        }
        #endregion
        
        #region Tier prices
        
        /// <summary>
        /// Deletes a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        public virtual void DeleteTierPrice(TierPrice tierPrice)
        {
            if (tierPrice == null)
                throw new ArgumentNullException("tierPrice");

            _tierPriceRepository.Delete(tierPrice);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);

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
            
            var tierPrice = _tierPriceRepository.GetById(tierPriceId);
            return tierPrice;
        }

        /// <summary>
        /// Gets tier prices by product variant identifier
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Tier price collection</returns>
        public virtual IList<TierPrice> GetTierPricesByProductVariantId(int productVariantId)
        {
            if (productVariantId == 0)
                return new List<TierPrice>();

            string key = string.Format(TIERPRICES_ALLBYPRODUCTVARIANTID_KEY, productVariantId);
            return _cacheManager.Get(key, () =>
            {
                var query = from tp in _tierPriceRepository.Table
                            orderby tp.Quantity
                            where tp.ProductVariantId == productVariantId
                            select tp;
                var tierPrices = query.ToList();
                return tierPrices;
            });
        }

        /// <summary>
        /// Inserts a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        public virtual void InsertTierPrice(TierPrice tierPrice)
        {
            if (tierPrice == null)
                throw new ArgumentNullException("tierPrice");

            _tierPriceRepository.Insert(tierPrice);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);

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
                throw new ArgumentNullException("tierPrice");

            _tierPriceRepository.Update(tierPrice);

            _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TIERPRICES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(tierPrice);
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
                throw new ArgumentNullException("productPicture");

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
                        orderby pp.DisplayOrder
                        select pp;
            var productPictures = query.ToList();
            return productPictures;
        }

        /// <summary>
        /// Gets a product picture
        /// </summary>
        /// <param name="productPictureId">Product picture identifer</param>
        /// <returns>Product picture</returns>
        public virtual ProductPicture GetProductPictureById(int productPictureId)
        {
            if (productPictureId == 0)
                return null;

            var pp = _productPictureRepository.GetById(productPictureId);
            return pp;
        }

        /// <summary>
        /// Inserts a product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        public virtual void InsertProductPicture(ProductPicture productPicture)
        {
            if (productPicture == null)
                throw new ArgumentNullException("productPicture");

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
                throw new ArgumentNullException("productPicture");

            _productPictureRepository.Update(productPicture);

            //event notification
            _eventPublisher.EntityUpdated(productPicture);
        }

        #endregion

        #endregion
    }
}
