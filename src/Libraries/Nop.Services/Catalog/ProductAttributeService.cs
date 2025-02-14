using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data;

namespace Nop.Services.Catalog;

/// <summary>
/// Product attribute service
/// </summary>
public partial class ProductAttributeService : IProductAttributeService
{
    #region Fields

    protected readonly IRepository<Picture> _pictureRepository;
    protected readonly IRepository<PredefinedProductAttributeValue> _predefinedProductAttributeValueRepository;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IRepository<ProductAttribute> _productAttributeRepository;
    protected readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
    protected readonly IRepository<ProductAttributeCombinationPicture> _productAttributeCombinationPictureRepository;
    protected readonly IRepository<ProductAttributeMapping> _productAttributeMappingRepository;
    protected readonly IRepository<ProductAttributeValue> _productAttributeValueRepository;
    protected readonly IRepository<ProductAttributeValuePicture> _productAttributeValuePictureRepository;
    protected readonly IRepository<ProductPicture> _productPictureRepository;
    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public ProductAttributeService(IRepository<Picture> pictureRepository,
        IRepository<PredefinedProductAttributeValue> predefinedProductAttributeValueRepository,
        IRepository<Product> productRepository,
        IRepository<ProductAttribute> productAttributeRepository,
        IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
        IRepository<ProductAttributeCombinationPicture> productAttributeCombinationPictureRepository,
        IRepository<ProductAttributeMapping> productAttributeMappingRepository,
        IRepository<ProductAttributeValue> productAttributeValueRepository,
        IRepository<ProductAttributeValuePicture> productAttributeValuePictureRepository,
        IRepository<ProductPicture> productPictureRepository,
        IStaticCacheManager staticCacheManager)
    {
        _pictureRepository = pictureRepository;
        _predefinedProductAttributeValueRepository = predefinedProductAttributeValueRepository;
        _productRepository = productRepository;
        _productAttributeRepository = productAttributeRepository;
        _productAttributeCombinationRepository = productAttributeCombinationRepository;
        _productAttributeCombinationPictureRepository = productAttributeCombinationPictureRepository;
        _productAttributeMappingRepository = productAttributeMappingRepository;
        _productAttributeValueRepository = productAttributeValueRepository;
        _productAttributeValuePictureRepository = productAttributeValuePictureRepository;
        _productPictureRepository = productPictureRepository;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    #region Product attributes

    /// <summary>
    /// Deletes a product attribute
    /// </summary>
    /// <param name="productAttribute">Product attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeAsync(ProductAttribute productAttribute)
    {
        await _productAttributeRepository.DeleteAsync(productAttribute);
    }

    /// <summary>
    /// Deletes product attributes
    /// </summary>
    /// <param name="productAttributes">Product attributes</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributesAsync(IList<ProductAttribute> productAttributes)
    {
        ArgumentNullException.ThrowIfNull(productAttributes);

        foreach (var productAttribute in productAttributes)
            await DeleteProductAttributeAsync(productAttribute);
    }

    /// <summary>
    /// Gets all product attributes
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attributes
    /// </returns>
    public virtual async Task<IPagedList<ProductAttribute>> GetAllProductAttributesAsync(int pageIndex = 0,
        int pageSize = int.MaxValue)
    {
        var productAttributes = await _productAttributeRepository.GetAllPagedAsync(query =>
        {
            return from pa in query
                orderby pa.Name
                select pa;
        }, pageIndex, pageSize);

        return productAttributes;
    }

    /// <summary>
    /// Gets a product attribute 
    /// </summary>
    /// <param name="productAttributeId">Product attribute identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute 
    /// </returns>
    public virtual async Task<ProductAttribute> GetProductAttributeByIdAsync(int productAttributeId)
    {
        return await _productAttributeRepository.GetByIdAsync(productAttributeId, cache => default);
    }

    /// <summary>
    /// Gets product attributes 
    /// </summary>
    /// <param name="productAttributeIds">Product attribute identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attributes 
    /// </returns>
    public virtual async Task<IList<ProductAttribute>> GetProductAttributeByIdsAsync(int[] productAttributeIds)
    {
        return await _productAttributeRepository.GetByIdsAsync(productAttributeIds);
    }

    /// <summary>
    /// Inserts a product attribute
    /// </summary>
    /// <param name="productAttribute">Product attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeAsync(ProductAttribute productAttribute)
    {
        await _productAttributeRepository.InsertAsync(productAttribute);
    }

    /// <summary>
    /// Updates the product attribute
    /// </summary>
    /// <param name="productAttribute">Product attribute</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeAsync(ProductAttribute productAttribute)
    {
        await _productAttributeRepository.UpdateAsync(productAttribute);
    }

    /// <summary>
    /// Returns a list of IDs of not existing attributes
    /// </summary>
    /// <param name="attributeId">The IDs of the attributes to check</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of IDs not existing attributes
    /// </returns>
    public virtual async Task<int[]> GetNotExistingAttributesAsync(int[] attributeId)
    {
        ArgumentNullException.ThrowIfNull(attributeId);

        var query = _productAttributeRepository.Table;
        var queryFilter = attributeId.Distinct().ToArray();
        var filter = await query.Select(a => a.Id)
            .Where(m => queryFilter.Contains(m))
            .ToListAsync();

        return queryFilter.Except(filter).ToArray();
    }

    #endregion

    #region Product attributes mappings

    /// <summary>
    /// Deletes a product attribute mapping
    /// </summary>
    /// <param name="productAttributeMapping">Product attribute mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
    {
        await _productAttributeMappingRepository.DeleteAsync(productAttributeMapping);
    }

    /// <summary>
    /// Gets product attribute mappings by product identifier
    /// </summary>
    /// <param name="productId">The product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute mapping collection
    /// </returns>
    public virtual async Task<IList<ProductAttributeMapping>> GetProductAttributeMappingsByProductIdAsync(int productId)
    {
        var allCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeMappingsByProductCacheKey, productId);

        var query = from pam in _productAttributeMappingRepository.Table
            orderby pam.DisplayOrder, pam.Id
            where pam.ProductId == productId
            select pam;

        var attributes = await _staticCacheManager.GetAsync(allCacheKey, async () => await query.ToListAsync()) ?? new List<ProductAttributeMapping>();

        return attributes;
    }

