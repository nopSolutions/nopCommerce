using Nop.Core.Domain.Tax;

namespace Nop.Services.Tax;

/// <summary>
/// Tax category service interface
/// </summary>
public partial interface ITaxCategoryService
{
    /// <summary>
    /// Deletes a tax category
    /// </summary>
    /// <param name="taxCategory">Tax category</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteTaxCategoryAsync(TaxCategory taxCategory);

    /// <summary>
    /// Gets all tax categories
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax categories
    /// </returns>
    Task<IList<TaxCategory>> GetAllTaxCategoriesAsync();

    /// <summary>
    /// Gets a tax category
    /// </summary>
    /// <param name="taxCategoryId">Tax category identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax category
    /// </returns>
    Task<TaxCategory> GetTaxCategoryByIdAsync(int taxCategoryId);

    /// <summary>
    /// Inserts a tax category
    /// </summary>
    /// <param name="taxCategory">Tax category</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertTaxCategoryAsync(TaxCategory taxCategory);

    /// <summary>
    /// Updates the tax category
    /// </summary>
    /// <param name="taxCategory">Tax category</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateTaxCategoryAsync(TaxCategory taxCategory);
}