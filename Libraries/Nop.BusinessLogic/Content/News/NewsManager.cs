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

namespace NopSolutions.NopCommerce.BusinessLogic.Content.NewsManagement
{
    /// <summary>
    /// News manager
    /// </summary>
    public partial class NewsManager
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
        public static News GetNewsById(int newsId)
        {
            if (newsId == 0)
                return null;

            string key = string.Format(NEWS_BY_ID_KEY, newsId);
            object obj2 = NopRequestCache.Get(key);
            if (NewsManager.CacheEnabled && (obj2 != null))
            {
                return (News)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from n in context.News
                        where n.NewsId == newsId
                        select n;
            var news = query.SingleOrDefault();

            if (NewsManager.CacheEnabled)
            {
                NopRequestCache.Add(key, news);
            }
            return news;
        }

        /// <summary>
        /// Deletes a news
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        public static void DeleteNews(int newsId)
        {
            var news = GetNewsById(newsId);
            if (news == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(news))
                context.News.Attach(news);
            context.DeleteObject(news);
            context.SaveChanges();

            if (NewsManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(NEWS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all news
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <returns>News item collection</returns>
        public static List<News> GetAllNews(int languageId)
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
        public static List<News> GetAllNews(int languageId, bool showHidden)
        {
            return GetAllNews(languageId, showHidden, Int32.MaxValue);
        }
        
        /// <summary>
        /// Gets news item collection
        /// </summary>
        /// <param name="languageId">Language identifier. 0 if you want to get all news</param>
        /// <param name="count">News count to return</param>
        /// <returns>News item collection</returns>
        public static List<News> GetAllNews(int languageId, int count)
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
        public static List<News> GetAllNews(int languageId, bool showHidden, int count)
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
        public static List<News> GetAllNews(int languageId, int pageIndex, int pageSize,
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
        public static List<News> GetAllNews(int languageId, bool showHidden, 
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
            var news = context.Sp_NewsLoadAll(languageId, showHidden,
                pageSize, pageIndex, out totalRecords).ToList();

            return news;
        }

        /// <summary>
        /// Inserts a news item
        /// </summary>
        /// <param name="languageId">The language identifier</param>
        /// <param name="title">The news title</param>
        /// <param name="shortText">The short text</param>
        /// <param name="fullText">The full text</param>
        /// <param name="published">A value indicating whether the entity is published</param>
        /// <param name="allowComments">A value indicating whether the entity allows comments</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <returns>News item</returns>
        public static News InsertNews(int languageId, string title, string shortText,
            string fullText, bool published, bool allowComments, DateTime createdOn)
        {
            title = CommonHelper.EnsureMaximumLength(title, 1000);
            shortText = CommonHelper.EnsureMaximumLength(shortText, 4000);

            var context = ObjectContextHelper.CurrentObjectContext;

            var news = context.News.CreateObject();
            news.LanguageId =languageId;
            news.Title =title;
            news.Short =shortText;
            news.Full =fullText;
            news.Published =published;
            news.AllowComments =allowComments;
            news.CreatedOn =createdOn;

            context.News.AddObject(news);
            context.SaveChanges();

            if (NewsManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(NEWS_PATTERN_KEY);
            }

            return news;
        }

        /// <summary>
        /// Updates the news item
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        /// <param name="languageId">The language identifier</param>
        /// <param name="title">The news title</param>
        /// <param name="shortText">The short text</param>
        /// <param name="fullText">The full text</param>
        /// <param name="published">A value indicating whether the entity is published</param>
        /// <param name="allowComments">A value indicating whether the entity allows comments</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <returns>News item</returns>
        public static News UpdateNews(int newsId, int languageId,
            string title, string shortText, string fullText,
            bool published, bool allowComments, DateTime createdOn)
        {
            title = CommonHelper.EnsureMaximumLength(title, 1000);
            shortText = CommonHelper.EnsureMaximumLength(shortText, 4000);

            var news = GetNewsById(newsId);
            if (news == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(news))
                context.News.Attach(news);

            news.LanguageId = languageId;
            news.Title = title;
            news.Short = shortText;
            news.Full = fullText;
            news.Published = published;
            news.AllowComments = allowComments;
            news.CreatedOn = createdOn;
            context.SaveChanges();

            if (NewsManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(NEWS_PATTERN_KEY);
            }

            return news;
        }

        /// <summary>
        /// Gets a news comment
        /// </summary>
        /// <param name="newsCommentId">News comment identifer</param>
        /// <returns>News comment</returns>
        public static NewsComment GetNewsCommentById(int newsCommentId)
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
        public static List<NewsComment> GetNewsCommentsByNewsId(int newsId)
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
        public static void DeleteNewsComment(int newsCommentId)
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
        public static List<NewsComment> GetAllNewsComments()
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
        public static NewsComment InsertNewsComment(int newsId, int customerId,
            string title, string comment, DateTime createdOn)
        {
            return InsertNewsComment(newsId, customerId, title, comment, 
                createdOn, NewsManager.NotifyAboutNewNewsComments);
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
        public static NewsComment InsertNewsComment(int newsId, int customerId,
            string title, string comment, DateTime createdOn, bool notify)
        {
            string IPAddress = string.Empty;
            if(HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                IPAddress = HttpContext.Current.Request.UserHostAddress;
            }
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
        public static NewsComment InsertNewsComment(int newsId, int customerId, string ipAddress,
            string title, string comment, DateTime createdOn, bool notify)
        {
            ipAddress = CommonHelper.EnsureMaximumLength(ipAddress, 100);
            title = CommonHelper.EnsureMaximumLength(title, 1000);

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
                MessageManager.SendNewsCommentNotificationMessage(newsComment, LocalizationManager.DefaultAdminLanguage.LanguageId);
            }

            return newsComment;
        }

        /// <summary>
        /// Updates the news comment
        /// </summary>
        /// <param name="newsCommentId">The news comment identifier</param>
        /// <param name="newsId">The news identifier</param>
        /// <param name="customerId">The customer identifier</param>
        /// <param name="ipAddress">The IP address</param>
        /// <param name="title">The title</param>
        /// <param name="comment">The comment</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <returns>News comment</returns>
        public static NewsComment UpdateNewsComment(int newsCommentId,
            int newsId, int customerId, string ipAddress, string title,
            string comment, DateTime createdOn)
        {
            ipAddress = CommonHelper.EnsureMaximumLength(ipAddress, 100);
            title = CommonHelper.EnsureMaximumLength(title, 1000);

            var newsComment = GetNewsCommentById(newsCommentId);
            if (newsComment == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(newsComment))
                context.NewsComments.Attach(newsComment);

            newsComment.NewsId = newsId;
            newsComment.CustomerId = customerId;
            newsComment.IPAddress = ipAddress;
            newsComment.Title = title;
            newsComment.Comment = comment;
            newsComment.CreatedOn = createdOn;
            context.SaveChanges();
            return newsComment;
        }

        /// <summary>
        /// Formats the text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string FormatCommentText(string text)
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
        public static bool CacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.NewsManager.CacheEnabled");
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether news are enabled
        /// </summary>
        public static bool NewsEnabled
        {
            get
            {
                bool newsEnabled = SettingManager.GetSettingValueBoolean("News.NewsEnabled");
                return newsEnabled;
            }
            set
            {
                SettingManager.SetParam("News.NewsEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether not registered user can leave comments
        /// </summary>
        public static bool AllowNotRegisteredUsersToLeaveComments
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("News.AllowNotRegisteredUsersToLeaveComments");
            }
            set
            {
                SettingManager.SetParam("News.AllowNotRegisteredUsersToLeaveComments", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to notify about new news comments
        /// </summary>
        public static bool NotifyAboutNewNewsComments
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("News.NotifyAboutNewNewsComments");
            }
            set
            {
                SettingManager.SetParam("News.NotifyAboutNewNewsComments", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show news on the main page
        /// </summary>
        public static bool ShowNewsOnMainPage
        {
            get
            {
                bool showNewsOnMainPage = SettingManager.GetSettingValueBoolean("Display.ShowNewsOnMainPage");
                return showNewsOnMainPage;
            }
            set
            {
                SettingManager.SetParam("Display.ShowNewsOnMainPage", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating news count displayed on the main page
        /// </summary>
        public static int MainPageNewsCount
        {
            get
            {
                int mainPageNewsCount = SettingManager.GetSettingValueInteger("Display.MainPageNewsCount");
                return mainPageNewsCount;
            }
            set
            {
                SettingManager.SetParam("Display.MainPageNewsCount", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the page size for news archive
        /// </summary>
        public static int NewsArchivePageSize
        {
            get
            {
                return SettingManager.GetSettingValueInteger("Display.NewsArchivePageSize", 10);
            }
            set
            {
                SettingManager.SetParam("Display.NewsArchivePageSize", value.ToString());
            }
        }
        #endregion
    }
}
