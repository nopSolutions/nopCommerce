using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Caching.CachingDefaults;
using Nop.Services.Caching.Extensions;
using Nop.Services.Customers;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Manufacturer service
    /// </summary>
    public partial class ManufacturerService : IManufacturerService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<DiscountManufacturerMapping> _discountManufacturerMappingRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ManufacturerService(CatalogSettings catalogSettings,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IRepository<AclRecord> aclRepository,
            IRepository<DiscountManufacturerMapping> discountManufacturerMappingRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Product> productRepository,
            IRepository<ProductManufacturer> productManufacturerRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _aclRepository = aclRepository;
            _discountManufacturerMappingRepository = discountManufacturerMappingRepository;
            _manufacturerRepository = manufacturerRepository;
            _productRepository = productRepository;
            _productManufacturerRepository = productManufacturerRepository;
            _storeMappingRepository = storeMappingRepository;
            _storeContext = storeContext;
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

            if (!mappings.Any())
                return;

            _discountManufacturerMappingRepository.Delete(mappings);
        }

        /// <summary>
        /// Deletes a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public virtual void DeleteManufacturer(Manufacturer manufacturer)
        {
            if (manufacturer == null)
                throw new ArgumentNullException(nameof(manufacturer));

            manufacturer.Deleted = true;
            UpdateManufacturer(manufacturer);

            //event notification
            _eventPublisher.EntityDeleted(manufacturer);
        }

        /// <summary>
        /// Delete manufacturers
        /// </summary>
        /// <param name="manufacturers">Manufacturers</param>
        public virtual void DeleteManufacturers(IList<Manufacturer> manufacturers)
        {
            if (manufacturers == null)
                throw new ArgumentNullException(nameof(manufacturers));

            foreach (var manufacturer in manufacturers)
            {
                DeleteManufacturer(manufacturer);
            }
        }

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <param name="manufacturerName">Manufacturer name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Manufacturers</returns>
        public virtual IPagedList<Manufacturer> GetAllManufacturers(string manufacturerName = "",
            int storeId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false)
        {
            var query = _manufacturerRepository.Table;
            if (!showHidden)
                query = query.Where(m => m.Published);
            if (!string.IsNullOrWhiteSpace(manufacturerName))
                query = query.Where(m => m.Name.Contains(manufacturerName));
            query = query.Where(m => !m.Deleted);
            query = query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Id);

            if ((storeId <= 0 || _catalogSettings.IgnoreStoreLimitations) && (showHidden || _catalogSettings.IgnoreAcl))
                return new PagedList<Manufacturer>(query, pageIndex, pageSize);

            if (!showHidden && !_catalogSettings.IgnoreAcl)
            {
                //ACL (access control list)
                var allowedCustomerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);
                query = from m in query
                        join acl in _aclRepository.Table
                            on new { c1 = m.Id, c2 = nameof(Manufacturer) } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into m_acl
                        from acl in m_acl.DefaultIfEmpty()
                        where !m.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select m;
            }

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                //store mapping
                query = from m in query
                        join sm in _storeMappingRepository.Table
                            on new { c1 = m.Id, c2 = nameof(Manufacturer) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into m_sm
                        from sm in m_sm.DefaultIfEmpty()
                        where !m.LimitedToStores || storeId == sm.StoreId
                        select m;
            }

            query = query.Distinct().OrderBy(m => m.DisplayOrder).ThenBy(m => m.Id);

            return new PagedList<Manufacturer>(query, pageIndex, pageSize);
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

            var discountId = discount.Id;
            var cacheKey = string.Format(NopDiscountCachingDefaults.DiscountManufacturerIdsModelCacheKey,
                discountId,
                string.Join(",", _customerService.GetCustomerRoleIds(customer)),
                _storeContext.CurrentStore.Id);

            var result = _discountManufacturerMappingRepository.Table.Where(dmm => dmm.DiscountId == discountId)
                .Select(dmm => dmm.EntityId).ToCachedList(cacheKey);

            return result;
        }

        /// <summary>
        /// Gets a manufacturer
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>Manufacturer</returns>
        public virtual Manufacturer GetManufacturerById(int manufacturerId)
        {
            if (manufacturerId == 0)
                return null;

            var key = string.Format(NopCatalogCachingDefaults.ManufacturersByIdCacheKey, manufacturerId);

            return _manufacturerRepository.ToCachedGetById(manufacturerId, key);
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
        public virtual List<Manufacturer> GetManufacturersByIds(int[] manufacturerIds)
        {
            if (manufacturerIds == null || manufacturerIds.Length == 0)
                return new List<Manufacturer>();

            var query = from p in _manufacturerRepository.Table
                        where manufacturerIds.Contains(p.Id) && !p.Deleted
                        select p;

            return query.ToList();
        }

        /// <summary>
        /// Inserts a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public virtual void InsertManufacturer(Manufacturer manufacturer)
        {
            if (manufacturer == null)
                throw new ArgumentNullException(nameof(manufacturer));

            _manufacturerRepository.Insert(manufacturer);
            
            //event notification
            _eventPublisher.EntityInserted(manufacturer);
        }

        /// <summary>
        /// Updates the manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public virtual void UpdateManufacturer(Manufacturer manufacturer)
        {
            if (manufacturer == null)
                throw new ArgumentNullException(nameof(manufacturer));

            _manufacturerRepository.Update(manufacturer);
            
            //event notification
            _eventPublisher.EntityUpdated(manufacturer);
        }

        /// <summary>
        /// Deletes a product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        public virtual void DeleteProductManufacturer(ProductManufacturer productManufacturer)
        {
            if (productManufacturer == null)
                throw new ArgumentNullException(nameof(productManufacturer));

            _productManufacturerRepository.Delete(productManufacturer);
            
            //event notification
            _eventPublisher.EntityDeleted(productManufacturer);
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

            var key = string.Format(NopCatalogCachingDefaults.ProductManufacturersAllByManufacturerIdCacheKey, showHidden,
                manufacturerId, pageIndex, pageSize, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);

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

                if (!_catalogSettings.IgnoreStoreLimitations)
                {
                    //store mapping
                    var currentStoreId = _storeContext.CurrentStore.Id;
                    query = from pm in query
                        join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                        join sm in _storeMappingRepository.Table
                            on new
                            {
                                c1 = m.Id,
                                c2 = nameof(Manufacturer)
                            } 
                            equals new
                            {
                                c1 = sm.EntityId,
                                c2 = sm.EntityName
                            } 
                            into m_sm
                        from sm in m_sm.DefaultIfEmpty()
                        where !m.LimitedToStores || currentStoreId == sm.StoreId
                        select pm;
                }

                query = query.Distinct().OrderBy(pm => pm.DisplayOrder).ThenBy(pm => pm.Id);
            }

            var productManufacturers = query.ToCachedPagedList(key, pageIndex, pageSize);

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

            var key = string.Format(NopCatalogCachingDefaults.ProductManufacturersAllByProductIdCacheKey, showHidden,
                productId, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);

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

                if (!_catalogSettings.IgnoreStoreLimitations)
                {
                    //store mapping
                    var currentStoreId = _storeContext.CurrentStore.Id;
                    query = from pm in query
                        join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                        join sm in _storeMappingRepository.Table
                            on new
                            {
                                c1 = m.Id,
                                c2 = nameof(Manufacturer)
                            } 
                            equals new
                            {
                                c1 = sm.EntityId,
                                c2 = sm.EntityName
                            } 
                            into m_sm
                        from sm in m_sm.DefaultIfEmpty()
                        where !m.LimitedToStores || currentStoreId == sm.StoreId
                        select pm;
                }

                query = query.Distinct().OrderBy(pm => pm.DisplayOrder).ThenBy(pm => pm.Id);
            }

            var productManufacturers = query.ToCachedList(key);

            return productManufacturers;
        }

        /// <summary>
        /// Gets a product manufacturer mapping 
        /// </summary>
        /// <param name="productManufacturerId">Product manufacturer mapping identifier</param>
        /// <returns>Product manufacturer mapping</returns>
        public virtual ProductManufacturer GetProductManufacturerById(int productManufacturerId)
        {
            if (productManufacturerId == 0)
                return null;

            return _productManufacturerRepository.ToCachedGetById(productManufacturerId);
        }

        /// <summary>
        /// Inserts a product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        public virtual void InsertProductManufacturer(ProductManufacturer productManufacturer)
        {
            if (productManufacturer == null)
                throw new ArgumentNullException(nameof(productManufacturer));

            _productManufacturerRepository.Insert(productManufacturer);
            
            //event notification
            _eventPublisher.EntityInserted(productManufacturer);
        }

        /// <summary>
        /// Updates the product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        public virtual void UpdateProductManufacturer(ProductManufacturer productManufacturer)
        {
            if (productManufacturer == null)
                throw new ArgumentNullException(nameof(productManufacturer));

            _productManufacturerRepository.Update(productManufacturer);
            
            //event notification
            _eventPublisher.EntityUpdated(productManufacturer);
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
            if (discountManufacturerMapping is null)
                throw new ArgumentNullException(nameof(discountManufacturerMapping));

            _discountManufacturerMappingRepository.Insert(discountManufacturerMapping);

            //event notification
            _eventPublisher.EntityInserted(discountManufacturerMapping);
        }

        /// <summary>
        /// Deletes a discount-manufacturer mapping record
        /// </summary>
        /// <param name="discountManufacturerMapping">Discount-manufacturer mapping</param>
        public void DeleteDiscountManufacturerMapping(DiscountManufacturerMapping discountManufacturerMapping)
        {
            if (discountManufacturerMapping is null)
                throw new ArgumentNullException(nameof(discountManufacturerMapping));

            _discountManufacturerMappingRepository.Delete(discountManufacturerMapping);

            //event notification
            _eventPublisher.EntityDeleted(discountManufacturerMapping);
        }

        #endregion
    }
}