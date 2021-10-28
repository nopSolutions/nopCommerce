using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Tax;
using Nop.Data;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Tax category service
    /// </summary>
    public partial class TaxCategoryService : ITaxCategoryService
    {
        #region Fields

        protected IRepository<TaxCategory> TaxCategoryRepository { get; }

        #endregion

        #region Ctor

        public TaxCategoryService(IRepository<TaxCategory> taxCategoryRepository)
        {
            TaxCategoryRepository = taxCategoryRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTaxCategoryAsync(TaxCategory taxCategory)
        {
            await TaxCategoryRepository.DeleteAsync(taxCategory);
        }

        /// <summary>
        /// Gets all tax categories
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax categories
        /// </returns>
        public virtual async Task<IList<TaxCategory>> GetAllTaxCategoriesAsync()
        {
            var taxCategories = await TaxCategoryRepository.GetAllAsync(query=>
            {
                return from tc in query
                    orderby tc.DisplayOrder, tc.Id
                    select tc;
            }, cache => default);

            return taxCategories;
        }

        /// <summary>
        /// Gets a tax category
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax category
        /// </returns>
        public virtual async Task<TaxCategory> GetTaxCategoryByIdAsync(int taxCategoryId)
        {
            return await TaxCategoryRepository.GetByIdAsync(taxCategoryId, cache => default);
        }

        /// <summary>
        /// Inserts a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTaxCategoryAsync(TaxCategory taxCategory)
        {
            await TaxCategoryRepository.InsertAsync(taxCategory);
        }

        /// <summary>
        /// Updates the tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTaxCategoryAsync(TaxCategory taxCategory)
        {
            await TaxCategoryRepository.UpdateAsync(taxCategory);
        }

        #endregion
    }
}