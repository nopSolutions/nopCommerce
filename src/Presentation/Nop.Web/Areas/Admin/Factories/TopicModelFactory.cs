using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Topics;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the topic model factory implementation
    /// </summary>
    public partial class TopicModelFactory : ITopicModelFactory
    {
        #region Fields

        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly ITopicService _topicService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public TopicModelFactory(IAclSupportedModelFactory aclSupportedModelFactory,
            IActionContextAccessor actionContextAccessor,
            IBaseAdminModelFactory baseAdminModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            ITopicService topicService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper)
        {
            this._aclSupportedModelFactory = aclSupportedModelFactory;
            this._actionContextAccessor = actionContextAccessor;
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._localizationService = localizationService;
            this._localizedModelFactory = localizedModelFactory;
            this._storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            this._topicService = topicService;
            this._urlHelperFactory = urlHelperFactory;
            this._urlRecordService = urlRecordService;
            this._webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare topic search model
        /// </summary>
        /// <param name="searchModel">Topic search model</param>
        /// <returns>Topic search model</returns>
        public virtual TopicSearchModel PrepareTopicSearchModel(TopicSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged topic list model
        /// </summary>
        /// <param name="searchModel">Topic search model</param>
        /// <returns>Topic list model</returns>
        public virtual TopicListModel PrepareTopicListModel(TopicSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get topics
            var topics = _topicService.GetAllTopics(showHidden: true,
                storeId: searchModel.SearchStoreId,
                ignorAcl: true);

            //filter topics
            //TODO: move filter to topic service
            if (!string.IsNullOrEmpty(searchModel.SearchKeywords))
            {
                topics = topics.Where(topic => (topic.Title?.Contains(searchModel.SearchKeywords) ?? false) ||
                                               (topic.Body?.Contains(searchModel.SearchKeywords) ?? false)).ToList();
            }

            //prepare grid model
            var model = new TopicListModel
            {
                Data = topics.PaginationByRequestModel(searchModel).Select(topic =>
                {
                    //fill in model values from the entity
                    var topicModel = topic.ToModel<TopicModel>();

                    //little performance optimization: ensure that "Body" is not returned
                    topicModel.Body = string.Empty;

                    return topicModel;
                }),
                Total = topics.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare topic model
        /// </summary>
        /// <param name="model">Topic model</param>
        /// <param name="topic">Topic</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Topic model</returns>
        public virtual TopicModel PrepareTopicModel(TopicModel model, Topic topic, bool excludeProperties = false)
        {
            Action<TopicLocalizedModel, int> localizedModelConfiguration = null;

            if (topic != null)
            {
                //fill in model values from the entity
                model = model ?? topic.ToModel<TopicModel>();

                model.Url = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext)
                    .RouteUrl("Topic", new { SeName = _urlRecordService.GetSeName(topic) }, _webHelper.CurrentRequestProtocol);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Title = _localizationService.GetLocalized(topic, entity => entity.Title, languageId, false, false);
                    locale.Body = _localizationService.GetLocalized(topic, entity => entity.Body, languageId, false, false);
                    locale.MetaKeywords = _localizationService.GetLocalized(topic, entity => entity.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = _localizationService.GetLocalized(topic, entity => entity.MetaDescription, languageId, false, false);
                    locale.MetaTitle = _localizationService.GetLocalized(topic, entity => entity.MetaTitle, languageId, false, false);
                    locale.SeName = _urlRecordService.GetSeName(topic, languageId, false, false);
                };
            }

            //set default values for the new model
            if (topic == null)
            {
                model.DisplayOrder = 1;
                model.Published = true;
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            //prepare available topic templates
            _baseAdminModelFactory.PrepareTopicTemplates(model.AvailableTopicTemplates, false);

            //prepare model customer roles
            _aclSupportedModelFactory.PrepareModelCustomerRoles(model, topic, excludeProperties);

            //prepare model stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, topic, excludeProperties);

            return model;
        }

        #endregion
    }
}