    /// <summary>
    /// Gets a product attribute mapping
    /// </summary>
    /// <param name="productAttributeMappingId">Product attribute mapping identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute mapping
    /// </returns>
    public virtual async Task<ProductAttributeMapping> GetProductAttributeMappingByIdAsync(int productAttributeMappingId)
    {
        return await _productAttributeMappingRepository.GetByIdAsync(productAttributeMappingId, cache => default);
    }

    /// <summary>
    /// Inserts a product attribute mapping
    /// </summary>
    /// <param name="productAttributeMapping">The product attribute mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
    {
        await _productAttributeMappingRepository.InsertAsync(productAttributeMapping);
    }

    /// <summary>
    /// Updates the product attribute mapping
    /// </summary>
    /// <param name="productAttributeMapping">The product attribute mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
    {
        await _productAttributeMappingRepository.UpdateAsync(productAttributeMapping);
    }

    #endregion

    #region Product attribute values

    /// <summary>
    /// Deletes a product attribute value
    /// </summary>
    /// <param name="productAttributeValue">Product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
    {
        await _productAttributeValueRepository.DeleteAsync(productAttributeValue);
    }

    /// <summary>
    /// Gets product attribute values by product attribute mapping identifier
    /// </summary>
    /// <param name="productAttributeMappingId">The product attribute mapping identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute mapping collection
    /// </returns>
    public virtual async Task<IList<ProductAttributeValue>> GetProductAttributeValuesAsync(int productAttributeMappingId)
    {
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, productAttributeMappingId);

        var query = from pav in _productAttributeValueRepository.Table
            orderby pav.DisplayOrder, pav.Id
            where pav.ProductAttributeMappingId == productAttributeMappingId
            select pav;
        var productAttributeValues = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

