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

namespace NopSolutions.NopCommerce.BusinessLogic.Content.Topics
{
    /// <summary>
    /// Message manager
    /// </summary>
    public partial class TopicManager
    {
        #region Methods

        /// <summary>
        /// Deletes a topic
        /// </summary>
        /// <param name="topicId">Topic identifier</param>
        public static void DeleteTopic(int topicId)
        {
            var topic = GetTopicById(topicId);
            if (topic == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(topic))
                context.Topics.Attach(topic);
            context.DeleteObject(topic);
            context.SaveChanges();
        }

        /// <summary>
        /// Inserts a topic
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>Topic</returns>
        public static Topic InsertTopic(string name)
        {
            name = CommonHelper.EnsureMaximumLength(name, 200);

            var context = ObjectContextHelper.CurrentObjectContext;

            var topic = context.Topics.CreateObject();
            topic.Name = name;

            context.Topics.AddObject(topic);
            context.SaveChanges();

            return topic;
        }

        /// <summary>
        /// Updates the topic
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <param name="name">The name</param>
        /// <returns>Topic</returns>
        public static Topic UpdateTopic(int topicId, string name)
        {
            name = CommonHelper.EnsureMaximumLength(name, 200);

            var topic = GetTopicById(topicId);
            if (topic == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(topic))
                context.Topics.Attach(topic);

            topic.Name = name;
            context.SaveChanges();

            return topic;
        }

        /// <summary>
        /// Gets a topic by template identifier
        /// </summary>
        /// <param name="topicId">topic identifier</param>
        /// <returns>topic</returns>
        public static Topic GetTopicById(int topicId)
        {
            if (topicId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from t in context.Topics
                        where t.TopicId == topicId
                        select t;
            var topic = query.SingleOrDefault();

            return topic;
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <returns>topic collection</returns>
        public static List<Topic> GetAllTopics()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from t in context.Topics
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
        public static LocalizedTopic GetLocalizedTopicById(int localizedTopicId)
        {
            if (localizedTopicId == 0)
                return null;
           
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from tl in context.LocalizedTopics
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
        public static LocalizedTopic GetLocalizedTopic(int topicId, int languageId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from tl in context.LocalizedTopics
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
        public static LocalizedTopic GetLocalizedTopic(string topicName, int languageId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from tl in context.LocalizedTopics
                        join t in context.Topics on tl.TopicId equals t.TopicId
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
        public static void DeleteLocalizedTopic(int localizedTopicId)
        {
            var localizedTopic = GetLocalizedTopicById(localizedTopicId);
            if (localizedTopic == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(localizedTopic))
                context.LocalizedTopics.Attach(localizedTopic);
            context.DeleteObject(localizedTopic);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets all localized topics
        /// </summary>
        /// <param name="topicName">topic name</param>
        /// <returns>Localized topic collection</returns>
        public static List<LocalizedTopic> GetAllLocalizedTopics(string topicName)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from tl in context.LocalizedTopics
                        join t in context.Topics on tl.TopicId equals t.TopicId
                        where t.Name == topicName
                        orderby tl.LanguageId
                        select tl;
            var localizedTopics = query.ToList();
            return localizedTopics;
        }

        /// <summary>
        /// Inserts a localized topic
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <param name="languageId">The language identifier</param>
        /// <param name="title">The title</param>
        /// <param name="body">The body</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <param name="metaKeywords">The meta keywords</param>
        /// <param name="metaDescription">The meta description</param>
        /// <param name="metaTitle">The meta title</param>
        /// <returns>Localized topic</returns>
        public static LocalizedTopic InsertLocalizedTopic(int topicId,
            int languageId, string title, string body,
            DateTime createdOn, DateTime updatedOn,
            string metaKeywords, string metaDescription, string metaTitle)
        {
            title = CommonHelper.EnsureMaximumLength(title, 200);
            metaTitle = CommonHelper.EnsureMaximumLength(metaTitle, 400);
            metaKeywords = CommonHelper.EnsureMaximumLength(metaKeywords, 400);
            metaDescription = CommonHelper.EnsureMaximumLength(metaDescription, 4000);

            var context = ObjectContextHelper.CurrentObjectContext;

            var localizedTopic = context.LocalizedTopics.CreateObject();
            localizedTopic.TopicId = topicId;
            localizedTopic.LanguageId = languageId;
            localizedTopic.Title = title;
            localizedTopic.Body = body;
            localizedTopic.CreatedOn = createdOn;
            localizedTopic.UpdatedOn = updatedOn;
            localizedTopic.MetaKeywords = metaKeywords;
            localizedTopic.MetaDescription = metaDescription;
            localizedTopic.MetaTitle = metaTitle;

            context.LocalizedTopics.AddObject(localizedTopic);
            context.SaveChanges();

            return localizedTopic;
        }

        /// <summary>
        /// Updates the localized topic
        /// </summary>
        /// <param name="topicLocalizedId">The localized topic identifier</param>
        /// <param name="topicId">The topic identifier</param>
        /// <param name="languageId">The language identifier</param>
        /// <param name="title">The title</param>
        /// <param name="body">The body</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <param name="metaKeywords">The meta keywords</param>
        /// <param name="metaDescription">The meta description</param>
        /// <param name="metaTitle">The meta title</param>
        /// <returns>Localized topic</returns>
        public static LocalizedTopic UpdateLocalizedTopic(int topicLocalizedId,
            int topicId, int languageId, string title, string body,
            DateTime createdOn, DateTime updatedOn,
            string metaKeywords, string metaDescription, string metaTitle)
        {
            title = CommonHelper.EnsureMaximumLength(title, 200);
            metaTitle = CommonHelper.EnsureMaximumLength(metaTitle, 400);
            metaKeywords = CommonHelper.EnsureMaximumLength(metaKeywords, 400);
            metaDescription = CommonHelper.EnsureMaximumLength(metaDescription, 4000);

            var localizedTopic = GetLocalizedTopicById(topicLocalizedId);
            if (localizedTopic == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(localizedTopic))
                context.LocalizedTopics.Attach(localizedTopic);

            localizedTopic.TopicId = topicId;
            localizedTopic.LanguageId = languageId;
            localizedTopic.Title = title;
            localizedTopic.Body = body;
            localizedTopic.CreatedOn = createdOn;
            localizedTopic.UpdatedOn = updatedOn;
            localizedTopic.MetaKeywords = metaKeywords;
            localizedTopic.MetaDescription = metaDescription;
            localizedTopic.MetaTitle = metaTitle;
            context.SaveChanges();

            return localizedTopic;
        }

        #endregion
    }
}