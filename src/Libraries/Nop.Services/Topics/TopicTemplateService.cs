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

        protected IRepository<TopicTemplate> TopicTemplateRepository { get; }

        #endregion

        #region Ctor

        public TopicTemplateService(IRepository<TopicTemplate> topicTemplateRepository)
        {
            TopicTemplateRepository = topicTemplateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTopicTemplateAsync(TopicTemplate topicTemplate)
        {
            await TopicTemplateRepository.DeleteAsync(topicTemplate);
        }

        /// <summary>
        /// Gets all topic templates
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic templates
        /// </returns>
        public virtual async Task<IList<TopicTemplate>> GetAllTopicTemplatesAsync()
        {
            var templates = await TopicTemplateRepository.GetAllAsync(query=>
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic template
        /// </returns>
        public virtual async Task<TopicTemplate> GetTopicTemplateByIdAsync(int topicTemplateId)
        {
            return await TopicTemplateRepository.GetByIdAsync(topicTemplateId, cache => default);
        }

        /// <summary>
        /// Inserts topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTopicTemplateAsync(TopicTemplate topicTemplate)
        {
            await TopicTemplateRepository.InsertAsync(topicTemplate);
        }

        /// <summary>
        /// Updates the topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTopicTemplateAsync(TopicTemplate topicTemplate)
        {
            await TopicTemplateRepository.UpdateAsync(topicTemplate);
        }

        #endregion
    }
}