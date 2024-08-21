using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Catalog;

/// <summary>
/// Specification attribute service
/// </summary>
public partial class SpecificationAttributeService : ISpecificationAttributeService
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAclService _aclService;
    protected readonly ICategoryService _categoryService;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IRepository<ProductCategory> _productCategoryRepository;
    protected readonly IRepository<ProductManufacturer> _productManufacturerRepository;
    protected readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
    protected readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
    protected readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
    protected readonly IRepository<SpecificationAttributeGroup> _specificationAttributeGroupRepository;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public SpecificationAttributeService(
        CatalogSettings catalogSettings,
        IAclService aclService,
        ICategoryService categoryService,
        IRepository<Product> productRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IRepository<ProductManufacturer> productManufacturerRepository,
        IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
        IRepository<SpecificationAttribute> specificationAttributeRepository,
        IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
        IRepository<SpecificationAttributeGroup> specificationAttributeGroupRepository,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IStaticCacheManager staticCacheManager,
        IWorkContext workContext)
    {
        _catalogSettings = catalogSettings;
        _aclService = aclService;
        _categoryService = categoryService;
        _productRepository = productRepository;
        _productCategoryRepository = productCategoryRepository;
        _productManufacturerRepository = productManufacturerRepository;
        _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
        _specificationAttributeRepository = specificationAttributeRepository;
        _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
        _specificationAttributeGroupRepository = specificationAttributeGroupRepository;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _staticCacheManager = staticCacheManager;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets the available products query
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the available products query
    /// </returns>
    protected virtual async Task<IQueryable<Product>> GetAvailableProductsQueryAsync()
    {
        var productsQuery =
            from p in _productRepository.Table
            where !p.Deleted && p.Published &&
                  (p.ParentGroupedProductId == 0 || p.VisibleIndividually) &&
                  (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc <= DateTime.UtcNow) &&
                  (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc >= DateTime.UtcNow)
            select p;

        var store = await _storeContext.GetCurrentStoreAsync();
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();

        //apply store mapping constraints
        productsQuery = await _storeMappingService.ApplyStoreMapping(productsQuery, store.Id);

        //apply ACL constraints
        productsQuery = await _aclService.ApplyAcl(productsQuery, currentCustomer);

        return productsQuery;
    }

    #endregion

    #region Methods

    #region Specification attribute group

    /// <summary>
    /// Gets a specification attribute group
    /// </summary>
    /// <param name="specificationAttributeGroupId">The specification attribute group identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute group
    /// </returns>
    public virtual async Task<SpecificationAttributeGroup> GetSpecificationAttributeGroupByIdAsync(int specificationAttributeGroupId)
    {
        return await _specificationAttributeGroupRepository.GetByIdAsync(specificationAttributeGroupId, cache => default);
    }

    /// <summary>
    /// Gets specification attribute groups
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute groups
    /// </returns>
    public virtual async Task<IPagedList<SpecificationAttributeGroup>> GetSpecificationAttributeGroupsAsync(int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = from sag in _specificationAttributeGroupRepository.Table
            orderby sag.DisplayOrder, sag.Id
            select sag;

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets product specification attribute groups
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute groups
    /// </returns>
    public virtual async Task<IList<SpecificationAttributeGroup>> GetProductSpecificationAttributeGroupsAsync(int productId)
    {
        var productAttributesForGroupQuery =
            from sa in _specificationAttributeRepository.Table
            join sao in _specificationAttributeOptionRepository.Table
                on sa.Id equals sao.SpecificationAttributeId
            join psa in _productSpecificationAttributeRepository.Table
                on sao.Id equals psa.SpecificationAttributeOptionId
            where psa.ProductId == productId && psa.ShowOnProductPage
            select sa.SpecificationAttributeGroupId;

        var availableGroupsQuery =
            from sag in _specificationAttributeGroupRepository.Table
            where productAttributesForGroupQuery.Any(groupId => groupId == sag.Id)
            select sag;

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.SpecificationAttributeGroupByProductCacheKey, productId);

        return await _staticCacheManager.GetAsync(key, async () => await availableGroupsQuery.ToListAsync());
    }

    /// <summary>
    /// Deletes a specification attribute group
    /// </summary>
    /// <param name="specificationAttributeGroup">The specification attribute group</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSpecificationAttributeGroupAsync(SpecificationAttributeGroup specificationAttributeGroup)
    {
        await _specificationAttributeGroupRepository.DeleteAsync(specificationAttributeGroup);
    }

    /// <summary>
    /// Inserts a specification attribute group
    /// </summary>
    /// <param name="specificationAttributeGroup">The specification attribute group</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertSpecificationAttributeGroupAsync(SpecificationAttributeGroup specificationAttributeGroup)
    {
        await _specificationAttributeGroupRepository.InsertAsync(specificationAttributeGroup);
    }

    /// <summary>
    /// Updates the specification attribute group
    /// </summary>
    /// <param name="specificationAttributeGroup">The specification attribute group</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateSpecificationAttributeGroupAsync(SpecificationAttributeGroup specificationAttributeGroup)
    {
        await _specificationAttributeGroupRepository.UpdateAsync(specificationAttributeGroup);
    }

    #endregion

    #region Specification attribute

    /// <summary>
    /// Gets a specification attribute
    /// </summary>
    /// <param name="specificationAttributeId">The specification attribute identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute
    /// </returns>
    public virtual async Task<SpecificationAttribute> GetSpecificationAttributeByIdAsync(int specificationAttributeId)
    {
        return await _specificationAttributeRepository.GetByIdAsync(specificationAttributeId, cache => default);
    }

    /// <summary>
    /// Gets specification attributes
    /// </summary>
    /// <param name="specificationAttributeIds">The specification attribute identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attributes
    /// </returns>
    public virtual async Task<IList<SpecificationAttribute>> GetSpecificationAttributeByIdsAsync(int[] specificationAttributeIds)
    {
        return await _specificationAttributeRepository.GetByIdsAsync(specificationAttributeIds);
    }

    /// <summary>
    /// Gets specification attributes
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attributes
    /// </returns>
    public virtual async Task<IPagedList<SpecificationAttribute>> GetSpecificationAttributesAsync(int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = from sa in _specificationAttributeRepository.Table
            orderby sa.DisplayOrder, sa.Id
            select sa;

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets specification attributes that have options
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attributes that have available options
    /// </returns>
    public virtual async Task<IList<SpecificationAttribute>> GetSpecificationAttributesWithOptionsAsync()
    {
        var query = from sa in _specificationAttributeRepository.Table
            where _specificationAttributeOptionRepository.Table.Any(o => o.SpecificationAttributeId == sa.Id)
            orderby sa.DisplayOrder, sa.Id
            select sa;

        return await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.SpecificationAttributesWithOptionsCacheKey), async () => await query.ToListAsync());
    }

    /// <summary>
    /// Gets specification attributes by group identifier
    /// </summary>
    /// <param name="specificationAttributeGroupId">The specification attribute group identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attributes
    /// </returns>
    public virtual async Task<IList<SpecificationAttribute>> GetSpecificationAttributesByGroupIdAsync(int? specificationAttributeGroupId = null)
    {
        var query = _specificationAttributeRepository.Table;
        if (!specificationAttributeGroupId.HasValue || specificationAttributeGroupId > 0)
            query = query.Where(sa => sa.SpecificationAttributeGroupId == specificationAttributeGroupId);

        query = query.OrderBy(sa => sa.DisplayOrder).ThenBy(sa => sa.Id);

        return await query.ToListAsync();
    }

    /// <summary>
    /// Deletes a specification attribute
    /// </summary>
    /// <param name="specificationAttribute">The specification attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSpecificationAttributeAsync(SpecificationAttribute specificationAttribute)
    {
        await _specificationAttributeRepository.DeleteAsync(specificationAttribute);
    }

    /// <summary>
    /// Deletes specifications attributes
    /// </summary>
    /// <param name="specificationAttributes">Specification attributes</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSpecificationAttributesAsync(IList<SpecificationAttribute> specificationAttributes)
    {
        ArgumentNullException.ThrowIfNull(specificationAttributes);

        foreach (var specificationAttribute in specificationAttributes)
            await DeleteSpecificationAttributeAsync(specificationAttribute);
    }

    /// <summary>
    /// Inserts a specification attribute
    /// </summary>
    /// <param name="specificationAttribute">The specification attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertSpecificationAttributeAsync(SpecificationAttribute specificationAttribute)
    {
        await _specificationAttributeRepository.InsertAsync(specificationAttribute);
    }

    /// <summary>
    /// Updates the specification attribute
    /// </summary>
    /// <param name="specificationAttribute">The specification attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateSpecificationAttributeAsync(SpecificationAttribute specificationAttribute)
    {
        await _specificationAttributeRepository.UpdateAsync(specificationAttribute);
    }

    #endregion

    #region Specification attribute option

    /// <summary>
    /// Gets a specification attribute option
    /// </summary>
    /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute option
    /// </returns>
    public virtual async Task<SpecificationAttributeOption> GetSpecificationAttributeOptionByIdAsync(int specificationAttributeOptionId)
    {
        return await _specificationAttributeOptionRepository.GetByIdAsync(specificationAttributeOptionId, cache => default);
    }

    /// <summary>
    /// Get specification attribute options by identifiers
    /// </summary>
    /// <param name="specificationAttributeOptionIds">Identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute options
    /// </returns>
    public virtual async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByIdsAsync(int[] specificationAttributeOptionIds)
    {
        return await _specificationAttributeOptionRepository.GetByIdsAsync(specificationAttributeOptionIds);
    }

    /// <summary>
    /// Gets a specification attribute option by specification attribute id
    /// </summary>
    /// <param name="specificationAttributeId">The specification attribute identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute option
    /// </returns>
    public virtual async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsBySpecificationAttributeAsync(int specificationAttributeId)
    {
        var query = from sao in _specificationAttributeOptionRepository.Table
            orderby sao.DisplayOrder, sao.Id
            where sao.SpecificationAttributeId == specificationAttributeId
            select sao;

        var specificationAttributeOptions = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.SpecificationAttributeOptionsCacheKey, specificationAttributeId), async () => await query.ToListAsync());

        return specificationAttributeOptions;
    }

    /// <summary>
    /// Deletes a specification attribute option
    /// </summary>
    /// <param name="specificationAttributeOption">The specification attribute option</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSpecificationAttributeOptionAsync(SpecificationAttributeOption specificationAttributeOption)
    {
        await _specificationAttributeOptionRepository.DeleteAsync(specificationAttributeOption);
    }

    /// <summary>
    /// Inserts a specification attribute option
    /// </summary>
    /// <param name="specificationAttributeOption">The specification attribute option</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertSpecificationAttributeOptionAsync(SpecificationAttributeOption specificationAttributeOption)
    {
        await _specificationAttributeOptionRepository.InsertAsync(specificationAttributeOption);
    }

    /// <summary>
    /// Updates the specification attribute
    /// </summary>
    /// <param name="specificationAttributeOption">The specification attribute option</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateSpecificationAttributeOptionAsync(SpecificationAttributeOption specificationAttributeOption)
    {
        await _specificationAttributeOptionRepository.UpdateAsync(specificationAttributeOption);
    }

    /// <summary>
    /// Returns a list of IDs of not existing specification attribute options
    /// </summary>
    /// <param name="attributeOptionIds">The IDs of the attribute options to check</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of IDs not existing specification attribute options
    /// </returns>
    public virtual async Task<int[]> GetNotExistingSpecificationAttributeOptionsAsync(int[] attributeOptionIds)
    {
        ArgumentNullException.ThrowIfNull(attributeOptionIds);

        var query = _specificationAttributeOptionRepository.Table;
        var queryFilter = attributeOptionIds.Distinct().ToArray();
        var filter = await query.Select(a => a.Id)
            .Where(m => queryFilter.Contains(m))
            .ToListAsync();
        return queryFilter.Except(filter).ToArray();
    }

    /// <summary>
    /// Gets the filtrable specification attribute options by category id
    /// </summary>
    /// <param name="categoryId">The category id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute options
    /// </returns>
    public virtual async Task<IList<SpecificationAttributeOption>> GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(int categoryId)
    {
        if (categoryId <= 0)
            return new List<SpecificationAttributeOption>();

        var productsQuery = await GetAvailableProductsQueryAsync();

        IList<int> subCategoryIds = null;

        if (_catalogSettings.ShowProductsFromSubcategories)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            subCategoryIds = await _categoryService.GetChildCategoryIdsAsync(categoryId, store.Id);
        }

        var productCategoryQuery =
            from pc in _productCategoryRepository.Table
            where (pc.CategoryId == categoryId || (_catalogSettings.ShowProductsFromSubcategories && subCategoryIds.Contains(pc.CategoryId))) &&
                  (_catalogSettings.IncludeFeaturedProductsInNormalLists || !pc.IsFeaturedProduct)
            select pc;

        var result =
            from sao in _specificationAttributeOptionRepository.Table
            join psa in _productSpecificationAttributeRepository.Table on sao.Id equals psa.SpecificationAttributeOptionId
            join p in productsQuery on psa.ProductId equals p.Id
            join pc in productCategoryQuery on p.Id equals pc.ProductId
            join sa in _specificationAttributeRepository.Table on sao.SpecificationAttributeId equals sa.Id
            where psa.AllowFiltering
            orderby
                sa.DisplayOrder, sa.Name,
                sao.DisplayOrder, sao.Name
            //linq2db don't specify 'sa' in 'SELECT' statement
            //see also https://github.com/nopSolutions/nopCommerce/issues/5425
            select new { sa, sao };

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
            NopCatalogDefaults.SpecificationAttributeOptionsByCategoryCacheKey, categoryId.ToString());

        return await _staticCacheManager.GetAsync(cacheKey, async () => (await result.Distinct().ToListAsync()).Select(query => query.sao).ToList());
    }

    /// <summary>
    /// Gets the filtrable specification attribute options by manufacturer id
    /// </summary>
    /// <param name="manufacturerId">The manufacturer id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification attribute options
    /// </returns>
    public virtual async Task<IList<SpecificationAttributeOption>> GetFiltrableSpecificationAttributeOptionsByManufacturerIdAsync(int manufacturerId)
    {
        if (manufacturerId <= 0)
            return new List<SpecificationAttributeOption>();

        var productsQuery = await GetAvailableProductsQueryAsync();

        var productManufacturerQuery =
            from pm in _productManufacturerRepository.Table
            where pm.ManufacturerId == manufacturerId &&
                  (_catalogSettings.IncludeFeaturedProductsInNormalLists || !pm.IsFeaturedProduct)
            select pm;

        var result =
            from sao in _specificationAttributeOptionRepository.Table
            join psa in _productSpecificationAttributeRepository.Table on sao.Id equals psa.SpecificationAttributeOptionId
            join p in productsQuery on psa.ProductId equals p.Id
            join pm in productManufacturerQuery on p.Id equals pm.ProductId
            join sa in _specificationAttributeRepository.Table on sao.SpecificationAttributeId equals sa.Id
            where psa.AllowFiltering
            orderby
                sa.DisplayOrder, sa.Name,
                sao.DisplayOrder, sao.Name
            //linq2db don't specify 'sa' in 'SELECT' statement
            //see also https://github.com/nopSolutions/nopCommerce/issues/5425
            select new { sa, sao };

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
            NopCatalogDefaults.SpecificationAttributeOptionsByManufacturerCacheKey, manufacturerId.ToString());

        return await _staticCacheManager.GetAsync(cacheKey, async () => (await result.Distinct().ToListAsync()).Select(query => query.sao).ToList());
    }

    #endregion

    #region Product specification attribute

    /// <summary>
    /// Deletes a product specification attribute mapping
    /// </summary>
    /// <param name="productSpecificationAttribute">Product specification attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductSpecificationAttributeAsync(ProductSpecificationAttribute productSpecificationAttribute)
    {
        await _productSpecificationAttributeRepository.DeleteAsync(productSpecificationAttribute);
    }

    /// <summary>
    /// Gets a product specification attribute mapping collection
    /// </summary>
    /// <param name="productId">Product identifier; 0 to load all records</param>
    /// <param name="specificationAttributeOptionId">Specification attribute option identifier; 0 to load all records</param>
    /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 1 to load attributes with AllowFiltering set to true, null to load all attributes</param>
    /// <param name="showOnProductPage">0 to load attributes with ShowOnProductPage set to false, 1 to load attributes with ShowOnProductPage set to true, null to load all attributes</param>
    /// <param name="specificationAttributeGroupId">Specification attribute group identifier; 0 to load all records; null to load attributes without group</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product specification attribute mapping collection
    /// </returns>
    public virtual async Task<IList<ProductSpecificationAttribute>> GetProductSpecificationAttributesAsync(int productId = 0,
        int specificationAttributeOptionId = 0, bool? allowFiltering = null, bool? showOnProductPage = null, int? specificationAttributeGroupId = 0)
    {
        var allowFilteringCacheStr = allowFiltering.HasValue ? allowFiltering.ToString() : "null";
        var showOnProductPageCacheStr = showOnProductPage.HasValue ? showOnProductPage.ToString() : "null";
        var specificationAttributeGroupIdCacheStr = specificationAttributeGroupId.HasValue ? specificationAttributeGroupId.ToString() : "null";

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductSpecificationAttributeByProductCacheKey,
            productId, specificationAttributeOptionId, allowFilteringCacheStr, showOnProductPageCacheStr, specificationAttributeGroupIdCacheStr);

        var query = _productSpecificationAttributeRepository.Table;
        if (productId > 0)
            query = query.Where(psa => psa.ProductId == productId);
        if (specificationAttributeOptionId > 0)
            query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);
        if (allowFiltering.HasValue)
            query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
        if (!specificationAttributeGroupId.HasValue || specificationAttributeGroupId > 0)
        {
            query = from psa in query
                join sao in _specificationAttributeOptionRepository.Table
                    on psa.SpecificationAttributeOptionId equals sao.Id
                join sa in _specificationAttributeRepository.Table
                    on sao.SpecificationAttributeId equals sa.Id
                where sa.SpecificationAttributeGroupId == specificationAttributeGroupId
                select psa;
        }
        if (showOnProductPage.HasValue)
            query = query.Where(psa => psa.ShowOnProductPage == showOnProductPage.Value);
        query = query.OrderBy(psa => psa.DisplayOrder).ThenBy(psa => psa.Id);

        var productSpecificationAttributes = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

        return productSpecificationAttributes;
    }

    /// <summary>
    /// Gets a product specification attribute mapping 
    /// </summary>
    /// <param name="productSpecificationAttributeId">Product specification attribute mapping identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product specification attribute mapping
    /// </returns>
    public virtual async Task<ProductSpecificationAttribute> GetProductSpecificationAttributeByIdAsync(int productSpecificationAttributeId)
    {
        return await _productSpecificationAttributeRepository.GetByIdAsync(productSpecificationAttributeId);
    }

    /// <summary>
    /// Inserts a product specification attribute mapping
    /// </summary>
    /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductSpecificationAttributeAsync(ProductSpecificationAttribute productSpecificationAttribute)
    {
        await _productSpecificationAttributeRepository.InsertAsync(productSpecificationAttribute);
    }

    /// <summary>
    /// Updates the product specification attribute mapping
    /// </summary>
    /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductSpecificationAttributeAsync(ProductSpecificationAttribute productSpecificationAttribute)
    {
        await _productSpecificationAttributeRepository.UpdateAsync(productSpecificationAttribute);
    }

    /// <summary>
    /// Gets a count of product specification attribute mapping records
    /// </summary>
    /// <param name="productId">Product identifier; 0 to load all records</param>
    /// <param name="specificationAttributeOptionId">The specification attribute option identifier; 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the count
    /// </returns>
    public virtual async Task<int> GetProductSpecificationAttributeCountAsync(int productId = 0, int specificationAttributeOptionId = 0)
    {
        var query = _productSpecificationAttributeRepository.Table;
        if (productId > 0)
            query = query.Where(psa => psa.ProductId == productId);
        if (specificationAttributeOptionId > 0)
            query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);

        return await query.CountAsync();
    }

    /// <summary>
    /// Get mapped products for specification attribute
    /// </summary>
    /// <param name="specificationAttributeId">The specification attribute identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the products
    /// </returns>
    public virtual async Task<IPagedList<Product>> GetProductsBySpecificationAttributeIdAsync(int specificationAttributeId, int pageIndex, int pageSize)
    {
        var query = from product in _productRepository.Table
            join psa in _productSpecificationAttributeRepository.Table on product.Id equals psa.ProductId
            join spao in _specificationAttributeOptionRepository.Table on psa.SpecificationAttributeOptionId equals spao.Id
            where spao.SpecificationAttributeId == specificationAttributeId
            orderby product.Name
            select product;

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    #endregion

    #endregion
}