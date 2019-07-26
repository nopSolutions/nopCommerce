using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Tax;
using Nop.Services.Events;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Tax category service
    /// </summary>
    public partial class TaxCategoryService : ITaxCategoryService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<TaxCategory> _taxCategoryRepository;

        #endregion

        #region Ctor

        public TaxCategoryService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<TaxCategory> taxCategoryRepository)
        {
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _taxCategoryRepository = taxCategoryRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        public virtual void DeleteTaxCategory(TaxCategory taxCategory)
        {
            if (taxCategory == null)
                throw new ArgumentNullException(nameof(taxCategory));

            _taxCategoryRepository.Delete(taxCategory);

            _cacheManager.RemoveByPrefix(NopTaxDefaults.TaxCategoriesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(taxCategory);
        }

        /// <summary>
        /// Gets all tax categories
        /// </summary>
        /// <returns>Tax categories</returns>
        public virtual IList<TaxCategory> GetAllTaxCategories()
        {
            return _cacheManager.Get(NopTaxDefaults.TaxCategoriesAllCacheKey, () =>
            {
                var query = from tc in _taxCategoryRepository.Table
                            orderby tc.DisplayOrder, tc.Id
                            select tc;
                var taxCategories = query.ToList();
                return taxCategories;
            });
        }

        /// <summary>
        /// Gets a tax category
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <returns>Tax category</returns>
        public virtual TaxCategory GetTaxCategoryById(int taxCategoryId)
        {
            if (taxCategoryId == 0)
                return null;

            var key = string.Format(NopTaxDefaults.TaxCategoriesByIdCacheKey, taxCategoryId);
            return _cacheManager.Get(key, () => _taxCategoryRepository.GetById(taxCategoryId));
        }

        /// <summary>
        /// Inserts a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        public virtual void InsertTaxCategory(TaxCategory taxCategory)
        {
            if (taxCategory == null)
                throw new ArgumentNullException(nameof(taxCategory));

            _taxCategoryRepository.Insert(taxCategory);

            _cacheManager.RemoveByPrefix(NopTaxDefaults.TaxCategoriesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(taxCategory);
        }

        /// <summary>
        /// Updates the tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        public virtual void UpdateTaxCategory(TaxCategory taxCategory)
        {
            if (taxCategory == null)
                throw new ArgumentNullException(nameof(taxCategory));

            _taxCategoryRepository.Update(taxCategory);

            _cacheManager.RemoveByPrefix(NopTaxDefaults.TaxCategoriesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(taxCategory);
        }

        #endregion
    }
}