using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Catalog;

/// <summary>
/// Category service
/// </summary>
public partial class CategoryService : ICategoryService
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IRepository<Category> _categoryRepository;
    protected readonly IRepository<DiscountCategoryMapping> _discountCategoryMappingRepository;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IRepository<ProductCategory> _productCategoryRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IStoreService _storeService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public CategoryService(
        IAclService aclService,
        ICustomerService customerService,
        ILocalizationService localizationService,
        IRepository<Category> categoryRepository,
        IRepository<DiscountCategoryMapping> discountCategoryMappingRepository,
        IRepository<Product> productRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IStoreService storeService,
        IWorkContext workContext)
    {
        _aclService = aclService;
        _customerService = customerService;
        _localizationService = localizationService;
        _categoryRepository = categoryRepository;
        _discountCategoryMappingRepository = discountCategoryMappingRepository;
        _productRepository = productRepository;
        _productCategoryRepository = productCategoryRepository;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _storeService = storeService;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets a product category mapping collection
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
    /// <param name="showHidden"> A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product category mapping collection
    /// </returns>
    protected virtual async Task<IList<ProductCategory>> GetProductCategoriesByProductIdAsync(int productId, int storeId,
        bool showHidden = false)
    {
        if (productId == 0)
            return new List<ProductCategory>();

        var customer = await _workContext.GetCurrentCustomerAsync();
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);

        return await _productCategoryRepository.GetAllAsync(async query =>
        {
            if (!showHidden)
            {
                var categoriesQuery = _categoryRepository.Table.Where(c => c.Published);

                //apply store mapping constraints
                categoriesQuery = await _storeMappingService.ApplyStoreMapping(categoriesQuery, storeId);

                //apply ACL constraints
                categoriesQuery = await _aclService.ApplyAcl(categoriesQuery, customerRoleIds);

                query = query.Where(pc => categoriesQuery.Any(c => !c.Deleted && c.Id == pc.CategoryId));
            }

            return query
                .Where(pc => pc.ProductId == productId)
                .OrderBy(pc => pc.DisplayOrder)
                .ThenBy(pc => pc.Id);

        }, cache => _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductCategoriesByProductCacheKey,
            productId, showHidden, customerRoleIds, storeId));
    }

    /// <summary>
    /// Sort categories for tree representation
    /// </summary>
    /// <param name="categoriesByParentId">Categories for sort</param>
    /// <param name="parentId">Parent category identifier</param>
    /// <param name="ignoreCategoriesWithoutExistingParent">A value indicating whether categories without parent category in provided category list (source) should be ignored</param>
    /// <returns>
    /// An enumerable containing the sorted categories
    /// </returns>
    protected virtual IEnumerable<Category> SortCategoriesForTree(
        ILookup<int, Category> categoriesByParentId,
        int parentId = 0,
        bool ignoreCategoriesWithoutExistingParent = false)
    {
        ArgumentNullException.ThrowIfNull(categoriesByParentId);

        var remaining = parentId > 0
            ? new HashSet<int>(0)
            : categoriesByParentId.Select(g => g.Key).ToHashSet();
        remaining.Remove(parentId);

        foreach (var cat in categoriesByParentId[parentId].OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id))
        {
            yield return cat;

            remaining.Remove(cat.Id);

            foreach (var subCategory in SortCategoriesForTree(categoriesByParentId, cat.Id, true))
            {
                yield return subCategory;
                remaining.Remove(subCategory.Id);
            }
        }

        if (ignoreCategoriesWithoutExistingParent)
            yield break;

        //find categories without parent in provided category source and return them
        var orphans = remaining
            .SelectMany(id => categoriesByParentId[id])
            .OrderBy(c => c.ParentCategoryId)
            .ThenBy(c => c.DisplayOrder)
            .ThenBy(c => c.Id);

        foreach (var orphan in orphans)
            yield return orphan;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Check the possibility of adding products to the category for the current vendor
    /// </summary>
    /// <param name="category">Category</param>
    /// <param name="allCategories">All categories</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<bool> CanVendorAddProductsAsync(Category category, IList<Category> allCategories = null)
    {
        ArgumentNullException.ThrowIfNull(category);

        if (await _workContext.GetCurrentVendorAsync() is null) // check vendors only
            return true;

        if (category.RestrictFromVendors)
            return false;

        var breadcrumb = await GetCategoryBreadCrumbAsync(category, allCategories, showHidden: true);

        return !breadcrumb.Any(c => c.RestrictFromVendors);
    }

    /// <summary>
    /// Clean up category references for a  specified discount
    /// </summary>
    /// <param name="discount">Discount</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ClearDiscountCategoryMappingAsync(Discount discount)
    {
        ArgumentNullException.ThrowIfNull(discount);

        var mappings = _discountCategoryMappingRepository.Table.Where(dcm => dcm.DiscountId == discount.Id);

        await _discountCategoryMappingRepository.DeleteAsync(await mappings.ToListAsync());
    }

    /// <summary>
    /// Delete category
    /// </summary>
    /// <param name="category">Category</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteCategoryAsync(Category category)
    {
        await _categoryRepository.DeleteAsync(category);

        //reset a "Parent category" property of all child subcategories
        var subcategories = await GetAllCategoriesByParentCategoryIdAsync(category.Id, true);
        foreach (var subcategory in subcategories)
        {
            subcategory.ParentCategoryId = 0;
            await UpdateCategoryAsync(subcategory);
        }
    }

    /// <summary>
    /// Delete Categories
    /// </summary>
    /// <param name="categories">Categories</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteCategoriesAsync(IList<Category> categories)
    {
        ArgumentNullException.ThrowIfNull(categories);

        foreach (var category in categories)
            await DeleteCategoryAsync(category);
    }

    /// <summary>
    /// Gets all categories
    /// </summary>
    /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the categories
    /// </returns>
    public virtual async Task<IList<Category>> GetAllCategoriesAsync(int storeId = 0, bool showHidden = false)
    {
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoriesAllCacheKey,
            storeId,
            await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()),
            showHidden);

        var categories = await _staticCacheManager
            .GetAsync(key, async () => (await GetAllCategoriesAsync(string.Empty, storeId, showHidden: showHidden)).ToList());

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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the categories
    /// </returns>
    public virtual async Task<IPagedList<Category>> GetAllCategoriesAsync(string categoryName, int storeId = 0,
        int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool? overridePublished = null)
    {
        var unsortedCategories = await _categoryRepository.GetAllAsync(async query =>
        {
            if (!showHidden)
                query = query.Where(c => c.Published);
            else if (overridePublished.HasValue)
                query = query.Where(c => c.Published == overridePublished.Value);

            if (!showHidden || storeId > 0)
            {
                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);
            }

            if (!showHidden)
            {
                //apply ACL constraints
                var customer = await _workContext.GetCurrentCustomerAsync();
                query = await _aclService.ApplyAcl(query, customer);
            }

            if (!string.IsNullOrWhiteSpace(categoryName))
                query = query.Where(c => c.Name.Contains(categoryName));

            return query.Where(c => !c.Deleted);
        });

        //sort categories
        var sortedCategories = SortCategoriesForTree(unsortedCategories.ToLookup(c => c.ParentCategoryId))
            .ToList();

        //paging
        return new PagedList<Category>(sortedCategories, pageIndex, pageSize);
    }

    /// <summary>
    /// Gets all categories filtered by parent category identifier
    /// </summary>
    /// <param name="parentCategoryId">Parent category identifier</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the categories
    /// </returns>
    public virtual async Task<IList<Category>> GetAllCategoriesByParentCategoryIdAsync(int parentCategoryId,
        bool showHidden = false)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);

        var categories = await _categoryRepository.GetAllAsync(async query =>
        {
            if (!showHidden)
            {
                query = query.Where(c => c.Published);

                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, store.Id);

                //apply ACL constraints
                query = await _aclService.ApplyAcl(query, customerRoleIds);
            }

            query = query.Where(c => !c.Deleted && c.ParentCategoryId == parentCategoryId);

            return query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
        }, cache => cache.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoriesByParentCategoryCacheKey,
            parentCategoryId, showHidden, customerRoleIds, store));

        return categories;
    }

    /// <summary>
    /// Gets all categories displayed on the home page
    /// </summary>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the categories
    /// </returns>
    public virtual async Task<IList<Category>> GetAllCategoriesDisplayedOnHomepageAsync(bool showHidden = false)
    {
        var categories = await _categoryRepository.GetAllAsync(query =>
        {
            return from c in query
                orderby c.DisplayOrder, c.Id
                where c.Published &&
                      !c.Deleted &&
                      c.ShowOnHomepage
                select c;
        }, cache => cache.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoriesHomepageCacheKey));

        if (showHidden)
            return categories;

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoriesHomepageWithoutHiddenCacheKey,
            await _storeContext.GetCurrentStoreAsync(), await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()));

        var result = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            return await categories
                .WhereAwait(async c => await _aclService.AuthorizeAsync(c) && await _storeMappingService.AuthorizeAsync(c))
                .ToListAsync();
        });

        return result;
    }

    /// <summary>
    /// Get category identifiers to which a discount is applied
    /// </summary>
    /// <param name="discount">Discount</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category identifiers
    /// </returns>
    public virtual async Task<IList<int>> GetAppliedCategoryIdsAsync(Discount discount, Customer customer)
    {
        ArgumentNullException.ThrowIfNull(discount);

        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopDiscountDefaults.CategoryIdsByDiscountCacheKey,
            discount,
            await _customerService.GetCustomerRoleIdsAsync(customer),
            store);

        var result = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var ids = await _discountCategoryMappingRepository.Table
                .Where(dmm => dmm.DiscountId == discount.Id).Select(dmm => dmm.EntityId)
                .Distinct()
                .ToListAsync();

            if (!discount.AppliedToSubCategories)
                return ids;

            ids.AddRange(await ids.SelectManyAwait(async categoryId =>
                    await GetChildCategoryIdsAsync(categoryId, store.Id))
                .ToListAsync());

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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category identifiers
    /// </returns>
    public virtual async Task<IList<int>> GetChildCategoryIdsAsync(int parentCategoryId, int storeId = 0, bool showHidden = false)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoriesChildIdsCacheKey,
            parentCategoryId,
            await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()),
            storeId,
            showHidden);

        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            //little hack for performance optimization
            //there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once (we know they are cached) and process them server-side.
            //Store full Category objects in the lookup to avoid follow-up DB queries in call sites.
            var lookup = await _staticCacheManager.GetAsync(
                _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ChildCategoryIdLookupCacheKey, storeId, showHidden),
                async () => (await GetAllCategoriesAsync(storeId: storeId, showHidden: showHidden))
                    .ToGroupedDictionary(c => c.ParentCategoryId, x => x));

            var categoryIds = new List<int>();
            if (lookup.TryGetValue(parentCategoryId, out var categories))
            {
                categoryIds.AddRange(categories.Select(c => c.Id));
                var childCategoryIds = categories.SelectAwait(async c => await GetChildCategoryIdsAsync(c.Id, storeId, showHidden));
                // avoid allocating a new list or blocking with ToEnumerable
                await foreach (var cIds in childCategoryIds)
                    categoryIds.AddRange(cIds);
            }

            return categoryIds;
        });
    }

    /// <summary>
    /// Get category breadcrumb 
    /// </summary>
    /// <param name="category">Category</param>
    /// <param name="allCategories">All categories</param>
    /// <param name="showHidden">A value indicating whether to load hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category breadcrumb 
    /// </returns>
    public virtual async Task<IList<Category>> GetCategoryBreadCrumbAsync(Category category, IList<Category> allCategories = null, bool showHidden = false)
    {
        ArgumentNullException.ThrowIfNull(category);

        var breadcrumbCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.CategoryBreadcrumbCacheKey,
            category,
            await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()),
            await _storeContext.GetCurrentStoreAsync(),
            await _workContext.GetWorkingLanguageAsync(),
            showHidden);

        return await _staticCacheManager.GetAsync(breadcrumbCacheKey, async () =>
        {
            var result = new List<Category>();

            //used to prevent circular references
            var alreadyProcessedCategoryIds = new HashSet<int>();

            while (category != null && //not null
                   !category.Deleted && //not deleted
                   (showHidden || category.Published) && //published
                   (showHidden || await _aclService.AuthorizeAsync(category)) && //ACL
                   (showHidden || await _storeMappingService.AuthorizeAsync(category)) && //Store mapping
                   !alreadyProcessedCategoryIds.Contains(category.Id)) //prevent circular references
            {
                result.Add(category);

                alreadyProcessedCategoryIds.Add(category.Id);

                category = allCategories != null
                    ? allCategories.FirstOrDefault(c => c.Id == category.ParentCategoryId)
                    : await GetCategoryByIdAsync(category.ParentCategoryId);
            }

            result.Reverse();

            return result;
        });
    }

    /// <summary>
    /// Update category store mappings
    /// </summary>
    /// <param name="category">Category</param>
    /// <param name="limitedToStoresIds">A list of store ids for mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateCategoryStoreMappingsAsync(Category category, IList<int> limitedToStoresIds)
    {
        category.LimitedToStores = limitedToStoresIds.Any();
        await UpdateCategoryAsync(category);

        var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(category);
        var allStores = await _storeService.GetAllStoresAsync();
        foreach (var store in allStores)
        {
            if (limitedToStoresIds.Contains(store.Id))
            {
                //new store
                if (existingStoreMappings.All(sm => sm.StoreId != store.Id))
                    await _storeMappingService.InsertStoreMappingAsync(category, store.Id);
            }
            else
            {
                //remove store
                var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                if (storeMappingToDelete != null)
                    await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
            }
        }
    }

    #endregion
}