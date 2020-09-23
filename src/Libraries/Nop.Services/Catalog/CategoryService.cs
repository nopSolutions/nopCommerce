using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
using Nop.Services.Customers;
using Nop.Services.Discounts;
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
        private readonly IAclService _aclService;
        private readonly ICacheKeyService _cacheKeyService;
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
            IAclService aclService,
            ICacheKeyService cacheKeyService,
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
            _aclService = aclService;
            _cacheKeyService = cacheKeyService;
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
        public virtual async Task ClearDiscountCategoryMapping(Discount discount)
        {
            if (discount is null)
                throw new ArgumentNullException(nameof(discount));

            var mappings = _discountCategoryMappingRepository.Table.Where(dcm => dcm.DiscountId == discount.Id);

            if (!await mappings.AnyAsync())
                return;

            await _discountCategoryMappingRepository.Delete(mappings);
        }

        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="category">Category</param>
        public virtual async Task DeleteCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            category.Deleted = true;
            await UpdateCategory(category);

            //event notification
            await _eventPublisher.EntityDeleted(category);

            //reset a "Parent category" property of all child subcategories
            var subcategories = await GetAllCategoriesByParentCategoryId(category.Id, true);
            foreach (var subcategory in subcategories)
            {
                subcategory.ParentCategoryId = 0;
                await UpdateCategory(subcategory);
            }
        }

        /// <summary>
        /// Delete Categories
        /// </summary>
        /// <param name="categories">Categories</param>
        public virtual async Task DeleteCategories(IList<Category> categories)
        {
            if (categories == null)
                throw new ArgumentNullException(nameof(categories));

            foreach (var category in categories) 
                await DeleteCategory(category);
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual async Task<IList<Category>> GetAllCategories(int storeId = 0, bool showHidden = false)
        {
            var key = _cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoriesAllCacheKey,
                storeId,
                await _customerService.GetCustomerRoleIds(await _workContext.GetCurrentCustomer()),
                showHidden);

            var categories = await _staticCacheManager.Get(key, async () => (await GetAllCategories(string.Empty, storeId, showHidden: showHidden)).ToList());

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
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>Categories</returns>
        public virtual async Task<IPagedList<Category>> GetAllCategories(string categoryName, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool? overridePublished = null)
        {
            var query = _categoryRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            if (!string.IsNullOrWhiteSpace(categoryName))
                query = query.Where(c => c.Name.Contains(categoryName));
            query = query.Where(c => !c.Deleted);
            query = query.OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);
            if (overridePublished.HasValue)
                query = query.Where(c => c.Published == overridePublished.Value);

            if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
            {
                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = await _customerService.GetCustomerRoleIds(await _workContext.GetCurrentCustomer());
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

            var unsortedCategories = await query.ToListAsync();

            //sort categories
            var sortedCategories = await SortCategoriesForTree(unsortedCategories);

            //paging
            return new PagedList<Category>(sortedCategories, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual async Task<IList<Category>> GetAllCategoriesByParentCategoryId(int parentCategoryId,
            bool showHidden = false)
        {
            var key = _cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoriesByParentCategoryIdCacheKey,
                parentCategoryId, showHidden, await _workContext.GetCurrentCustomer(), await _storeContext.GetCurrentStore());

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
                    var allowedCustomerRolesIds = await _customerService.GetCustomerRoleIds(await _workContext.GetCurrentCustomer());
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
                    var currentStoreId = (await _storeContext.GetCurrentStore()).Id;
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

            var categories = await query.ToCachedList(key);

            return categories;
        }

        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual async Task<IList<Category>> GetAllCategoriesDisplayedOnHomepage(bool showHidden = false)
        {
            var query = from c in _categoryRepository.Table
                        orderby c.DisplayOrder, c.Id
                        where c.Published &&
                        !c.Deleted &&
                        c.ShowOnHomepage
                        select c;

            var categories = await query.ToCachedList(_cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoriesAllDisplayedOnHomepageCacheKey));

            if (showHidden)
                return categories;

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoriesDisplayedOnHomepageWithoutHiddenCacheKey,
                await _storeContext.GetCurrentStore(), await _customerService.GetCustomerRoleIds(await _workContext.GetCurrentCustomer()));

            var result = await _staticCacheManager.Get(cacheKey, () =>
            {
                return Task.FromResult(categories
                    .Where(c => _aclService.Authorize(c).Result && _storeMappingService.Authorize(c).Result)
                    .ToList());
            });

            return result;
        }

        /// <summary>
        /// Get category identifiers to which a discount is applied
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Category identifiers</returns>
        public virtual async Task<IList<int>> GetAppliedCategoryIds(Discount discount, Customer customer)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopDiscountDefaults.DiscountCategoryIdsModelCacheKey,
                discount,
                await _customerService.GetCustomerRoleIds(customer),
                await _storeContext.GetCurrentStore());

            var result = await _staticCacheManager.Get(cacheKey, async () =>
            {
                var ids = await _discountCategoryMappingRepository.Table.Where(dmm => dmm.DiscountId == discount.Id).Select(dmm => dmm.EntityId).Distinct().ToListAsync();

                if (!discount.AppliedToSubCategories)
                    return ids;

                ids.AddRange(ids.SelectMany(categoryId => GetChildCategoryIds(categoryId, _storeContext.GetCurrentStore().Result.Id).Result).ToList());

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
        public virtual async Task<IList<int>> GetChildCategoryIds(int parentCategoryId, int storeId = 0, bool showHidden = false)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoriesChildIdentifiersCacheKey,
                parentCategoryId,
                await _customerService.GetCustomerRoleIds(await _workContext.GetCurrentCustomer()),
                await _storeContext.GetCurrentStore(),
                showHidden);

            return await _staticCacheManager.Get(cacheKey, async () =>
            {
                //little hack for performance optimization
                //there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
                //so we load all categories at once (we know they are cached) and process them server-side
                var categoriesIds = new List<int>();
                var categories = (await GetAllCategories(storeId: storeId, showHidden: showHidden))
                    .Where(c => c.ParentCategoryId == parentCategoryId)
                    .Select(c => c.Id)
                    .ToList();
                categoriesIds.AddRange(categories);
                categoriesIds.AddRange(categories.SelectMany(cId => GetChildCategoryIds(cId, storeId, showHidden).Result));

                return categoriesIds;
            });
        }

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        public virtual async Task<Category> GetCategoryById(int categoryId)
        {
            if (categoryId == 0)
                return null;

            return await _categoryRepository.ToCachedGetById(categoryId);
        }

        /// <summary>
        /// Get categories for which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted categories</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of categories</returns>
        public virtual async Task<IPagedList<Category>> GetCategoriesByAppliedDiscount(int? discountId = null,
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

            return await categories.ToPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        public virtual async Task InsertCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            await _categoryRepository.Insert(category);

            //event notification
            await _eventPublisher.EntityInserted(category);
        }

        /// <summary>
        /// Get a value indicating whether discount is applied to category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Result</returns>
        public virtual async Task<DiscountCategoryMapping> GetDiscountAppliedToCategory(int categoryId, int discountId)
        {
            return await _discountCategoryMappingRepository.Table.FirstOrDefaultAsync(dcm => dcm.EntityId == categoryId && dcm.DiscountId == discountId);
        }

        /// <summary>
        /// Inserts a discount-category mapping record
        /// </summary>
        /// <param name="discountCategoryMapping">Discount-category mapping</param>
        public virtual async Task InsertDiscountCategoryMapping(DiscountCategoryMapping discountCategoryMapping)
        {
            if (discountCategoryMapping is null)
                throw new ArgumentNullException(nameof(discountCategoryMapping));

            await _discountCategoryMappingRepository.Insert(discountCategoryMapping);

            //event notification
            await _eventPublisher.EntityInserted(discountCategoryMapping);
        }

        /// <summary>
        /// Deletes a discount-category mapping record
        /// </summary>
        /// <param name="discountCategoryMapping">Discount-category mapping</param>
        public virtual async Task DeleteDiscountCategoryMapping(DiscountCategoryMapping discountCategoryMapping)
        {
            if (discountCategoryMapping is null)
                throw new ArgumentNullException(nameof(discountCategoryMapping));

            await _discountCategoryMappingRepository.Delete(discountCategoryMapping);

            //event notification
            await _eventPublisher.EntityDeleted(discountCategoryMapping);
        }

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        public virtual async Task UpdateCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            //validate category hierarchy
            var parentCategory = await GetCategoryById(category.ParentCategoryId);
            while (parentCategory != null)
            {
                if (category.Id == parentCategory.Id)
                {
                    category.ParentCategoryId = 0;
                    break;
                }

                parentCategory = await GetCategoryById(parentCategory.ParentCategoryId);
            }

            await _categoryRepository.Update(category);

            //event notification
            await _eventPublisher.EntityUpdated(category);
        }

        /// <summary>
        /// Deletes a product category mapping
        /// </summary>
        /// <param name="productCategory">Product category</param>
        public virtual async Task DeleteProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
                throw new ArgumentNullException(nameof(productCategory));

            await _productCategoryRepository.Delete(productCategory);

            //event notification
            await _eventPublisher.EntityDeleted(productCategory);
        }

        /// <summary>
        /// Gets product category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product a category mapping collection</returns>
        public virtual async Task<IPagedList<ProductCategory>> GetProductCategoriesByCategoryId(int categoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (categoryId == 0)
                return new PagedList<ProductCategory>(new List<ProductCategory>(), pageIndex, pageSize);

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
                    var allowedCustomerRolesIds = await _customerService.GetCustomerRoleIds(await _workContext.GetCurrentCustomer());
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
                    var currentStoreId = (await _storeContext.GetCurrentStore()).Id;
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

            var productCategories = await query.ToPagedList(pageIndex, pageSize);

            return productCategories;
        }

        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Product category mapping collection</returns>
        public virtual async Task<IList<ProductCategory>> GetProductCategoriesByProductId(int productId, bool showHidden = false)
        {
            return await GetProductCategoriesByProductId(productId, (await _storeContext.GetCurrentStore()).Id, showHidden);
        }

        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Product category mapping collection</returns>
        public virtual async Task<IList<ProductCategory>> GetProductCategoriesByProductId(int productId, int storeId,
            bool showHidden = false)
        {
            if (productId == 0)
                return new List<ProductCategory>();

            var key = _cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductCategoriesAllByProductIdCacheKey,
                productId, showHidden, await _workContext.GetCurrentCustomer(), storeId);

            var query = from pc in _productCategoryRepository.Table
                        join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                        where pc.ProductId == productId &&
                              !c.Deleted &&
                              (showHidden || c.Published)
                        orderby pc.DisplayOrder, pc.Id
                        select pc;

            if (showHidden)
                return await query.ToCachedList(key);

            var categoryIds = (await GetCategoriesByIds(query.Select(pc => pc.CategoryId).ToArray()))
                .Where(category => _aclService.Authorize(category).Result && _storeMappingService.Authorize(category, storeId).Result)
                .Select(c => c.Id).ToArray();

            query = from pc in query
                    where categoryIds.Contains(pc.CategoryId)
                    select pc;

            return await query.ToCachedList(key);
        }

        /// <summary>
        /// Gets a product category mapping 
        /// </summary>
        /// <param name="productCategoryId">Product category mapping identifier</param>
        /// <returns>Product category mapping</returns>
        public virtual async Task<ProductCategory> GetProductCategoryById(int productCategoryId)
        {
            if (productCategoryId == 0)
                return null;

            return await _productCategoryRepository.ToCachedGetById(productCategoryId);
        }

        /// <summary>
        /// Inserts a product category mapping
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public virtual async Task InsertProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
                throw new ArgumentNullException(nameof(productCategory));

            await _productCategoryRepository.Insert(productCategory);

            //event notification
            await _eventPublisher.EntityInserted(productCategory);
        }

        /// <summary>
        /// Updates the product category mapping 
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public virtual async Task UpdateProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
                throw new ArgumentNullException(nameof(productCategory));

            await _productCategoryRepository.Update(productCategory);

            //event notification
            await _eventPublisher.EntityUpdated(productCategory);
        }

        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="categoryIdsNames">The names and/or IDs of the categories to check</param>
        /// <returns>List of names and/or IDs not existing categories</returns>
        public virtual async Task<string[]> GetNotExistingCategories(string[] categoryIdsNames)
        {
            if (categoryIdsNames == null)
                throw new ArgumentNullException(nameof(categoryIdsNames));

            var query = _categoryRepository.Table;
            var queryFilter = categoryIdsNames.Distinct().ToArray();
            //filtering by name
            var filter = await query.Select(c => c.Name).Where(c => queryFilter.Contains(c)).ToListAsync();

            //if some names not found
            if (!queryFilter.Except(filter).ToArray().Any())
                return queryFilter.ToArray();

            //filtering by IDs
            filter = await query.Select(c => c.Id.ToString()).Where(c => queryFilter.Contains(c)).ToListAsync();

            return queryFilter.Except(filter).ToArray();
        }

        /// <summary>
        /// Get category IDs for products
        /// </summary>
        /// <param name="productIds">Products IDs</param>
        /// <returns>Category IDs for products</returns>
        public virtual async Task<IDictionary<int, int[]>> GetProductCategoryIds(int[] productIds)
        {
            var query = _productCategoryRepository.Table;

            return (await query.Where(p => productIds.Contains(p.ProductId))
                .Select(p => new { p.ProductId, p.CategoryId }).ToListAsync())
                .GroupBy(a => a.ProductId)
                .ToDictionary(items => items.Key, items => items.Select(a => a.CategoryId).ToArray());
        }

        /// <summary>
        /// Gets categories by identifier
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <returns>Categories</returns>
        public virtual async Task<List<Category>> GetCategoriesByIds(int[] categoryIds)
        {
            if (categoryIds == null || categoryIds.Length == 0)
                return new List<Category>();

            var query = from p in _categoryRepository.Table
                        where categoryIds.Contains(p.Id) && !p.Deleted
                        select p;

            return await query.ToListAsync();
        }

        /// <summary>
        /// Sort categories for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent category identifier</param>
        /// <param name="ignoreCategoriesWithoutExistingParent">A value indicating whether categories without parent category in provided category list (source) should be ignored</param>
        /// <returns>Sorted categories</returns>
        public virtual async Task<IList<Category>> SortCategoriesForTree(IList<Category> source, int parentId = 0,
            bool ignoreCategoriesWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var result = new List<Category>();

            foreach (var cat in source.Where(c => c.ParentCategoryId == parentId).ToList())
            {
                result.Add(cat);
                result.AddRange(await SortCategoriesForTree(source, cat.Id, true));
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
        public virtual async Task<string> GetFormattedBreadCrumb(Category category, IList<Category> allCategories = null,
            string separator = ">>", int languageId = 0)
        {
            var result = string.Empty;

            var breadcrumb = await GetCategoryBreadCrumb(category, allCategories, true);
            for (var i = 0; i <= breadcrumb.Count - 1; i++)
            {
                var categoryName = await _localizationService.GetLocalized(breadcrumb[i], x => x.Name, languageId);
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
        public virtual async Task<IList<Category>> GetCategoryBreadCrumb(Category category, IList<Category> allCategories = null, bool showHidden = false)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var breadcrumbCacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoryBreadcrumbCacheKey,
                category,
                await _customerService.GetCustomerRoleIds(await _workContext.GetCurrentCustomer()),
                await _storeContext.GetCurrentStore(),
                await _workContext.GetWorkingLanguage());

            return await _staticCacheManager.Get(breadcrumbCacheKey, async () =>
            {
                var result = new List<Category>();

                //used to prevent circular references
                var alreadyProcessedCategoryIds = new List<int>();

                while (category != null && //not null
                       !category.Deleted && //not deleted
                       (showHidden || category.Published) && //published
                       (showHidden || await _aclService.Authorize(category)) && //ACL
                       (showHidden || await _storeMappingService.Authorize(category)) && //Store mapping
                       !alreadyProcessedCategoryIds.Contains(category.Id)) //prevent circular references
                {
                    result.Add(category);

                    alreadyProcessedCategoryIds.Add(category.Id);

                    category = allCategories != null
                        ? allCategories.FirstOrDefault(c => c.Id == category.ParentCategoryId)
                        : await GetCategoryById(category.ParentCategoryId);
                }

                result.Reverse();

                return result;
            });
        }

        #endregion
    }
}