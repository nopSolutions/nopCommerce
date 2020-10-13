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

        private readonly IRepository<TaxCategory> _taxCategoryRepository;

        #endregion

        #region Ctor

        public TaxCategoryService(IRepository<TaxCategory> taxCategoryRepository)
        {
            _taxCategoryRepository = taxCategoryRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        public virtual async Task DeleteTaxCategory(TaxCategory taxCategory)
        {
            await _taxCategoryRepository.Delete(taxCategory);
        }

        /// <summary>
        /// Gets all tax categories
        /// </summary>
        /// <returns>Tax categories</returns>
        public virtual async Task<IList<TaxCategory>> GetAllTaxCategories()
        {
            var taxCategories = await _taxCategoryRepository.GetAll(query=>
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
        /// <returns>Tax category</returns>
        public virtual async Task<TaxCategory> GetTaxCategoryById(int taxCategoryId)
        {
            return await _taxCategoryRepository.GetById(taxCategoryId, cache => default);
        }

        /// <summary>
        /// Inserts a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        public virtual async Task InsertTaxCategory(TaxCategory taxCategory)
        {
            await _taxCategoryRepository.Insert(taxCategory);
        }

        /// <summary>
        /// Updates the tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        public virtual async Task UpdateTaxCategory(TaxCategory taxCategory)
        {
            await _taxCategoryRepository.Update(taxCategory);
        }

        #endregion
    }
}