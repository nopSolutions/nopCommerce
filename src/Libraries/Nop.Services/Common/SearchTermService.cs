using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Data;

namespace Nop.Services.Common;

/// <summary>
/// Search term service
/// </summary>
public partial class SearchTermService : ISearchTermService
{
    #region Fields

    protected readonly IRepository<SearchTerm> _searchTermRepository;

    #endregion

    #region Ctor

    public SearchTermService(IRepository<SearchTerm> searchTermRepository)
    {
        _searchTermRepository = searchTermRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a search term records by keyword
    /// </summary>
    /// <param name="keyword">Search term keyword</param>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    public virtual async Task DeleteSearchTermsByAsyncKeyword(string keyword, int customerId, int storeId)
    {
        if (string.IsNullOrEmpty(keyword))
            return;

        await _searchTermRepository.DeleteAsync(t => t.CustomerId == customerId && t.StoreId == storeId && t.Keyword == keyword);
    }

    /// <summary>
    /// Gets a search term record by keyword
    /// </summary>
    /// <param name="keyword">Search term keyword</param>
    /// <param name="customerId">Customer identifier; pass 0 to load all records</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search terms
    /// </returns>
    public virtual async Task<IPagedList<SearchTerm>> GetSearchTermsAsync(string keyword, int customerId = 0, int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var searchTerms = _searchTermRepository.Table;

        if (storeId > 0)
            searchTerms = searchTerms.Where(st => st.StoreId == storeId);

        if (customerId > 0)
            searchTerms = searchTerms.Where(st => st.CustomerId == customerId);

        if (!string.IsNullOrEmpty(keyword))
            searchTerms = searchTerms.Where(st => st.Keyword == keyword);

        var query = from t in searchTerms
                    group t by t.Keyword into groupedResult
                    select new { Id = groupedResult.Max(x => x.Id) };

        return await (from t in searchTerms
                      join ut in query on t.Id equals ut.Id
                      orderby t.CreatedOnUtc descending
                      select t).ToPagedListAsync(pageIndex, pageSize);
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
                         Count = groupedResult.Count()
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