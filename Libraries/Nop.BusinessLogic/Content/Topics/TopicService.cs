//------------------------------------------------------------------------------
// The contents of this file are title to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
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
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Caching;

namespace NopSolutions.NopCommerce.BusinessLogic.Content.Topics
{
    /// <summary>
    /// Message service
    /// </summary>
    public partial class TopicService : ITopicService
    {
        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        protected readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        protected readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public TopicService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a topic
        /// </summary>
        /// <param name="topicId">Topic identifier</param>
        public void DeleteTopic(int topicId)
        {
            var topic = GetTopicById(topicId);
            if (topic == null)
                return;

            
            if (!_context.IsAttached(topic))
                _context.Topics.Attach(topic);
            _context.DeleteObject(topic);
            _context.SaveChanges();
        }

        /// <summary>
        /// Inserts a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public void InsertTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            topic.Name = CommonHelper.EnsureNotNull(topic.Name);
            topic.Name = CommonHelper.EnsureMaximumLength(topic.Name, 200);
            topic.Password = CommonHelper.EnsureNotNull(topic.Password);
            topic.Password = CommonHelper.EnsureMaximumLength(topic.Password, 200);

            
            
            _context.Topics.AddObject(topic);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public void UpdateTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            topic.Name = CommonHelper.EnsureNotNull(topic.Name);
            topic.Name = CommonHelper.EnsureMaximumLength(topic.Name, 200);
            topic.Password = CommonHelper.EnsureNotNull(topic.Password);
            topic.Password = CommonHelper.EnsureMaximumLength(topic.Password, 200);

            
            if (!_context.IsAttached(topic))
                _context.Topics.Attach(topic);

            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a topic by template identifier
        /// </summary>
        /// <param name="topicId">topic identifier</param>
        /// <returns>topic</returns>
        public Topic GetTopicById(int topicId)
        {
            if (topicId == 0)
                return null;

            
            var query = from t in _context.Topics
                        where t.TopicId == topicId
                        select t;
            var topic = query.SingleOrDefault();

            return topic;
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <returns>topic collection</returns>
        public List<Topic> GetAllTopics()
        {
            
            var query = from t in _context.Topics
                        orderby t.Name
                        select t;
            var topics = query.ToList();
            return topics;
        }

        /// <summary>
        /// Gets a localized topic by identifier
        /// </summary>
        /// <param name="localizedTopicId">Localized topic identifier</param>
        /// <returns>Localized topic</returns>
        public LocalizedTopic GetLocalizedTopicById(int localizedTopicId)
        {
            if (localizedTopicId == 0)
                return null;
           
            
            var query = from tl in _context.LocalizedTopics
                        where tl.TopicLocalizedId == localizedTopicId
                        select tl;
            var localizedTopic = query.SingleOrDefault();
            return localizedTopic;
        }

        /// <summary>
        /// Gets a localized topic by parent topic identifier and language identifier
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized topic</returns>
        public LocalizedTopic GetLocalizedTopic(int topicId, int languageId)
        {
            
            var query = from tl in _context.LocalizedTopics
                        where tl.TopicId == topicId &&
                        tl.LanguageId == languageId 
                        select tl;
            var localizedTopic = query.FirstOrDefault();

            return localizedTopic;
        }
        
        /// <summary>
        /// Gets a localized topic by name and language identifier
        /// </summary>
        /// <param name="topicName">Topic name</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized topic</returns>
        public LocalizedTopic GetLocalizedTopic(string topicName, int languageId)
        {
            
            var query = from tl in _context.LocalizedTopics
                        join t in _context.Topics on tl.TopicId equals t.TopicId
                        where tl.LanguageId == languageId &&
                        t.Name == topicName 
                        select tl;
            var localizedTopic = query.FirstOrDefault();

            return localizedTopic;
        }

        /// <summary>
        /// Deletes a localized topic
        /// </summary>
        /// <param name="localizedTopicId">topic identifier</param>
        public void DeleteLocalizedTopic(int localizedTopicId)
        {
            var localizedTopic = GetLocalizedTopicById(localizedTopicId);
            if (localizedTopic == null)
                return;

            
            if (!_context.IsAttached(localizedTopic))
                _context.LocalizedTopics.Attach(localizedTopic);
            _context.DeleteObject(localizedTopic);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets all localized topics
        /// </summary>
        /// <param name="topicName">topic name</param>
        /// <returns>Localized topic collection</returns>
        public List<LocalizedTopic> GetAllLocalizedTopics(string topicName)
        {
            
            var query = from tl in _context.LocalizedTopics
                        join t in _context.Topics on tl.TopicId equals t.TopicId
                        where t.Name == topicName
                        orderby tl.LanguageId
                        select tl;
            var localizedTopics = query.ToList();
            return localizedTopics;
        }

        /// <summary>
        /// Inserts a localized topic
        /// </summary>
        /// <param name="localizedTopic">Localized topic</param>
        public void InsertLocalizedTopic(LocalizedTopic localizedTopic)
        {
            if (localizedTopic == null)
                throw new ArgumentNullException("localizedTopic");
            
            localizedTopic.Title = CommonHelper.EnsureNotNull(localizedTopic.Title);
            localizedTopic.Title = CommonHelper.EnsureMaximumLength(localizedTopic.Title, 200);
            localizedTopic.Body = CommonHelper.EnsureNotNull(localizedTopic.Body);
            localizedTopic.MetaTitle = CommonHelper.EnsureNotNull(localizedTopic.MetaTitle);
            localizedTopic.MetaTitle = CommonHelper.EnsureMaximumLength(localizedTopic.MetaTitle, 400);
            localizedTopic.MetaKeywords = CommonHelper.EnsureNotNull(localizedTopic.MetaKeywords);
            localizedTopic.MetaKeywords = CommonHelper.EnsureMaximumLength(localizedTopic.MetaKeywords, 400);
            localizedTopic.MetaDescription = CommonHelper.EnsureNotNull(localizedTopic.MetaDescription);
            localizedTopic.MetaDescription = CommonHelper.EnsureMaximumLength(localizedTopic.MetaDescription, 4000);

            

            _context.LocalizedTopics.AddObject(localizedTopic);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the localized topic
        /// </summary>
        /// <param name="localizedTopic">Localized topic</param>
        public void UpdateLocalizedTopic(LocalizedTopic localizedTopic)
        {
            if (localizedTopic == null)
                throw new ArgumentNullException("localizedTopic");

            localizedTopic.Title = CommonHelper.EnsureNotNull(localizedTopic.Title);
            localizedTopic.Title = CommonHelper.EnsureMaximumLength(localizedTopic.Title, 200);
            localizedTopic.Body = CommonHelper.EnsureNotNull(localizedTopic.Body);
            localizedTopic.MetaTitle = CommonHelper.EnsureNotNull(localizedTopic.MetaTitle);
            localizedTopic.MetaTitle = CommonHelper.EnsureMaximumLength(localizedTopic.MetaTitle, 400);
            localizedTopic.MetaKeywords = CommonHelper.EnsureNotNull(localizedTopic.MetaKeywords);
            localizedTopic.MetaKeywords = CommonHelper.EnsureMaximumLength(localizedTopic.MetaKeywords, 400);
            localizedTopic.MetaDescription = CommonHelper.EnsureNotNull(localizedTopic.MetaDescription);
            localizedTopic.MetaDescription = CommonHelper.EnsureMaximumLength(localizedTopic.MetaDescription, 4000);

            
            if (!_context.IsAttached(localizedTopic))
                _context.LocalizedTopics.Attach(localizedTopic);

            _context.SaveChanges();
        }

        #endregion
    }
}