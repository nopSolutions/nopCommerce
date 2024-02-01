using LinqToDB;
using LinqToDB.Mapping;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Catalog
{
    public partial interface IProductService
    {
        Task<IPagedList<ProductCustom>> ProductsByShoppingCartTypeAsync(IList<int> productIds = null, int customerId = 0,
           ShoppingCartType? shoppingCartType = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<ProductCustom>> SearchProductsCustomAsync(
            IList<int> filterableSpecificationAttributeOptionIds,
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
            bool? overridePublished = null);

        Task<IPagedList<ProductCustom>> SearchProductsSimpleCustomAsync(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> productIds = null,
            int customerId = 0,
            int profileTypeId = 0,
            ProductSortingEnum orderBy = ProductSortingEnum.Position);

        Task<IList<Product>> GetRelatedProductsByProductId(int specificationAttributeId, int productId);


    }

    public partial class ProductService
    {
        //dependencis
        private IRepository<GenericAttribute> _genericAttributeRepository;
        private IRepository<ShoppingCartItem> _sciRepository;
        private IRepository<Customer> _customerRepository;
        private IOrderService _orderService;
        private IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        private IRepository<SpecificationAttribute> _specificationAttributeRepository;

        private void ResolveDependencies()
        {
            //resolve the dependencies using engine context

            if (_genericAttributeRepository == null)
                _genericAttributeRepository = EngineContext.Current.Resolve<IRepository<GenericAttribute>>();

            if (_sciRepository == null)
                _sciRepository = EngineContext.Current.Resolve<IRepository<ShoppingCartItem>>();

            if (_customerRepository == null)
                _customerRepository = EngineContext.Current.Resolve<IRepository<Customer>>();

            if (_specificationAttributeOptionRepository == null)
                _specificationAttributeOptionRepository = EngineContext.Current.Resolve<IRepository<SpecificationAttributeOption>>();

            if (_specificationAttributeRepository == null)
                _specificationAttributeRepository = EngineContext.Current.Resolve<IRepository<SpecificationAttribute>>();

            if (_orderService == null)
                _orderService = EngineContext.Current.Resolve<IOrderService>();
        }

        #region Methods

        public virtual async Task<IPagedList<ProductCustom>> ProductsByShoppingCartTypeAsync(IList<int> productIds = null, int customerId = 0,
           ShoppingCartType? shoppingCartType = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            ProductSortingEnum orderBy = ProductSortingEnum.Position;
            var products = new List<ProductCustom>();

            //validate "productIds" parameter
            if (productIds != null && productIds.Contains(0))
                productIds.Remove(0);

            //pass category identifiers as comma-delimited string
            var commaSeparatedProductIds = productIds == null ? string.Empty : string.Join(",", productIds);

            //some databases don't support int.MaxValue
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            //prepare input parameters
            var pProductIds = SqlParameterHelper.GetStringParameter("ProductIds", commaSeparatedProductIds);
            var pCustomerId = SqlParameterHelper.GetInt32Parameter("CustomerId", customerId);
            var pShoppingCartTypeId = SqlParameterHelper.GetInt32Parameter("ShoppingCartTypeId", (int?)shoppingCartType);
            var pOrderBy = SqlParameterHelper.GetInt32Parameter("OrderBy", (int)orderBy);
            var pPageIndex = SqlParameterHelper.GetInt32Parameter("PageIndex", pageIndex);
            var pPageSize = SqlParameterHelper.GetInt32Parameter("PageSize", pageSize);

            //prepare output parameters
            var pTotalRecords = SqlParameterHelper.GetOutputInt32Parameter("TotalRecords");

            //invoke stored procedure
            if (shoppingCartType == ShoppingCartType.Wishlist)
            {
                products = (await _productRepository.EntityFromSqlCustom("ProductShortList",
                   pProductIds,
                   pCustomerId,
                   pShoppingCartTypeId,
                   pOrderBy,
                   pPageIndex,
                   pPageSize,
                   pTotalRecords)).ToList();
            }
            if (shoppingCartType == ShoppingCartType.ShortListedMe)
            {
                products = (await _productRepository.EntityFromSqlCustom("ProductShortList",
                   pProductIds,
                   pCustomerId,
                   pShoppingCartTypeId,
                   pOrderBy,
                   pPageIndex,
                   pPageSize,
                   pTotalRecords)).ToList();
            }

            //return products
            var totalRecords = pTotalRecords.Value != DBNull.Value ? Convert.ToInt32(pTotalRecords.Value) : 0;

            var productsPaged = await products.ToPagedListAsync<ProductCustom>(pageIndex, pageSize, false);

            return productsPaged;
        }

        public virtual async Task<IPagedList<ProductCustom>> SearchProductsCustomAsync(
            IList<int> filterableSpecificationAttributeOptionIds,
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
                    var totalPublishedLanguages = (await _languageService.GetAllLanguagesAsync()).Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
                }
            }

            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            //Access control list. Allowed customer roles
            var allowedCustomerRolesIds = await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync());

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
            var pUseFullTextSearch = SqlParameterHelper.GetBooleanParameter("UseFullTextSearch", false);
            var pFullTextMode = SqlParameterHelper.GetInt32Parameter("FullTextMode", 0);
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
            var products = await _productRepository.EntityFromSqlCustom("ProductLoadAllPaged_V4",
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
                //pUseFullTextSearch,
                //pFullTextMode,
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
                pTotalRecords);

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

            var productsPaged = await products.ToPagedListAsync<ProductCustom>(pageIndex, pageSize, false);
            return productsPaged;
        }

        public virtual async Task<IPagedList<ProductCustom>> SearchProductsSimpleCustomAsync(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> productIds = null,
            int customerId = 0,
            int profileTypeId = 0,
            ProductSortingEnum orderBy = ProductSortingEnum.Position)
        {

            ResolveDependencies();

            //validate "productIds" parameter
            if (productIds != null && productIds.Contains(0))
                productIds.Remove(0);

            //pass category identifiers as comma-delimited string
            var commaSeparatedProductIds = productIds == null ? string.Empty : string.Join(",", productIds);

            //some databases don't support int.MaxValue
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            //customization start 
            //to remove stored proc (ProductLoadAllPaged_V5) implementation

            //var customerTempQuery = GetCustomerTempTable(productIds);
            //var customerOrderTempQuery = _orderService.GetCustomerOrderTempTable();
            //var productSpecsTempQuery = GetProductSpecsTempTable(productIds);

            //customization end 

            //prepare input parameters
            var pProductIds = SqlParameterHelper.GetStringParameter("productIds", commaSeparatedProductIds);
            var pCustomerId = SqlParameterHelper.GetInt32Parameter("CustomerId", customerId);
            var pProfileTypeId = SqlParameterHelper.GetInt32Parameter("ProfileTypeId", profileTypeId);

            //invoke stored procedure           
            var products = await _productRepository.EntityFromSqlCustom("ProductLoadAllPaged_V6",
                pProductIds,
                pCustomerId,
                pProfileTypeId);

            var productsPaged = await products.ToPagedListAsync<ProductCustom>(pageIndex, pageSize, false);
            return productsPaged;
        }

        public virtual IQueryable<CustomerTemp> GetCustomerTempTable(IList<int> productIds)
        {
            var productsQuery = _productRepository.Table;
            productsQuery = productsQuery.Where(p => productIds.Contains(p.Id));

            var customerTempQuery = from p in productsQuery
                                    from c in LinqToDB.LinqExtensions.LeftJoin(_customerRepository.Table, c => c.Id == p.VendorId)
                                    from ga in LinqToDB.LinqExtensions.LeftJoin(_genericAttributeRepository.Table, ga => ga.EntityId == c.Id)
                                    select new { p, c, ga };

            var columnList = new List<string> { "FirstName", "LastName", "Phone", "DOB", "Gender", "Company", "CountryId",
                                                "StateProvinceId", "LanguageId", "TimeZoneId", "AvatarPictureId", "CustomerProfileTypeId", "City"};

            customerTempQuery = customerTempQuery.Where(ct => columnList.Contains(ct.ga.Key));

            // #CustomerTemp table
            var customerTemp = customerTempQuery.Select(tmp => new CustomerTemp
            {
                Id = tmp.p.Id,
                Key = tmp.ga.Key,
                Value = tmp.ga.Value,
                LastLoginDateUtc = tmp.c.LastLoginDateUtc,
                LastActivityDateUtc = tmp.c.LastActivityDateUtc
            });

            return customerTemp;
        }

        public virtual IQueryable<ProductSpecsTemp> GetProductSpecsTempTable(IList<int> productIds)
        {
            var productSpecsTempQuery = from ps in _productSpecificationAttributeRepository.Table
                                        join p in _productRepository.Table on ps.ProductId equals p.Id
                                        join spao in _specificationAttributeOptionRepository.Table on ps.SpecificationAttributeOptionId equals spao.Id
                                        join spa in _specificationAttributeRepository.Table on spao.SpecificationAttributeId equals spa.Id
                                        select new { ps, p, spao, spa };


            var query = productSpecsTempQuery.GroupBy(row => new { row.p.Id, row.spa.Name })
                                             .Select(g => new
                                             {
                                                 Product = g.Key,
                                                 spa = g.Key.Name,
                                                 Technology = g.StringAggregate(" , ", x => x.spao.Name).ToValue()
                                             });
            // #ProductSpecsTemp table
            var productSpecsTemp = query.Select(tmp => new ProductSpecsTemp
            {
                ProductId = tmp.Product.Id,
                Name = tmp.spa,
                Technology = tmp.Technology
            });

            return productSpecsTemp;
        }

        public virtual async Task<IList<Product>> GetRelatedProductsByProductId(int specificationAttributeId, int productId)
        {
            ResolveDependencies();

            var currentProductSpecificationAttributeOptions = from product in _productRepository.Table
                                                              join psa in _productSpecificationAttributeRepository.Table on product.Id equals psa.ProductId
                                                              join spao in _specificationAttributeOptionRepository.Table on psa.SpecificationAttributeOptionId equals spao.Id
                                                              where
                                                              spao.SpecificationAttributeId == specificationAttributeId && psa.ProductId == productId
                                                              orderby product.Name
                                                              select spao;

            var query = from product in _productRepository.Table
                        join psa in _productSpecificationAttributeRepository.Table on product.Id equals psa.ProductId
                        where currentProductSpecificationAttributeOptions.Any(x => x.Id == psa.SpecificationAttributeOptionId)
                        && product.Id != productId // it should not be same product
                        select product;

            //get only distinct products
            query = query.Distinct();

            return await query.ToListAsync();
        }

        #endregion
    }

    [Table(Name = "CustomerTemp")]
    public class CustomerTemp
    {
        [Column(Name = "Id")]
        public int Id { get; set; }

        [Column(Name = "Key")]
        public string Key { get; set; }

        [Column(Name = "Value")]
        public string Value { get; set; }

        [Column(Name = "LastLoginDateUtc", CanBeNull = true)]
        public DateTime? LastLoginDateUtc { get; set; }

        [Column(Name = "LastActivityDateUtc", CanBeNull = true)]
        public DateTime? LastActivityDateUtc { get; set; }
    }

    [Table(Name = "CustomerOrderTemp")]
    public class CustomerOrderTemp
    {
        [Column(Name = "ProductId")]
        public int ProductId { get; set; }

        [Column(Name = "PremiumCustomer")]
        public bool PremiumCustomer { get; set; }
    }

    [Table(Name = "ProductSpecsTemp")]
    public class ProductSpecsTemp
    {
        [Column(Name = "ProductId")]
        public int ProductId { get; set; }

        [Column(Name = "Name")]
        public string Name { get; set; }

        [Column(Name = "Technology")]
        public string Technology { get; set; }
    }

    [Table(Name = "ProductSpecs")]
    public class ProductSpecs
    {
        [Column(Name = "ProductId")]
        public int ProductId { get; set; }

        [Column(Name = "PrimaryTechnology")]
        public string PrimaryTechnology { get; set; }

        [Column(Name = "SecondaryTechnology")]
        public string SecondaryTechnology { get; set; }

        [Column(Name = "CurrentAvalibility")]
        public string CurrentAvalibility { get; set; }

        [Column(Name = "ProfileType")]
        public string ProfileType { get; set; }

        [Column(Name = "MotherTongue")]
        public string MotherTongue { get; set; }

        [Column(Name = "WorkExperience")]
        public string WorkExperience { get; set; }
    }
}
