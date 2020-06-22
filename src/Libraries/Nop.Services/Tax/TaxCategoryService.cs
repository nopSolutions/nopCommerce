using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Tax category service
    /// </summary>
    public partial class TaxCategoryService : ITaxCategoryService
    {
        #region Fields

        private readonly ICacheKeyService _cacheKeyService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<TaxCategory> _taxCategoryRepository;

        #endregion

        #region Ctor

        public TaxCategoryService(ICacheKeyService cacheKeyService,
            IEventPublisher eventPublisher,
            IRepository<TaxCategory> taxCategoryRepository)
        {
            _cacheKeyService = cacheKeyService;
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

            //event notification
            _eventPublisher.EntityDeleted(taxCategory);
        }

        /// <summary>
        /// Gets all tax categories
        /// </summary>
        /// <returns>Tax categories</returns>
        public virtual IList<TaxCategory> GetAllTaxCategories()
        {
            var query = from tc in _taxCategoryRepository.Table
                orderby tc.DisplayOrder, tc.Id
                select tc;

            var taxCategories = query.ToCachedList(_cacheKeyService.PrepareKeyForDefaultCache(NopTaxDefaults.TaxCategoriesAllCacheKey));

            return taxCategories;
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

            return _taxCategoryRepository.ToCachedGetById(taxCategoryId);
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

            //event notification
            _eventPublisher.EntityUpdated(taxCategory);
        }

        #endregion
    }
}