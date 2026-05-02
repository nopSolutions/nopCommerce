using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog;

/// <summary>
/// Recently viewed products service
/// </summary>
public partial interface IRecentlyViewedProductsService
{
    /// <summary>
    /// Gets a "recently viewed products" list
    /// </summary>
    /// <param name="number">Number of products to load</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the "recently viewed products" list
    /// </returns>
    Task<IList<Product>> GetRecentlyViewedProductsAsync(int number);

    /// <summary>
    /// Adds a product to a recently viewed products list
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddProductToRecentlyViewedListAsync(int productId);
}