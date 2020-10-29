using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Category template service interface
    /// </summary>
    public partial interface ICategoryTemplateService
    {
        /// <summary>
        /// Delete category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        Task DeleteCategoryTemplateAsync(CategoryTemplate categoryTemplate);

        /// <summary>
        /// Gets all category templates
        /// </summary>
        /// <returns>Category templates</returns>
        Task<IList<CategoryTemplate>> GetAllCategoryTemplatesAsync();

        /// <summary>
        /// Gets a category template
        /// </summary>
        /// <param name="categoryTemplateId">Category template identifier</param>
        /// <returns>Category template</returns>
        Task<CategoryTemplate> GetCategoryTemplateByIdAsync(int categoryTemplateId);

        /// <summary>
        /// Inserts category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        Task InsertCategoryTemplateAsync(CategoryTemplate categoryTemplate);

        /// <summary>
        /// Updates the category template
        /// </summary>
        /// <param name="categoryTemplate">Category template</param>
        Task UpdateCategoryTemplateAsync(CategoryTemplate categoryTemplate);
    }
}
