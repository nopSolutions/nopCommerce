using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Topics;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
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
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ITopicService _topicService;
        private readonly ITopicTemplateService _topicTemplateService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public TopicModelFactory(IAclService aclService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            ITopicService topicService,
            ITopicTemplateService topicTemplateService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _customerService = customerService;
            _localizationService = localizationService;
            _cacheManager = cacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
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
                Title = topic.IsPasswordProtected ? string.Empty : _localizationService.GetLocalized(topic, x => x.Title),
                Body = topic.IsPasswordProtected ? string.Empty : _localizationService.GetLocalized(topic, x => x.Body),
                MetaKeywords = _localizationService.GetLocalized(topic, x => x.MetaKeywords),
                MetaDescription = _localizationService.GetLocalized(topic, x => x.MetaDescription),
                MetaTitle = _localizationService.GetLocalized(topic, x => x.MetaTitle),
                SeName = _urlRecordService.GetSeName(topic),
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
        public virtual TopicModel PrepareTopicModelById(int topicId, bool showHidden = false)
        {
            var cacheKey = string.Format(NopModelCacheDefaults.TopicModelByIdKey,
                topicId,
                _workContext.WorkingLanguage.Id,
                _storeContext.CurrentStore.Id,
                string.Join(",", _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer)),
                showHidden);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var topic = _topicService.GetTopicById(topicId);
                if (topic == null)
                    return null;

                if (!showHidden)
                {
                    if (!topic.Published ||
                        //ACL (access control list)
                        !_aclService.Authorize(topic) ||
                        //store mapping
                        !_storeMappingService.Authorize(topic))
                        return null;
                }

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
                string.Join(",", _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer)));
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
            var template = _topicTemplateService.GetTopicTemplateById(topicTemplateId) ??
                           _topicTemplateService.GetAllTopicTemplates().FirstOrDefault();

            if (template == null)
                throw new Exception("No default template could be loaded");

            return template.ViewPath;
        }

        #endregion
    }
}