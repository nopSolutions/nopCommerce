using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Services.Customers;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Manufacturer service
    /// </summary>
    public partial class ManufacturerService : IManufacturerService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer ID
        /// </remarks>
        private const string MANUFACTURERS_BY_ID_KEY = "Nop.manufacturer.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : manufacturer ID
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        private const string PRODUCTMANUFACTURERS_ALLBYMANUFACTURERID_KEY = "Nop.productmanufacturer.allbymanufacturerid-{0}-{1}-{2}-{3}-{4}-{5}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : product ID
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        private const string PRODUCTMANUFACTURERS_ALLBYPRODUCTID_KEY = "Nop.productmanufacturer.allbyproductid-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string MANUFACTURERS_PATTERN_KEY = "Nop.manufacturer.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTMANUFACTURERS_PATTERN_KEY = "Nop.productmanufacturer.";

        #endregion

        #region Fields

        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="manufacturerRepository">Category repository</param>
        /// <param name="productManufacturerRepository">ProductCategory repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event published</param>
        public ManufacturerService(ICacheManager cacheManager,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<ProductManufacturer> productManufacturerRepository,
            IRepository<Product> productRepository,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IWorkContext workContext,
            IStoreContext storeContext,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._manufacturerRepository = manufacturerRepository;
            this._productManufacturerRepository = productManufacturerRepository;
            this._productRepository = productRepository;
            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Deletes a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public virtual void DeleteManufacturer(Manufacturer manufacturer)
        {
            if (manufacturer == null)
                throw new ArgumentNullException("manufacturer");
            
            manufacturer.Deleted = true;
            UpdateManufacturer(manufacturer);
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
            if (!String.IsNullOrWhiteSpace(manufacturerName))
                query = query.Where(m => m.Name.Contains(manufacturerName));
            query = query.Where(m => !m.Deleted);
            query = query.OrderBy(m => m.DisplayOrder);

            if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
            {
                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                    query = from m in query
                            join acl in _aclRepository.Table
                            on new { c1 = m.Id, c2 = "Manufacturer" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into m_acl
                            from acl in m_acl.DefaultIfEmpty()
                            where !m.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select m;
                }
                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    query = from m in query
                            join sm in _storeMappingRepository.Table
                            on new { c1 = m.Id, c2 = "Manufacturer" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into m_sm
                            from sm in m_sm.DefaultIfEmpty()
                            where !m.LimitedToStores || storeId == sm.StoreId
                            select m;
                }
                //only distinct manufacturers (group by ID)
                query = from m in query
                        group m by m.Id
                            into mGroup
                            orderby mGroup.Key
                            select mGroup.FirstOrDefault();
                query = query.OrderBy(m => m.DisplayOrder);
            }

            return new PagedList<Manufacturer>(query, pageIndex, pageSize);
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
            
            string key = string.Format(MANUFACTURERS_BY_ID_KEY, manufacturerId);
            return _cacheManager.Get(key, () => _manufacturerRepository.GetById(manufacturerId));
        }

        /// <summary>
        /// Inserts a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        public virtual void InsertManufacturer(Manufacturer manufacturer)
        {
            if (manufacturer == null)
                throw new ArgumentNullException("manufacturer");

            _manufacturerRepository.Insert(manufacturer);

            //cache
            _cacheManager.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);

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
                throw new ArgumentNullException("manufacturer");

            _manufacturerRepository.Update(manufacturer);

            //cache
            _cacheManager.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);

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
                throw new ArgumentNullException("productManufacturer");

            _productManufacturerRepository.Delete(productManufacturer);

            //cache
            _cacheManager.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);

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

            string key = string.Format(PRODUCTMANUFACTURERS_ALLBYMANUFACTURERID_KEY, showHidden, manufacturerId, pageIndex, pageSize, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from pm in _productManufacturerRepository.Table
                            join p in _productRepository.Table on pm.ProductId equals p.Id
                            where pm.ManufacturerId == manufacturerId &&
                                  !p.Deleted &&
                                  (showHidden || p.Published)
                            orderby pm.DisplayOrder
                            select pm;

                if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from pm in query
                                join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                                join acl in _aclRepository.Table
                                on new { c1 = m.Id, c2 = "Manufacturer" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into m_acl
                                from acl in m_acl.DefaultIfEmpty()
                                where !m.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select pm;
                    }
                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from pm in query
                                join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                                join sm in _storeMappingRepository.Table
                                on new { c1 = m.Id, c2 = "Manufacturer" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into m_sm
                                from sm in m_sm.DefaultIfEmpty()
                                where !m.LimitedToStores || currentStoreId == sm.StoreId
                                select pm;
                    }

                    //only distinct manufacturers (group by ID)
                    query = from pm in query
                            group pm by pm.Id
                            into pmGroup
                            orderby pmGroup.Key
                            select pmGroup.FirstOrDefault();
                    query = query.OrderBy(pm => pm.DisplayOrder);
                }

                var productManufacturers = new PagedList<ProductManufacturer>(query, pageIndex, pageSize);
                return productManufacturers;
            });
        }

        /// <summary>
        /// Gets a product manufacturer mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product manufacturer mapping collection</returns>
        public virtual IList<ProductManufacturer> GetProductManufacturersByProductId(int productId, bool showHidden = false)
        {
            if (productId == 0)
                return new List<ProductManufacturer>();

            string key = string.Format(PRODUCTMANUFACTURERS_ALLBYPRODUCTID_KEY, showHidden, productId, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from pm in _productManufacturerRepository.Table
                            join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                            where pm.ProductId == productId &&
                                !m.Deleted &&
                                (showHidden || m.Published)
                            orderby pm.DisplayOrder
                            select pm;


                if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from pm in query
                                join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                                join acl in _aclRepository.Table
                                on new { c1 = m.Id, c2 = "Manufacturer" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into m_acl
                                from acl in m_acl.DefaultIfEmpty()
                                where !m.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select pm;
                    }

                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from pm in query
                                join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                                join sm in _storeMappingRepository.Table
                                on new { c1 = m.Id, c2 = "Manufacturer" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into m_sm
                                from sm in m_sm.DefaultIfEmpty()
                                where !m.LimitedToStores || currentStoreId == sm.StoreId
                                select pm;
                    }

                    //only distinct manufacturers (group by ID)
                    query = from pm in query
                            group pm by pm.Id
                            into mGroup
                            orderby mGroup.Key
                            select mGroup.FirstOrDefault();
                    query = query.OrderBy(pm => pm.DisplayOrder);
                }

                var productManufacturers = query.ToList();
                return productManufacturers;
            });
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

            return _productManufacturerRepository.GetById(productManufacturerId);
        }

        /// <summary>
        /// Inserts a product manufacturer mapping
        /// </summary>
        /// <param name="productManufacturer">Product manufacturer mapping</param>
        public virtual void InsertProductManufacturer(ProductManufacturer productManufacturer)
        {
            if (productManufacturer == null)
                throw new ArgumentNullException("productManufacturer");

            _productManufacturerRepository.Insert(productManufacturer);

            //cache
            _cacheManager.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);

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
                throw new ArgumentNullException("productManufacturer");

            _productManufacturerRepository.Update(productManufacturer);

            //cache
            _cacheManager.RemoveByPattern(MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTMANUFACTURERS_PATTERN_KEY);

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
                .Select(p => new {p.ProductId, p.ManufacturerId}).ToList()
                .GroupBy(a => a.ProductId)
                .ToDictionary(items => items.Key, items => items.Select(a => a.ManufacturerId).ToArray());
        }


        /// <summary>
        /// Returns a list of names of not existing manufacturers
        /// </summary>
        /// <param name="manufacturerNames">The names of the manufacturers to check</param>
        /// <returns>List of names not existing manufacturers</returns>
        public virtual string[] GetNotExistingManufacturers(string[] manufacturerNames)
        {
            if (manufacturerNames == null)
                throw new ArgumentNullException("manufacturerNames");

            var query = _manufacturerRepository.Table;
            var queryFilter = manufacturerNames.Distinct().ToArray();
            var filter = query.Select(m => m.Name).Where(m => queryFilter.Contains(m)).ToList();
            return queryFilter.Except(filter).ToArray();
        }

        #endregion
    }
}
