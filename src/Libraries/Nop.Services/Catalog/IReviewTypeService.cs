using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Review type service interface
    /// </summary>
    public partial interface IReviewTypeService
    {
        #region ReviewType

        /// <summary>
        /// Delete the review type
        /// </summary>
        /// <param name="reviewType">Review type</param>
        Task DeleteReviewTypeAsync(ReviewType reviewType);

        /// <summary>
        /// Get all review types
        /// </summary>
        /// <returns>Review types</returns>
        Task<IList<ReviewType>> GetAllReviewTypesAsync();

        /// <summary>
        /// Get the review type 
        /// </summary>
        /// <param name="reviewTypeId">Review type identifier</param>
        /// <returns>Review type</returns>
        Task<ReviewType> GetReviewTypeByIdAsync(int reviewTypeId);

        /// <summary>
        /// Insert the review type
        /// </summary>
        /// <param name="reviewType">Review type</param>
        Task InsertReviewTypeAsync(ReviewType reviewType);

        /// <summary>
        /// Update the review type
        /// </summary>
        /// <param name="reviewType">Review type</param>
        Task UpdateReviewTypeAsync(ReviewType reviewType);

        #endregion

        #region ProductReviewReviewTypeMapping

        /// <summary>
        /// Get product review and review type mappings by product review identifier
        /// </summary>
        /// <param name="productReviewId">The product review identifier</param>
        /// <returns>Product review and review type mapping collection</returns>
        Task<IList<ProductReviewReviewTypeMapping>> GetProductReviewReviewTypeMappingsByProductReviewIdAsync(int productReviewId);

        /// <summary>
        /// Inserts a product review and review type mapping
        /// </summary>
        /// <param name="productReviewReviewType">Product review and review type mapping</param>
        Task InsertProductReviewReviewTypeMappingsAsync(ProductReviewReviewTypeMapping productReviewReviewType);

        #endregion
    }
}
