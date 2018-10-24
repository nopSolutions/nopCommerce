using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;

namespace Nop.Plugin.Data.MySQL.Services.Catalog
{
    public class ProductService : Nop.Services.Catalog.ProductService
    {
        #region Fileds

        private readonly CatalogSettings _catalogSettings;
        private readonly IDbContext _dbContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ProductService(CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            IAclService aclService,
            ICacheManager cacheManager,
            IDataProvider dataProvider,
            IDateRangeService dateRangeService,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IRepository<AclRecord> aclRepository,
            IRepository<CrossSellProduct> crossSellProductRepository,
            IRepository<Product> productRepository,
            IRepository<ProductPicture> productPictureRepository,
            IRepository<ProductReview> productReviewRepository,
            IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
            IRepository<RelatedProduct> relatedProductRepository,
            IRepository<StockQuantityHistory> stockQuantityHistoryRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IRepository<TierPrice> tierPriceRepository,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            LocalizationSettings localizationSettings)
            : base(catalogSettings,
                commonSettings,
                aclService,
                cacheManager,
                dataProvider,
                dateRangeService,
                dbContext,
                eventPublisher,
                languageService,
                localizationService,
                productAttributeParser,
                productAttributeService,
                aclRepository,
                crossSellProductRepository,
                productRepository,
                productPictureRepository,
                productReviewRepository,
                productWarehouseInventoryRepository,
                relatedProductRepository,
                stockQuantityHistoryRepository,
                storeMappingRepository,
                tierPriceRepository,
                storeMappingService,
                workContext,
                localizationSettings)
        {
            _catalogSettings = catalogSettings;
            _dbContext = dbContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

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
        public override IPagedList<Product> SearchProducts(
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

            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            //Access control list. Allowed customer roles
            var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();

            //pass category identifiers as comma-delimited string
            var commaSeparatedCategoryIds = categoryIds == null ? string.Empty : string.Join(",", categoryIds);

            //pass customer role identifiers as comma-delimited string
            var commaSeparatedAllowedCustomerRoleIds = string.Join(",", allowedCustomerRolesIds);

            //some databases don't support int.MaxValue
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            var query = new StringBuilder();
            query.AppendLine(@"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ;
SELECT p.* 
FROM Product p");

            var filter = new StringBuilder();
            filter.AppendLine(" WHERE p.Deleted = 0");

            if (!string.IsNullOrEmpty(keywords))
            {
                filter.AppendLine($"AND (p.Name LIKE '%{keywords}%'");

                if (searchSku) filter.Append($" OR p.Sku = '{keywords}'");

                filter.Append(")");
            }

            if (categoryIds != null && categoryIds.Any())
            {
                query.AppendLine("INNER JOIN Product_Category_Mapping pcm ON p.Id = pcm.ProductId");
                filter.AppendLine($"AND pcm.CategoryId IN ({commaSeparatedCategoryIds})");

                if (featuredProducts.HasValue)
                    filter.AppendLine($"AND pcm.IsFeaturedProduct = {(featuredProducts.Value ? 1 : 0)}");
            }

            if (manufacturerId > 0)
            {
                query.AppendLine("INNER JOIN Product_Manufacturer_Mapping pmm ON p.Id = pmm.ProductId");
                filter.AppendLine($"AND pmm.ManufacturerId = {manufacturerId}");

                if (featuredProducts.HasValue)
                    filter.AppendLine($"AND pmm.IsFeaturedProduct = {(featuredProducts.Value ? 1 : 0)}");
            }

            if (vendorId > 0) filter.AppendLine($"AND p.VendorId = {vendorId}");

            if (warehouseId > 0)
            {
                filter.AppendLine($@"AND  
			    (
				    (p.UseMultipleWarehouses = 0 AND p.WarehouseId = {warehouseId})
				    OR
				    (p.UseMultipleWarehouses > 0 AND EXISTS (SELECT 1 FROM ProductWarehouseInventory pwi
					    WHERE pwi.WarehouseId = {warehouseId} AND pwi.ProductId = p.Id))
			    )");
            }

            if (productType != null) filter.AppendLine($"AND p.ProductTypeId = {(int)productType}");

            if (visibleIndividuallyOnly) filter.AppendLine("AND p.VisibleIndividually = 1");

            if (markedAsNewOnly) filter.AppendLine(@"AND p.MarkAsNew = 1
		        AND (UTC_TIMESTAMP() BETWEEN IFNULL(p.MarkAsNewStartDateTimeUtc, '1900-1-1') and IFNULL(p.MarkAsNewEndDateTimeUtc, '2999-1-1'))");

            if (productTagId != 0)
            {
                query.AppendLine("INNER JOIN Product_ProductTag_Mapping pptm ON p.Id = pptm.Product_Id");
                filter.AppendLine($"AND pptm.ProductTag_Id = {productTagId}");
            }

            if (overridePublished == null && !showHidden) filter.AppendLine("AND p.Published = 1");
            else if (overridePublished.HasValue && overridePublished.Value) filter.AppendLine("AND p.Published = 1");
            else if (overridePublished.HasValue && !overridePublished.Value) filter.AppendLine("AND p.Published = 0");

            if (!showHidden)
            {
                filter.AppendLine(@"AND p.Deleted = 0
                    AND(UTC_TIMESTAMP() BETWEEN IFNULL(p.AvailableStartDateTimeUtc, '1900-1-1') and IFNULL(p.AvailableEndDateTimeUtc, '2999-1-1'))");
            }

            if (priceMin != null) filter.AppendLine($"AND p.Price >= {priceMin}");
            if (priceMax != null) filter.AppendLine($"AND p.Price <= {priceMax}");

            if (!_catalogSettings.IgnoreAcl && !showHidden && allowedCustomerRolesIds != null && allowedCustomerRolesIds.Any())
            {
                filter.AppendLine($@"AND (p.SubjectToAcl = 0 OR 
                    EXISTS (
			            SELECT 1
					    FROM AclRecord acl
					    WHERE acl.EntityId = p.Id AND acl.EntityName = 'Product' 
                            AND acl.CustomerRoleId IN ({commaSeparatedAllowedCustomerRoleIds})
			        ))");
            }

            if (!_catalogSettings.IgnoreStoreLimitations && storeId > 0)
            {
                filter.AppendLine($@"AND (p.LimitedToStores = 0 OR EXISTS (
			        SELECT 1 FROM StoreMapping sm
			        WHERE sm.EntityId = p.Id AND sm.EntityName = 'Product' and sm.StoreId={storeId}
			        ))");
            }

            var orderBySql = string.Empty;
            switch (orderBy)
            {
                case ProductSortingEnum.NameAsc:
                    orderBySql = " p.Name ASC";
                    break;
                case ProductSortingEnum.NameDesc:
                    orderBySql = " p.Name DESC";
                    break;
                case ProductSortingEnum.PriceAsc:
                    orderBySql = " p.Price ASC";
                    break;
                case ProductSortingEnum.PriceDesc:
                    orderBySql = " p.Price ASC";
                    break;
                case ProductSortingEnum.CreatedOn:
                    orderBySql = " p.CreatedOnUtc DESC";
                    break;
                default:
                    if (categoryIds?.Count > 0) orderBySql = " pcm.DisplayOrder ASC";
                    if (manufacturerId > 0)
                    {
                        if (!string.IsNullOrEmpty(orderBySql)) orderBySql = orderBySql + ", ";
                        orderBySql = orderBySql + " pmm.DisplayOrder ASC";
                    }
                    if (!string.IsNullOrEmpty(orderBySql)) orderBySql = orderBySql + ", ";
                    orderBySql = orderBySql + " p.Name ASC";
                    break;
            }

            filter.AppendLine($"ORDER BY {orderBySql}");

            query.Append(filter);
            query.Append(";");
            query.AppendLine("COMMIT ;");

            var queryable = _dbContext.EntityFromSql<Product>(query.ToString()).AsNoTracking();
            var totalRecords = queryable.Count();
            var products = queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            //return products
            return new PagedList<Product>(products, pageIndex, pageSize, totalRecords);
        }
        
        #endregion
    }
}
