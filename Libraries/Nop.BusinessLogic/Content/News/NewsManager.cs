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
using NopSolutions.NopCommerce.BusinessLogic.IoC;
using System.Data.Objects;

namespace NopSolutions.NopCommerce.BusinessLogic.Content.NewsManagement
{
    /// <summary>
    /// News manager
    /// </summary>
    public partial class NewsManager : INewsManager
    {
        #region Constants
        private const string NEWS_BY_ID_KEY = "Nop.news.id-{0}";
        private const string NEWS_PATTERN_KEY = "Nop.news.";
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
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (News)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from n in context.News
                        where n.NewsId == newsId
                        select n;
            var news = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, news);
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

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(news))
                context.News.Attach(news);
            context.DeleteObject(news);
            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(NEWS_PATTERN_KEY);
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
            int totalRecords = 0;

            return GetAllNews(languageId, showHidden, 0, count, out totalRecords);
        }

        /// <summary>
        /// Gets news item collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>News item collection</returns>
        public List<News> GetAllNews(int languageId, int pageIndex, int pageSize,
            out int totalRecords)
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetAllNews(languageId, showHidden, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets news item collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="totalRecords">Total records</param>
        /// <returns>News item collection</returns>
        public List<News> GetAllNews(int languageId, bool showHidden, 
            int pageIndex, int pageSize, out int totalRecords)
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

            var context = ObjectContextHelper.CurrentObjectContext;
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));
            var news = context.Sp_NewsLoadAll(languageId, showHidden,
                pageIndex, pageSize, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);

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

            var context = ObjectContextHelper.CurrentObjectContext;

            context.News.AddObject(news);
            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(NEWS_PATTERN_KEY);
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

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(news))
                context.News.Attach(news);

            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(NEWS_PATTERN_KEY);
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

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from nc in context.NewsComments
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
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from nc in context.NewsComments
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

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(newsComment))
                context.NewsComments.Attach(newsComment);
            context.DeleteObject(newsComment);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets all news comments
        /// </summary>
        /// <returns>News comment collection</returns>
        public List<NewsComment> GetAllNewsComments()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from nc in context.NewsComments
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

            var context = ObjectContextHelper.CurrentObjectContext;

            var newsComment = context.NewsComments.CreateObject();
            newsComment.NewsId = newsId;
            newsComment.CustomerId = customerId;
            newsComment.IPAddress = ipAddress;
            newsComment.Title = title;
            newsComment.Comment = comment;
            newsComment.CreatedOn = createdOn;

            context.NewsComments.AddObject(newsComment);
            context.SaveChanges();
            
            //notifications
            if (notify)
            {
                IoCFactory.Resolve<IMessageManager>().SendNewsCommentNotificationMessage(newsComment, LocalizationManager.DefaultAdminLanguage.LanguageId);
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

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(newsComment))
                context.NewsComments.Attach(newsComment);

            context.SaveChanges();
        }

        /// <summary>
        /// Formats the text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public string FormatCommentText(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);
            return text;
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
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.NewsManager.CacheEnabled");
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether news are enabled
        /// </summary>
        public bool NewsEnabled
        {
            get
            {
                bool newsEnabled = IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("News.NewsEnabled");
                return newsEnabled;
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("News.NewsEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether not registered user can leave comments
        /// </summary>
        public bool AllowNotRegisteredUsersToLeaveComments
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("News.AllowNotRegisteredUsersToLeaveComments");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("News.AllowNotRegisteredUsersToLeaveComments", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to notify about new news comments
        /// </summary>
        public bool NotifyAboutNewNewsComments
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("News.NotifyAboutNewNewsComments");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("News.NotifyAboutNewNewsComments", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show news on the main page
        /// </summary>
        public bool ShowNewsOnMainPage
        {
            get
            {
                bool showNewsOnMainPage = IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ShowNewsOnMainPage");
                return showNewsOnMainPage;
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("Display.ShowNewsOnMainPage", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating news count displayed on the main page
        /// </summary>
        public int MainPageNewsCount
        {
            get
            {
                int mainPageNewsCount = IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("Display.MainPageNewsCount");
                return mainPageNewsCount;
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("Display.MainPageNewsCount", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the page size for news archive
        /// </summary>
        public int NewsArchivePageSize
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("Display.NewsArchivePageSize", 10);
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("Display.NewsArchivePageSize", value.ToString());
            }
        }


        #endregion
    }
}
