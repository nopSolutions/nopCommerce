using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Stores;

namespace Nop.Services.Messages
{
    public partial class MessageTemplateService: IMessageTemplateService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        private const string MESSAGETEMPLATES_ALL_KEY = "Nop.messagetemplate.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// {1} : store ID
        /// </remarks>
        private const string MESSAGETEMPLATES_BY_NAME_KEY = "Nop.messagetemplate.name-{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string MESSAGETEMPLATES_PATTERN_KEY = "Nop.messagetemplate.";

        #endregion

        #region Fields

        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly ILanguageService _languageService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="languageService">Language service</param>
        /// <param name="localizedEntityService">Localized entity service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="messageTemplateRepository">Message template repository</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event published</param>
        public MessageTemplateService(ICacheManager cacheManager,
            IRepository<StoreMapping> storeMappingRepository,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IStoreMappingService storeMappingService,
            IRepository<MessageTemplate> messageTemplateRepository,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._storeMappingRepository = storeMappingRepository;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._storeMappingService = storeMappingService;
            this._messageTemplateRepository = messageTemplateRepository;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public virtual void DeleteMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

            _messageTemplateRepository.Delete(messageTemplate);

            _cacheManager.RemoveByPattern(MESSAGETEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(messageTemplate);
        }

        /// <summary>
        /// Inserts a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public virtual void InsertMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

            _messageTemplateRepository.Insert(messageTemplate);

            _cacheManager.RemoveByPattern(MESSAGETEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(messageTemplate);
        }

        /// <summary>
        /// Updates a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public virtual void UpdateMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

            _messageTemplateRepository.Update(messageTemplate);

            _cacheManager.RemoveByPattern(MESSAGETEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(messageTemplate);
        }

        /// <summary>
        /// Gets a message template
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <returns>Message template</returns>
        public virtual MessageTemplate GetMessageTemplateById(int messageTemplateId)
        {
            if (messageTemplateId == 0)
                return null;

            return _messageTemplateRepository.GetById(messageTemplateId);
        }

        /// <summary>
        /// Gets a message template
        /// </summary>
        /// <param name="messageTemplateName">Message template name</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Message template</returns>
        public virtual MessageTemplate GetMessageTemplateByName(string messageTemplateName, int storeId)
        {
            if (string.IsNullOrWhiteSpace(messageTemplateName))
                throw new ArgumentException("messageTemplateName");

            string key = string.Format(MESSAGETEMPLATES_BY_NAME_KEY, messageTemplateName, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = _messageTemplateRepository.Table;
                query = query.Where(t => t.Name == messageTemplateName);
                query = query.OrderBy(t => t.Id);
                var templates = query.ToList();

                //store mapping
                if (storeId > 0)
                {
                    templates = templates
                        .Where(t => _storeMappingService.Authorize(t, storeId))
                        .ToList();
                }

                return templates.FirstOrDefault();
            });

        }

        /// <summary>
        /// Gets all message templates
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <returns>Message template list</returns>
        public virtual IList<MessageTemplate> GetAllMessageTemplates(int storeId)
        {
            string key = string.Format(MESSAGETEMPLATES_ALL_KEY, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = _messageTemplateRepository.Table;
                query = query.OrderBy(t => t.Name);

                //Store mapping
                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    query = from t in query
                            join sm in _storeMappingRepository.Table
                            on new { c1 = t.Id, c2 = "MessageTemplate" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into t_sm
                            from sm in t_sm.DefaultIfEmpty()
                            where !t.LimitedToStores || storeId == sm.StoreId
                            select t;

                    //only distinct items (group by ID)
                    query = from t in query
                            group t by t.Id
                            into tGroup
                            orderby tGroup.Key
                            select tGroup.FirstOrDefault();
                    query = query.OrderBy(t => t.Name);
                }

                return query.ToList();
            });
        }

        /// <summary>
        /// Create a copy of message template with all depended data
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Message template copy</returns>
        public virtual MessageTemplate CopyMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

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

            InsertMessageTemplate(mtCopy);

            var languages = _languageService.GetAllLanguages(true);

            //localization
            foreach (var lang in languages)
            {
                var bccEmailAddresses = messageTemplate.GetLocalized(x => x.BccEmailAddresses, lang.Id, false, false);
                if (!String.IsNullOrEmpty(bccEmailAddresses))
                    _localizedEntityService.SaveLocalizedValue(mtCopy, x => x.BccEmailAddresses, bccEmailAddresses, lang.Id);

                var subject = messageTemplate.GetLocalized(x => x.Subject, lang.Id, false, false);
                if (!String.IsNullOrEmpty(subject))
                    _localizedEntityService.SaveLocalizedValue(mtCopy, x => x.Subject, subject, lang.Id);

                var body = messageTemplate.GetLocalized(x => x.Body, lang.Id, false, false);
                if (!String.IsNullOrEmpty(body))
                    _localizedEntityService.SaveLocalizedValue(mtCopy, x => x.Body, body, lang.Id);

                var emailAccountId = messageTemplate.GetLocalized(x => x.EmailAccountId, lang.Id, false, false);
                if (emailAccountId > 0)
                    _localizedEntityService.SaveLocalizedValue(mtCopy, x => x.EmailAccountId, emailAccountId, lang.Id);
            }

            //store mapping
            var selectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(messageTemplate);
            foreach (var id in selectedStoreIds)
            {
                _storeMappingService.InsertStoreMapping(mtCopy, id);
            }

            return mtCopy;
        }

        #endregion
    }
}
