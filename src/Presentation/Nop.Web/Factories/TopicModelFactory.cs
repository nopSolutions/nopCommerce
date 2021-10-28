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

        protected IAclService AclService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected ITopicService TopicService { get; }
        protected ITopicTemplateService TopicTemplateService { get; }
        protected IUrlRecordService UrlRecordService { get; }

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
            AclService = aclService;
            LocalizationService = localizationService;
            StoreContext = storeContext;
            StoreMappingService = storeMappingService;
            TopicService = topicService;
            TopicTemplateService = topicTemplateService;
            UrlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the topic model
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic model
        /// </returns>
        protected virtual async Task<TopicModel> PrepareTopicModelAsync(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));

            var model = new TopicModel
            {
                Id = topic.Id,
                SystemName = topic.SystemName,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                Title = topic.IsPasswordProtected ? string.Empty : await LocalizationService.GetLocalizedAsync(topic, x => x.Title),
                Body = topic.IsPasswordProtected ? string.Empty : await LocalizationService.GetLocalizedAsync(topic, x => x.Body),
                MetaKeywords = await LocalizationService.GetLocalizedAsync(topic, x => x.MetaKeywords),
                MetaDescription = await LocalizationService.GetLocalizedAsync(topic, x => x.MetaDescription),
                MetaTitle = await LocalizationService.GetLocalizedAsync(topic, x => x.MetaTitle),
                SeName = await UrlRecordService.GetSeNameAsync(topic),
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic model
        /// </returns>
        public virtual async Task<TopicModel> PrepareTopicModelByIdAsync(int topicId, bool showHidden = false)
        {
            var topic = await TopicService.GetTopicByIdAsync(topicId);

            if (topic == null)
                return null;

            if (showHidden)
                return await PrepareTopicModelAsync(topic);

            if (!topic.Published ||
                //ACL (access control list)
                !await AclService.AuthorizeAsync(topic) ||
                //store mapping
                !await StoreMappingService.AuthorizeAsync(topic))

                return null;

            return await PrepareTopicModelAsync(topic);
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
            var store = await StoreContext.GetCurrentStoreAsync();
            var topic = await TopicService.GetTopicBySystemNameAsync(systemName, store.Id);
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
            var template = await TopicTemplateService.GetTopicTemplateByIdAsync(topicTemplateId) ??
                           (await TopicTemplateService.GetAllTopicTemplatesAsync()).FirstOrDefault();

            if (template == null)
                throw new Exception("No default template could be loaded");

            return template.ViewPath;
        }

        #endregion
    }
}