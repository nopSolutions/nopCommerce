using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;

namespace Nop.Services.Messages
{
    public partial class MessageTemplateService: IMessageTemplateService
    {
        #region Constants

        private const string MESSAGETEMPLATES_ALL_KEY = "Nop.messagetemplate.all";
        private const string MESSAGETEMPLATES_BY_ID_KEY = "Nop.messagetemplate.id-{0}";
        private const string MESSAGETEMPLATES_BY_NAME_KEY = "Nop.messagetemplate.name-{0}";
        private const string MESSAGETEMPLATES_PATTERN_KEY = "Nop.messagetemplate.";

        #endregion

        #region Fields

        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="messageTemplateRepository">Message template repository</param>
        /// <param name="eventPublisher"></param>
        public MessageTemplateService(ICacheManager cacheManager,
            IRepository<MessageTemplate> messageTemplateRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _messageTemplateRepository = messageTemplateRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

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

            string key = string.Format(MESSAGETEMPLATES_BY_ID_KEY, messageTemplateId);
            return _cacheManager.Get(key, () =>
            {
                var manufacturer = _messageTemplateRepository.GetById(messageTemplateId);
                return manufacturer;
            });
        }

        /// <summary>
        /// Gets a message template
        /// </summary>
        /// <param name="messageTemplateName">Message template name</param>
        /// <returns>Message template</returns>
        public virtual MessageTemplate GetMessageTemplateByName(string messageTemplateName)
        {
            if (string.IsNullOrWhiteSpace(messageTemplateName))
                throw new ArgumentException("messageTemplateName");

            string key = string.Format(MESSAGETEMPLATES_BY_NAME_KEY, messageTemplateName);
            return _cacheManager.Get(key, () =>
            {
                var query = from mt in _messageTemplateRepository.Table
                                   where mt.Name == messageTemplateName
                                   select mt;
                return query.FirstOrDefault();
            });

        }

        /// <summary>
        /// Gets all message templates
        /// </summary>
        /// <returns>Message template list</returns>
        public virtual IList<MessageTemplate> GetAllMessageTemplates()
        {
            return _cacheManager.Get(MESSAGETEMPLATES_ALL_KEY, () =>
            {
                var query = from mt in _messageTemplateRepository.Table
                            orderby mt.Name                            
                            select mt;
                var messageTemplates = query.ToList();
                return messageTemplates;
            });
        }

        #endregion
    }
}
