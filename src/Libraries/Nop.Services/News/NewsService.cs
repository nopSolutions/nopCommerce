using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.News;
using Nop.Data;
using Nop.Services.Stores;

namespace Nop.Services.News
{
    /// <summary>
    /// News service
    /// </summary>
    public partial class NewsService : INewsService
    {
        #region Fields

        private readonly IRepository<NewsComment> _newsCommentRepository;
        private readonly IRepository<NewsItem> _newsItemRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        public NewsService(
            IRepository<NewsComment> newsCommentRepository,
            IRepository<NewsItem> newsItemRepository,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService)
        {
            _newsCommentRepository = newsCommentRepository;
            _newsItemRepository = newsItemRepository;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;

        }

        #endregion

        #region Methods

        #region News

        /// <summary>
        /// Deletes a news
        /// </summary>
        /// <param name="newsItem">News item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteNewsAsync(NewsItem newsItem)
        {
            await _newsItemRepository.DeleteAsync(newsItem);
        }

        /// <summary>
        /// Gets a news
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the news
        /// </returns>
        public virtual async Task<NewsItem> GetNewsByIdAsync(int newsId)
        {
            return await _newsItemRepository.GetByIdAsync(newsId, cache => default);
        }

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
        public virtual async Task<IPagedList<NewsItem>> GetAllNewsAsync(int languageId = 0, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, string title = null)
        {
            var news = await _newsItemRepository.GetAllPagedAsync(async query =>
            {
                if (languageId > 0)
                    query = query.Where(n => languageId == n.LanguageId);

                if (!string.IsNullOrEmpty(title))
                    query = query.Where(n => n.Title.Contains(title));

                if (!showHidden)
                {
                    var utcNow = DateTime.UtcNow;
                    query = query.Where(n => n.Published);
                    query = query.Where(n => !n.StartDateUtc.HasValue || n.StartDateUtc <= utcNow);
                    query = query.Where(n => !n.EndDateUtc.HasValue || n.EndDateUtc >= utcNow);
                }

                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                return query.OrderByDescending(n => n.StartDateUtc ?? n.CreatedOnUtc);
            }, pageIndex, pageSize);

            return news;
        }

        /// <summary>
        /// Inserts a news item
        /// </summary>
        /// <param name="news">News item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertNewsAsync(NewsItem news)
        {
            await _newsItemRepository.InsertAsync(news);
        }

        /// <summary>
        /// Updates the news item
        /// </summary>
        /// <param name="news">News item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateNewsAsync(NewsItem news)
        {
            await _newsItemRepository.UpdateAsync(news);
        }

        /// <summary>
        /// Get a value indicating whether a news item is available now (availability dates)
        /// </summary>
        /// <param name="newsItem">News item</param>
        /// <param name="dateTime">Datetime to check; pass null to use current date</param>
        /// <returns>Result</returns>
        public virtual bool IsNewsAvailable(NewsItem newsItem, DateTime? dateTime = null)
        {
            if (newsItem == null)
                throw new ArgumentNullException(nameof(newsItem));

            if (newsItem.StartDateUtc.HasValue && newsItem.StartDateUtc.Value >= dateTime)
                return false;

            if (newsItem.EndDateUtc.HasValue && newsItem.EndDateUtc.Value <= dateTime)
                return false;

            return true;
        }
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
        public virtual async Task<IList<NewsComment>> GetAllCommentsAsync(int customerId = 0, int storeId = 0, int? newsItemId = null,
            bool? approved = null, DateTime? fromUtc = null, DateTime? toUtc = null, string commentText = null)
        {
            return await _newsCommentRepository.GetAllAsync(query =>
            {
                if (approved.HasValue)
                    query = query.Where(comment => comment.IsApproved == approved);

                if (newsItemId > 0)
                    query = query.Where(comment => comment.NewsItemId == newsItemId);

                if (customerId > 0)
                    query = query.Where(comment => comment.CustomerId == customerId);

                if (storeId > 0)
                    query = query.Where(comment => comment.StoreId == storeId);

                if (fromUtc.HasValue)
                    query = query.Where(comment => fromUtc.Value <= comment.CreatedOnUtc);

                if (toUtc.HasValue)
                    query = query.Where(comment => toUtc.Value >= comment.CreatedOnUtc);

                if (!string.IsNullOrEmpty(commentText))
                    query = query.Where(
                        c => c.CommentText.Contains(commentText) || c.CommentTitle.Contains(commentText));

                query = query.OrderBy(nc => nc.CreatedOnUtc);

                return query;
            });
        }

        /// <summary>
        /// Gets a news comment
        /// </summary>
        /// <param name="newsCommentId">News comment identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the news comment
        /// </returns>
        public virtual async Task<NewsComment> GetNewsCommentByIdAsync(int newsCommentId)
        {
            return await _newsCommentRepository.GetByIdAsync(newsCommentId, cache => default);
        }

        /// <summary>
        /// Get news comments by identifiers
        /// </summary>
        /// <param name="commentIds">News comment identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the news comments
        /// </returns>
        public virtual async Task<IList<NewsComment>> GetNewsCommentsByIdsAsync(int[] commentIds)
        {
            return await _newsCommentRepository.GetByIdsAsync(commentIds);
        }

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
        public virtual async Task<int> GetNewsCommentsCountAsync(NewsItem newsItem, int storeId = 0, bool? isApproved = null)
        {
            var query = _newsCommentRepository.Table.Where(comment => comment.NewsItemId == newsItem.Id);

            if (storeId > 0)
                query = query.Where(comment => comment.StoreId == storeId);

            if (isApproved.HasValue)
                query = query.Where(comment => comment.IsApproved == isApproved.Value);

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopNewsDefaults.NewsCommentsNumberCacheKey, newsItem, storeId, isApproved);

            return await _staticCacheManager.GetAsync(cacheKey, async () => await query.CountAsync());
        }

        /// <summary>
        /// Deletes a news comment
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteNewsCommentAsync(NewsComment newsComment)
        {
            await _newsCommentRepository.DeleteAsync(newsComment);
        }

        /// <summary>
        /// Deletes a news comments
        /// </summary>
        /// <param name="newsComments">News comments</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteNewsCommentsAsync(IList<NewsComment> newsComments)
        {
            if (newsComments == null)
                throw new ArgumentNullException(nameof(newsComments));

            foreach (var newsComment in newsComments)
                await DeleteNewsCommentAsync(newsComment);
        }

        /// <summary>
        /// Inserts a news comment
        /// </summary>
        /// <param name="comment">News comment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertNewsCommentAsync(NewsComment comment)
        {
            await _newsCommentRepository.InsertAsync(comment);
        }

        /// <summary>
        /// Update a news comment
        /// </summary>
        /// <param name="comment">News comment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateNewsCommentAsync(NewsComment comment)
        {
            await _newsCommentRepository.UpdateAsync(comment);
        }

        #endregion

        #endregion
    }
}