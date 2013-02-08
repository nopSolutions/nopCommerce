using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Topics;
using Nop.Services.Events;

namespace Nop.Services.Topics
{
    /// <summary>
    /// Topic service
    /// </summary>
    public partial class TopicService : ITopicService
    {
        #region Fields

        private readonly IRepository<Topic> _topicRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public TopicService(IRepository<Topic> topicRepository, IEventPublisher eventPublisher)
        {
            _topicRepository = topicRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public virtual void DeleteTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            _topicRepository.Delete(topic);

            //event notification
            _eventPublisher.EntityDeleted(topic);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <returns>Topic</returns>
        public virtual Topic GetTopicById(int topicId)
        {
            if (topicId == 0)
                return null;

            return _topicRepository.GetById(topicId);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="systemName">The topic system name</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Topic</returns>
        public virtual Topic GetTopicBySystemName(string systemName, int storeId)
        {
            if (String.IsNullOrEmpty(systemName))
                return null;

            var query = from t in _topicRepository.Table
                        where t.SystemName == systemName && t.StoreId == storeId
                        select t;

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <returns>Topics</returns>
        public virtual IList<Topic> GetAllTopics(int storeId)
        {
            var query = _topicRepository.Table;
            query = query.OrderBy(t => t.SystemName).ThenBy(t => t.StoreId);
            var allTopics = query.ToList();
            if (storeId == 0)
                return allTopics;

            //filter topics
            var topics  = new List<Topic>();
            var allSystemNames = allTopics
                .Select(x => x.SystemName)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .ToList();
            foreach (var systemName in allSystemNames)
            {
                //find a topic assigned to the passed storeId
                var topic = allTopics
                    .FirstOrDefault(x => x.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase) && 
                        x.StoreId == storeId);

                //not found. let's find a topic assigned to all stores in this case
                if (topic == null)
                    topic = allTopics
                        .FirstOrDefault(x => x.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase) && 
                        x.StoreId == 0);

                if (topic != null)
                    topics.Add(topic);
            }
            return topics;
        }

        /// <summary>
        /// Inserts a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public virtual void InsertTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            _topicRepository.Insert(topic);

            //event notification
            _eventPublisher.EntityInserted(topic);
        }

        /// <summary>
        /// Updates the topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public virtual void UpdateTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            _topicRepository.Update(topic);

            //event notification
            _eventPublisher.EntityUpdated(topic);
        }

        #endregion
    }
}
