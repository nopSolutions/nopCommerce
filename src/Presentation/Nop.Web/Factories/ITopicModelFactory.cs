using Nop.Core.Domain.Topics;
using Nop.Web.Models.Topics;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the topic model factory
    /// </summary>
    public partial interface ITopicModelFactory
    {
        /// <summary>
        /// Prepare the topic model
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the topic model
        /// </returns>
        Task<TopicModel> PrepareTopicModelAsync(Topic topic);

        /// <summary>
        /// Get the topic model by topic system name
        /// </summary>
        /// <param name="systemName">Topic system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the topic model
        /// </returns>
        Task<TopicModel> PrepareTopicModelBySystemNameAsync(string systemName);

        /// <summary>
        /// Get the topic template view path
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view path
        /// </returns>
        Task<string> PrepareTemplateViewPathAsync(int topicTemplateId);
    }
}
