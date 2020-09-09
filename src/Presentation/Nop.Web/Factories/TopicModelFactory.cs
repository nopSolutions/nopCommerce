using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Topics;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
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

        private readonly IAclService _aclService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ITopicService _topicService;
        private readonly ITopicTemplateService _topicTemplateService;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public TopicModelFactory(IAclService aclService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            ITopicService topicService,
            ITopicTemplateService topicTemplateService,
            IUrlRecordService urlRecordService)
        {
            _aclService = aclService;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _topicService = topicService;
            _topicTemplateService = topicTemplateService;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the topic model
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>Topic model</returns>
        protected virtual async Task<TopicModel> PrepareTopicModel(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            var model = new TopicModel
            {
                Id = topic.Id,
                SystemName = topic.SystemName,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                Title = topic.IsPasswordProtected ? string.Empty : await _localizationService.GetLocalized(topic, x => x.Title),
                Body = topic.IsPasswordProtected ? string.Empty : await _localizationService.GetLocalized(topic, x => x.Body),
                MetaKeywords = await _localizationService.GetLocalized(topic, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalized(topic, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalized(topic, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeName(topic),
                TopicTemplateId = topic.TopicTemplateId
            };

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the topic model by topic identifier
        /// </summary>
        /// <param name="topicId">Topic identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Topic model</returns>
        public virtual async Task<TopicModel> PrepareTopicModelById(int topicId, bool showHidden = false)
        {
            var topic = await _topicService.GetTopicById(topicId);

            if (topic == null)
                return null;

            if (showHidden)
                return await PrepareTopicModel(topic);

            if (!topic.Published ||
                //ACL (access control list)
                !await _aclService.Authorize(topic) ||
                //store mapping
                !await _storeMappingService.Authorize(topic))

                return null;

            return await PrepareTopicModel(topic);
        }

        /// <summary>
        /// Get the topic model by topic system name
        /// </summary>
        /// <param name="systemName">Topic system name</param>
        /// <returns>Topic model</returns>
        public virtual async Task<TopicModel> PrepareTopicModelBySystemName(string systemName)
        {
            //load by store
            var topic = await _topicService.GetTopicBySystemName(systemName, (await _storeContext.GetCurrentStore()).Id);
            if (topic == null)
                return null;

            return await PrepareTopicModel(topic);
        }

        /// <summary>
        /// Get topic template view path
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>View path</returns>
        public virtual async Task<string> PrepareTemplateViewPath(int topicTemplateId)
        {
            var template = await _topicTemplateService.GetTopicTemplateById(topicTemplateId) ??
                           (await _topicTemplateService.GetAllTopicTemplates()).FirstOrDefault();

            if (template == null)
                throw new Exception("No default template could be loaded");

            return template.ViewPath;
        }

        #endregion
    }
}