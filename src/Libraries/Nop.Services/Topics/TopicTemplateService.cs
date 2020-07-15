using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain.Topics;
using Nop.Data;

namespace Nop.Services.Topics
{
    /// <summary>
    /// Topic template service
    /// </summary>
    public partial class TopicTemplateService : ITopicTemplateService
    {
        #region Fields

        private readonly IRepository<TopicTemplate> _topicTemplateRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public TopicTemplateService(IRepository<TopicTemplate> topicTemplateRepository,
            IStaticCacheManager staticCacheManager)
        {
            _topicTemplateRepository = topicTemplateRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public virtual void DeleteTopicTemplate(TopicTemplate topicTemplate)
        {
            _topicTemplateRepository.Delete(topicTemplate);
        }

        /// <summary>
        /// Gets all topic templates
        /// </summary>
        /// <returns>Topic templates</returns>
        public virtual IList<TopicTemplate> GetAllTopicTemplates()
        {
            var templates = _topicTemplateRepository.GetAll(query=>
            {
                return from pt in query
                    orderby pt.DisplayOrder, pt.Id
                    select pt;
            }, _staticCacheManager.PrepareKeyForDefaultCache(NopTopicDefaults.TopicTemplatesAllCacheKey));

            return templates;
        }

        /// <summary>
        /// Gets a topic template
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>Topic template</returns>
        public virtual TopicTemplate GetTopicTemplateById(int topicTemplateId)
        {
            return _topicTemplateRepository.GetById(topicTemplateId);
        }

        /// <summary>
        /// Inserts topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public virtual void InsertTopicTemplate(TopicTemplate topicTemplate)
        {
            _topicTemplateRepository.Insert(topicTemplate);
        }

        /// <summary>
        /// Updates the topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public virtual void UpdateTopicTemplate(TopicTemplate topicTemplate)
        {
            _topicTemplateRepository.Update(topicTemplate);
        }

        #endregion
    }
}