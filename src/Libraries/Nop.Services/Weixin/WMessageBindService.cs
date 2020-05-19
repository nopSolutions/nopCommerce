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
    public partial class WMessageBindService : IWMessageBindService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<WMessageBindMapping> _wMessageBindRepository;

        #endregion

        #region Ctor

        public WMessageBindService(IEventPublisher eventPublisher,
            IRepository<WMessageBindMapping> wMessageBindRepository)
        {
            _eventPublisher = eventPublisher;
            _wMessageBindRepository = wMessageBindRepository;
        }

        #endregion

        #region Methods

        public virtual void InsertEntity(WMessageBindMapping messageBind)
        {
            if (messageBind == null)
                throw new ArgumentNullException(nameof(messageBind));

            _wMessageBindRepository.Insert(messageBind);

            //event notification
            _eventPublisher.EntityInserted(messageBind);
        }

        public virtual void DeleteEntity(WMessageBindMapping messageBind)
        {
            if (messageBind == null)
                throw new ArgumentNullException(nameof(messageBind));

            //Delete
            _wMessageBindRepository.Delete(messageBind);

            //event notification
            _eventPublisher.EntityDeleted(messageBind);
        }

        public virtual void DeleteEntities(IList<WMessageBindMapping> messageBinds)
        {
            if (messageBinds == null)
                throw new ArgumentNullException(nameof(messageBinds));

            //delete
            _wMessageBindRepository.Delete(messageBinds);

            foreach (var messageBind in messageBinds)
            {
                //event notification
                _eventPublisher.EntityDeleted(messageBind);
            }
        }

        public virtual void UpdateEntity(WMessageBindMapping messageBind)
        {
            if (messageBind == null)
                throw new ArgumentNullException(nameof(messageBind));

            _wMessageBindRepository.Update(messageBind);

            //event notification
            _eventPublisher.EntityUpdated(messageBind);
        }

        public virtual void UpdateEntities(IList<WMessageBindMapping> messageBinds)
        {
            if (messageBinds == null)
                throw new ArgumentNullException(nameof(messageBinds));

            //update
            _wMessageBindRepository.Update(messageBinds);

            //event notification
            foreach (var messageBind in messageBinds)
            {
                _eventPublisher.EntityUpdated(messageBind);
            }
        }

        public virtual WMessageBindMapping GetEntityById(int id)
        {
            if (id == 0)
                return null;

            return _wMessageBindRepository.GetById(id);
        }

        public virtual List<WMessageBindMapping> GetEntitiesByIds(int[] messageBindIds)
        {
            if (messageBindIds is null)
                return new List<WMessageBindMapping>();

            var query = from t in _wMessageBindRepository.Table
                        where messageBindIds.Contains(t.Id)
                        select t;

            return query.ToList();
        }

        public virtual List<int> GetMessageBindIds(int bindSceneId, WMessageBindSceneType messageBindSceneType)
        {
            if (bindSceneId <= 0)
                return new List<int>();

            var query = from t in _wMessageBindRepository.Table
                        where t.BindSceneId == bindSceneId &&
                        t.MessageBindSceneType == messageBindSceneType &&
                        t.Published
                        orderby t.DisplayOrder
                        select t;

            var messageBinds = query.ToList();

            //sort by passed identifiers
            var sortedIds = new List<int>();
            foreach (var messageBind in messageBinds)
            {
                sortedIds.Add(messageBind.WMessageId);
            }

            return sortedIds;
        }

        public virtual List<WMessageBindMapping> GetEntities(
            int messageId = 0,
            int bindSceneId = 0,
            WMessageBindSceneType? messageBindSceneType = null,
            bool? published = null)
        {
            var query = _wMessageBindRepository.Table;

            if (messageId > 0)
                query = query.Where(v => v.WMessageId == messageId);
            if (bindSceneId > 0)
                query = query.Where(v => v.BindSceneId == bindSceneId);
            if (messageBindSceneType != null)
                query = query.Where(v => v.MessageBindSceneType == messageBindSceneType);
            if (published.HasValue)
                query = query.Where(v => v.Published == published);

            query = query.OrderBy(v => v.DisplayOrder);

            return query.ToList();
        }

        public virtual IPagedList<WMessageBindMapping> GetEntities(
            int messageId = 0, 
            int bindSceneId = 0, 
            WMessageBindSceneType? messageBindSceneType = null,
            bool? published = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _wMessageBindRepository.Table;

            if (messageId > 0)
                query = query.Where(v => v.WMessageId == messageId);
            if (bindSceneId > 0)
                query = query.Where(v => v.BindSceneId == bindSceneId);
            if (messageBindSceneType != null)
                query = query.Where(v => v.MessageBindSceneType == messageBindSceneType);
            if (published.HasValue)
                query = query.Where(v => v.Published == published);

            query = query.OrderBy(v => v.DisplayOrder);

            return new PagedList<WMessageBindMapping>(query, pageIndex, pageSize);
        }

        #endregion
    }
}