using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product template interface
    /// </summary>
    public partial interface IProductTemplateService
    {
        /// <summary>
        /// Delete product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        Task DeleteProductTemplateAsync(ProductTemplate productTemplate);

        /// <summary>
        /// Gets all product templates
        /// </summary>
        /// <returns>Product templates</returns>
        Task<IList<ProductTemplate>> GetAllProductTemplatesAsync();

        /// <summary>
        /// Gets a product template
        /// </summary>
        /// <param name="productTemplateId">Product template identifier</param>
        /// <returns>Product template</returns>
        Task<ProductTemplate> GetProductTemplateByIdAsync(int productTemplateId);

        /// <summary>
        /// Inserts product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        Task InsertProductTemplateAsync(ProductTemplate productTemplate);

        /// <summary>
        /// Updates the product template
        /// </summary>
        /// <param name="productTemplate">Product template</param>
        Task UpdateProductTemplateAsync(ProductTemplate productTemplate);
    }
}
