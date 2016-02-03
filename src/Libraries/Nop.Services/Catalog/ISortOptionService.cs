using System.Collections.Generic;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Sort option service interface
    /// </summary>
    public partial interface ISortOptionService
    {
        /// <summary>
        /// Delete a sort option
        /// </summary>
        /// <param name="sortOption">Sort option</param>
        void DeleteSortOption(SortOption sortOption);

        /// <summary>
        /// Gets all sort options
        /// </summary>
        /// <returns>Return all sort options</returns>
        IList<SortOption> GetAllSortOptions();

        /// <summary>
        /// Gets active sort options
        /// </summary>
        /// <returns>Return active sort options</returns>
        IList<SortOption> GetActiveSortOptions();

        /// <summary>
        /// Gets a sort option
        /// </summary>
        /// <param name="sortOptionId">Sort option identifier</param>
        /// <returns>Return sort option</returns>
        SortOption GetSortOptionById(int sortOptionId);

        /// <summary>
        /// Inserts a sort option
        /// </summary>
        /// <param name="sortOption">Sort option</param>
        void InsertSortOption(SortOption sortOption);

        /// <summary>
        /// Updates the sort option
        /// </summary>
        /// <param name="sortOption">Sort option</param>
        void UpdateSortOption(SortOption sortOption);
    }
}
