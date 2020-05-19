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
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// WConfig Service
    /// </summary>
    public partial class WConfigService : IWConfigService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<WConfig> _wConfigRepository;

        #endregion

        #region Ctor

        public WConfigService(IEventPublisher eventPublisher,
            IRepository<WConfig> wConfigRepository)
        {
            _eventPublisher = eventPublisher;
            _wConfigRepository = wConfigRepository;
        }

        #endregion

        #region Methods

        public virtual void InsertWConfig(WConfig wConfig)
        {
            if (wConfig == null)
                throw new ArgumentNullException(nameof(wConfig));

            _wConfigRepository.Insert(wConfig);

            //event notification
            _eventPublisher.EntityInserted(wConfig);
        }

        public virtual void DeleteWConfig(WConfig wConfig, bool delete = false)
        {
            if (wConfig == null)
                throw new ArgumentNullException(nameof(wConfig));

            if(delete)
            {
                _wConfigRepository.Delete(wConfig);
            }
            else
            {
                wConfig.Deleted = true;
                UpdateWConfig(wConfig);
            }
            
            //event notification
            _eventPublisher.EntityDeleted(wConfig);
        }

        public virtual void DeleteWConfigs(IList<WConfig> wConfigs, bool deleted = false)
        {
            if (wConfigs == null)
                throw new ArgumentNullException(nameof(wConfigs));

            if(deleted)
            {
                _wConfigRepository.Delete(wConfigs);
            }
            else
            {
                foreach (var wConfig in wConfigs)
                {
                    wConfig.Deleted = true;
                }
                //delete wUser
                UpdateWConfigs(wConfigs);
            }
            
            foreach (var wConfig in wConfigs)
            {
                //event notification
                _eventPublisher.EntityDeleted(wConfig);
            }
        }

        public virtual void UpdateWConfig(WConfig wConfig)
        {
            if (wConfig == null)
                throw new ArgumentNullException(nameof(wConfig));

            _wConfigRepository.Update(wConfig);

            //event notification
            _eventPublisher.EntityUpdated(wConfig);
        }

        public virtual void UpdateWConfigs(IList<WConfig> wConfigs)
        {
            if (wConfigs == null)
                throw new ArgumentNullException(nameof(wConfigs));

            //update
            _wConfigRepository.Update(wConfigs);

            //event notification
            foreach (var wConfig in wConfigs)
            {
                _eventPublisher.EntityUpdated(wConfig);
            }
        }

        public virtual WConfig GetWConfigById(int id)
        {
            if (id == 0)
                return null;

            return _wConfigRepository.ToCachedGetById(id);
        }

        public virtual WConfig GetWUserByOriginalId(string originalId)
        {
            if (string.IsNullOrEmpty(originalId))
                return null;

            originalId = originalId.Trim();

            var query = from t in _wConfigRepository.Table
                        where t.OriginalId == originalId &&
                        t.Published &&
                        !t.Deleted
                        select t;

            return query.FirstOrDefault();
        }

        public virtual WConfig GetWConfigByStoreId(int storeId)
        {
            if (storeId == 0)
                return null;

            var query = from t in _wConfigRepository.Table
                        where t.StoreId == storeId &&
                        t.Published &&
                        !t.Deleted
                        select t;

            return query.FirstOrDefault();
        }

        public virtual List<WConfig> GetWConfigsByIds(int[] wConfigIds)
        {
            if (wConfigIds is null)
                return new List<WConfig>();

            var query = from t in _wConfigRepository.Table
                        where wConfigIds.Contains(t.Id) &&
                        !t.Deleted &&
                        t.Published
                        select t;

            return query.ToList();
        }

        public virtual IPagedList<WConfig> GetUsers(bool showDeleted = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _wConfigRepository.Table;

            if (!showDeleted)
                query = query.Where(v => v.Deleted);

            query = query.OrderBy(v => v.Id);

            return new PagedList<WConfig>(query, pageIndex, pageSize);
        }

        #endregion
    }
}