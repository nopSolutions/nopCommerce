using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the message template model factory implementation
    /// </summary>
    public partial class MessageTemplateModelFactory : IMessageTemplateModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public MessageTemplateModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreService storeService)
        {
            _catalogSettings = catalogSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _messageTemplateService = messageTemplateService;
            _messageTokenProvider = messageTokenProvider;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _storeService = storeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare message template search model
        /// </summary>
        /// <param name="searchModel">Message template search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the message template search model
        /// </returns>
        public virtual async Task<MessageTemplateSearchModel> PrepareMessageTemplateSearchModelAsync(MessageTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged message template list model
        /// </summary>
        /// <param name="searchModel">Message template search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the message template list model
        /// </returns>
        public virtual async Task<MessageTemplateListModel> PrepareMessageTemplateListModelAsync(MessageTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get message templates
            var messageTemplates = (await _messageTemplateService
                .GetAllMessageTemplatesAsync(searchModel.SearchStoreId, searchModel.SearchKeywords)).ToPagedList(searchModel);

            //prepare store names (to avoid loading for each message template)
            var stores = (await _storeService.GetAllStoresAsync()).Select(store => new { store.Id, store.Name }).ToList();

            //prepare list model
            var model = await new MessageTemplateListModel().PrepareToGridAsync(searchModel, messageTemplates, () =>
            {
                return messageTemplates.SelectAwait(async messageTemplate =>
                {
                    //fill in model values from the entity
                    var messageTemplateModel = messageTemplate.ToModel<MessageTemplateModel>();

                    //fill in additional values (not existing in the entity)
                    var storeNames = stores.Select(store => store.Name);
                    if (messageTemplate.LimitedToStores)
                    {
                        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(messageTemplateModel, messageTemplate, false);
                        storeNames = stores
                            .Where(store => messageTemplateModel.SelectedStoreIds.Contains(store.Id)).Select(store => store.Name);
                    }

                    messageTemplateModel.ListOfStores = string.Join(", ", storeNames);

                    return messageTemplateModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare message template model
        /// </summary>
        /// <param name="model">Message template model</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the message template model
        /// </returns>
        public virtual async Task<MessageTemplateModel> PrepareMessageTemplateModelAsync(MessageTemplateModel model,
            MessageTemplate messageTemplate, bool excludeProperties = false)
        {
            Func<MessageTemplateLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (messageTemplate != null)
            {
                //fill in model values from the entity
                model ??= messageTemplate.ToModel<MessageTemplateModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.BccEmailAddresses = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.BccEmailAddresses, languageId, false, false);
                    locale.Subject = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.Subject, languageId, false, false);
                    locale.Body = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.Body, languageId, false, false);
                    locale.EmailAccountId = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.EmailAccountId, languageId, false, false);

                    //prepare available email accounts
                    await _baseAdminModelFactory.PrepareEmailAccountsAsync(locale.AvailableEmailAccounts,
                        defaultItemText: await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount.Standard"));

                    //PrepareEmailAccounts only gets available accounts, we need to set the item as selected manually
                    if (locale.AvailableEmailAccounts?.FirstOrDefault(x => x.Value == locale.EmailAccountId.ToString()) is SelectListItem emailAccountListItem)
                    {
                        emailAccountListItem.Selected = true;
                    }

                };
            }

            model.SendImmediately = !model.DelayBeforeSend.HasValue;
            model.HasAttachedDownload = model.AttachedDownloadId > 0;

            var allowedTokens = string.Join(", ", await _messageTokenProvider.GetListOfAllowedTokensAsync(_messageTokenProvider.GetTokenGroups(messageTemplate)));
            model.AllowedTokens = $"{allowedTokens}{Environment.NewLine}{Environment.NewLine}" +
                $"{await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Tokens.ConditionalStatement")}{Environment.NewLine}";

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare available email accounts
            await _baseAdminModelFactory.PrepareEmailAccountsAsync(model.AvailableEmailAccounts);

            //prepare available stores
            await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, messageTemplate, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare test message template model
        /// </summary>
        /// <param name="model">Test message template model</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the st message template model
        /// </returns>
        public virtual async Task<TestMessageTemplateModel> PrepareTestMessageTemplateModelAsync(TestMessageTemplateModel model,
            MessageTemplate messageTemplate, int languageId)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            model.Id = messageTemplate.Id;
            model.LanguageId = languageId;

            //filter tokens to the current template
            var subject = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.Subject, languageId);
            var body = await _localizationService.GetLocalizedAsync(messageTemplate, entity => entity.Body, languageId);
            model.Tokens = (await _messageTokenProvider.GetListOfAllowedTokensAsync())
                .Where(token => subject.Contains(token) || body.Contains(token)).ToList();

            return model;
        }

        #endregion
    }
}