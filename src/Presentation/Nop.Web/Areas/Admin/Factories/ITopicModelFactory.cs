using Nop.Core.Domain.Topics;
using Nop.Web.Areas.Admin.Models.Topics;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the topic model factory
    /// </summary>
    public partial interface ITopicModelFactory
    {
        /// <summary>
        /// Prepare topic search model
        /// </summary>
        /// <param name="searchModel">Topic search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the topic search model
        /// </returns>
        Task<TopicSearchModel> PrepareTopicSearchModelAsync(TopicSearchModel searchModel);

        /// <summary>
        /// Prepare paged topic list model
        /// </summary>
        /// <param name="searchModel">Topic search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the topic list model
        /// </returns>
        Task<TopicListModel> PrepareTopicListModelAsync(TopicSearchModel searchModel);

        /// <summary>
        /// Prepare topic model
        /// </summary>
        /// <param name="model">Topic model</param>
        /// <param name="topic">Topic</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the topic model
        /// </returns>
        Task<TopicModel> PrepareTopicModelAsync(TopicModel model, Topic topic, bool excludeProperties = false);
    }
}