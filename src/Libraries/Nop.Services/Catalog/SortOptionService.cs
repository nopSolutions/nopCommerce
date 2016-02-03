using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Return sort options service
    /// </summary>
    public partial class SortOptionService : ISortOptionService
    {
        #region Fields

        private readonly IRepository<SortOption> _sortOptionRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="sortOptionRepository">Sort option repository</param>
        /// <param name="eventPublisher">Event published</param>
        public SortOptionService(IRepository<SortOption> sortOptionRepository,
            IEventPublisher eventPublisher)
        {
            this._sortOptionRepository = sortOptionRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a sort option
        /// </summary>
        /// <param name="sortOption">sort option </param>
        public virtual void DeleteSortOption(SortOption sortOption)
        {
            if (sortOption == null)
                throw new ArgumentNullException("sortOption");

            _sortOptionRepository.Delete(sortOption);

            //event notification
            _eventPublisher.EntityDeleted(sortOption);
        }

        /// <summary>
        /// Gets all sort options
        /// </summary>
        /// <returns>sort options</returns>
        public virtual IList<SortOption> GetAllSortOptions()
        {
            var query = from option in _sortOptionRepository.Table
                        orderby option.DisplayOrder, option.SortOptionTypeID
                        select option;
            return query.ToList();
        }

        /// <summary>
        /// Gets active sort options
        /// </summary>
        /// <returns>sort options</returns>
        public virtual IList<SortOption> GetActiveSortOptions()
        {
            var query = from option in _sortOptionRepository.Table
                        where option.IsActive
                        orderby option.DisplayOrder, option.SortOptionTypeID
                        select option;
            return query.ToList();
        }

        /// <summary>
        /// Gets a sort option 
        /// </summary>
        /// <param name="sortOptionId">sort option identifier</param>
        /// <returns>sort option</returns>
        public virtual SortOption GetSortOptionById(int sortOptionId)
        {
            if (sortOptionId == 0)
                return null;

            return _sortOptionRepository.GetById(sortOptionId);
        }

        /// <summary>
        /// Inserts a sort option
        /// </summary>
        /// <param name="sortOption">sort option</param>
        public virtual void InsertSortOption(SortOption sortOption)
        {
            if (sortOption == null)
                throw new ArgumentNullException("sortOption");

            _sortOptionRepository.Insert(sortOption);

            //event notification
            _eventPublisher.EntityInserted(sortOption);
        }

        /// <summary>
        /// Updates the sort option
        /// </summary>
        /// <param name="sortOption">sort option</param>
        public virtual void UpdateSortOption(SortOption sortOption)
        {
            if (sortOption == null)
                throw new ArgumentNullException("sortOption");

            _sortOptionRepository.Update(sortOption);

            //event notification
            _eventPublisher.EntityUpdated(sortOption);
        }

        #endregion
    }
}
