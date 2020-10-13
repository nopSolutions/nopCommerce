using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        #endregion

        #region Ctor

        public TopicTemplateService(IRepository<TopicTemplate> topicTemplateRepository)
        {
            _topicTemplateRepository = topicTemplateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public virtual async Task DeleteTopicTemplate(TopicTemplate topicTemplate)
        {
            await _topicTemplateRepository.Delete(topicTemplate);
        }

        /// <summary>
        /// Gets all topic templates
        /// </summary>
        /// <returns>Topic templates</returns>
        public virtual async Task<IList<TopicTemplate>> GetAllTopicTemplates()
        {
            var templates = await _topicTemplateRepository.GetAll(query=>
            {
                return from pt in query
                    orderby pt.DisplayOrder, pt.Id
                    select pt;
            }, cache => default);

            return templates;
        }

        /// <summary>
        /// Gets a topic template
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>Topic template</returns>
        public virtual async Task<TopicTemplate> GetTopicTemplateById(int topicTemplateId)
        {
            return await _topicTemplateRepository.GetById(topicTemplateId, cache => default);
        }

        /// <summary>
        /// Inserts topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public virtual async Task InsertTopicTemplate(TopicTemplate topicTemplate)
        {
            await _topicTemplateRepository.Insert(topicTemplate);
        }

        /// <summary>
        /// Updates the topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public virtual async Task UpdateTopicTemplate(TopicTemplate topicTemplate)
        {
            await _topicTemplateRepository.Update(topicTemplate);
        }

        #endregion
    }
}