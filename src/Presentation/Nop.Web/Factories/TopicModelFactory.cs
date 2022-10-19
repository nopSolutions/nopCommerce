using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Topics;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Models.Topics;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the topic model factory
    /// </summary>
    public partial class TopicModelFactory : ITopicModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly ITopicService _topicService;
        private readonly ITopicTemplateService _topicTemplateService;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public TopicModelFactory(ILocalizationService localizationService,
            IStoreContext storeContext,
            ITopicService topicService,
            ITopicTemplateService topicTemplateService,
            IUrlRecordService urlRecordService)
        {
            _localizationService = localizationService;
            _storeContext = storeContext;
            _topicService = topicService;
            _topicTemplateService = topicTemplateService;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the topic model
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic model
        /// </returns>
        public virtual async Task<TopicModel> PrepareTopicModelAsync(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            return new TopicModel
            {
                Id = topic.Id,
                SystemName = topic.SystemName,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                Title = topic.IsPasswordProtected ? string.Empty : await _localizationService.GetLocalizedAsync(topic, x => x.Title),
                Body = topic.IsPasswordProtected ? string.Empty : await _localizationService.GetLocalizedAsync(topic, x => x.Body),
                MetaKeywords = await _localizationService.GetLocalizedAsync(topic, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(topic, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(topic, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(topic),
                TopicTemplateId = topic.TopicTemplateId
            };
        }

        /// <summary>
        /// Get the topic model by topic system name
        /// </summary>
        /// <param name="systemName">Topic system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic model
        /// </returns>
        public virtual async Task<TopicModel> PrepareTopicModelBySystemNameAsync(string systemName)
        {
            //load by store
            var store = await _storeContext.GetCurrentStoreAsync();
            var topic = await _topicService.GetTopicBySystemNameAsync(systemName, store.Id);
            if (topic == null)
                return null;

            return await PrepareTopicModelAsync(topic);
        }

        /// <summary>
        /// Get topic template view path
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view path
        /// </returns>
        public virtual async Task<string> PrepareTemplateViewPathAsync(int topicTemplateId)
        {
            var template = await _topicTemplateService.GetTopicTemplateByIdAsync(topicTemplateId) ??
                           (await _topicTemplateService.GetAllTopicTemplatesAsync()).FirstOrDefault();

            if (template == null)
                throw new Exception("No default template could be loaded");

            return template.ViewPath;
        }

        #endregion
    }
}