using Nop.Core.Domain.Topics;
using Nop.Data;

namespace Nop.Services.Topics;

/// <summary>
/// Topic template service
/// </summary>
public partial class TopicTemplateService : ITopicTemplateService
{
    #region Fields

    protected readonly IRepository<TopicTemplate> _topicTemplateRepository;

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
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteTopicTemplateAsync(TopicTemplate topicTemplate)
    {
        await _topicTemplateRepository.DeleteAsync(topicTemplate);
    }

    /// <summary>
    /// Gets all topic templates
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the topic templates
    /// </returns>
    public virtual async Task<IList<TopicTemplate>> GetAllTopicTemplatesAsync()
    {
        var templates = await _topicTemplateRepository.GetAllAsync(query =>
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
    /// The task result contains the topic template
    /// </returns>
    public virtual async Task<TopicTemplate> GetTopicTemplateByIdAsync(int topicTemplateId)
    {
        return await _topicTemplateRepository.GetByIdAsync(topicTemplateId, cache => default);
    }

    /// <summary>
    /// Inserts topic template
    /// </summary>
    /// <param name="topicTemplate">Topic template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertTopicTemplateAsync(TopicTemplate topicTemplate)
    {
        await _topicTemplateRepository.InsertAsync(topicTemplate);
    }

    /// <summary>
    /// Updates the topic template
    /// </summary>
    /// <param name="topicTemplate">Topic template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateTopicTemplateAsync(TopicTemplate topicTemplate)
    {
        await _topicTemplateRepository.UpdateAsync(topicTemplate);
    }

    #endregion
}