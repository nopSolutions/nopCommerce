//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.Common.Utils.Html;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using System.Data.Objects;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.BusinessLogic.Content.NewsManagement
{
    /// <summary>
    /// News service
    /// </summary>
    public partial class NewsService : INewsService
    {
        #region Constants
        private const string NEWS_BY_ID_KEY = "Nop.news.id-{0}";
        private const string NEWS_PATTERN_KEY = "Nop.news.";
        #endregion

        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        private readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public NewsService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a news
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        /// <returns>News</returns>
        public News GetNewsById(int newsId)
        {
            if (newsId == 0)
                return null;

            string key = string.Format(NEWS_BY_ID_KEY, newsId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (News)obj2;
            }

            
            var query = from n in _context.News
                        where n.NewsId == newsId
                        select n;
            var news = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, news);
            }
            return news;
        }

        /// <summary>
        /// Deletes a news
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        public void DeleteNews(int newsId)
        {
            var news = GetNewsById(newsId);
            if (news == null)
                return;

            
            if (!_context.IsAttached(news))
                _context.News.Attach(news);
            _context.DeleteObject(news);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all news
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <returns>News item collection</returns>
        public List<News> GetAllNews(int languageId)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetAllNews(languageId, showHidden);
        }

        /// <summary>
        /// Gets news item collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>News item collection</returns>
        public List<News> GetAllNews(int languageId, bool showHidden)
        {
            return GetAllNews(languageId, showHidden, Int32.MaxValue);
        }
        
        /// <summary>
        /// Gets news item collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="count">News count to return</param>
        /// <returns>News item collection</returns>
        public List<News> GetAllNews(int languageId, int count)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetAllNews(languageId, showHidden, count);
        }

        /// <summary>
        /// Gets news item collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="count">News count to return</param>
        /// <returns>News item collection</returns>
        public List<News> GetAllNews(int languageId, bool showHidden, int count)
        {
            return GetAllNews(languageId, showHidden, 0, count);
        }

        /// <summary>
        /// Gets news item collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <returns>News item collection</returns>
        public PagedList<News> GetAllNews(int languageId, int pageIndex, int pageSize)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetAllNews(languageId, showHidden, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets news item collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <returns>News item collection</returns>
        public PagedList<News> GetAllNews(int languageId, bool showHidden, 
            int pageIndex, int pageSize)
        {
            if(pageSize <= 0)
            {
                pageSize = 10;
            }
            if(pageSize == Int32.MaxValue)
            {
                pageSize = Int32.MaxValue - 1;
            }
            if(pageIndex < 0)
            {
                pageIndex = 0;
            }
            if(pageIndex == Int32.MaxValue)
            {
                pageIndex = Int32.MaxValue - 1;
            }

            
            var query = from n in _context.News
                        where (showHidden || n.Published) &&
                        (languageId == 0 || languageId == n.LanguageId)
                        orderby n.CreatedOn descending
                        select n;
            var news = new PagedList<News>(query, pageIndex, pageSize);
            return news;
        }

        /// <summary>
        /// Inserts a news item
        /// </summary>
        /// <param name="news">News item</param>
        public void InsertNews(News news)
        {
            if (news == null)
                throw new ArgumentNullException("news");

            news.Title = CommonHelper.EnsureNotNull(news.Title);
            news.Title = CommonHelper.EnsureMaximumLength(news.Title, 1000);
            news.Short = CommonHelper.EnsureNotNull(news.Short);
            news.Short = CommonHelper.EnsureMaximumLength(news.Short, 4000);
            news.Full = CommonHelper.EnsureNotNull(news.Full);

            

            _context.News.AddObject(news);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the news item
        /// </summary>
        /// <param name="news">News item</param>
        public void UpdateNews(News news)
        {
            if (news == null)
                throw new ArgumentNullException("news");

            news.Title = CommonHelper.EnsureNotNull(news.Title);
            news.Title = CommonHelper.EnsureMaximumLength(news.Title, 1000);
            news.Short = CommonHelper.EnsureNotNull(news.Short);
            news.Short = CommonHelper.EnsureMaximumLength(news.Short, 4000);
            news.Full = CommonHelper.EnsureNotNull(news.Full);

            
            if (!_context.IsAttached(news))
                _context.News.Attach(news);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a news comment
        /// </summary>
        /// <param name="newsCommentId">News comment identifer</param>
        /// <returns>News comment</returns>
        public NewsComment GetNewsCommentById(int newsCommentId)
        {
            if (newsCommentId == 0)
                return null;

            
            var query = from nc in _context.NewsComments
                        where nc.NewsCommentId == newsCommentId
                        select nc;
            var newsComment = query.SingleOrDefault();
            return newsComment;
        }

        /// <summary>
        /// Gets a news comment collection by news identifier
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        /// <returns>News comment collection</returns>
        public List<NewsComment> GetNewsCommentsByNewsId(int newsId)
        {
            
            var query = from nc in _context.NewsComments
                        orderby nc.CreatedOn
                        where nc.NewsId == newsId
                        select nc;
            var newsComments = query.ToList();
            return newsComments;
        }

        /// <summary>
        /// Deletes a news comment
        /// </summary>
        /// <param name="newsCommentId">The news comment identifier</param>
        public void DeleteNewsComment(int newsCommentId)
        {
            var newsComment = GetNewsCommentById(newsCommentId);
            if (newsComment == null)
                return;

            
            if (!_context.IsAttached(newsComment))
                _context.NewsComments.Attach(newsComment);
            _context.DeleteObject(newsComment);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets all news comments
        /// </summary>
        /// <returns>News comment collection</returns>
        public List<NewsComment> GetAllNewsComments()
        {
            
            var query = from nc in _context.NewsComments
                        orderby nc.CreatedOn
                        select nc;
            var newsComments = query.ToList();
            return newsComments;
        }

        /// <summary>
        /// Inserts a news comment
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="title">The title</param>
        /// <param name="comment">The comment</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <returns>News comment</returns>
        public NewsComment InsertNewsComment(int newsId, int customerId,
            string title, string comment, DateTime createdOn)
        {
            return InsertNewsComment(newsId, customerId, title, comment, 
                createdOn, this.NotifyAboutNewNewsComments);
        }

        /// <summary>
        /// Inserts a news comment
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="title">The title</param>
        /// <param name="comment">The comment</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="notify">A value indicating whether to notify the store owner</param>
        /// <returns>News comment</returns>
        public NewsComment InsertNewsComment(int newsId, int customerId,
            string title, string comment, DateTime createdOn, bool notify)
        {
            string IPAddress = NopContext.Current.UserHostAddress;
            return InsertNewsComment(newsId, customerId, IPAddress, title, comment, createdOn, notify);
        }

        /// <summary>
        /// Inserts a news comment
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="ipAddress">The IP address</param>
        /// <param name="title">The title</param>
        /// <param name="comment">The comment</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="notify">A value indicating whether to notify the store owner</param>
        /// <returns>News comment</returns>
        public NewsComment InsertNewsComment(int newsId, int customerId, string ipAddress,
            string title, string comment, DateTime createdOn, bool notify)
        {
            ipAddress = CommonHelper.EnsureNotNull(ipAddress);
            ipAddress = CommonHelper.EnsureMaximumLength(ipAddress, 100);
            title = CommonHelper.EnsureNotNull(title);
            title = CommonHelper.EnsureMaximumLength(title, 1000);
            comment = CommonHelper.EnsureNotNull(comment);

            

            var newsComment = _context.NewsComments.CreateObject();
            newsComment.NewsId = newsId;
            newsComment.CustomerId = customerId;
            newsComment.IPAddress = ipAddress;
            newsComment.Title = title;
            newsComment.Comment = comment;
            newsComment.CreatedOn = createdOn;

            _context.NewsComments.AddObject(newsComment);
            _context.SaveChanges();
            
            //notifications
            if (notify)
            {
                IoC.Resolve<IMessageService>().SendNewsCommentNotificationMessage(newsComment, IoC.Resolve<ILocalizationManager>().DefaultAdminLanguage.LanguageId);
            }

            return newsComment;
        }

        /// <summary>
        /// Updates the news comment
        /// </summary>
        /// <param name="newsComment">News comment</param>
        public void UpdateNewsComment(NewsComment newsComment)
        {
            if (newsComment == null)
                throw new ArgumentNullException("newsComment");

            newsComment.IPAddress = CommonHelper.EnsureNotNull(newsComment.IPAddress);
            newsComment.IPAddress = CommonHelper.EnsureMaximumLength(newsComment.IPAddress, 100);
            newsComment.Title = CommonHelper.EnsureNotNull(newsComment.Title);
            newsComment.Title = CommonHelper.EnsureMaximumLength(newsComment.Title, 1000);
            newsComment.Comment = CommonHelper.EnsureNotNull(newsComment.Comment);

            
            if (!_context.IsAttached(newsComment))
                _context.NewsComments.Attach(newsComment);

            _context.SaveChanges();
        }
                
        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.NewsManager.CacheEnabled");
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether news are enabled
        /// </summary>
        public bool NewsEnabled
        {
            get
            {
                bool newsEnabled = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("News.NewsEnabled");
                return newsEnabled;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("News.NewsEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether not registered user can leave comments
        /// </summary>
        public bool AllowNotRegisteredUsersToLeaveComments
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("News.AllowNotRegisteredUsersToLeaveComments");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("News.AllowNotRegisteredUsersToLeaveComments", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to notify about new news comments
        /// </summary>
        public bool NotifyAboutNewNewsComments
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("News.NotifyAboutNewNewsComments");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("News.NotifyAboutNewNewsComments", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show news on the main page
        /// </summary>
        public bool ShowNewsOnMainPage
        {
            get
            {
                bool showNewsOnMainPage = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ShowNewsOnMainPage");
                return showNewsOnMainPage;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Display.ShowNewsOnMainPage", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating news count displayed on the main page
        /// </summary>
        public int MainPageNewsCount
        {
            get
            {
                int mainPageNewsCount = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Display.MainPageNewsCount");
                return mainPageNewsCount;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Display.MainPageNewsCount", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the page size for news archive
        /// </summary>
        public int NewsArchivePageSize
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueInteger("Display.NewsArchivePageSize", 10);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Display.NewsArchivePageSize", value.ToString());
            }
        }


        #endregion
    }
}
