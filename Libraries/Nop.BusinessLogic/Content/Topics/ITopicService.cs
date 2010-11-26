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

using System.Collections.Generic;

namespace NopSolutions.NopCommerce.BusinessLogic.Content.Topics
{
    /// <summary>
    /// Message service interface
    /// </summary>
    public partial interface ITopicService
    {
        /// <summary>
        /// Deletes a topic
        /// </summary>
        /// <param name="topicId">Topic identifier</param>
        void DeleteTopic(int topicId);

        /// <summary>
        /// Inserts a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        void InsertTopic(Topic topic);

        /// <summary>
        /// Updates the topic
        /// </summary>
        /// <param name="topic">Topic</param>
        void UpdateTopic(Topic topic);

        /// <summary>
        /// Gets a topic by template identifier
        /// </summary>
        /// <param name="topicId">topic identifier</param>
        /// <returns>topic</returns>
        Topic GetTopicById(int topicId);

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <returns>topic collection</returns>
        List<Topic> GetAllTopics();

        /// <summary>
        /// Gets a localized topic by identifier
        /// </summary>
        /// <param name="localizedTopicId">Localized topic identifier</param>
        /// <returns>Localized topic</returns>
        LocalizedTopic GetLocalizedTopicById(int localizedTopicId);

        /// <summary>
        /// Gets a localized topic by parent topic identifier and language identifier
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized topic</returns>
        LocalizedTopic GetLocalizedTopic(int topicId, int languageId);

        /// <summary>
        /// Gets a localized topic by name and language identifier
        /// </summary>
        /// <param name="topicName">Topic name</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized topic</returns>
        LocalizedTopic GetLocalizedTopic(string topicName, int languageId);

        /// <summary>
        /// Deletes a localized topic
        /// </summary>
        /// <param name="localizedTopicId">topic identifier</param>
        void DeleteLocalizedTopic(int localizedTopicId);

        /// <summary>
        /// Gets all localized topics
        /// </summary>
        /// <param name="topicName">topic name</param>
        /// <returns>Localized topic collection</returns>
        List<LocalizedTopic> GetAllLocalizedTopics(string topicName);

        /// <summary>
        /// Inserts a localized topic
        /// </summary>
        /// <param name="localizedTopic">Localized topic</param>
        void InsertLocalizedTopic(LocalizedTopic localizedTopic);

        /// <summary>
        /// Updates the localized topic
        /// </summary>
        /// <param name="localizedTopic">Localized topic</param>
        void UpdateLocalizedTopic(LocalizedTopic localizedTopic);
    }
}