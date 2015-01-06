using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Topics;
using Nop.Services.Events;

namespace Nop.Services.Topics
{
    /// <summary>
    /// Topic template service
    /// </summary>
    public partial class TopicTemplateService : ITopicTemplateService
    {
        #region Fields

        private readonly IRepository<TopicTemplate> _topicTemplateRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="topicTemplateRepository">Topic template repository</param>
        /// <param name="eventPublisher">Event published</param>
        public TopicTemplateService(IRepository<TopicTemplate> topicTemplateRepository, 
            IEventPublisher eventPublisher)
        {
            this._topicTemplateRepository = topicTemplateRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public virtual void DeleteTopicTemplate(TopicTemplate topicTemplate)
        {
            if (topicTemplate == null)
                throw new ArgumentNullException("topicTemplate");

            _topicTemplateRepository.Delete(topicTemplate);

            //event notification
            _eventPublisher.EntityDeleted(topicTemplate);
        }

        /// <summary>
        /// Gets all topic templates
        /// </summary>
        /// <returns>Topic templates</returns>
        public virtual IList<TopicTemplate> GetAllTopicTemplates()
        {
            var query = from pt in _topicTemplateRepository.Table
                        orderby pt.DisplayOrder
                        select pt;

            var templates = query.ToList();
            return templates;
        }
 
        /// <summary>
        /// Gets a topic template
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>Topic template</returns>
        public virtual TopicTemplate GetTopicTemplateById(int topicTemplateId)
        {
            if (topicTemplateId == 0)
                return null;

            return _topicTemplateRepository.GetById(topicTemplateId);
        }

        /// <summary>
        /// Inserts topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public virtual void InsertTopicTemplate(TopicTemplate topicTemplate)
        {
            if (topicTemplate == null)
                throw new ArgumentNullException("topicTemplate");

            _topicTemplateRepository.Insert(topicTemplate);

            //event notification
            _eventPublisher.EntityInserted(topicTemplate);
        }

        /// <summary>
        /// Updates the topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public virtual void UpdateTopicTemplate(TopicTemplate topicTemplate)
        {
            if (topicTemplate == null)
                throw new ArgumentNullException("topicTemplate");

            _topicTemplateRepository.Update(topicTemplate);

            //event notification
            _eventPublisher.EntityUpdated(topicTemplate);
        }
        
        #endregion
    }
}
