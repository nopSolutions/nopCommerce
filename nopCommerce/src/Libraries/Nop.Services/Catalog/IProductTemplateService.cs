using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog;

/// <summary>
/// Product template interface
/// </summary>
public partial interface IProductTemplateService
{
    /// <summary>
    /// Delete product template
    /// </summary>
    /// <param name="productTemplate">Product template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteProductTemplateAsync(ProductTemplate productTemplate);

    /// <summary>
    /// Gets all product templates
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product templates
    /// </returns>
    Task<IList<ProductTemplate>> GetAllProductTemplatesAsync();

    /// <summary>
    /// Gets a product template
    /// </summary>
    /// <param name="productTemplateId">Product template identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product template
    /// </returns>
    Task<ProductTemplate> GetProductTemplateByIdAsync(int productTemplateId);

    /// <summary>
    /// Inserts product template
    /// </summary>
    /// <param name="productTemplate">Product template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertProductTemplateAsync(ProductTemplate productTemplate);

    /// <summary>
    /// Updates the product template
    /// </summary>
    /// <param name="productTemplate">Product template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateProductTemplateAsync(ProductTemplate productTemplate);
}