using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Tax;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Tax category service interface
    /// </summary>
    public partial interface ITaxCategoryService
    {
        /// <summary>
        /// Deletes a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        Task DeleteTaxCategoryAsync(TaxCategory taxCategory);

        /// <summary>
        /// Gets all tax categories
        /// </summary>
        /// <returns>Tax categories</returns>
        Task<IList<TaxCategory>> GetAllTaxCategoriesAsync();

        /// <summary>
        /// Gets a tax category
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <returns>Tax category</returns>
        Task<TaxCategory> GetTaxCategoryByIdAsync(int taxCategoryId);

        /// <summary>
        /// Inserts a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        Task InsertTaxCategoryAsync(TaxCategory taxCategory);

        /// <summary>
        /// Updates the tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        Task UpdateTaxCategoryAsync(TaxCategory taxCategory);
    }
}
