using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the product review model factory
/// </summary>
public partial interface IProductReviewModelFactory
{
    #region ProducrReview

    /// <summary>
    /// Prepare product review search model
    /// </summary>
    /// <param name="searchModel">Product review search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product review search model
    /// </returns>
    Task<ProductReviewSearchModel> PrepareProductReviewSearchModelAsync(ProductReviewSearchModel searchModel);

    /// <summary>
    /// Prepare paged product review list model
    /// </summary>
    /// <param name="searchModel">Product review search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product review list model
    /// </returns>
    Task<ProductReviewListModel> PrepareProductReviewListModelAsync(ProductReviewSearchModel searchModel);

    /// <summary>
    /// Prepare product review model
    /// </summary>
    /// <param name="model">Product review model</param>
    /// <param name="productReview">Product review</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product review model
    /// </returns>
    Task<ProductReviewModel> PrepareProductReviewModelAsync(ProductReviewModel model,
        ProductReview productReview, bool excludeProperties = false);

    #endregion

    #region ProductReviewReveiwTypeMapping

    /// <summary>
    /// Prepare paged product reviews mapping list model
    /// </summary>
    /// <param name="searchModel">Product review and review type mapping search model</param>
    /// <param name="productReview">Product review</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product review and review type mapping list model
    /// </returns>
    Task<ProductReviewReviewTypeMappingListModel> PrepareProductReviewReviewTypeMappingListModelAsync(ProductReviewReviewTypeMappingSearchModel searchModel,
        ProductReview productReview);

    #endregion
}