using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Data.Extensions;

namespace Nop.Services.Common
{
    /// <summary>
    /// Search term service
    /// </summary>
    public partial class SearchTermService : ISearchTermService
    {
        #region Fields

        private readonly IRepository<SearchTerm> _searchTermRepository;

        #endregion

        #region Ctor

        public SearchTermService(IRepository<SearchTerm> searchTermRepository)
        {
            _searchTermRepository = searchTermRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a search term record by keyword
        /// </summary>
        /// <param name="keyword">Search term keyword</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the search term
        /// </returns>
        public virtual async Task<SearchTerm> GetSearchTermByKeywordAsync(string keyword, int storeId)
        {
            if (string.IsNullOrEmpty(keyword))
                return null;

            var query = from st in _searchTermRepository.Table
                        where st.Keyword == keyword && st.StoreId == storeId
                        orderby st.Id
                        select st;
            var searchTerm = await query.FirstOrDefaultAsync();

            return searchTerm;
        }

        /// <summary>
        /// Gets a search term statistics
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a list search term report lines
        /// </returns>
        public virtual async Task<IPagedList<SearchTermReportLine>> GetStatsAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = (from st in _searchTermRepository.Table
                         group st by st.Keyword into groupedResult
                         select new
                         {
                             Keyword = groupedResult.Key,
                             Count = groupedResult.Sum(o => o.Count)
                         })
                        .OrderByDescending(m => m.Count)
                        .Select(r => new SearchTermReportLine
                        {
                            Keyword = r.Keyword,
                            Count = r.Count
                        });

            var result = await query.ToPagedListAsync(pageIndex, pageSize);

            return result;
        }

        /// <summary>
        /// Inserts a search term record
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertSearchTermAsync(SearchTerm searchTerm)
        {
            await _searchTermRepository.InsertAsync(searchTerm);
        }

        /// <summary>
        /// Updates the search term record
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateSearchTermAsync(SearchTerm searchTerm)
        {
            await _searchTermRepository.UpdateAsync(searchTerm);
        }

        #endregion
    }
}