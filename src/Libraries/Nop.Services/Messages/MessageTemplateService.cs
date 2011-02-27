using System;
using System.Collections.Generic;
using System.Linq;

using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Nop.Data;

namespace Nop.Services.Messages
{
    public partial class MessageTemplateService: IMessageTemplateService
    {
        #region Constants

        private const string MESSAGETEMPLATES_ALL_KEY = "Nop.messagetemplate.all";
        private const string MESSAGETEMPLATES_BY_ID_KEY = "Nop.messagetemplate.id-{0}";
        private const string MESSAGETEMPLATES_PATTERN_KEY = "Nop.messagetemplate.";

        #endregion

        #region Fields

        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="messageTemplateRepository">Message template repository</param>
        public MessageTemplateService(ICacheManager cacheManager,
            IRepository<MessageTemplate> messageTemplateRepository)
        {
            this._cacheManager = cacheManager;
            this._messageTemplateRepository = messageTemplateRepository;
        }

        #endregion

        /// <summary>
        /// Inserts a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public void InsertMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

            _messageTemplateRepository.Insert(messageTemplate);

            _cacheManager.RemoveByPattern(MESSAGETEMPLATES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public void UpdateMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

            _messageTemplateRepository.Update(messageTemplate);

            _cacheManager.RemoveByPattern(MESSAGETEMPLATES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a message template
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <returns>Message template</returns>
        public MessageTemplate GetMessageTemplateById(int messageTemplateId)
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
        /// Gets all message templates
        /// </summary>
        /// <returns>Message template list</returns>
        public IList<MessageTemplate> GetAllMessageTemplates()
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
    }
}