        return productAttributeValues;
    }

    /// <summary>
    /// Gets a product attribute value
    /// </summary>
    /// <param name="productAttributeValueId">Product attribute value identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute value
    /// </returns>
    public virtual async Task<ProductAttributeValue> GetProductAttributeValueByIdAsync(int productAttributeValueId)
    {
        return await _productAttributeValueRepository.GetByIdAsync(productAttributeValueId, cache => default);
    }

    /// <summary>
    /// Inserts a product attribute value
    /// </summary>
    /// <param name="productAttributeValue">The product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
    {
        await _productAttributeValueRepository.InsertAsync(productAttributeValue);
    }

    /// <summary>
    /// Updates the product attribute value
    /// </summary>
    /// <param name="productAttributeValue">The product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
    {
        await _productAttributeValueRepository.UpdateAsync(productAttributeValue);
    }

    #endregion

    #region Product attribute value pictures

    /// <summary>
    /// Deletes a product attribute value picture
    /// </summary>
    /// <param name="value">Product attribute value picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeValuePictureAsync(ProductAttributeValuePicture valuePicture)
    {
        await _productAttributeValuePictureRepository.DeleteAsync(valuePicture);
    }

    /// <summary>
    /// Inserts a product attribute value picture
    /// </summary>
    /// <param name="value">Product attribute value picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeValuePictureAsync(ProductAttributeValuePicture valuePicture)
    {
        await _productAttributeValuePictureRepository.InsertAsync(valuePicture);
    }

    /// <summary>
    /// Updates a product attribute value picture
    /// </summary>
    /// <param name="value">Product attribute value picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeValuePictureAsync(ProductAttributeValuePicture valuePicture)
    {
        await _productAttributeValuePictureRepository.UpdateAsync(valuePicture);
    }

    /// <summary>
    /// Get product attribute value pictures
    /// </summary>
    /// <param name="valueId">Value id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute value pictures
    /// </returns>
    public virtual async Task<IList<ProductAttributeValuePicture>> GetProductAttributeValuePicturesAsync(int valueId)
    {
        var allCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeValuePicturesByValueCacheKey, valueId);

        var query = from pacp in _productAttributeValuePictureRepository.Table
            join p in _pictureRepository.Table on pacp.PictureId equals p.Id
            join pp in _productPictureRepository.Table on p.Id equals pp.PictureId
            where pacp.ProductAttributeValueId == valueId
            orderby pp.DisplayOrder, pacp.PictureId
            select pacp;

        var valuePictures = await _staticCacheManager.GetAsync(allCacheKey, async () => await query.ToListAsync())
                            ?? new List<ProductAttributeValuePicture>();

        return valuePictures;
    }

    /// <summary>
    /// Returns a ProductAttributeValuePicture that has the specified values
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="valueId">Product attribute value identifier</param>
    /// <param name="pictureId">Picture identifier</param>
    /// <returns>A ProductAttributeValuePicture that has the specified values; otherwise null</returns>
    public virtual ProductAttributeValuePicture FindProductAttributeValuePicture(IList<ProductAttributeValuePicture> source, int valueId, int pictureId)
    {
        foreach (var valuePicture in source)
            if (valuePicture.ProductAttributeValueId == valueId && valuePicture.PictureId == pictureId)
                return valuePicture;

        return null;
    }

    #endregion

    #region Predefined product attribute values

    /// <summary>
    /// Deletes a predefined product attribute value
    /// </summary>
    /// <param name="ppav">Predefined product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePredefinedProductAttributeValueAsync(PredefinedProductAttributeValue ppav)
    {
        await _predefinedProductAttributeValueRepository.DeleteAsync(ppav);
    }

    /// <summary>
    /// Gets predefined product attribute values by product attribute identifier
    /// </summary>
    /// <param name="productAttributeId">The product attribute identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute mapping collection
    /// </returns>
    public virtual async Task<IList<PredefinedProductAttributeValue>> GetPredefinedProductAttributeValuesAsync(int productAttributeId)
    {
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.PredefinedProductAttributeValuesByAttributeCacheKey, productAttributeId);

        var query = from ppav in _predefinedProductAttributeValueRepository.Table
            orderby ppav.DisplayOrder, ppav.Id
            where ppav.ProductAttributeId == productAttributeId
            select ppav;

        var values = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

        return values;
    }

    /// <summary>
    /// Gets a predefined product attribute value
    /// </summary>
    /// <param name="id">Predefined product attribute value identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the predefined product attribute value
    /// </returns>
    public virtual async Task<PredefinedProductAttributeValue> GetPredefinedProductAttributeValueByIdAsync(int id)
    {
        return await _predefinedProductAttributeValueRepository.GetByIdAsync(id, cache => default);
    }

    /// <summary>
    /// Inserts a predefined product attribute value
    /// </summary>
    /// <param name="ppav">The predefined product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertPredefinedProductAttributeValueAsync(PredefinedProductAttributeValue ppav)
    {
        await _predefinedProductAttributeValueRepository.InsertAsync(ppav);
    }

    /// <summary>
    /// Updates the predefined product attribute value
    /// </summary>
    /// <param name="ppav">The predefined product attribute value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdatePredefinedProductAttributeValueAsync(PredefinedProductAttributeValue ppav)
    {
        await _predefinedProductAttributeValueRepository.UpdateAsync(ppav);
    }

    #endregion

    #region Product attribute combinations

    /// <summary>
    /// Deletes a product attribute combination
    /// </summary>
    /// <param name="combination">Product attribute combination</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeCombinationAsync(ProductAttributeCombination combination)
    {
        await _productAttributeCombinationRepository.DeleteAsync(combination);
    }

    /// <summary>
    /// Gets all product attribute combinations
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute combinations
    /// </returns>
    public virtual async Task<IList<ProductAttributeCombination>> GetAllProductAttributeCombinationsAsync(int productId)
    {
        if (productId == 0)
            return new List<ProductAttributeCombination>();

        var combinations = await _productAttributeCombinationRepository.GetAllAsync(query =>
        {
            return from c in query
                orderby c.Id
                where c.ProductId == productId
                select c;
        }, cache => cache.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeCombinationsByProductCacheKey, productId));

        return combinations;
    }

    /// <summary>
    /// Gets a product attribute combination
    /// </summary>
    /// <param name="productAttributeCombinationId">Product attribute combination identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute combination
    /// </returns>
    public virtual async Task<ProductAttributeCombination> GetProductAttributeCombinationByIdAsync(int productAttributeCombinationId)
    {
        return await _productAttributeCombinationRepository.GetByIdAsync(productAttributeCombinationId, cache => default);
    }

    /// <summary>
    /// Gets a product attribute combination by SKU
    /// </summary>
    /// <param name="sku">SKU</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute combination
    /// </returns>
    public virtual async Task<ProductAttributeCombination> GetProductAttributeCombinationBySkuAsync(string sku)
    {
        if (string.IsNullOrEmpty(sku))
            return null;

        sku = sku.Trim();

        var query = from pac in _productAttributeCombinationRepository.Table
            join p in _productRepository.Table on pac.ProductId equals p.Id
            orderby pac.Id
            where !p.Deleted && pac.Sku == sku
            select pac;
        var combination = await query.FirstOrDefaultAsync();

        return combination;
    }

    /// <summary>
    /// Inserts a product attribute combination
    /// </summary>
    /// <param name="combination">Product attribute combination</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeCombinationAsync(ProductAttributeCombination combination)
    {
        await _productAttributeCombinationRepository.InsertAsync(combination);
    }

    /// <summary>
    /// Updates a product attribute combination
    /// </summary>
    /// <param name="combination">Product attribute combination</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeCombinationAsync(ProductAttributeCombination combination)
    {
        await _productAttributeCombinationRepository.UpdateAsync(combination);
    }

    #endregion

    #region Product attribute combination pictures

    /// <summary>
    /// Deletes a product attribute combination picture
    /// </summary>
    /// <param name="combination">Product attribute combination picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductAttributeCombinationPictureAsync(ProductAttributeCombinationPicture combinationPicture)
    {
        await _productAttributeCombinationPictureRepository.DeleteAsync(combinationPicture);
    }

    /// <summary>
    /// Inserts a product attribute combination picture
    /// </summary>
    /// <param name="combination">Product attribute combination picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductAttributeCombinationPictureAsync(ProductAttributeCombinationPicture combinationPicture)
    {
        await _productAttributeCombinationPictureRepository.InsertAsync(combinationPicture);
    }

    /// <summary>
    /// Updates a product attribute combination picture
    /// </summary>
    /// <param name="combination">Product attribute combination picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductAttributeCombinationPictureAsync(ProductAttributeCombinationPicture combinationPicture)
    {
        await _productAttributeCombinationPictureRepository.UpdateAsync(combinationPicture);
    }

    /// <summary>
    /// Get product attribute combination pictures
    /// </summary>
    /// <param name="combinationId">Combination id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute combination pictures
    /// </returns>
    public virtual async Task<IList<ProductAttributeCombinationPicture>> GetProductAttributeCombinationPicturesAsync(int combinationId)
    {
        var allCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeCombinationPicturesByCombinationCacheKey, combinationId);

        var query = from pacp in _productAttributeCombinationPictureRepository.Table
            join p in _pictureRepository.Table on pacp.PictureId equals p.Id
            join pp in _productPictureRepository.Table on p.Id equals pp.PictureId
            where pacp.ProductAttributeCombinationId == combinationId
            orderby pp.DisplayOrder, pacp.PictureId
            select pacp;

        var combinationPictures = await _staticCacheManager.GetAsync(allCacheKey, async () => await query.ToListAsync()) 
                                  ?? new List<ProductAttributeCombinationPicture>();

        return combinationPictures;
    }

    /// <summary>
    /// Returns a ProductAttributeCombinationPicture that has the specified values
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="combinationId">Product attribute combination identifier</param>
    /// <param name="pictureId">Picture identifier</param>
    /// <returns>A ProductAttributeCombinationPicture that has the specified values; otherwise null</returns>
    public virtual ProductAttributeCombinationPicture FindProductAttributeCombinationPicture(IList<ProductAttributeCombinationPicture> source, int combinationId, int pictureId)
    {
        foreach (var combinationPicture in source)
            if (combinationPicture.ProductAttributeCombinationId == combinationId && combinationPicture.PictureId == pictureId)
                return combinationPicture;

        return null;
    }

    #endregion

    #endregion
}