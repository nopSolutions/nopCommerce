using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Topics;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Infrastructure.Cache;
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
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ITopicService _topicService;
        private readonly ITopicTemplateService _topicTemplateService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public TopicModelFactory(IAclService aclService,
            ILocalizationService localizationService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            ITopicService topicService,
            ITopicTemplateService topicTemplateService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _localizationService = localizationService;
            _cacheManager = cacheManager;
            _storeContext = storeContext;
            _topicService = topicService;
            _topicTemplateService = topicTemplateService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the topic model
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>Topic model</returns>
        protected virtual TopicModel PrepareTopicModel(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            var model = new TopicModel
            {
                Id = topic.Id,
                SystemName = topic.SystemName,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                Title = topic.IsPasswordProtected ? "" : _localizationService.GetLocalized(topic, x => x.Title),
                Body = topic.IsPasswordProtected ? "" : _localizationService.GetLocalized(topic, x => x.Body),
                MetaKeywords = _localizationService.GetLocalized(topic, x => x.MetaKeywords),
                MetaDescription = _localizationService.GetLocalized(topic, x => x.MetaDescription),
                MetaTitle = _localizationService.GetLocalized(topic, x => x.MetaTitle),
                SeName = _urlRecordService.GetSeName(topic),
                TopicTemplateId = topic.TopicTemplateId,
                Published = topic.Published
            };
            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the topic model by topic identifier
        /// </summary>
        /// <param name="topicId">Topic identifier</param>
        /// <returns>Topic model</returns>
        public virtual TopicModel PrepareTopicModelById(int topicId)
        {
            var cacheKey = string.Format(NopModelCacheDefaults.TopicModelByIdKey,
                topicId,
                _workContext.WorkingLanguage.Id,
                _storeContext.CurrentStore.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var topic = _topicService.GetTopicById(topicId);
                //ACL (access control list)
                if (topic == null || !_aclService.Authorize(topic))
                    return null;
                return PrepareTopicModel(topic);
            });

            return cachedModel;
        }

        /// <summary>
        /// Get the topic model by topic system name
        /// </summary>
        /// <param name="systemName">Topic system name</param>
        /// <returns>Topic model</returns>
        public virtual TopicModel PrepareTopicModelBySystemName(string systemName)
        {
            var cacheKey = string.Format(NopModelCacheDefaults.TopicModelBySystemNameKey,
                systemName,
                _workContext.WorkingLanguage.Id,
                _storeContext.CurrentStore.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                //load by store
                var topic = _topicService.GetTopicBySystemName(systemName, _storeContext.CurrentStore.Id);
                if (topic == null)
                    return null;
                return PrepareTopicModel(topic);
            });

            return cachedModel;
        }

        /// <summary>
        /// Get topic template view path
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>View path</returns>
        public virtual string PrepareTemplateViewPath(int topicTemplateId)
        {
            var templateCacheKey = string.Format(NopModelCacheDefaults.TopicTemplateModelKey, topicTemplateId);
            var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _topicTemplateService.GetTopicTemplateById(topicTemplateId);
                if (template == null)
                    template = _topicTemplateService.GetAllTopicTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });
            return templateViewPath;
        }

        #endregion
    }
}