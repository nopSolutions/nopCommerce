using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;

namespace Nop.Services.Catalog;

/// <summary>
/// Review type service implementation
/// </summary>
public partial class ReviewTypeService : IReviewTypeService
{
    #region Fields

    protected readonly IRepository<ProductReviewReviewTypeMapping> _productReviewReviewTypeMappingRepository;
    protected readonly IRepository<ReviewType> _reviewTypeRepository;
    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public ReviewTypeService(IRepository<ProductReviewReviewTypeMapping> productReviewReviewTypeMappingRepository,
        IRepository<ReviewType> reviewTypeRepository,
        IStaticCacheManager staticCacheManager)
    {
        _productReviewReviewTypeMappingRepository = productReviewReviewTypeMappingRepository;
        _reviewTypeRepository = reviewTypeRepository;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    #region Review type

    /// <summary>
    /// Gets all review types
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the review types
    /// </returns>
    public virtual async Task<IList<ReviewType>> GetAllReviewTypesAsync()
    {
        return await _reviewTypeRepository.GetAllAsync(
            query => query.OrderBy(reviewType => reviewType.DisplayOrder).ThenBy(reviewType => reviewType.Id),
            cache => default);
    }

    /// <summary>
    /// Gets a review type 
    /// </summary>
    /// <param name="reviewTypeId">Review type identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the review type
    /// </returns>
    public virtual async Task<ReviewType> GetReviewTypeByIdAsync(int reviewTypeId)
    {
        return await _reviewTypeRepository.GetByIdAsync(reviewTypeId, cache => default);
    }

    /// <summary>
    /// Inserts a review type
    /// </summary>
    /// <param name="reviewType">Review type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertReviewTypeAsync(ReviewType reviewType)
    {
        await _reviewTypeRepository.InsertAsync(reviewType);
    }

    /// <summary>
    /// Updates a review type
    /// </summary>
    /// <param name="reviewType">Review type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateReviewTypeAsync(ReviewType reviewType)
    {
        await _reviewTypeRepository.UpdateAsync(reviewType);
    }

    /// <summary>
    /// Delete review type
    /// </summary>
    /// <param name="reviewType">Review type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteReviewTypeAsync(ReviewType reviewType)
    {
        await _reviewTypeRepository.DeleteAsync(reviewType);
    }

    #endregion

    #region Product review type mapping

    /// <summary>
    /// Gets product review and review type mappings by product review identifier
    /// </summary>
    /// <param name="productReviewId">The product review identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product review and review type mapping collection
    /// </returns>
    public async Task<IList<ProductReviewReviewTypeMapping>> GetProductReviewReviewTypeMappingsByProductReviewIdAsync(
        int productReviewId)
    {
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductReviewTypeMappingByReviewTypeCacheKey, productReviewId);

        var query = from pam in _productReviewReviewTypeMappingRepository.Table
            orderby pam.Id
            where pam.ProductReviewId == productReviewId
            select pam;

        var productReviewReviewTypeMappings = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

        return productReviewReviewTypeMappings;
    }

    /// <summary>
    /// Inserts a product review and review type mapping
    /// </summary>
    /// <param name="productReviewReviewType">Product review and review type mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductReviewReviewTypeMappingsAsync(ProductReviewReviewTypeMapping productReviewReviewType)
    {
        await _productReviewReviewTypeMappingRepository.InsertAsync(productReviewReviewType);
    }

    #endregion

    #endregion
}