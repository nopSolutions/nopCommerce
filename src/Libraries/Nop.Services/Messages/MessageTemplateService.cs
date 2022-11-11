using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Stores;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Message template service
    /// </summary>
    public partial class MessageTemplateService : IMessageTemplateService
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        public MessageTemplateService(
            IStaticCacheManager staticCacheManager,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IRepository<MessageTemplate> messageTemplateRepository,
            IStoreMappingService storeMappingService)
        {
            _staticCacheManager = staticCacheManager;
            _languageService = languageService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _messageTemplateRepository = messageTemplateRepository;
            _storeMappingService = storeMappingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteMessageTemplateAsync(MessageTemplate messageTemplate)
        {
            await _messageTemplateRepository.DeleteAsync(messageTemplate);
        }

        /// <summary>
        /// Inserts a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertMessageTemplateAsync(MessageTemplate messageTemplate)
        {
            await _messageTemplateRepository.InsertAsync(messageTemplate);
        }

        /// <summary>
        /// Updates a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateMessageTemplateAsync(MessageTemplate messageTemplate)
        {
            await _messageTemplateRepository.UpdateAsync(messageTemplate);
        }

        /// <summary>
        /// Gets a message template
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the message template
        /// </returns>
        public virtual async Task<MessageTemplate> GetMessageTemplateByIdAsync(int messageTemplateId)
        {
            return await _messageTemplateRepository.GetByIdAsync(messageTemplateId, cache => default);
        }

        /// <summary>
        /// Gets message templates by the name
        /// </summary>
        /// <param name="messageTemplateName">Message template name</param>
        /// <param name="storeId">Store identifier; pass null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of message templates
        /// </returns>
        public virtual async Task<IList<MessageTemplate>> GetMessageTemplatesByNameAsync(string messageTemplateName, int? storeId = null)
        {
            if (string.IsNullOrWhiteSpace(messageTemplateName))
                throw new ArgumentException(nameof(messageTemplateName));

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopMessageDefaults.MessageTemplatesByNameCacheKey, messageTemplateName, storeId);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                //get message templates with the passed name
                var templates = await _messageTemplateRepository.Table
                    .Where(messageTemplate => messageTemplate.Name.Equals(messageTemplateName))
                    .OrderBy(messageTemplate => messageTemplate.Id)
                    .ToListAsync();

                //filter by the store
                if (storeId.HasValue && storeId.Value > 0)
                    templates = await templates.WhereAwait(async messageTemplate => await _storeMappingService.AuthorizeAsync(messageTemplate, storeId.Value)).ToListAsync();

                return templates;
            });
        }

        /// <summary>
        /// Gets all message templates
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="keywords">Keywords to search by name, body, or subject</param>
        /// <param name="isActive">A value indicating whether to get active records; "null" to load all records; "false" to load only inactive records; "true" to load only active records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the message template list
        /// </returns>
        public virtual async Task<IList<MessageTemplate>> GetAllMessageTemplatesAsync(int storeId, string keywords = null, bool? isActive = null)
        {
            var messageTemplates = await _messageTemplateRepository.GetAllAsync(async query =>
            {
                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                if (isActive.HasValue)
                    query = query.Where(mt => mt.IsActive == isActive);

                return query.OrderBy(t => t.Name);
            }, cache => cache.PrepareKeyForDefaultCache(NopMessageDefaults.MessageTemplatesAllCacheKey, storeId, isActive));

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                messageTemplates = messageTemplates.Where(x => (x.Subject?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false)
                    || (x.Body?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false)
                    || (x.Name?.Contains(keywords, StringComparison.InvariantCultureIgnoreCase) ?? false)).ToList();
            }

            return messageTemplates;
        }

        /// <summary>
        /// Create a copy of message template with all depended data
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the message template copy
        /// </returns>
        public virtual async Task<MessageTemplate> CopyMessageTemplateAsync(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            var mtCopy = new MessageTemplate
            {
                Name = messageTemplate.Name,
                BccEmailAddresses = messageTemplate.BccEmailAddresses,
                Subject = messageTemplate.Subject,
                Body = messageTemplate.Body,
                IsActive = messageTemplate.IsActive,
                AttachedDownloadId = messageTemplate.AttachedDownloadId,
                EmailAccountId = messageTemplate.EmailAccountId,
                LimitedToStores = messageTemplate.LimitedToStores,
                DelayBeforeSend = messageTemplate.DelayBeforeSend,
                DelayPeriod = messageTemplate.DelayPeriod
            };

            await InsertMessageTemplateAsync(mtCopy);

            var languages = await _languageService.GetAllLanguagesAsync(true);

            //localization
            foreach (var lang in languages)
            {
                var bccEmailAddresses = await _localizationService.GetLocalizedAsync(messageTemplate, x => x.BccEmailAddresses, lang.Id, false, false);
                if (!string.IsNullOrEmpty(bccEmailAddresses))
                    await _localizedEntityService.SaveLocalizedValueAsync(mtCopy, x => x.BccEmailAddresses, bccEmailAddresses, lang.Id);

                var subject = await _localizationService.GetLocalizedAsync(messageTemplate, x => x.Subject, lang.Id, false, false);
                if (!string.IsNullOrEmpty(subject))
                    await _localizedEntityService.SaveLocalizedValueAsync(mtCopy, x => x.Subject, subject, lang.Id);

                var body = await _localizationService.GetLocalizedAsync(messageTemplate, x => x.Body, lang.Id, false, false);
                if (!string.IsNullOrEmpty(body))
                    await _localizedEntityService.SaveLocalizedValueAsync(mtCopy, x => x.Body, body, lang.Id);

                var emailAccountId = await _localizationService.GetLocalizedAsync(messageTemplate, x => x.EmailAccountId, lang.Id, false, false);
                if (emailAccountId > 0)
                    await _localizedEntityService.SaveLocalizedValueAsync(mtCopy, x => x.EmailAccountId, emailAccountId, lang.Id);
            }

            //store mapping
            var selectedStoreIds = await _storeMappingService.GetStoresIdsWithAccessAsync(messageTemplate);
            foreach (var id in selectedStoreIds)
                await _storeMappingService.InsertStoreMappingAsync(mtCopy, id);

            return mtCopy;
        }

        #endregion
    }
}