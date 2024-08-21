using Nop.Core;
using Nop.Core.Domain.News;

namespace Nop.Services.News;

/// <summary>
/// News service interface
/// </summary>
public partial interface INewsService
{
    #region News

    /// <summary>
    /// Deletes a news
    /// </summary>
    /// <param name="newsItem">News item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteNewsAsync(NewsItem newsItem);

    /// <summary>
    /// Gets a news
    /// </summary>
    /// <param name="newsId">The news identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news
    /// </returns>
    Task<NewsItem> GetNewsByIdAsync(int newsId);

    /// <summary>
    /// Gets all news
    /// </summary>
    /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
    /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <param name="title">Filter by news item title</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news items
    /// </returns>
    Task<IPagedList<NewsItem>> GetAllNewsAsync(int languageId = 0, int storeId = 0,
        int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, string title = null);

    /// <summary>
    /// Inserts a news item
    /// </summary>
    /// <param name="news">News item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertNewsAsync(NewsItem news);

    /// <summary>
    /// Updates the news item
    /// </summary>
    /// <param name="news">News item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateNewsAsync(NewsItem news);

    /// <summary>
    /// Get a value indicating whether a news item is available now (availability dates)
    /// </summary>
    /// <param name="newsItem">News item</param>
    /// <param name="dateTime">Datetime to check; pass null to use current date</param>
    /// <returns>Result</returns>
    bool IsNewsAvailable(NewsItem newsItem, DateTime? dateTime = null);

    #endregion

    #region News comments

    /// <summary>
    /// Gets all comments
    /// </summary>
    /// <param name="customerId">Customer identifier; 0 to load all records</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="newsItemId">News item ID; 0 or null to load all records</param>
    /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
    /// <param name="fromUtc">Item creation from; null to load all records</param>
    /// <param name="toUtc">Item creation to; null to load all records</param>
    /// <param name="commentText">Search comment text; null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the comments
    /// </returns>
    Task<IList<NewsComment>> GetAllCommentsAsync(int customerId = 0, int storeId = 0, int? newsItemId = null,
        bool? approved = null, DateTime? fromUtc = null, DateTime? toUtc = null, string commentText = null);

    /// <summary>
    /// Gets a news comment
    /// </summary>
    /// <param name="newsCommentId">News comment identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news comment
    /// </returns>
    Task<NewsComment> GetNewsCommentByIdAsync(int newsCommentId);

    /// <summary>
    /// Get news comments by identifiers
    /// </summary>
    /// <param name="commentIds">News comment identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news comments
    /// </returns>
    Task<IList<NewsComment>> GetNewsCommentsByIdsAsync(int[] commentIds);

    /// <summary>
    /// Get the count of news comments
    /// </summary>
    /// <param name="newsItem">News item</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="isApproved">A value indicating whether to count only approved or not approved comments; pass null to get number of all comments</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of news comments
    /// </returns>
    Task<int> GetNewsCommentsCountAsync(NewsItem newsItem, int storeId = 0, bool? isApproved = null);

    /// <summary>
    /// Deletes a news comment
    /// </summary>
    /// <param name="newsComment">News comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteNewsCommentAsync(NewsComment newsComment);

    /// <summary>
    /// Deletes a news comments
    /// </summary>
    /// <param name="newsComments">News comments</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteNewsCommentsAsync(IList<NewsComment> newsComments);

    /// <summary>
    /// Inserts a news comment
    /// </summary>
    /// <param name="comment">News comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertNewsCommentAsync(NewsComment comment);

    /// <summary>
    /// Update a news comment
    /// </summary>
    /// <param name="comment">News comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateNewsCommentAsync(NewsComment comment);

    #endregion
}