using Nop.Core.Domain.Topics;

namespace Nop.Services.Topics
{
    /// <summary>
    /// Topic template service interface
    /// </summary>
    public partial interface ITopicTemplateService
    {
        /// <summary>
        /// Delete topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTopicTemplateAsync(TopicTemplate topicTemplate);

        /// <summary>
        /// Gets all topic templates
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the topic templates
        /// </returns>
        Task<IList<TopicTemplate>> GetAllTopicTemplatesAsync();

        /// <summary>
        /// Gets a topic template
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the topic template
        /// </returns>
        Task<TopicTemplate> GetTopicTemplateByIdAsync(int topicTemplateId);

        /// <summary>
        /// Inserts topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertTopicTemplateAsync(TopicTemplate topicTemplate);

        /// <summary>
        /// Updates the topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTopicTemplateAsync(TopicTemplate topicTemplate);
    }
}
