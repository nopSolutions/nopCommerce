using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;

namespace Nop.Services.News
{
    /// <summary>
    /// News service
    /// </summary>
    public partial class NewsService : INewsService
    {
        #region Fields

        private readonly IRepository<NewsItem> _newsItemRepository;
        private readonly IRepository<NewsComment> _newsCommentRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public NewsService(IRepository<NewsItem> newsItemRepository, 
            IRepository<NewsComment> newsCommentRepository,
            IRepository<StoreMapping> storeMappingRepository,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher)
        {
            this._newsItemRepository = newsItemRepository;
            this._newsCommentRepository = newsCommentRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a news
        /// </summary>
        /// <param name="newsItem">News item</param>
        public virtual void DeleteNews(NewsItem newsItem)
        {
            if (newsItem == null)
                throw new ArgumentNullException("newsItem");

            _newsItemRepository.Delete(newsItem);
            
            //event notification
            _eventPublisher.EntityDeleted(newsItem);
        }

        /// <summary>
        /// Gets a news
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        /// <returns>News</returns>
        public virtual NewsItem GetNewsById(int newsId)
        {
            if (newsId == 0)
                return null;

            return _newsItemRepository.GetById(newsId);
        }

        /// <summary>
        /// Gets news
        /// </summary>
        /// <param name="newsIds">The news identifiers</param>
        /// <returns>News</returns>
        public virtual IList<NewsItem> GetNewsByIds(int[] newsIds)
        {
            var query = _newsItemRepository.Table;
            return query.Where(p => newsIds.Contains(p.Id)).ToList();
        }

        /// <summary>
        /// Gets all news
        /// </summary>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>News items</returns>
        public virtual IPagedList<NewsItem> GetAllNews(int languageId = 0, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _newsItemRepository.Table;
            if (languageId > 0)
                query = query.Where(n => languageId == n.LanguageId);
            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(n => n.Published);
                query = query.Where(n => !n.StartDateUtc.HasValue || n.StartDateUtc <= utcNow);
                query = query.Where(n => !n.EndDateUtc.HasValue || n.EndDateUtc >= utcNow);
            }
            query = query.OrderByDescending(n => n.StartDateUtc ?? n.CreatedOnUtc);

            //Store mapping
            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                query = from n in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = n.Id, c2 = "NewsItem" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into n_sm
                        from sm in n_sm.DefaultIfEmpty()
                        where !n.LimitedToStores || storeId == sm.StoreId
                        select n;

                //only distinct items (group by ID)
                query = from n in query
                        group n by n.Id
                        into nGroup
                        orderby nGroup.Key
                        select nGroup.FirstOrDefault();
                query = query.OrderByDescending(n => n.StartDateUtc ?? n.CreatedOnUtc);
            }

            var news = new PagedList<NewsItem>(query, pageIndex, pageSize);
            return news;
        }

        /// <summary>
        /// Inserts a news item
        /// </summary>
        /// <param name="news">News item</param>
        public virtual void InsertNews(NewsItem news)
        {
            if (news == null)
                throw new ArgumentNullException("news");

            _newsItemRepository.Insert(news);

            //event notification
            _eventPublisher.EntityInserted(news);
        }

        /// <summary>
        /// Updates the news item
        /// </summary>
        /// <param name="news">News item</param>
        public virtual void UpdateNews(NewsItem news)
        {
            if (news == null)
                throw new ArgumentNullException("news");

            _newsItemRepository.Update(news);
            
            //event notification
            _eventPublisher.EntityUpdated(news);
        }
        
        /// <summary>
        /// Gets all comments
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <returns>Comments</returns>
        public virtual IList<NewsComment> GetAllComments(int customerId)
        {
            var query = _newsCommentRepository.Table;
            if (customerId > 0)
                query = query.Where(nc => nc.CustomerId == customerId);
            query = query.OrderBy(nc => nc.CreatedOnUtc);
            var comments = query.ToList();
            return comments;
        }

        /// <summary>
        /// Gets a news comment
        /// </summary>
        /// <param name="newsCommentId">News comment identifier</param>
        /// <returns>News comment</returns>
        public virtual NewsComment GetNewsCommentById(int newsCommentId)
        {
            if (newsCommentId == 0)
                return null;

            return _newsCommentRepository.GetById(newsCommentId);
        }

        /// <summary>
        /// Get news comments by identifiers
        /// </summary>
        /// <param name="commentIds">News comment identifiers</param>
        /// <returns>News comments</returns>
        public virtual IList<NewsComment> GetNewsCommentsByIds(int[] commentIds)
        {
            if (commentIds == null || commentIds.Length == 0)
                return new List<NewsComment>();

            var query = from nc in _newsCommentRepository.Table
                        where commentIds.Contains(nc.Id)
                        select nc;
            var comments = query.ToList();
            //sort by passed identifiers
            var sortedComments = new List<NewsComment>();
            foreach (int id in commentIds)
            {
                var comment = comments.Find(x => x.Id == id);
                if (comment != null)
                    sortedComments.Add(comment);
            }
            return sortedComments;
        }

        /// <summary>
        /// Deletes a news comment
        /// </summary>
        /// <param name="newsComment">News comment</param>
        public virtual void DeleteNewsComment(NewsComment newsComment)
        {
            if (newsComment == null)
                throw new ArgumentNullException("newsComment");

            _newsCommentRepository.Delete(newsComment);
        }

        /// <summary>
        /// Deletes a news comments
        /// </summary>
        /// <param name="newsComments">News comments</param>
        public virtual void DeleteNewsComments(IList<NewsComment> newsComments)
        {
            if (newsComments == null)
                throw new ArgumentNullException("newsComments");

            _newsCommentRepository.Delete(newsComments);
        }
        #endregion
    }
}
