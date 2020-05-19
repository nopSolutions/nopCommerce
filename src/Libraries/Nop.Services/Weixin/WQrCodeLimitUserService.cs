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
    /// WQrCodeLimitUser Service
    /// </summary>
    public partial class WQrCodeLimitUserService : IWQrCodeLimitUserService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<WQrCodeLimitUserMapping> _wQrCodeLimitUserMappingRepository;

        #endregion

        #region Ctor

        public WQrCodeLimitUserService(IEventPublisher eventPublisher,
            IRepository<WQrCodeLimitUserMapping> wQrCodeLimitUserMappingRepository)
        {
            _eventPublisher = eventPublisher;
            _wQrCodeLimitUserMappingRepository = wQrCodeLimitUserMappingRepository;
        }

        #endregion

        #region Methods

        public virtual void InsertEntity(WQrCodeLimitUserMapping entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _wQrCodeLimitUserMappingRepository.Insert(entity);

            //event notification
            _eventPublisher.EntityInserted(entity);
        }

        public virtual void DeleteEntity(WQrCodeLimitUserMapping entity, bool delete = false)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if(delete)
            {
                _wQrCodeLimitUserMappingRepository.Delete(entity);
            }
            else
            {
                entity.Deleted = true;
                UpdateEntity(entity);
            }

            //event notification
            _eventPublisher.EntityDeleted(entity);
        }

        public virtual void DeleteEntities(IList<WQrCodeLimitUserMapping> entities, bool deleted = false)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if(deleted)
            {
                _wQrCodeLimitUserMappingRepository.Delete(entities);
            }
            else
            {
                foreach (var entity in entities)
                {
                    entity.Deleted = true;
                }
                //delete wUser
                UpdateEntities(entities);
            }

            foreach (var entity in entities)
            {
                //event notification
                _eventPublisher.EntityDeleted(entity);
            }
        }

        public virtual void UpdateEntity(WQrCodeLimitUserMapping entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _wQrCodeLimitUserMappingRepository.Update(entity);

            //event notification
            _eventPublisher.EntityUpdated(entity);
        }

        public virtual void UpdateEntities(IList<WQrCodeLimitUserMapping> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            //update
            _wQrCodeLimitUserMappingRepository.Update(entities);

            //event notification
            foreach (var entity in entities)
            {
                _eventPublisher.EntityUpdated(entity);
            }
        }

        public virtual WQrCodeLimitUserMapping GetEntityById(int id)
        {
            if (id == 0)
                return null;

            return _wQrCodeLimitUserMappingRepository.GetById(id);
        }

        public virtual WQrCodeLimitUserMapping GetEntityByQrcodeLimitId(int qrCodeLimitId)
        {
            if (qrCodeLimitId == 0)
                return null;

            var query = from t in _wQrCodeLimitUserMappingRepository.Table
                        where t.QrCodeLimitId == qrCodeLimitId &&
                        t.ExpireTime > Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now) &&
                        t.Published &&
                        !t.Deleted
                        orderby t.Id descending
                        select t;

            return query.FirstOrDefault();
        }

        public virtual List<WQrCodeLimitUserMapping> GetEntitiesByIds(int[] wEntityIds)
        {
            if (wEntityIds is null)
                return new List<WQrCodeLimitUserMapping>();

            var query = from t in _wQrCodeLimitUserMappingRepository.Table
                        where wEntityIds.Contains(t.Id) &&
                        !t.Deleted
                        select t;

            return query.ToList();
        }

        public virtual IPagedList<WQrCodeLimitUserMapping> GetEntities(
            string openId = "", 
            int qrCodeLimitId = 0,
            int? expireTime = null, 
            bool? published = null, 
            bool showDeleted = false, 
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _wQrCodeLimitUserMappingRepository.Table;

            if (!string.IsNullOrEmpty(openId))
                query = query.Where(v => v.OpenId == openId);
            if (qrCodeLimitId > 0)
                query = query.Where(v => v.QrCodeLimitId == qrCodeLimitId);
            if (expireTime.HasValue)
            {
                if (expireTime.Value == 0)
                    query = query.Where(v => v.ExpireTime == expireTime);
                else if (expireTime.Value > 0)
                {
                    query = query.Where(v => v.ExpireTime >= expireTime);
                }
            }
            if(published.HasValue)
                query = query.Where(v => v.Published == published);
            if (!showDeleted)
                query = query.Where(v => v.Deleted);

            query = query.OrderBy(v => v.Id);

            return new PagedList<WQrCodeLimitUserMapping>(query, pageIndex, pageSize);
        }


        #endregion
    }
}