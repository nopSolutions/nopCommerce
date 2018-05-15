using Nop.Web.Models.Topics;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the topic model factory
    /// </summary>
    public partial interface ITopicModelFactory
    {
        /// <summary>
        /// Get the topic model by topic identifier
        /// </summary>
        /// <param name="topicId">Topic identifier</param>
        /// <returns>Topic model</returns>
        TopicModel PrepareTopicModelById(int topicId);

        /// <summary>
        /// Get the topic model by topic system name
        /// </summary>
        /// <param name="systemName">Topic system name</param>
        /// <returns>Topic model</returns>
        TopicModel PrepareTopicModelBySystemName(string systemName);

        /// <summary>
        /// Get the topic template view path
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>View path</returns>
        string PrepareTemplateViewPath(int topicTemplateId);
    }
}
