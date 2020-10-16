using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Data.DataProviders.SQL;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Manufacturer service
    /// </summary>
    public partial class ManufacturerService : IManufacturerService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<DiscountManufacturerMapping> _discountManufacturerMappingRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        protected readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ManufacturerService(CatalogSettings catalogSettings,
            IAclService aclService,
            ICustomerService customerService,
            IRepository<AclRecord> aclRepository,
            IRepository<DiscountManufacturerMapping> discountManufacturerMappingRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Product> productRepository,
            IRepository<ProductManufacturer> productManufacturerRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _customerService = customerService;
            _aclRepository = aclRepository;
            _discountManufacturerMappingRepository = discountManufacturerMappingRepository;
            _manufacturerRepository = manufacturerRepository;
            _productRepository = productRepository;
            _productManufacturerRepository = productManufacturerRepository;
            _storeMappingRepository = storeMappingRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clean up manufacturer references for a specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
        public virtual void ClearDiscountManufacturerMapping(Discount discount)
        {
            if (discount is null)
                throw new ArgumentNullException(nameof(discount));

            var mappings = _discountManufacturerMappingRepository.Table.Where(dcm => dcm.DiscountId == discount.Id);

            _discountManufacturerMappingRepository.Delete(mappings.ToList());
        }

        /// <summary>
        /// Deletes a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public virtual void DeleteManufacturer(Manufacturer manufacturer)
        {
            _manufacturerRepository.Delete(manufacturer);
        }

        /// <summary>
        /// Delete manufacturers
        /// </summary>
        /// <param name="manufacturers">Manufacturers</param>
        public virtual void DeleteManufacturers(IList<Manufacturer> manufacturers)
        {
            _manufacturerRepository.Delete(manufacturers);
        }

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <param name="manufacturerName">Manufacturer name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>Manufacturers</returns>
        public virtual IPagedList<Manufacturer> GetAllManufacturers(string manufacturerName = "",
            int storeId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false,
            bool? overridePublished = null)
        {
            return _manufacturerRepository.GetAllPaged(query =>
            {
                if (!showHidden)
                    query = query.Where(m => m.Published);
                if (!string.IsNullOrWhiteSpace(manufacturerName))
                    query = query.Where(m => m.Name.Contains(manufacturerName));
                query = query.Where(m => !m.Deleted);
                if (overridePublished.HasValue)
                    query = query.Where(m => m.Published == overridePublished.Value);

                query = query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Id);

                if ((storeId <= 0 || _catalogSettings.IgnoreStoreLimitations) &&
                    (showHidden || _catalogSettings.IgnoreAcl))
                    return query;

                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);
                    query = from m in query
                        join acl in _aclRepository.Table
                            on new {c1 = m.Id, c2 = nameof(Manufacturer)} equals new
                            {
                                c1 = acl.EntityId, c2 = acl.EntityName
                            } into m_acl
                        from acl in m_acl.DefaultIfEmpty()
                        where !m.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select m;
                }

                //store mapping
                if (!_catalogSettings.IgnoreStoreLimitations && _storeMappingService.IsEntityMappingExists<Manufacturer>(storeId))
                    query = query.Where(m => m.LimitedToStores(_storeMappingRepository.Table, storeId));

                return query.Distinct();
            }, pageIndex, pageSize);
        }

        /// <summary>
        /// Get manufacturer identifiers to which a discount is applied
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Manufacturer identifiers</returns>
        public virtual IList<int> GetAppliedManufacturerIds(Discount discount, Customer customer)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopDiscountDefaults.ManufacturerIdsByDiscountCacheKey, 
                discount,
                _customerService.GetCustomerRoleIds(customer),
                _storeContext.CurrentStore);

            var query = _discountManufacturerMappingRepository.Table.Where(dmm => dmm.DiscountId == discount.Id)
                .Select(dmm => dmm.EntityId);

            var result = _staticCacheManager.Get(cacheKey, query.ToList);

            return result;
        }

        /// <summary>
        /// Gets a manufacturer
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>Manufacturer</returns>
        public virtual Manufacturer GetManufacturerById(int manufacturerId)
        {
            return _manufacturerRepository.GetById(manufacturerId, cache => default);
        }

        /// <summary>
        /// Get manufacturers for which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted manufacturers</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of manufacturers</returns>
        public virtual IPagedList<Manufacturer> GetManufacturersWithAppliedDiscount(int? discountId = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var manufacturers = _manufacturerRepository.Table;

            if (discountId.HasValue)
                manufacturers = from manufacturer in manufacturers
                    join dmm in _discountManufacturerMappingRepository.Table on manufacturer.Id equals dmm.EntityId
                    where dmm.DiscountId == discountId.Value
                    select manufacturer;

            if (!showHidden)
                manufacturers = manufacturers.Where(manufacturer => !manufacturer.Deleted);

            manufacturers = manufacturers.OrderBy(manufacturer => manufacturer.DisplayOrder).ThenBy(manufacturer => manufacturer.Id);

            return new PagedList<Manufacturer>(manufacturers, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets manufacturers by identifier
        /// </summary>
        /// <param name="manufacturerIds">manufacturer identifiers</param>
        /// <returns>Manufacturers</returns>
        public virtual IList<Manufacturer> GetManufacturersByIds(int[] manufacturerIds)
        {
            return _manufacturerRepository.GetByIds(manufacturerIds);
        }

        /// <summary>
        /// Inserts a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public virtual void InsertManufacturer(Manufacturer manufacturer)
        {
            _manufacturerRepository.Insert(manufacturer);
        }

        /// <summary>
        /// Updates the manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public virtual void UpdateManufacturer(Manufacturer manufacturer)
        {
            _manufacturerRepository.Update(manufacturer);
        }

        /// <summary>
        /// Deletes a product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        public virtual void DeleteProductManufacturer(ProductManufacturer productManufacturer)
        {
            _productManufacturerRepository.Delete(productManufacturer);
        }

        /// <summary>
        /// Gets featured products by manufacturer identifier
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>List of featured products</returns>
        public virtual IList<Product> GetManufacturerFeaturedProducts(int manufacturerId, int storeId = 0)
        {
            List<Product> featuredProducts = new List<Product>();

            if (manufacturerId == 0)
                return featuredProducts;

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ManufacturerFeaturedProductIdsKey, manufacturerId, storeId);

            var featuredProductIds = _staticCacheManager.Get(cacheKey, () =>
            {
                var skipSroreMapping = _catalogSettings.IgnoreStoreLimitations || !_storeMappingService.IsEntityMappingExists<Product>(storeId);

                featuredProducts = (from p in _productRepository.Table
                                    join pm in _productManufacturerRepository.Table on p.Id equals pm.ProductId
                                    where p.VisibleIndividually && pm.IsFeaturedProduct && manufacturerId == pm.ManufacturerId &&
                                    (skipSroreMapping || p.LimitedToStores(_storeMappingRepository.Table, storeId))
                                    select p).ToList();

                return featuredProducts.Select(p => (p.Id, p.SubjectToAcl));
            }).Cast<(int EntityId, bool SubjectToAcl)>();

            if(featuredProducts.Count > 0)
                return featuredProducts.Where(p => _aclService.Authorize(p)).ToList();

            var authorizedIds = featuredProductIds.Where(fp => 
                    _aclService.Authorize(new Product { Id = fp.EntityId, SubjectToAcl = fp.SubjectToAcl }, _workContext.CurrentCustomer))
                    .Select(fp => fp.EntityId);

            if (authorizedIds?.Count() > 0)
                return _productRepository.Table.Where(p => authorizedIds.Contains(p.Id)).ToList();

            return featuredProducts;
        }

        /// <summary>
        /// Gets product manufacturer collection
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product manufacturer collection</returns>
        public virtual IPagedList<ProductManufacturer> GetProductManufacturersByManufacturerId(int manufacturerId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (manufacturerId == 0)
                return new PagedList<ProductManufacturer>(new List<ProductManufacturer>(), pageIndex, pageSize);

            var query = from pm in _productManufacturerRepository.Table
                join p in _productRepository.Table on pm.ProductId equals p.Id
                where pm.ManufacturerId == manufacturerId &&
                      !p.Deleted &&
                      (showHidden || p.Published)
                orderby pm.DisplayOrder, pm.Id
                select pm;

            if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
            {
                if (!_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);
                    query = from pm in query
                        join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                        join acl in _aclRepository.Table
                            on new
                            {
                                c1 = m.Id,
                                c2 = nameof(Manufacturer)
                            } 
                            equals new
                            {
                                c1 = acl.EntityId,
                                c2 = acl.EntityName
                            } 
                            into m_acl
                        from acl in m_acl.DefaultIfEmpty()
                        where !m.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select pm;
                }
                
                //store mapping
                var storeId = _storeContext.CurrentStore.Id;

                if (!_catalogSettings.IgnoreStoreLimitations && _storeMappingService.IsEntityMappingExists<Manufacturer>(storeId))
                {
                    query = from pm in query
                        join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                        where m.LimitedToStores(_storeMappingRepository.Table, storeId)
                        select pm;
                }

                query = query.Distinct();
            }

            var productManufacturers = new PagedList<ProductManufacturer>(query, pageIndex, pageSize);

            return productManufacturers;
        }

        /// <summary>
        /// Gets a product manufacturer mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product manufacturer mapping collection</returns>
        public virtual IList<ProductManufacturer> GetProductManufacturersByProductId(int productId,
            bool showHidden = false)
        {
            if (productId == 0)
                return new List<ProductManufacturer>();

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductManufacturersByProductCacheKey, productId,
                showHidden, _workContext.CurrentCustomer, _storeContext.CurrentStore);

            var query = from pm in _productManufacturerRepository.Table
                join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                where pm.ProductId == productId &&
                      !m.Deleted &&
                      (showHidden || m.Published)
                orderby pm.DisplayOrder, pm.Id
                select pm;

            if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
            {
                if (!_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);
                    query = from pm in query
                        join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                        join acl in _aclRepository.Table
                            on new
                            {
                                c1 = m.Id,
                                c2 = nameof(Manufacturer)
                            } 
                            equals new
                            {
                                c1 = acl.EntityId,
                                c2 = acl.EntityName
                            } 
                            into m_acl
                        from acl in m_acl.DefaultIfEmpty()
                        where !m.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select pm;
                }

                //store mapping
                var storeId = _storeContext.CurrentStore.Id;

                if (!_catalogSettings.IgnoreStoreLimitations && _storeMappingService.IsEntityMappingExists<Manufacturer>(storeId))
                {
                    query = from pm in query
                        join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                        where m.LimitedToStores(_storeMappingRepository.Table, storeId)
                        select pm;
                }

                query = query.Distinct();
            }

            var productManufacturers = _staticCacheManager.Get(key, query.ToList);

            return productManufacturers;
        }

        /// <summary>
        /// Gets a product manufacturer mapping 
        /// </summary>
        /// <param name="productManufacturerId">Product manufacturer mapping identifier</param>
        /// <returns>Product manufacturer mapping</returns>
        public virtual ProductManufacturer GetProductManufacturerById(int productManufacturerId)
        {
            return _productManufacturerRepository.GetById(productManufacturerId, cache => default);
        }

        /// <summary>
        /// Inserts a product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        public virtual void InsertProductManufacturer(ProductManufacturer productManufacturer)
        {
            _productManufacturerRepository.Insert(productManufacturer);
        }

        /// <summary>
        /// Updates the product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        public virtual void UpdateProductManufacturer(ProductManufacturer productManufacturer)
        {
            _productManufacturerRepository.Update(productManufacturer);
        }

        /// <summary>
        /// Get manufacturer IDs for products
        /// </summary>
        /// <param name="productIds">Products IDs</param>
        /// <returns>Manufacturer IDs for products</returns>
        public virtual IDictionary<int, int[]> GetProductManufacturerIds(int[] productIds)
        {
            var query = _productManufacturerRepository.Table;

            return query.Where(p => productIds.Contains(p.ProductId))
                .Select(p => new { p.ProductId, p.ManufacturerId }).ToList()
                .GroupBy(a => a.ProductId)
                .ToDictionary(items => items.Key, items => items.Select(a => a.ManufacturerId).ToArray());
        }

        /// <summary>
        /// Returns a list of names of not existing manufacturers
        /// </summary>
        /// <param name="manufacturerIdsNames">The names and/or IDs of the manufacturers to check</param>
        /// <returns>List of names and/or IDs not existing manufacturers</returns>
        public virtual string[] GetNotExistingManufacturers(string[] manufacturerIdsNames)
        {
            if (manufacturerIdsNames == null)
                throw new ArgumentNullException(nameof(manufacturerIdsNames));

            var query = _manufacturerRepository.Table;
            var queryFilter = manufacturerIdsNames.Distinct().ToArray();
            //filtering by name
            var filter = query.Select(m => m.Name).Where(m => queryFilter.Contains(m)).ToList();
            queryFilter = queryFilter.Except(filter).ToArray();

            //if some names not found
            if (!queryFilter.Any())
                return queryFilter.ToArray();

            //filtering by IDs
            filter = query.Select(c => c.Id.ToString()).Where(c => queryFilter.Contains(c)).ToList();
            queryFilter = queryFilter.Except(filter).ToArray();

            return queryFilter.ToArray();
        }

        /// <summary>
        /// Returns a ProductManufacturer that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>A ProductManufacturer that has the specified values; otherwise null</returns>
        public virtual ProductManufacturer FindProductManufacturer(IList<ProductManufacturer> source, int productId, int manufacturerId)
        {
            foreach (var productManufacturer in source)
                if (productManufacturer.ProductId == productId && productManufacturer.ManufacturerId == manufacturerId)
                    return productManufacturer;

            return null;
        }

        /// <summary>
        /// Get a discount-manufacturer mapping record
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Result</returns>
        public DiscountManufacturerMapping GetDiscountAppliedToManufacturer(int manufacturerId, int discountId)
        {
            return _discountManufacturerMappingRepository.Table.FirstOrDefault(dcm => dcm.EntityId == manufacturerId && dcm.DiscountId == discountId);
        }

        /// <summary>
        /// Inserts a discount-manufacturer mapping record
        /// </summary>
        /// <param name="discountManufacturerMapping">Discount-manufacturer mapping</param>
        public void InsertDiscountManufacturerMapping(DiscountManufacturerMapping discountManufacturerMapping)
        {
            _discountManufacturerMappingRepository.Insert(discountManufacturerMapping);
        }

        /// <summary>
        /// Deletes a discount-manufacturer mapping record
        /// </summary>
        /// <param name="discountManufacturerMapping">Discount-manufacturer mapping</param>
        public void DeleteDiscountManufacturerMapping(DiscountManufacturerMapping discountManufacturerMapping)
        {
            _discountManufacturerMappingRepository.Delete(discountManufacturerMapping);
        }

        #endregion
    }
}