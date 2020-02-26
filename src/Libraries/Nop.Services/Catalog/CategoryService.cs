using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Caching.CachingDefaults;
using Nop.Services.Caching.Extensions;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Category service
    /// </summary>
    public partial class CategoryService : ICategoryService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CommonSettings _commonSettings;
        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<DiscountCategoryMapping> _discountCategoryMappingRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CategoryService(CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            IAclService aclService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IRepository<AclRecord> aclRepository,
            IRepository<Category> categoryRepository,
            IRepository<DiscountCategoryMapping> discountCategoryMappingRepository,
            IRepository<Product> productRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _aclService = aclService;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _aclRepository = aclRepository;
            _categoryRepository = categoryRepository;
            _discountCategoryMappingRepository = discountCategoryMappingRepository;
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _storeMappingRepository = storeMappingRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clean up category references for a  specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
        public virtual void ClearDiscountCategoryMapping(Discount discount)
        {
            if (discount is null)
                throw new ArgumentNullException(nameof(discount));

            var mappings = _discountCategoryMappingRepository.Table.Where(dcm => dcm.DiscountId == discount.Id);

            if (!mappings.Any())
                return;

            _discountCategoryMappingRepository.Delete(mappings);
        }

        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="category">Category</param>
        public virtual void DeleteCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            category.Deleted = true;
            UpdateCategory(category);

            //event notification
            _eventPublisher.EntityDeleted(category);

            //reset a "Parent category" property of all child subcategories
            var subcategories = GetAllCategoriesByParentCategoryId(category.Id, true);
            foreach (var subcategory in subcategories)
            {
                subcategory.ParentCategoryId = 0;
                UpdateCategory(subcategory);
            }
        }

        /// <summary>
        /// Delete Categories
        /// </summary>
        /// <param name="categories">Categories</param>
        public virtual void DeleteCategories(IList<Category> categories)
        {
            if (categories == null)
                throw new ArgumentNullException(nameof(categories));

            foreach (var category in categories)
            {
                DeleteCategory(category);
            }
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IList<Category> GetAllCategories(int storeId = 0, bool showHidden = false)
        {
            var key = string.Format(NopCatalogCachingDefaults.CategoriesAllCacheKey,
                storeId,
                string.Join(",", _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer)),
                showHidden);

            var categories = _staticCacheManager.Get(key, () => GetAllCategories(string.Empty, storeId, showHidden: showHidden));

            return categories;
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IPagedList<Category> GetAllCategories(string categoryName, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (_commonSettings.UseStoredProcedureForLoadingCategories)
            {
                //stored procedures are enabled for loading categories and supported by the database. 
                //It's much faster with a large number of categories than the LINQ implementation below 

                //prepare parameters
                var showHiddenParameter = SqlParameterHelper.GetBooleanParameter("ShowHidden", showHidden);
                var nameParameter = SqlParameterHelper.GetStringParameter("Name", categoryName ?? string.Empty);
                var storeIdParameter = SqlParameterHelper.GetInt32Parameter("StoreId", !_catalogSettings.IgnoreStoreLimitations ? storeId : 0);
                var pageIndexParameter = SqlParameterHelper.GetInt32Parameter("PageIndex", pageIndex);
                var pageSizeParameter = SqlParameterHelper.GetInt32Parameter("PageSize", pageSize);
                //pass allowed customer role identifiers as comma-delimited string
                var customerRoleIdsParameter = SqlParameterHelper.GetStringParameter("CustomerRoleIds", !_catalogSettings.IgnoreAcl ? string.Join(",", _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer)) : string.Empty);

                var totalRecordsParameter = SqlParameterHelper.GetOutputInt32Parameter("TotalRecords");

                //invoke stored procedure
                var categories = _categoryRepository.EntityFromSql("CategoryLoadAllPaged",
                    showHiddenParameter, nameParameter, storeIdParameter, customerRoleIdsParameter,
                    pageIndexParameter, pageSizeParameter, totalRecordsParameter).ToList();

                var totalRecords = totalRecordsParameter.Value != DBNull.Value ? Convert.ToInt32(totalRecordsParameter.Value) : 0;

                //paging
                return new PagedList<Category>(categories, pageIndex, pageSize, totalRecords);
            }

            //don't use a stored procedure. Use LINQ
            var query = _categoryRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            if (!string.IsNullOrWhiteSpace(categoryName))
                query = query.Where(c => c.Name.Contains(categoryName));
            query = query.Where(c => !c.Deleted);
            query = query.OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);

            if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
            {
                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);
                    query = from c in query
                            join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = nameof(Category) } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                            from acl in c_acl.DefaultIfEmpty()
                            where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select c;
                }

                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    query = from c in query
                            join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = nameof(Category) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                            from sm in c_sm.DefaultIfEmpty()
                            where !c.LimitedToStores || storeId == sm.StoreId
                            select c;
                }

                query = query.Distinct().OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);
            }

            var unsortedCategories = query.ToList();

            //sort categories
            var sortedCategories = SortCategoriesForTree(unsortedCategories);

            //paging
            return new PagedList<Category>(sortedCategories, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IList<Category> GetAllCategoriesByParentCategoryId(int parentCategoryId,
            bool showHidden = false)
        {
            var key = string.Format(NopCatalogCachingDefaults.CategoriesByParentCategoryIdCacheKey, parentCategoryId,
                showHidden, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            
            var query = _categoryRepository.Table;

            if (!showHidden)
                query = query.Where(c => c.Published);

            query = query.Where(c => c.ParentCategoryId == parentCategoryId);
            query = query.Where(c => !c.Deleted);
            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);

            if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
            {
                if (!_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);
                    query = from c in query
                        join acl in _aclRepository.Table
                            on new
                            {
                                c1 = c.Id,
                                c2 = nameof(Category)
                            } 
                            equals new
                            {
                                c1 = acl.EntityId,
                                c2 = acl.EntityName
                            } 
                            into c_acl
                        from acl in c_acl.DefaultIfEmpty()
                        where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select c;
                }

                if (!_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    var currentStoreId = _storeContext.CurrentStore.Id;
                    query = from c in query
                        join sm in _storeMappingRepository.Table
                            on new
                            {
                                c1 = c.Id,
                                c2 = nameof(Category)
                            } 
                            equals new
                            {
                                c1 = sm.EntityId,
                                c2 = sm.EntityName
                            } 
                            into c_sm
                        from sm in c_sm.DefaultIfEmpty()
                        where !c.LimitedToStores || currentStoreId == sm.StoreId
                        select c;
                }

                query = query.Distinct().OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
            }

            var categories = query.ToCachedList(key);

            return categories;
        }

        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IList<Category> GetAllCategoriesDisplayedOnHomepage(bool showHidden = false)
        {
            var query = from c in _categoryRepository.Table
                        orderby c.DisplayOrder, c.Id
                        where c.Published &&
                        !c.Deleted &&
                        c.ShowOnHomepage
                        select c;

            var categories = query.ToCachedList(NopCatalogCachingDefaults.CategoriesAllDisplayedOnHomepageCacheKey);

            if (showHidden)
                return categories;

            var cacheKey =
                string.Format(NopCatalogCachingDefaults.CategoriesDisplayedOnHomepageWithoutHiddenCacheKey,
                    _storeContext.CurrentStore.Id,
                    string.Join(",", _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer)));
                
            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                return categories
                    .Where(c => _aclService.Authorize(c) && _storeMappingService.Authorize(c))
                    .ToList();
            });

            return result;
        }

        /// <summary>
        /// Get category identifiers to which a discount is applied
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Category identifiers</returns>
        public virtual IList<int> GetAppliedCategoryIds(Discount discount, Customer customer)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            var discountId = discount.Id;
            var cacheKey = string.Format(NopDiscountCachingDefaults.DiscountCategoryIdsModelCacheKey,
                discountId,
                string.Join(",", _customerService.GetCustomerRoleIds(customer)),
                _storeContext.CurrentStore.Id);

            var result = _staticCacheManager.Get(cacheKey, () =>
            {
                var ids = _discountCategoryMappingRepository.Table.Where(dmm => dmm.DiscountId == discountId).Select(dmm => dmm.EntityId).Distinct().ToList();

                if (!discount.AppliedToSubCategories)
                    return ids;

                ids.AddRange(ids.SelectMany(categoryId => GetChildCategoryIds(categoryId, _storeContext.CurrentStore.Id)));

                return ids.Distinct().ToList();
            });

            return result;
        }

        /// <summary>
        /// Gets child category identifiers
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category identifiers</returns>
        public virtual IList<int> GetChildCategoryIds(int parentCategoryId, int storeId = 0, bool showHidden = false)
        {
            var cacheKey = string.Format(NopCatalogCachingDefaults.CategoriesChildIdentifiersCacheKey,
                parentCategoryId,
                string.Join(",", _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer)),
                _storeContext.CurrentStore.Id,
                showHidden);

            return _staticCacheManager.Get(cacheKey, () =>
            {
                //little hack for performance optimization
                //there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
                //so we load all categories at once (we know they are cached) and process them server-side
                var categoriesIds = new List<int>();
                var categories = GetAllCategories(storeId: storeId, showHidden: showHidden)
                    .Where(c => c.ParentCategoryId == parentCategoryId)
                    .Select(c => c.Id)
                    .ToList();
                categoriesIds.AddRange(categories);
                categoriesIds.AddRange(categories.SelectMany(cId => GetChildCategoryIds(cId, storeId, showHidden)));
                
                return categoriesIds;
            });
        }

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        public virtual Category GetCategoryById(int categoryId)
        {
            if (categoryId == 0)
                return null;

            var key = string.Format(NopCatalogCachingDefaults.CategoriesByIdCacheKey, categoryId);

            return _categoryRepository.ToCachedGetById(categoryId, key);
        }

        /// <summary>
        /// Get categories for which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted categories</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of categories</returns>
        public virtual IPagedList<Category> GetCategoriesByAppliedDiscount(int? discountId = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var categories = _categoryRepository.Table;

            if (discountId.HasValue)
                categories = from category in categories
                    join dcm in _discountCategoryMappingRepository.Table on category.Id equals dcm.EntityId
                    where dcm.DiscountId == discountId.Value
                    select category;

            if (!showHidden)
                categories = categories.Where(category => !category.Deleted);

            categories = categories.OrderBy(category => category.DisplayOrder).ThenBy(category => category.Id);

            return new PagedList<Category>(categories, pageIndex, pageSize);
        }

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        public virtual void InsertCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            _categoryRepository.Insert(category);

            //event notification
            _eventPublisher.EntityInserted(category);
        }

        /// <summary>
        /// Get a value indicating whether discount is applied to category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Result</returns>
        public virtual DiscountCategoryMapping GetDiscountAppliedToCategory(int categoryId, int discountId)
        {
            return _discountCategoryMappingRepository.Table.FirstOrDefault(dcm => dcm.EntityId == categoryId && dcm.DiscountId == discountId);
        }

        /// <summary>
        /// Inserts a discount-category mapping record
        /// </summary>
        /// <param name="discountCategoryMapping">Discount-category mapping</param>
        public virtual void InsertDiscountCategoryMapping(DiscountCategoryMapping discountCategoryMapping)
        {
            if (discountCategoryMapping is null)
                throw new ArgumentNullException(nameof(discountCategoryMapping));

            _discountCategoryMappingRepository.Insert(discountCategoryMapping);

            //event notification
            _eventPublisher.EntityInserted(discountCategoryMapping);
        }

        /// <summary>
        /// Deletes a discount-category mapping record
        /// </summary>
        /// <param name="discountCategoryMapping">Discount-category mapping</param>
        public virtual void DeleteDiscountCategoryMapping(DiscountCategoryMapping discountCategoryMapping)
        {
            if (discountCategoryMapping is null)
                throw new ArgumentNullException(nameof(discountCategoryMapping));

            _discountCategoryMappingRepository.Delete(discountCategoryMapping);

            //event notification
            _eventPublisher.EntityDeleted(discountCategoryMapping);
        }

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        public virtual void UpdateCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            //validate category hierarchy
            var parentCategory = GetCategoryById(category.ParentCategoryId);
            while (parentCategory != null)
            {
                if (category.Id == parentCategory.Id)
                {
                    category.ParentCategoryId = 0;
                    break;
                }

                parentCategory = GetCategoryById(parentCategory.ParentCategoryId);
            }

            _categoryRepository.Update(category);

            //event notification
            _eventPublisher.EntityUpdated(category);
        }

        /// <summary>
        /// Deletes a product category mapping
        /// </summary>
        /// <param name="productCategory">Product category</param>
        public virtual void DeleteProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
                throw new ArgumentNullException(nameof(productCategory));

            _productCategoryRepository.Delete(productCategory);

            //event notification
            _eventPublisher.EntityDeleted(productCategory);
        }

        /// <summary>
        /// Gets product category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product a category mapping collection</returns>
        public virtual IPagedList<ProductCategory> GetProductCategoriesByCategoryId(int categoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (categoryId == 0)
                return new PagedList<ProductCategory>(new List<ProductCategory>(), pageIndex, pageSize);

            var key = string.Format(NopCatalogCachingDefaults.ProductCategoriesAllByCategoryIdCacheKey, showHidden, categoryId,
                pageIndex, pageSize, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);

            var query = from pc in _productCategoryRepository.Table
                join p in _productRepository.Table on pc.ProductId equals p.Id
                where pc.CategoryId == categoryId &&
                      !p.Deleted &&
                      (showHidden || p.Published)
                orderby pc.DisplayOrder, pc.Id
                select pc;

            if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
            {
                if (!_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer);
                    query = from pc in query
                        join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                        join acl in _aclRepository.Table
                            on new
                            {
                                c1 = c.Id,
                                c2 = nameof(Category)
                            } 
                            equals new
                            {
                                c1 = acl.EntityId,
                                c2 = acl.EntityName
                            } 
                            into c_acl
                        from acl in c_acl.DefaultIfEmpty()
                        where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select pc;
                }

                if (!_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    var currentStoreId = _storeContext.CurrentStore.Id;
                    query = from pc in query
                        join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                        join sm in _storeMappingRepository.Table
                            on new
                            {
                                c1 = c.Id,
                                c2 = nameof(Category)
                            } 
                            equals new
                            {
                                c1 = sm.EntityId,
                                c2 = sm.EntityName
                            } 
                            into c_sm
                        from sm in c_sm.DefaultIfEmpty()
                        where !c.LimitedToStores || currentStoreId == sm.StoreId
                        select pc;
                }

                query = query.Distinct().OrderBy(pc => pc.DisplayOrder).ThenBy(pc => pc.Id);
            }

            var productCategories = query.ToCachedPagedList(key, pageIndex, pageSize);

            return productCategories;
        }

        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Product category mapping collection</returns>
        public virtual IList<ProductCategory> GetProductCategoriesByProductId(int productId, bool showHidden = false)
        {
            return GetProductCategoriesByProductId(productId, _storeContext.CurrentStore.Id, showHidden);
        }

        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Product category mapping collection</returns>
        public virtual IList<ProductCategory> GetProductCategoriesByProductId(int productId, int storeId,
            bool showHidden = false)
        {
            if (productId == 0)
                return new List<ProductCategory>();

            var key = string.Format(NopCatalogCachingDefaults.ProductCategoriesAllByProductIdCacheKey, showHidden,
                productId, _workContext.CurrentCustomer.Id, storeId);

            var query = from pc in _productCategoryRepository.Table
                join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                where pc.ProductId == productId &&
                      !c.Deleted &&
                      (showHidden || c.Published)
                orderby pc.DisplayOrder, pc.Id
                select pc;

            if (showHidden)
                return query.ToCachedList(key);

            var categoryIds = GetCategoriesByIds(query.Select(pc => pc.CategoryId).ToArray())
                .Where(category => _aclService.Authorize(category) && _storeMappingService.Authorize(category, storeId))
                .Select(c => c.Id).ToArray();

            query = from pc in query
                where categoryIds.Contains(pc.CategoryId)
                select pc;

            return query.ToCachedList(key);
        }

        /// <summary>
        /// Gets a product category mapping 
        /// </summary>
        /// <param name="productCategoryId">Product category mapping identifier</param>
        /// <returns>Product category mapping</returns>
        public virtual ProductCategory GetProductCategoryById(int productCategoryId)
        {
            if (productCategoryId == 0)
                return null;

            return _productCategoryRepository.ToCachedGetById(productCategoryId);
        }

        /// <summary>
        /// Inserts a product category mapping
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public virtual void InsertProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
                throw new ArgumentNullException(nameof(productCategory));

            _productCategoryRepository.Insert(productCategory);

            //event notification
            _eventPublisher.EntityInserted(productCategory);
        }

        /// <summary>
        /// Updates the product category mapping 
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public virtual void UpdateProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
                throw new ArgumentNullException(nameof(productCategory));

            _productCategoryRepository.Update(productCategory);
            
            //event notification
            _eventPublisher.EntityUpdated(productCategory);
        }

        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="categoryIdsNames">The names and/or IDs of the categories to check</param>
        /// <returns>List of names and/or IDs not existing categories</returns>
        public virtual string[] GetNotExistingCategories(string[] categoryIdsNames)
        {
            if (categoryIdsNames == null)
                throw new ArgumentNullException(nameof(categoryIdsNames));

            var query = _categoryRepository.Table;
            var queryFilter = categoryIdsNames.Distinct().ToArray();
            //filtering by name
            var filter = query.Select(c => c.Name).Where(c => queryFilter.Contains(c)).ToList();
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
        /// Get category IDs for products
        /// </summary>
        /// <param name="productIds">Products IDs</param>
        /// <returns>Category IDs for products</returns>
        public virtual IDictionary<int, int[]> GetProductCategoryIds(int[] productIds)
        {
            var query = _productCategoryRepository.Table;

            return query.Where(p => productIds.Contains(p.ProductId))
                .Select(p => new { p.ProductId, p.CategoryId }).ToList()
                .GroupBy(a => a.ProductId)
                .ToDictionary(items => items.Key, items => items.Select(a => a.CategoryId).ToArray());
        }

        /// <summary>
        /// Gets categories by identifier
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <returns>Categories</returns>
        public virtual List<Category> GetCategoriesByIds(int[] categoryIds)
        {
            if (categoryIds == null || categoryIds.Length == 0)
                return new List<Category>();

            var query = from p in _categoryRepository.Table
                        where categoryIds.Contains(p.Id) && !p.Deleted
                        select p;

            return query.ToList();
        }

        /// <summary>
        /// Sort categories for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent category identifier</param>
        /// <param name="ignoreCategoriesWithoutExistingParent">A value indicating whether categories without parent category in provided category list (source) should be ignored</param>
        /// <returns>Sorted categories</returns>
        public virtual IList<Category> SortCategoriesForTree(IList<Category> source, int parentId = 0,
            bool ignoreCategoriesWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var result = new List<Category>();

            foreach (var cat in source.Where(c => c.ParentCategoryId == parentId).ToList())
            {
                result.Add(cat);
                result.AddRange(SortCategoriesForTree(source, cat.Id, true));
            }

            if (ignoreCategoriesWithoutExistingParent || result.Count == source.Count)
                return result;

            //find categories without parent in provided category source and insert them into result
            foreach (var cat in source)
                if (result.FirstOrDefault(x => x.Id == cat.Id) == null)
                    result.Add(cat);

            return result;
        }

        /// <summary>
        /// Returns a ProductCategory that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>A ProductCategory that has the specified values; otherwise null</returns>
        public virtual ProductCategory FindProductCategory(IList<ProductCategory> source, int productId, int categoryId)
        {
            foreach (var productCategory in source)
                if (productCategory.ProductId == productId && productCategory.CategoryId == categoryId)
                    return productCategory;

            return null;
        }

        /// <summary>
        /// Get formatted category breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>Formatted breadcrumb</returns>
        public virtual string GetFormattedBreadCrumb(Category category, IList<Category> allCategories = null,
            string separator = ">>", int languageId = 0)
        {
            var result = string.Empty;

            var breadcrumb = GetCategoryBreadCrumb(category, allCategories, true);
            for (var i = 0; i <= breadcrumb.Count - 1; i++)
            {
                var categoryName = _localizationService.GetLocalized(breadcrumb[i], x => x.Name, languageId);
                result = string.IsNullOrEmpty(result) ? categoryName : $"{result} {separator} {categoryName}";
            }

            return result;
        }

        /// <summary>
        /// Get category breadcrumb 
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>Category breadcrumb </returns>
        public virtual IList<Category> GetCategoryBreadCrumb(Category category, IList<Category> allCategories = null, bool showHidden = false)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var breadcrumbCacheKey = string.Format(NopCatalogCachingDefaults.CategoryBreadcrumbKey,
                category.Id,
                string.Join(",", _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer)),
                _storeContext.CurrentStore.Id,
                _workContext.WorkingLanguage.Id);

            return _staticCacheManager.Get(breadcrumbCacheKey, () =>
            {
                var result = new List<Category>();

                //used to prevent circular references
                var alreadyProcessedCategoryIds = new List<int>();

                while (category != null && //not null
                       !category.Deleted && //not deleted
                       (showHidden || category.Published) && //published
                       (showHidden || _aclService.Authorize(category)) && //ACL
                       (showHidden || _storeMappingService.Authorize(category)) && //Store mapping
                       !alreadyProcessedCategoryIds.Contains(category.Id)) //prevent circular references
                {
                    result.Add(category);

                    alreadyProcessedCategoryIds.Add(category.Id);

                    category = allCategories != null
                        ? allCategories.FirstOrDefault(c => c.Id == category.ParentCategoryId)
                        : GetCategoryById(category.ParentCategoryId);
                }

                result.Reverse();

                return result;
            });
        }

        #endregion
    }
}