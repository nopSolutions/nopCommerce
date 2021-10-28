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

        protected CatalogSettings CatalogSettings { get; }
        protected IBaseAdminModelFactory BaseAdminModelFactory { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedModelFactory LocalizedModelFactory { get; }
        protected IMessageTemplateService MessageTemplateService { get; }
        protected IMessageTokenProvider MessageTokenProvider { get; }
        protected IStoreMappingSupportedModelFactory StoreMappingSupportedModelFactory { get; }
        protected IStoreService StoreService { get; }

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
            CatalogSettings = catalogSettings;
            BaseAdminModelFactory = baseAdminModelFactory;
            LocalizationService = localizationService;
            LocalizedModelFactory = localizedModelFactory;
            MessageTemplateService = messageTemplateService;
            MessageTokenProvider = messageTokenProvider;
            StoreMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            StoreService = storeService;
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
            await BaseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = CatalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

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
            var messageTemplates = (await MessageTemplateService
                .GetAllMessageTemplatesAsync(searchModel.SearchStoreId, searchModel.SearchKeywords)).ToPagedList(searchModel);

            //prepare store names (to avoid loading for each message template)
            var stores = (await StoreService.GetAllStoresAsync()).Select(store => new { store.Id, store.Name }).ToList();

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
                        await StoreMappingSupportedModelFactory.PrepareModelStoresAsync(messageTemplateModel, messageTemplate, false);
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
            Action<MessageTemplateLocalizedModel, int> localizedModelConfiguration = null;

            if (messageTemplate != null)
            {
                //fill in model values from the entity
                model ??= messageTemplate.ToModel<MessageTemplateModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.BccEmailAddresses = await LocalizationService.GetLocalizedAsync(messageTemplate, entity => entity.BccEmailAddresses, languageId, false, false);
                    locale.Subject = await LocalizationService.GetLocalizedAsync(messageTemplate, entity => entity.Subject, languageId, false, false);
                    locale.Body = await LocalizationService.GetLocalizedAsync(messageTemplate, entity => entity.Body, languageId, false, false);
                    locale.EmailAccountId = await LocalizationService.GetLocalizedAsync(messageTemplate, entity => entity.EmailAccountId, languageId, false, false);

                    //prepare available email accounts
                    await BaseAdminModelFactory.PrepareEmailAccountsAsync(locale.AvailableEmailAccounts,
                        defaultItemText: await LocalizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount.Standard"));

                    //PrepareEmailAccounts only gets available accounts, we need to set the item as selected manually
                    if (locale.AvailableEmailAccounts?.FirstOrDefault(x => x.Value == locale.EmailAccountId.ToString()) is SelectListItem emailAccountListItem)
                    {
                        emailAccountListItem.Selected = true;
                    }

                };
            }

            model.SendImmediately = !model.DelayBeforeSend.HasValue;
            model.HasAttachedDownload = model.AttachedDownloadId > 0;

            var allowedTokens = string.Join(", ", await MessageTokenProvider.GetListOfAllowedTokensAsync(MessageTokenProvider.GetTokenGroups(messageTemplate)));
            model.AllowedTokens = $"{allowedTokens}{Environment.NewLine}{Environment.NewLine}" +
                $"{await LocalizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates.Tokens.ConditionalStatement")}{Environment.NewLine}";

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await LocalizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare available email accounts
            await BaseAdminModelFactory.PrepareEmailAccountsAsync(model.AvailableEmailAccounts);

            //prepare available stores
            await StoreMappingSupportedModelFactory.PrepareModelStoresAsync(model, messageTemplate, excludeProperties);

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
            var subject = await LocalizationService.GetLocalizedAsync(messageTemplate, entity => entity.Subject, languageId);
            var body = await LocalizationService.GetLocalizedAsync(messageTemplate, entity => entity.Body, languageId);
            model.Tokens = (await MessageTokenProvider.GetListOfAllowedTokensAsync())
                .Where(token => subject.Contains(token) || body.Contains(token)).ToList();

            return model;
        }

        #endregion
    }
}