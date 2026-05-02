using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog;

/// <summary>
/// Product review service
/// </summary>
public partial interface IProductReviewService
{
    /// <summary>
    /// Validate product review availability
    /// </summary>
    /// <param name="product">Product to validate review availability</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the validation error list if found
    /// </returns>
    Task<IList<string>> ValidateProductReviewAvailabilityAsync(Product product);

    /// <summary>
    /// Gets all product reviews
    /// </summary>
    /// <param name="customerId">Customer identifier (who wrote a review); 0 to load all records</param>
    /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
    /// <param name="fromUtc">Item creation from; null to load all records</param>
    /// <param name="toUtc">Item creation to; null to load all records</param>
    /// <param name="message">Search title or review text; null to load all records</param>
    /// <param name="storeId">The store identifier; pass 0 to load all records</param>
    /// <param name="productId">The product identifier; pass 0 to load all records</param>
    /// <param name="vendorId">The vendor identifier (limit to products of this vendor); pass 0 to load all records</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the reviews
    /// </returns>
    Task<IPagedList<ProductReview>> GetAllProductReviewsAsync(int customerId = 0, bool? approved = null,
        DateTime? fromUtc = null, DateTime? toUtc = null,
        string message = null, int storeId = 0, int productId = 0, int vendorId = 0, bool showHidden = false,
        int pageIndex = 0, int pageSize = int.MaxValue);

    /// <summary>
    /// Gets product review
    /// </summary>
    /// <param name="productReviewId">Product review identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product review
    /// </returns>
    Task<ProductReview> GetProductReviewByIdAsync(int productReviewId);

    /// <summary>
    /// Get product reviews by identifiers
    /// </summary>
    /// <param name="productReviewIds">Product review identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product reviews
    /// </returns>
    Task<IList<ProductReview>> GetProductReviewsByIdsAsync(int[] productReviewIds);

    /// <summary>
    /// Inserts a product review
    /// </summary>
    /// <param name="productReview">Product review</param>
    /// <param name="productReviewReviewTypeMappings">Review type mappings</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertProductReviewAsync(ProductReview productReview, IList<ProductReviewReviewTypeMapping> productReviewReviewTypeMappings = null);

    /// <summary>
    /// Deletes a product review
    /// </summary>
    /// <param name="productReview">Product review</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteProductReviewAsync(ProductReview productReview);

    /// <summary>
    /// Deletes product reviews
    /// </summary>
    /// <param name="productReviews">Product reviews</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteProductReviewsAsync(IList<ProductReview> productReviews);

    /// <summary>
    /// Sets or create a product review helpfulness record
    /// </summary>
    /// <param name="productReview">Product reviews</param>
    /// <param name="helpfulness">Value indicating whether a review a helpful</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task SetProductReviewHelpfulnessAsync(ProductReview productReview, bool helpfulness);

    /// <summary>
    /// Updates a totals helpfulness count for product review
    /// </summary>
    /// <param name="productReview">Product review</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    Task UpdateProductReviewHelpfulnessTotalsAsync(ProductReview productReview);

    /// <summary>
    /// Updates a product review
    /// </summary>
    /// <param name="productReview">Product review</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateProductReviewAsync(ProductReview productReview);

    /// <summary>
    /// Check possibility added review for current customer
    /// </summary>
    /// <param name="productId">Current product</param>
    /// <param name="storeId">The store identifier; pass 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the 
    /// </returns>
    Task<bool> CanAddReviewAsync(int productId, int storeId = 0);

    /// <summary>
    /// Update product review totals
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateProductReviewTotalsAsync(Product product);
}