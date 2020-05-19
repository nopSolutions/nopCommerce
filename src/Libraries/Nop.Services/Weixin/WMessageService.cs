using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Core.Domain.Weixin;
using Nop.Core.Html;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// WMessageService
    /// </summary>
    public partial class WMessageService : IWMessageService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<WMessage> _wMessageRepository;

        #endregion

        #region Ctor

        public WMessageService(IEventPublisher eventPublisher,
            IRepository<WMessage> wMessageRepository)
        {
            _eventPublisher = eventPublisher;
            _wMessageRepository = wMessageRepository;
        }

        #endregion

        #region Methods

        public virtual void InsertWMessage(WMessage wMessage)
        {
            if (wMessage == null)
                throw new ArgumentNullException(nameof(wMessage));

            _wMessageRepository.Insert(wMessage);

            //event notification
            _eventPublisher.EntityInserted(wMessage);
        }

        public virtual void DeleteWMessage(WMessage wMessage, bool delete = false)
        {
            if (wMessage == null)
                throw new ArgumentNullException(nameof(wMessage));

            if(delete)
            {
                _wMessageRepository.Delete(wMessage);
            }
            else
            {
                wMessage.Deleted = true;
                UpdateWMessage(wMessage);
            }

            //event notification
            _eventPublisher.EntityDeleted(wMessage);
        }

        public virtual void DeleteWMessages(IList<WMessage> wMessages, bool deleted = false)
        {
            if (wMessages == null)
                throw new ArgumentNullException(nameof(wMessages));

            if(deleted)
            {
                _wMessageRepository.Delete(wMessages);
            }
            else
            {
                foreach (var wMessage in wMessages)
                {
                    wMessage.Deleted = true;
                }
                //delete wUser
                UpdateWMessages(wMessages);
            }

            foreach (var wMessage in wMessages)
            {
                //event notification
                _eventPublisher.EntityDeleted(wMessage);
            }
        }

        public virtual void UpdateWMessage(WMessage wMessage)
        {
            if (wMessage == null)
                throw new ArgumentNullException(nameof(wMessage));

            _wMessageRepository.Update(wMessage);

            //event notification
            _eventPublisher.EntityUpdated(wMessage);
        }

        public virtual void UpdateWMessages(IList<WMessage> wMessages)
        {
            if (wMessages == null)
                throw new ArgumentNullException(nameof(wMessages));

            //update
            _wMessageRepository.Update(wMessages);

            //event notification
            foreach (var wMessage in wMessages)
            {
                _eventPublisher.EntityUpdated(wMessage);
            }
        }

        public virtual WMessage GetWMessageById(int id)
        {
            if (id == 0)
                return null;

            return _wMessageRepository.GetById(id);
        }

        public virtual List<WMessage> GetWMessagesByIds(int[] wMessageIds)
        {
            if (wMessageIds is null || wMessageIds.Length == 0)
                return new List<WMessage>();

            var query = from t in _wMessageRepository.Table
                        where wMessageIds.Contains(t.Id) &&
                        !t.Deleted &&
                        t.Published
                        select t;

            var messages = query.ToList();

            //sort by passed identifiers
            var sortedMessages = new List<WMessage>();
            foreach (var id in wMessageIds)
            {
                var message = messages.Find(x => x.Id == id);
                if (message != null)
                    sortedMessages.Add(message);
            }

            return sortedMessages;
        }

        public virtual IPagedList<WMessage> GetWMessages(
            string title = "", 
            bool? published = null,
            bool? deleted = null, 
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _wMessageRepository.Table;

            if (!string.IsNullOrEmpty(title))
                query = query.Where(v => v.Title.Contains(title));
            if (published.HasValue)
                query = query.Where(v => v.Published == published);
            if (deleted.HasValue)
                query = query.Where(v => v.Deleted == deleted);

            query = query.OrderBy(v => v.CreatTime).ThenBy(v => v.Id);

            return new PagedList<WMessage>(query, pageIndex, pageSize);
        }

        #endregion
    }
}